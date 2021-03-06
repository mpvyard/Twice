﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics.CodeAnalysis;
using Twice.Utilities.Os;

namespace Twice.Tests.Utilities
{
	[TestClass, ExcludeFromCodeCoverage]
	public class DpApiTests
	{
		[TestMethod, TestCategory( "Utilities" )]
		public void MachineAndUserKeysAreDifferent()
		{
			// Arrange
			const string plain = "Hello World this is a test with some text";

			// Act
			string machine = DpApi.Encrypt( DpApi.KeyType.MachineKey, plain );
			string user = DpApi.Encrypt( DpApi.KeyType.UserKey, plain );

			// Assert
			Assert.AreNotEqual( machine, user );
		}

		[TestMethod, TestCategory( "Utilities" )]
		public void MachineLevelEncryptionCanBeDecrypted()
		{
			// Arrange
			const string plain = "Hello World this is a test with some text";

			// Act
			string encrypted = DpApi.Encrypt( plain );
			string decrypted = DpApi.Decrypt( encrypted );

			// Assert
			Assert.AreNotEqual( plain, encrypted );
			Assert.AreEqual( plain, decrypted );
		}

		[TestMethod, TestCategory( "Utilities" )]
		public void UserLevelEncryptionCanBeDecrypted()
		{
			// Arrange
			const string plain = "Hello World this is a test with some text";

			// Act
			string encrypted = DpApi.Encrypt( DpApi.KeyType.UserKey, plain );
			string decrypted = DpApi.Decrypt( encrypted );

			// Assert
			Assert.AreNotEqual( plain, encrypted );
			Assert.AreEqual( plain, decrypted );
		}
	}
}