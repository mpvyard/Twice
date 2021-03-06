using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace Twice.ViewModels.Dialogs.Data
{
	internal class DialogStack : IDialogStack
	{
		public bool CanGoBack()
		{
			return Data.Count > 1;
		}

		public void Clear()
		{
			Data.Clear();
		}

		public bool Push( DialogData data )
		{
			if( Data.Count > 0 )
			{
				var topData = Data.Peek();
				if( topData.Equals( data ) )
				{
					return false;
				}
			}

			Data.Push( data );
			return true;
		}

		public void Remove()
		{
			Data.Pop();
		}

		public TResult ResultSetup<TViewModel, TResult>( TViewModel vm ) where TViewModel : class
		{
			var topData = Data.Peek();
			return (TResult)topData.GetResult( vm );
		}

		public void Setup( IContentChanger contentChanger )
		{
			var topData = Data.Peek();
			var content = Activator.CreateInstance( topData.ControlType ) as UserControl;
			contentChanger.ChangeContent( content );

			topData.Setup( content?.DataContext );
		}

		public void Setup<TViewModel>( TViewModel vm ) where TViewModel : class
		{
			var topData = Data.Peek();
			topData.Setup( vm );
		}

		private readonly Stack<DialogData> Data = new Stack<DialogData>();
	}
}