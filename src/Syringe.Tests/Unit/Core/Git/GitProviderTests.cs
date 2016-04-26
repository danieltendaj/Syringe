using System;
using System.Collections.Generic;
using NUnit.Framework;
using Syringe.Core.Configuration;
using Syringe.Core.Exceptions;
using Syringe.Core.Git;

namespace Syringe.Tests.Unit.Core.Git
{
	[TestFixture]
	public class GitProviderTests
	{
		[Test]
		public void should_throw_ArgumentNullException_for_null_configuration()
		{
			// given
			IConfiguration jsonConfig = null;

			// when + then
			Assert.Throws<ArgumentNullException>(() => new GitProvider(jsonConfig));
		}

		[Test]
		public void should_throw_ConfigurationException_for_empty_base_directory()
		{
			// given
			var jsonConfig = new JsonConfiguration();
			jsonConfig.TestFilesBaseDirectory = "";

			// when + then
			Assert.Throws<ConfigurationException>(() => new GitProvider(jsonConfig));
		}

		[Test]
		public void should_throw_ConfigurationException_for_null_gitconfiguration()
		{
			// given
			var jsonConfig = new JsonConfiguration();
			jsonConfig.TestFilesBaseDirectory = @"c:\foo";
			jsonConfig.GitConfiguration = null;

			// when + then
			Assert.Throws<ConfigurationException>(() => new GitProvider(jsonConfig));
		}

		[Test]
		public void should_throw_ConfigurationException_for_empty_repository_url()
		{
			// given
			var jsonConfig = new JsonConfiguration();
			jsonConfig.TestFilesBaseDirectory = @"c:\foo";
			jsonConfig.GitConfiguration.RepositoryUrl = "";

			// when + then
			Assert.Throws<ConfigurationException>(() => new GitProvider(jsonConfig));
		}

		[Test]
		public void should_throw_ConfigurationException_for_empty_branches_list()
		{
			// given
			var jsonConfig = new JsonConfiguration();
			jsonConfig.TestFilesBaseDirectory = @"c:\foo";
			jsonConfig.GitConfiguration.RepositoryUrl = "http://www.github.com";
			jsonConfig.GitConfiguration.Branches = new List<string>();

			// when + then
			Assert.Throws<ConfigurationException>(() => new GitProvider(jsonConfig));
		}
	}
}
