﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Syringe.Core.Configuration;
using Syringe.Core.Xml;

namespace Syringe.Core.Domain.Repository
{
    public class CaseRepository : ICaseRepository
    {
	    private readonly ITestCaseReader _testCaseReader;
	    private readonly IApplicationConfiguration _appConfig;

	    public CaseRepository() : this(new TestCaseReader(), new ApplicationConfig()) { }

	    internal CaseRepository(ITestCaseReader testCaseReader, IApplicationConfiguration appConfig)
	    {
		    _testCaseReader = testCaseReader;
		    _appConfig = appConfig;
	    }

	    public Case GetTestCase(string filename, string teamName, int caseId)
        {
			string fullPath = Path.Combine(_appConfig.TestCasesBaseDirectory, teamName, filename);
			if (!File.Exists(fullPath))
				throw new FileNotFoundException("The test case cannot be found", filename);


			string xml = File.ReadAllText(fullPath);
			using (var stringReader = new StringReader(xml))
            {
				CaseCollection collection = _testCaseReader.Read(stringReader);
                Case testCase = collection.TestCases.First(x => x.Id == caseId);

                if (testCase == null)
                {
                    throw new NullReferenceException("Could not find specified Test Case:" + caseId);
                }

                return testCase;
            }
        }

		public CaseCollection GetTestCaseCollection(string filename, string teamName)
		{
			string fullPath = Path.Combine(_appConfig.TestCasesBaseDirectory, teamName, filename);
			string xml = File.ReadAllText(fullPath);

			using (var stringReader = new StringReader(xml))
			{
				return _testCaseReader.Read(stringReader);
			}
		}

		public IEnumerable<string> ListCasesForTeam(string teamName)
		{
			string fullPath = Path.Combine(_appConfig.TestCasesBaseDirectory, teamName);

			foreach (string file in Directory.EnumerateFiles(fullPath))
			{
				var fileInfo = new FileInfo(file);
				yield return fileInfo.Name;
			}
		}
    }
}