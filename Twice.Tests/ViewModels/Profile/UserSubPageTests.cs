﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Twice.ViewModels.Profile;

namespace Twice.Tests.ViewModels.Profile
{
	[TestClass]
	public class UserSubPageTests
	{
		[TestMethod, TestCategory( "ViewModels.Profile" )]
		public void AsyncLoadCallsFunction()
		{
			// Arrange
			bool called = false;
			Func<Task<IEnumerable<object>>> loadAction = () =>
			{
				called = true;
				return Task.FromResult( Enumerable.Range( 1, 2 ).Cast<object>() );
			};

			var waitHandle = new ManualResetEvent( false );
			int loadChanges = 0;
			var page = new UserSubPage( "", loadAction, 2 );
			page.PropertyChanged += ( s, e ) =>
			{
				if( e.PropertyName == nameof( UserSubPage.IsLoading ) )
				{
					loadChanges++;

					if( loadChanges == 2 )
					{
						waitHandle.Set();
					}
				}
			};

			// Act
			var temp = page.Items;
			waitHandle.WaitOne( 1000 );
			var items = page.Items.ToArray();

			// Assert
			Assert.IsTrue( called );
			Assert.AreEqual( 2, loadChanges );
			Assert.AreEqual( 2, items.Length );
		}

		[TestMethod, TestCategory( "ViewModels.Profile" )]
		public void ConstructorSetsCorrectData()
		{
			// Arrange
			Func<Task<IEnumerable<object>>> loadAction = () => Task.FromResult( Enumerable.Empty<object>() );

			// Act
			var page = new UserSubPage( "TITLE", loadAction, 123 );

			// Assert
			Assert.AreEqual( 123, page.Count );
			Assert.AreEqual( "TITLE", page.Title );
		}

		[TestMethod, TestCategory( "ViewModels.Profile" )]
		public void NotifyPropertyChangedIsImplementedCorrectly()
		{
			// Arrange
			Func<Task<IEnumerable<object>>> loadAction = () => Task.FromResult( Enumerable.Empty<object>() );
			var page = new UserSubPage( "", loadAction, 0 );
			var tester = new PropertyChangedTester( page );

			// Act
			tester.Test();

			// Assert
			tester.Verify();
		}
	}
}