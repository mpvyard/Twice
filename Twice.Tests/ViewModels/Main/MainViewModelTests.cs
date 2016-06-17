﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NuGet;
using Twice.Models.Columns;
using Twice.Models.Configuration;
using Twice.Models.Twitter;
using Twice.Utilities;
using Twice.ViewModels;
using Twice.ViewModels.Columns;
using Twice.ViewModels.Main;
using Twice.Views.Services;

namespace Twice.Tests.ViewModels.Main
{
	[TestClass, ExcludeFromCodeCoverage]
	public class MainViewModelTests
	{
		[TestMethod, TestCategory( "ViewModels.Main" )]
		public void AccountCommandOpensDialog()
		{
			// Arrange
			var contextList = new Mock<ITwitterContextList>();
			var notifier = new Mock<INotifier>();
			var columnList = new Mock<IColumnDefinitionList>();
			var columnFactory = new Mock<IColumnFactory>();

			var vm = new MainViewModel( contextList.Object, notifier.Object, columnList.Object, columnFactory.Object );

			var viewServices = new Mock<IViewServiceRepository>();
			viewServices.Setup( v => v.ShowAccounts( false ) ).Returns( Task.CompletedTask );
			vm.ViewServiceRepository = viewServices.Object;

			// Act
			vm.AccountsCommand.Execute( null );

			// Assert
			viewServices.Verify( v => v.ShowAccounts( false ), Times.Once() );
		}

		[TestMethod, TestCategory( "ViewModels.Main" )]
		public async Task AccountDialogIsOpenedIfUserWantsToAddAccountWhenNoneExists()
		{
			// Arrange
			var contextList = new Mock<ITwitterContextList>();
			contextList.SetupGet( c => c.Contexts ).Returns( new IContextEntry[0] );
			var notifier = new Mock<INotifier>();
			var columnList = new Mock<IColumnDefinitionList>();
			var columnFactory = new Mock<IColumnFactory>();

			var config = new Mock<IConfig>();
			config.SetupGet( c => c.General ).Returns( new GeneralConfig() );

			var vm = new MainViewModel( contextList.Object, notifier.Object, columnList.Object, columnFactory.Object )
			{
				Configuration = config.Object,
				Dispatcher = new SyncDispatcher()
			};

			var viewServices = new Mock<IViewServiceRepository>();
			viewServices.Setup( v => v.Confirm( It.IsAny<ConfirmServiceArgs>() ) )
				.Returns( Task.FromResult( true ) );
			viewServices.Setup( v => v.ShowAccounts( true ) ).Returns( Task.CompletedTask ).Verifiable();

			vm.ViewServiceRepository = viewServices.Object;

			// Act
			await vm.OnLoad( null );

			// Assert
			viewServices.Verify( v => v.ShowAccounts( true ), Times.Once() );
		}

		[TestMethod, TestCategory( "ViewModels.Main" )]
		public void AddColumnCommandNeedsAccount()
		{
			// Arrange
			var contexts = new List<IContextEntry>();

			var contextList = new Mock<ITwitterContextList>();
			contextList.SetupGet( c => c.Contexts ).Returns( () => contexts );
			var notifier = new Mock<INotifier>();
			var columnList = new Mock<IColumnDefinitionList>();
			var columnFactory = new Mock<IColumnFactory>();

			var vm = new MainViewModel( contextList.Object, notifier.Object, columnList.Object, columnFactory.Object );

			// Act
			bool withoutAccount = vm.AddColumnCommand.CanExecute( null );
			contexts.Add( new Mock<IContextEntry>().Object );
			bool withAccount = vm.AddColumnCommand.CanExecute( null );

			// Assert
			Assert.IsFalse( withoutAccount );
			Assert.IsTrue( withAccount );
		}

		[TestMethod, TestCategory( "ViewModels.Main" )]
		public void AddColumnCommandOpensDialog()
		{
			// Arrange
			var contexts = new[] {new Mock<IContextEntry>().Object};
			var contextList = new Mock<ITwitterContextList>();
			contextList.SetupGet( c => c.Contexts ).Returns( contexts );
			var notifier = new Mock<INotifier>();
			var columnList = new Mock<IColumnDefinitionList>();
			var columnFactory = new Mock<IColumnFactory>();

			var vm = new MainViewModel( contextList.Object, notifier.Object, columnList.Object, columnFactory.Object );

			var viewServices = new Mock<IViewServiceRepository>();
			viewServices.Setup( v => v.ShowAddColumnDialog() ).Returns( Task.CompletedTask );
			vm.ViewServiceRepository = viewServices.Object;

			// Act
			vm.AddColumnCommand.Execute( null );

			// Assert
			viewServices.Verify( v => v.ShowAddColumnDialog(), Times.Once() );
		}

