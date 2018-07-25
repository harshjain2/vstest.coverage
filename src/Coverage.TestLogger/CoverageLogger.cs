// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.


namespace Microsoft.VisualStudio.TestPlatform.Extensions.CoverageLogger
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using Microsoft.TestPlatform.Extensions.CoverageLogger;
    using Microsoft.VisualStudio.TestPlatform.ObjectModel;
    using Microsoft.VisualStudio.TestPlatform.ObjectModel.Client;
    using Microsoft.VisualStudio.TestPlatform.Utilities;

    /// <summary>
    /// Logger for Generating CodeCoverage Analysis
    /// </summary>
    [FriendlyName(FriendlyName)]
    [ExtensionUri(CoverageUri)]
    internal class CoverageLogger : ITestLogger
    {
        #region Constants

        private const string CoverageUri = "datacollector://microsoft/CodeCoverage/2.0";

        private const string FriendlyName = "CoverageLogger";


        private const string SetupInteropx86 = @"x86\Microsoft.VisualStudio.Setup.Configuration.Native.dll";

        private const string SetupInteropx64 = @"x64\Microsoft.VisualStudio.Setup.Configuration.Native.dll";

        private static readonly Uri CodeCoverageDataCollectorUri = new Uri(CoverageUri);

        private ManualResetEvent coverageXmlGenerateEvent;

        private CodeCoverageUtility codeCoverageUtility;

        #endregion

        #region ITestLogger

        /// <inheritdoc/>
        public void Initialize(TestLoggerEvents events, string testResultsDirPath)
        {
            System.Diagnostics.Debugger.Launch();
            if (events == null)
            {
                throw new ArgumentNullException(nameof(events));
            }

            // Register for the events.
            events.TestRunComplete += this.TestRunCompleteHandler;

            this.codeCoverageUtility = new CodeCoverageUtility();
        }
        #endregion

        #region Event Handlers

        /// <summary>
        /// Called when a test run is completed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// Test run complete events arguments.
        /// </param>
        internal void TestRunCompleteHandler(object sender, TestRunCompleteEventArgs e)
        {
            if (e.AttachmentSets == null)
            {
                return;
            }

            var coverageAttachments = e.AttachmentSets
                .Where(dataCollectionAttachment => CodeCoverageDataCollectorUri.Equals(dataCollectionAttachment.Uri)).ToArray();

            if (coverageAttachments.Any())
            {
                var codeCoverageFiles = coverageAttachments.Select(coverageAttachment => coverageAttachment.Attachments[0].Uri.LocalPath).ToArray();

                foreach (var codeCoverageFile in codeCoverageFiles)
                {
                    var resultFile = Path.Combine(Path.GetDirectoryName(codeCoverageFile), Path.GetFileNameWithoutExtension(codeCoverageFile) + ".xml");
                    var arguments = "analyze /output:" + '"' + resultFile + '"' + " " + '"' + codeCoverageFile + '"';
                    try
                    {
                        this.codeCoverageUtility.AnalyzeCoverageFile(arguments, this.GetCodeCoverageExePath());
                    }
                    catch (Exception ex)
                    {
                        ConsoleOutput.Instance.WriteLine(ex.Message, OutputLevel.Information);
                    }

                    var summary =  this.codeCoverageUtility.GetCoverageSummary(resultFile);
                    ConsoleOutput.Instance.WriteLine(summary, OutputLevel.Information);
                }
            }
        }

        private string GetCodeCoverageExePath()
        {
            return @"C:\Users\harjain\.nuget\packages\microsoft.internal.codecoverage\1.0.9\contentFiles\any\any\CodeCoverage";
        }

        #endregion
    }
}