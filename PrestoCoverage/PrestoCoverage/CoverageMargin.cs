using EnvDTE;
using EnvDTE80;
using Microsoft.CodeAnalysis.Text;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.TestWindow.Extensibility;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;
using PrestoCoverage.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;

namespace PrestoCoverage
{



    public static class Settings
    {

        public static Coverage Coverage { get; set; }

        public const string WatchFolder = @"c:\coverlet";

        //public static string SolutionDirectory { get; set; }

        static Settings()
        {
            Coverage = new Coverage();

            //var dte = Package.GetGlobalService(typeof(DTE)) as DTE2;
            //SolutionDirectory = System.IO.Path.GetDirectoryName(dte.Solution.FullName);

            _coverageSession = new Dictionary<string, Coverlet.Core.Coverage>();
        }


        public static event EventHandler<OperationStateChangedEventArgs> TestExecutionFinished;


        private static Dictionary<string, Coverlet.Core.Coverage> _coverageSession;

        public static void OnTestExecutionStarting(object sender, OperationStateChangedEventArgs stateArgs)
        {
            lock (_coverageSession)
            {
                //var testDlls = Directory
                //    .GetFiles(SolutionDirectory, "*test.dll", SearchOption.AllDirectories)
                //    .Where(s => s.Contains(@"\bin\"))
                //    .ToArray();

                //if (testDlls.Length <= 0)
                //    return;

                //var conf = (Microsoft.VisualStudio.TestWindow.Extensibility.IRunSettingsConfigurationInfo)stateArgs.Operation;

                //var conf2 = (Microsoft.VisualStudio.TestWindow.Extensibility.IRunSettingsService)stateArgs;

                //var conf3 = (Microsoft.VisualStudio.TestWindow.Extensibility.ITest)stateArgs;


                //foreach (var testDll in stateArgs.Operation. .Configuration.)
                //{
                //    var _coverage = new Coverlet.Core.Coverage(testDll, new string[0], new string[0], new string[0], string.Empty);

                //    _coverage.PrepareModules();

                //    _coverageSession.Add(testDll, _coverage);
                //}
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
                        Coverage.AddUpdateCoverage(cd.SourceFile, cd.CoveredFile, cd.LineVisits);
                }
            }
        }


    }


    [Export(typeof(IViewTaggerProvider))]
    [ContentType("text")]
    [TagType(typeof(MarginCoverageTag))]
    internal class CommentTaggerProvider : IViewTaggerProvider
    {
        public ITagger<T> CreateTagger<T>(ITextView textView, ITextBuffer buffer) where T : ITag
        {
            if (textView == null)
                return null;

            return new CommentTagger(textView, buffer) as ITagger<T>;
        }
    }



    internal class MarginCoverageTag : IGlyphTag
    {
        public System.Windows.Media.Brush BrushColor;

        public MarginCoverageTag(System.Windows.Media.Brush color)
        {
            BrushColor = color;
        }
    }




    internal class CommentTagger : ITagger<MarginCoverageTag>
    {
        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

        private readonly ITextView _textView;
        private readonly ITextBuffer _buffer;
        //private readonly Coverage _coverage;
        private FileSystemWatcher _fileSystemWatcher;

        private readonly string _solutionDirectory;
        private readonly Microsoft.CodeAnalysis.VersionStamp _loadedDocVersion;

        //private readonly string[] testDlls;

        public CommentTagger(ITextView textView, ITextBuffer buffer)
        {
            var dte = Package.GetGlobalService(typeof(DTE)) as DTE2;
            _solutionDirectory = System.IO.Path.GetDirectoryName(dte.Solution.FullName);

            _textView = textView;
            _buffer = buffer;

            //_coverage = new Coverage();

            //var filename = GetFileName(buffer);

            //Todo refactor this into one place
            var doc = _textView.TextSnapshot.GetOpenDocumentInCurrentContextWithChanges();
            if (doc == null) //happens when comparing code and probably other places I have not come across yet
                return;

            doc.TryGetTextVersion(out _loadedDocVersion);

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


        public void loadCoverage(string fullPath)
        {
            //Coverlet.Core.Coverage coverage = new Coverlet.Core.Coverage(fullPath, new string[0], new string[0], new string[0], string.Empty);

            ////@"C:\Projects\PrestoCoverage\PrestoCoverage\PrestoCoverage.UnitTest\bin\Debug\netcoreapp2.1\PrestoCoverage.Sample.dll",

            //coverage.PrepareModules();

            //var result = coverage.GetCoverageResult();

            //var covergeDetails = Loaders.CoverletLoader.LoadCoverage(result);

            //foreach (var cd in covergeDetails)
            //    _coverage.AddUpdateCoverage(cd.SourceFile, cd.CoveredFile, cd.LineVisits);

        }


        IEnumerable<ITagSpan<MarginCoverageTag>> ITagger<MarginCoverageTag>.GetTags(NormalizedSnapshotSpanCollection spans)
        {
            foreach (SnapshotSpan curSpan in spans)
            {
                var doc = curSpan.Snapshot.GetOpenDocumentInCurrentContextWithChanges();

                if (doc == null) //happens on compare screens and while editing text files... just skip it
                    continue;

                //MonitorTests(doc.Project.FilePath);

                var currentLineCount = curSpan.Snapshot.LineCount;
                doc.TryGetTextVersion(out var currentDocVersion);

                var line_visits = Settings.Coverage.GetDocumentCoverage(doc.FilePath);

                List<int> lines = line_visits.Keys.ToList();

                if (lines.Count < 1)
                    continue;

                if (_loadedDocVersion != currentDocVersion)
                    continue;

                foreach (var ln in curSpan.Snapshot.Lines.Where(l => lines.Contains(l.LineNumber + 1)))
                {
                    var coverage = line_visits[ln.LineNumber + 1];

                    System.Windows.Media.Brush brushColor = coverage > 0 ? System.Windows.Media.Brushes.Green : System.Windows.Media.Brushes.Red;

                    //if (_loadedDocVersion != currentDocVersion)
                    //    brushColor = System.Windows.Media.Brushes.Orange;

                    SnapshotSpan todoSpan = new SnapshotSpan(ln.Start, ln.End);
                    yield return new TagSpan<MarginCoverageTag>(todoSpan, new MarginCoverageTag(brushColor));
                }
            }
        }


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

    }
}