		[TestMethod, TestCategory( "ViewModels.Main" )]
		public async Task ColumnsAreLoadedOnLoad()
		{
			// Arrange
			var context = new Mock<IContextEntry>();
			context.Setup( c => c.Twitter.LogCurrentRateLimits() ).Returns( Task.CompletedTask );
			var contextList = new Mock<ITwitterContextList>();
			contextList.SetupGet( c => c.Contexts ).Returns( new[] {context.Object} );
			var notifier = new Mock<INotifier>();
			var columnList = new Mock<IColumnDefinitionList>();
			var columnFactory = new Mock<IColumnFactory>();
			var config = new Mock<IConfig>();
			config.SetupGet( c => c.General ).Returns( new GeneralConfig() );
			var vm = new MainViewModel( contextList.Object, notifier.Object, columnList.Object, columnFactory.Object )
			{
				Configuration = config.Object,
				Dispatcher = new SyncDispatcher()
			};

			var column = new Mock<IColumnViewModel>();
			column.Setup( c => c.Load() ).Returns( Task.CompletedTask ).Verifiable();
			vm.Columns.Add( column.Object );
			vm.Columns.Add( column.Object );

			// Act
			await vm.OnLoad( null );

			// Assert
			column.Verify( c => c.Load(), Times.Exactly( 2 ) );
		}

		[TestMethod, TestCategory( "ViewModels.Main" )]
		public async Task ExceptionOnCredentialValidationDoesNotCrash()
		{
			// Arrange
			var context = new Mock<IContextEntry>();
			context.Setup( c => c.Twitter.VerifyCredentials() ).Throws(
				new WebException( "The remote name could not be resolved: 'api.twitter.com'" ) )
				.Verifiable();

			var columnList = new Mock<IColumnDefinitionList>();
			var columnFactory = new Mock<IColumnFactory>();

			var contextList = new Mock<ITwitterContextList>();
			contextList.SetupGet( c => c.Contexts ).Returns( new[] {context.Object} );

			var config = new Mock<IConfig>();
			config.SetupGet( c => c.General ).Returns( new GeneralConfig() );
			var vm = new MainViewModel( contextList.Object, null, columnList.Object, columnFactory.Object )
			{
				Configuration = config.Object,
				Dispatcher = new SyncDispatcher()
			};

			// Act
			await vm.OnLoad( null );

			// Assert
			context.Verify( c => c.Twitter.VerifyCredentials(), Times.AtLeastOnce() );
		}

		[TestMethod, TestCategory( "ViewModels.Main" )]
		public void InfoCommandOpensAboutDialog()
		{
			// Arrange
			var contextList = new Mock<ITwitterContextList>();
			var notifier = new Mock<INotifier>();
			var columnList = new Mock<IColumnDefinitionList>();
			var columnFactory = new Mock<IColumnFactory>();

			var vm = new MainViewModel( contextList.Object, notifier.Object, columnList.Object, columnFactory.Object );

			var viewServices = new Mock<IViewServiceRepository>();
			viewServices.Setup( v => v.ShowInfo() ).Returns( Task.CompletedTask );
			vm.ViewServiceRepository = viewServices.Object;

			// Act
			vm.InfoCommand.Execute( null );

			// Assert
			viewServices.Verify( v => v.ShowInfo(), Times.Once() );
		}

		[TestMethod, TestCategory( "ViewModels.Main" )]
		public async Task MissingUpdateExeDoesNotCrashUpdateCheck()
		{
			// Arrange
			var context = new Mock<IContextEntry>();
			context.Setup( c => c.Twitter.LogCurrentRateLimits() ).Returns( Task.CompletedTask );
			var contextList = new Mock<ITwitterContextList>();
			contextList.SetupGet( c => c.Contexts ).Returns( new[] {context.Object} );
			var notifier = new Mock<INotifier>();
			var columnList = new Mock<IColumnDefinitionList>();
			var columnFactory = new Mock<IColumnFactory>();
			var generalCfg = new GeneralConfig { CheckForUpdates = true };
			var config = new Mock<IConfig>();
			config.SetupGet( c => c.General ).Returns( generalCfg );
			var vm = new MainViewModel( contextList.Object, notifier.Object, columnList.Object, columnFactory.Object )
			{
				Configuration = config.Object,
				Dispatcher = new SyncDispatcher()
			};

			var updaterFactory = new Mock<IAppUpdaterFactory>();
			updaterFactory.Setup( f => f.Construct( It.IsAny<bool>() ) ).Throws( new Exception( "Update.exe not found" ) );

			vm.UpdateFactory = updaterFactory.Object;

			// Act
			var ex = await ExceptionAssert.CatchAsync<Exception>( () => vm.OnLoad( null ) );

			// Assert
			Assert.IsNull( ex );

			updaterFactory.Verify( f => f.Construct( It.IsAny<bool>() ), Times.Once() );
		}

