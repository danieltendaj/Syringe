using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using Syringe.Core.Configuration;
using Syringe.Core.Git;

namespace Syringe.Tests.Integration.Git
{
	[TestFixture]
	[Explicit]
	public class GitProviderTests
	{
		private string _basePath;
		private string _gitRepositoryUrl;
		private static readonly string BRANCH_NAME = "for-integration-tests";

		[SetUp]
		public void Setup()
		{
			_gitRepositoryUrl = "https://github.com/TotalJobsGroup/Syringe-Examples.git";

			_basePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Integration", "GitTests");
			if (Directory.Exists(_basePath))
				ForceDeleteDirectory(_basePath);

			Directory.CreateDirectory(_basePath);
		}

		private void ForceDeleteDirectory(string path)
		{
			// Some Git files are readonly, this wipes them.
			var directory = new DirectoryInfo(path)
			{
				Attributes = FileAttributes.Normal
			};

			foreach (FileSystemInfo info in directory.GetFileSystemInfos("*", SearchOption.AllDirectories))
			{
				info.Attributes = FileAttributes.Normal;
			}

			directory.Delete(true);
		}

		private IConfiguration CreateConfiguration()
		{
			var jsonConfig =  new JsonConfiguration();
			jsonConfig.TestFilesBaseDirectory = _basePath;
			jsonConfig.GitConfiguration = new GitConfiguration()
			{
				RepositoryUrl = _gitRepositoryUrl,
				Branches = new List<string>() { BRANCH_NAME, "dev-team1", "dev-team2" }
			};

			return jsonConfig;
		}

		[Test]
		public void InitializeAll_should_create_directories_for_all_branches()
		{
			// given
			IConfiguration jsonConfig = CreateConfiguration();
			var gitProvider = new GitProvider(jsonConfig);

			// when
			gitProvider.InitializeAll();

			// then
			AssertRepositoryFiles(BRANCH_NAME);
			AssertRepositoryFiles("dev-team1");
			AssertRepositoryFiles("dev-team2");
		}

		private void AssertRepositoryFiles(string branchName)
		{
			string fullPath = Path.Combine(_basePath, branchName);

			Assert.True(Directory.Exists(fullPath), "The directory {0} does not exist", fullPath);
			Assert.True(Directory.Exists(Path.Combine(fullPath, ".git")), "The .git directory does not exist for {0}", fullPath);
			Assert.True(File.Exists(Path.Combine(fullPath, "readme.md")), "The readme.md file does not exist for {0}", fullPath);
		}

		[Test]
		public void CommitAndPush_should_send_files_to_github()
		{
			string githubUsername = Environment.GetEnvironmentVariable("GithubUsername", EnvironmentVariableTarget.Machine);
			string githubPassword = Environment.GetEnvironmentVariable("GithubPassword", EnvironmentVariableTarget.Machine);

			if (string.IsNullOrEmpty(githubUsername) || string.IsNullOrEmpty(githubPassword))
			{
				Console.WriteLine("Ignored as there is no Github username/password environmental variables."+
					"To fix this, open Powershell and use:\n" +
					"[Environment]::SetEnvironmentVariable(\"GithubUsername\", \"myusername\", \"User\")\n" +
					"[Environment]::SetEnvironmentVariable(\"GithubPassword\", \"mypassword\", \"User\")\n");

				Console.WriteLine("** Don't use your login for this, use the 'syringe-integrationtests' account!! **");

				Assert.Ignore("");
			}

			// given
			IConfiguration jsonConfig = CreateConfiguration();
			jsonConfig.GitConfiguration.Username = githubUsername;
			jsonConfig.GitConfiguration.Password = githubPassword;

			var gitProvider = new GitProvider(jsonConfig);
			gitProvider.InitializeAll();

			string filename = Guid.NewGuid() + ".xml";
			string xmlFilePath = Path.Combine(jsonConfig.TestFilesBaseDirectory, BRANCH_NAME, filename);
			File.WriteAllText(xmlFilePath, "<created-by-an-integration-test/>");

			// when
			gitProvider.CommitAndPush(filename, "MrRobot", "syringerobot@localhost", BRANCH_NAME);

			// then
			ForceDeleteDirectory(_basePath);
			gitProvider.InitializeAll();

			Assert.True(File.Exists(xmlFilePath));
		}

		[Test]
		public void Pull_should_get_latest_changes()
		{
			// given
			IConfiguration jsonConfig = CreateConfiguration();
			var gitProvider = new GitProvider(jsonConfig);
			gitProvider.InitializeAll();

			// when
			gitProvider.Pull(BRANCH_NAME);

			// then
			// How do you test this, without a huge amount of setup?
		}
	}
}
