using Microsoft.VisualStudio.TestWindow.Extensibility;
using PrestoCoverage.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace PrestoCoverage
{
    public static class PrestoCoverageCore
    {
        //var dte = Package.GetGlobalService(typeof(DTE)) as DTE2;
        //SolutionDirectory = System.IO.Path.GetDirectoryName(dte.Solution.FullName);

        public static CoverageRepository CoverageRepository { get; set; }

        public const string WatchFolder = @"c:\coverlet";

        //public static string SolutionDirectory { get; set; }

        static PrestoCoverageCore()
        {
            CoverageRepository = new CoverageRepository();

            _coverageSession = new Dictionary<string, Coverlet.Core.Coverage>();
        }


        public static event EventHandler<OperationStateChangedEventArgs> TestExecutionFinished;


        private static Dictionary<string, Coverlet.Core.Coverage> _coverageSession;

        public static void OnTestExecutionStarting(object sender, OperationStateChangedEventArgs stateArgs)
        {
            // You can get this lovely DLL in your visual studio directory Microsoft.VisualStudio.TestWindow.Core.dll
            // var g = ((Microsoft.VisualStudio.TestWindow.Controller.Request)stateArgs.Operation); 
            // If you know a better to get the actual DLL without doing a Directory.Get files.. please let me know

            lock (_coverageSession)
            {

                var testRequestConfiguration =
                  stateArgs.Operation.GetType()
                      .GetProperty("Configuration", BindingFlags.NonPublic | BindingFlags.Instance)
                      .GetValue(stateArgs.Operation) as Microsoft.VisualStudio.TestWindow.Controller.RequestConfiguration;

                foreach (var testDll in testRequestConfiguration.TestSources)
                {
                    var _coverage = new Coverlet.Core.Coverage(testDll, new string[0], new string[0], new string[0], string.Empty);

                    _coverage.PrepareModules();

                    _coverageSession.Add(testDll, _coverage);
                }
            }
        }

        public static void OnTestExecutionFinished(object sender, OperationStateChangedEventArgs stateArgs)
        {
            lock (_coverageSession)
            {
                var _sessions = _coverageSession.Keys.ToList();

                if (!_sessions.Any())
                    return;

                foreach (var sessionKey in _sessions)
                {
                    var result = _coverageSession[sessionKey].GetCoverageResult();

                    var covergeDetails = Loaders.CoverletLoader.LoadCoverage(result);

                    foreach (var cd in covergeDetails)
                        CoverageRepository.AddUpdateCoverage(cd.SourceFile, cd.CoveredFile, cd.LineVisits);
                }
            }
        }

        //public void loadCoverage(string fullPath)
        //{
        //    //Coverlet.Core.Coverage coverage = new Coverlet.Core.Coverage(fullPath, new string[0], new string[0], new string[0], string.Empty);

        //    ////@"C:\Projects\PrestoCoverage\PrestoCoverage\PrestoCoverage.UnitTest\bin\Debug\netcoreapp2.1\PrestoCoverage.Sample.dll",

        //    //coverage.PrepareModules();

        //    //var result = coverage.GetCoverageResult();

        //    //var covergeDetails = Loaders.CoverletLoader.LoadCoverage(result);

        //    //foreach (var cd in covergeDetails)
        //    //    _coverage.AddUpdateCoverage(cd.SourceFile, cd.CoveredFile, cd.LineVisits);

        //}

        //private string GetFileName(ITextBuffer buffer)
        //{
        //    buffer.Properties.TryGetProperty(
        //        typeof(ITextDocument), out ITextDocument document);
        //    return document == null ? null : document.FilePath;
        //}


        //public void CreateFileWatcher(string path, string filter)
        //{
        //    if (_fileSystemWatcher != null)
        //        return;

        //    _fileSystemWatcher = new FileSystemWatcher();

        //    _fileSystemWatcher.Path = path;
        //    _fileSystemWatcher.IncludeSubdirectories = true;
        //    /* Watch for changes in LastAccess and LastWrite times, and 
        //       the renaming of files or directories. */
        //    _fileSystemWatcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName;
        //    // Only watch text files.
        //    _fileSystemWatcher.Filter = filter;

        //    // Add event handlers.
        //    _fileSystemWatcher.Changed += new FileSystemEventHandler(OnChanged);
        //    _fileSystemWatcher.Created += new FileSystemEventHandler(OnChanged);
        //    _fileSystemWatcher.Deleted += new FileSystemEventHandler(OnDeleted);

        //    // Begin watching.
        //    _fileSystemWatcher.EnableRaisingEvents = true;
        //}

        //private void OnChanged(object source, FileSystemEventArgs e)
        //{

        //    loadCoverage(e.FullPath);

        //    //var covergeDetails = Loaders.CoverletLoader.Load(e.FullPath);
        //    //
        //    //foreach (var cd in covergeDetails)
        //    //    _coverage.AddUpdateCoverage(cd.SourceFile, cd.CoveredFile, cd.LineVisits);

        //    TagsChanged?.Invoke(this, new SnapshotSpanEventArgs(new SnapshotSpan(_buffer.CurrentSnapshot, 0, _buffer.CurrentSnapshot.Length)));
        //}

        //private void OnDeleted(object source, FileSystemEventArgs e)
        //{
        //    //var ts = new Testing();
        //    //ts.CoverMe();

        //    _coverage.RemoveCoverage(e.FullPath);

        //    TagsChanged?.Invoke(this, new SnapshotSpanEventArgs(new SnapshotSpan(_buffer.CurrentSnapshot, 0, _buffer.CurrentSnapshot.Length)));
        //}


        //List<LineCoverageDetails> lcd = new List<LineCoverageDetails>();
        //foreach (var item in Directory.GetFiles(Settings.WatchFolder, "*coverage.json"))
        //{
        //    foreach (var lineCoverageDetail in Loaders.CoverletLoader.Load(item))
        //    {
        //        _coverage.AddUpdateCoverage(lineCoverageDetail.SourceFile, lineCoverageDetail.CoveredFile, lineCoverageDetail.LineVisits);
        //    }
        //}
        //CreateFileWatcher(Settings.WatchFolder, "*coverage.json");

    }

}