		[TestMethod, TestCategory( "ViewModels.Main" )]
		public void NewTweetCommandNeedsAccount()
		{
			// Arrange
			var contexts = new List<IContextEntry>();

			var contextList = new Mock<ITwitterContextList>();
			contextList.SetupGet( c => c.Contexts ).Returns( () => contexts );
			var notifier = new Mock<INotifier>();
			var columnList = new Mock<IColumnDefinitionList>();
			var columnFactory = new Mock<IColumnFactory>();

			var vm = new MainViewModel( contextList.Object, notifier.Object, columnList.Object, columnFactory.Object );

			// Act
			bool withoutAccount = vm.NewTweetCommand.CanExecute( null );
			contexts.Add( new Mock<IContextEntry>().Object );
			bool withAccount = vm.NewTweetCommand.CanExecute( null );

			// Assert
			Assert.IsFalse( withoutAccount );
			Assert.IsTrue( withAccount );
		}

		[TestMethod, TestCategory( "ViewModels.Main" )]
		public void NewTweetCommandOpensWindow()
		{
			// Arrange
			var contexts = new[] {new Mock<IContextEntry>().Object};
			var contextList = new Mock<ITwitterContextList>();
			contextList.SetupGet( c => c.Contexts ).Returns( contexts );
			var notifier = new Mock<INotifier>();
			var columnList = new Mock<IColumnDefinitionList>();
			var columnFactory = new Mock<IColumnFactory>();

			var vm = new MainViewModel( contextList.Object, notifier.Object, columnList.Object, columnFactory.Object );

			var viewServices = new Mock<IViewServiceRepository>();
			viewServices.Setup( v => v.ComposeTweet() ).Returns( Task.CompletedTask ).Verifiable();
			vm.ViewServiceRepository = viewServices.Object;

			// Act
			vm.NewTweetCommand.Execute( null );

			// Assert
			viewServices.Verify( v => v.ComposeTweet(), Times.Once() );
		}

		[TestMethod, TestCategory( "ViewModels.Main" )]
		public void SettingsCommandsOpensDialog()
		{
			// Arrange
			var contextList = new Mock<ITwitterContextList>();
			var notifier = new Mock<INotifier>();
			var columnList = new Mock<IColumnDefinitionList>();
			var columnFactory = new Mock<IColumnFactory>();

			var config = new Mock<IConfig>();
			config.SetupGet( c => c.General ).Returns( new GeneralConfig() );
			var vm = new MainViewModel( contextList.Object, notifier.Object, columnList.Object, columnFactory.Object )
			{
				Configuration = config.Object,
				Dispatcher = new SyncDispatcher()
			};

			var viewServices = new Mock<IViewServiceRepository>();
			viewServices.Setup( v => v.ShowSettings() ).Returns( Task.CompletedTask );
			vm.ViewServiceRepository = viewServices.Object;

			// Act
			vm.SettingsCommand.Execute( null );

			// Assert
			viewServices.Verify( v => v.ShowSettings(), Times.Once() );
		}

