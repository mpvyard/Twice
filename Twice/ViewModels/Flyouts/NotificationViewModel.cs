﻿using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using MahApps.Metro.Controls;
using Twice.Utilities.Os;
using Twice.Utilities.Ui;
using Twice.ViewModels.Twitter;

namespace Twice.ViewModels.Flyouts
{
	internal class NotificationViewModel : ObservableObject, IResetable, IViewController
	{
		public NotificationViewModel( ColumnItem item, bool top, IProcessStarter procStarter )
			: this( top, procStarter )
		{
			Type = NotificationType.Information;

			var status = item as StatusViewModel;
			if( status != null )
			{
				SetText( status.Model.Text );
			}

			var message = item as MessageViewModel;
			if( message != null )
			{
				SetText( message.Model.Text );
			}
		}

		public NotificationViewModel( ColumnItem item, Rect rect, IProcessStarter procStarter )
			: this( rect, procStarter )
		{
			Type = NotificationType.Information;

			var status = item as StatusViewModel;
			if( status != null )
			{
				SetText( status.Model.Text );
			}

			var message = item as MessageViewModel;
			if( message != null )
			{
				SetText( message.Model.Text );
			}
		}

		public NotificationViewModel( string message, NotificationType type, bool top, IProcessStarter procStarter )
			: this( top, procStarter )
		{
			SetText( message );
			Type = type;
		}

		public NotificationViewModel( string message, NotificationType type, Rect rect, IProcessStarter procStarter )
			: this( rect, procStarter )
		{
			SetText( message );
			Type = type;
		}

		private NotificationViewModel( Rect rect, IProcessStarter procStarter )
		{
			WindowRect = rect;
			ProcStarter = procStarter;
		}

		private NotificationViewModel( bool top, IProcessStarter procStarter )
		{
			ProcStarter = procStarter;
			FlyoutPosition = top
				? Position.Top
				: Position.Bottom;

			CloseDelay = TimeSpan.FromSeconds( 10 );
		}

		public void OnClosed()
		{
			CloseTimer.Stop();
			AutoCloseWatch.Stop();
		}

		private void Close( bool result = true )
		{
			CloseRequested?.Invoke( this, result
				? CloseEventArgs.Ok
				: CloseEventArgs.Cancel );
		}

		private void CloseTimer_Tick( object sender, EventArgs e )
		{
			AutoCloseProgress = 100 - (int)( AutoCloseWatch.ElapsedMilliseconds / CloseDelay.TotalMilliseconds * 100 );

			if( AutoCloseProgress <= 0 )
			{
				ExecuteDismissCommand();
			}
		}

		private void ExecuteDismissCommand()
		{
			Close();
			OnClosed();
		}

		private void ExecuteRestartAppCommand()
		{
			ProcStarter.Restart();
		}

		private void SetText( string text )
		{
			Text = text;
		}

		public Task Reset()
		{
			CloseTimer = new DispatcherTimer
			{
				Interval = TimeSpan.FromMilliseconds( 100 )
			};

			AutoCloseWatch = new Stopwatch();
			AutoCloseWatch.Start();

			AutoCloseProgress = 100;
			CloseTimer.Tick += CloseTimer_Tick;
			CloseTimer.Start();

			return Task.CompletedTask;
		}

		public void Center()
		{
			CenterRequested?.Invoke( this, EventArgs.Empty );
		}

		public event EventHandler CenterRequested;

		public event EventHandler<CloseEventArgs> CloseRequested;

		public int AutoCloseProgress
		{
			[DebuggerStepThrough] get { return _AutoCloseProgress; }
			set
			{
				if( _AutoCloseProgress == value )
				{
					return;
				}

				_AutoCloseProgress = value;
				RaisePropertyChanged();
			}
		}

		public TimeSpan CloseDelay { private get; set; }
		public ICommand DismissCommand => _DismissCommand ?? ( _DismissCommand = new RelayCommand( ExecuteDismissCommand ) );
		public IDispatcher Dispatcher { private get; set; }

		public bool DisplayRestartButton
		{
			[DebuggerStepThrough] get { return _DisplayRestartButton; }
			set
			{
				if( _DisplayRestartButton == value )
				{
					return;
				}

				_DisplayRestartButton = value;
				RaisePropertyChanged( nameof(DisplayRestartButton) );
			}
		}

		public Position FlyoutPosition
		{
			[DebuggerStepThrough] get { return _FlyoutPosition; }
			set
			{
				if( _FlyoutPosition == value )
				{
					return;
				}

				_FlyoutPosition = value;
				RaisePropertyChanged();
			}
		}

		public IMessenger MessengerInstance { private get; set; }

		public ICommand RestartAppCommand => _RestartAppCommand
		                                     ?? ( _RestartAppCommand = new RelayCommand( ExecuteRestartAppCommand ) );

		public string Text
		{
			[DebuggerStepThrough] get { return _Text; }
			set
			{
				if( _Text == value )
				{
					return;
				}

				_Text = value;
				RaisePropertyChanged();
			}
		}

		public NotificationType Type
		{
			[DebuggerStepThrough] get { return _Type; }
			set
			{
				if( _Type == value )
				{
					return;
				}

				_Type = value;
				RaisePropertyChanged();

				DisplayRestartButton = _Type.HasFlag( NotificationType.Restart );
			}
		}

		public Rect WindowRect { get; }

		private readonly IProcessStarter ProcStarter;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )] private int _AutoCloseProgress;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )] private RelayCommand _DismissCommand;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )] private bool _DisplayRestartButton;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )] private Position _FlyoutPosition;

		private RelayCommand _RestartAppCommand;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )] private string _Text;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )] private NotificationType _Type;

		private Stopwatch AutoCloseWatch;
		private DispatcherTimer CloseTimer;
	}
}