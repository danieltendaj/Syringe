﻿using System;
using System.Collections.Generic;
using Syringe.Core.Tests;

namespace Syringe.Core.Repositories
{
    public interface ITestRepository
    {
        IEnumerable<string> ListFiles();
        TestFile GetTestFile(string filename);
        Test GetTest(string filename, int position);
        bool SaveTest(Test test);
        bool CreateTest(Test test);
        bool DeleteTest(int position, string fileName);
        bool CreateTestFile(TestFile testFile);
        bool UpdateTestVariables(TestFile testFile);
        string GetXml(string filename);
        bool DeleteFile(string fileName);
    }
}