		[TestMethod, TestCategory( "ViewModels.Main" )]
		public async Task UpdateCheckIsDoneWithCorrectChannel()
		{
			// Arrange
			var context = new Mock<IContextEntry>();
			context.Setup( c => c.Twitter.LogCurrentRateLimits() ).Returns( Task.CompletedTask );
			var contextList = new Mock<ITwitterContextList>();
			contextList.SetupGet( c => c.Contexts ).Returns( new[] {context.Object} );
			var notifier = new Mock<INotifier>();
			var columnList = new Mock<IColumnDefinitionList>();
			var columnFactory = new Mock<IColumnFactory>();
			var generalCfg = new GeneralConfig();
			var config = new Mock<IConfig>();
			config.SetupGet( c => c.General ).Returns( generalCfg );
			var vm = new MainViewModel( contextList.Object, notifier.Object, columnList.Object, columnFactory.Object )
			{
				Configuration = config.Object,
				Dispatcher = new SyncDispatcher()
			};

			var updater = new Mock<IAppUpdater>();
			var updaterFactory = new Mock<IAppUpdaterFactory>( MockBehavior.Strict );
			updaterFactory.Setup( f => f.Construct( true ) ).Returns( Task.FromResult( updater.Object ) );
			updaterFactory.Setup( f => f.Construct( false ) ).Returns( Task.FromResult( updater.Object ) );

			vm.UpdateFactory = updaterFactory.Object;
			vm.Configuration = config.Object;

			// Act
			generalCfg.CheckForUpdates = false;
			await vm.OnLoad( null );

			generalCfg.CheckForUpdates = true;
			generalCfg.IncludePrereleaseUpdates = false;
			await vm.OnLoad( null );

			generalCfg.CheckForUpdates = true;
			generalCfg.IncludePrereleaseUpdates = true;
			await vm.OnLoad( null );

			// Assert
			updaterFactory.Verify( f => f.Construct( true ), Times.Once() );
			updaterFactory.Verify( f => f.Construct( false ), Times.Once() );
		}

		[TestMethod, TestCategory( "ViewModels.Main" )]
		public async Task UserIsAskedToAddAccountIfNoneExists()
		{
			// Arrange
			var contextList = new Mock<ITwitterContextList>();
			contextList.SetupGet( c => c.Contexts ).Returns( new IContextEntry[0] );
			var notifier = new Mock<INotifier>();
			var columnList = new Mock<IColumnDefinitionList>();
			var columnFactory = new Mock<IColumnFactory>();
			var config = new Mock<IConfig>();
			config.SetupGet( c => c.General ).Returns( new GeneralConfig() );
			var vm = new MainViewModel( contextList.Object, notifier.Object, columnList.Object, columnFactory.Object )
			{
				Configuration = config.Object,
				Dispatcher = new SyncDispatcher()
			};

			var viewServices = new Mock<IViewServiceRepository>();
			viewServices.Setup( v => v.Confirm( It.IsAny<ConfirmServiceArgs>() ) )
				.Returns( Task.FromResult( false ) ).Verifiable();

			vm.ViewServiceRepository = viewServices.Object;

			// Act
			await vm.OnLoad( null );

			// Assert
			viewServices.Verify( v => v.Confirm( It.IsAny<ConfirmServiceArgs>() ), Times.Once() );
		}

		[TestMethod, TestCategory( "ViewModels.Main" )]
		public async Task UserIsInformedAboutUpdatedVersion()
		{
			// Arrange
			var context = new Mock<IContextEntry>();
			context.Setup( c => c.Twitter.LogCurrentRateLimits() ).Returns( Task.CompletedTask );
			var contextList = new Mock<ITwitterContextList>();
			contextList.SetupGet( c => c.Contexts ).Returns( new[] {context.Object} );
			var notifier = new Mock<INotifier>();
			notifier.Setup( n => n.DisplayMessage( It.IsAny<string>(), NotificationType.Information, null ) ).Verifiable();
			var columnList = new Mock<IColumnDefinitionList>();
			var columnFactory = new Mock<IColumnFactory>();
			var generalCfg = new GeneralConfig { CheckForUpdates = true };
			var config = new Mock<IConfig>();
			config.SetupGet( c => c.General ).Returns( generalCfg );
			config.SetupGet( c => c.General ).Returns( new GeneralConfig() );
			var vm = new MainViewModel( contextList.Object, notifier.Object, columnList.Object, columnFactory.Object )
			{
				Configuration = config.Object,
				Dispatcher = new SyncDispatcher()
			};

			var version = new SemanticVersion( 999, 9, 9, 9 );
			var updater = new Mock<IAppUpdater>();
			updater.Setup( u => u.UpdateApp() ).Returns( Task.FromResult( new AppRelease( version ) ) );

			var updaterFactory = new Mock<IAppUpdaterFactory>();
			updaterFactory.Setup( f => f.Construct( It.IsAny<bool>() ) ).Returns( Task.FromResult( updater.Object ) );
			vm.UpdateFactory = updaterFactory.Object;

			vm.Configuration = config.Object;

			// Act
			await vm.OnLoad( null );

			// Assert
			notifier.Verify( n => n.DisplayMessage( It.IsAny<string>(), NotificationType.Information, null ), Times.Once() );
		}
	}
}