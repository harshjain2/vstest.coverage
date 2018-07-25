// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.


using System;

namespace Microsoft.TestPlatform.Extensions.CoverageLogger.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.IO;
    using System.Reflection;

    [TestClass]
    public class CodeCoverageUtilityTests
    {
        private CodeCoverageUtility codeCoverageUtility;

        [TestInitialize]
        public void Initialize()
        {
            this.codeCoverageUtility = new CodeCoverageUtility();
        }

        [TestMethod]
        public void AnalyzeCoverageFileShouldGenerateXMLFile()
        {
            var testContainerDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            this.codeCoverageUtility.AnalyzeCoverageFile(Path.Combine(testContainerDirectory, @"TestData\Sample.coverage"), Path.Combine(testContainerDirectory, @"TestData\CodeCoverageExe\CodeCoverage"));

            var expectedFileText = File.ReadAllText(Path.Combine(testContainerDirectory, @"TestData\Expected.xml"));
            var generatedFileText = File.ReadAllText(Path.Combine(testContainerDirectory, @"TestData\Sample.xml"));

            File.Delete(Path.Combine(testContainerDirectory, @"TestData\Sample.xml"));

            Assert.AreEqual(expectedFileText, generatedFileText, "Generated file doesn't have correct data.");


        }

        [TestMethod]
        public void GetCoverageXMLShouldReturnSummary()
        {
            var testContainerDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var xmlFilePath =Path.Combine(testContainerDirectory, @"TestData\Expected.xml");
            var expectedSummary = Environment.NewLine +
                                  "Code Coverage" + Environment.NewLine +
                                  "----------------" + Environment.NewLine +
                                  "Module Name\t\t\tNot Covered(Blocks)\t\tNot Covered(% Blocks)\t\tCovered(Blocks)\t\tCovered(% Blocks)" + Environment.NewLine +
                                  "unittestproject7.dll\t\t1\t\t\t\t\t100\t\t\t\t0\t\t\t\t0.00" + Environment.NewLine;

            var summary = this.codeCoverageUtility.GetCoverageSummary(xmlFilePath);

            Assert.AreEqual(expectedSummary, summary, "Summy is not correct");
        }
    }
}
