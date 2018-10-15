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
        public const string WatchFolder = @"c:\coverlet";

        public static event EventHandler TestExecutionFinished;

        public static void OnTestExecutionFinished(object sender)
        {
            TestExecutionFinished?.Invoke(sender, null);
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


    [Export(typeof(ITestContainerDiscoverer))]
    [Export(typeof(PrestoCoverageContainerDiscoverer))]
    internal class PrestoCoverageContainerDiscoverer : ITestContainerDiscoverer
    {
        private readonly IServiceProvider _serviceProvider;

        [ImportingConstructor]
        internal PrestoCoverageContainerDiscoverer([Import(typeof(SVsServiceProvider))]IServiceProvider serviceProvider, [Import(typeof(IOperationState))]IOperationState operationState)
        {
            _serviceProvider = serviceProvider;
            operationState.StateChanged += OperationState_StateChanged;
        }

        public Uri ExecutorUri => new Uri("executor://PrestoCoverageExecutor/v1");

        public IEnumerable<ITestContainer> TestContainers => null;

        public event EventHandler TestContainersUpdated;

        private void OperationState_StateChanged(object sender, OperationStateChangedEventArgs e)
        {
            if (e.State == TestOperationStates.TestExecutionFinished)
            {
                var s = e.Operation;

                Settings.OnTestExecutionFinished(this);
            }
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
        private readonly Coverage _coverage;
        private FileSystemWatcher _fileSystemWatcher;


        public CommentTagger(ITextView textView, ITextBuffer buffer)
        {
            var dte = Package.GetGlobalService(typeof(DTE)) as DTE2;
            string solutionDir = System.IO.Path.GetDirectoryName(dte.Solution.FullName);

            CreateFileWatcher(solutionDir, "*UnitTest.dll");

            _coverage = new Coverage();

            _textView = textView;
            _buffer = buffer;

            var filename = GetFileName(buffer);

            //Todo refactor this into one place
            var doc = _textView.TextSnapshot.GetOpenDocumentInCurrentContextWithChanges();
            if (doc == null) //happens when comparing code and probably other places I have not come across yet
                return;

            doc.TryGetTextVersion(out _loadedDocVersion);

            Settings.TestExecutionFinished += Settings_TestExecutionFinished;

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

        private void Settings_TestExecutionFinished(object sender, EventArgs e)
        {
            string g = sender.ToString();
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


        private readonly Microsoft.CodeAnalysis.VersionStamp _loadedDocVersion;

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

                var line_visits = _coverage.GetDocumentCoverage(doc.FilePath);

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


        private string GetFileName(ITextBuffer buffer)
        {
            buffer.Properties.TryGetProperty(
                typeof(ITextDocument), out ITextDocument document);
            return document == null ? null : document.FilePath;
        }


        public void CreateFileWatcher(string path, string filter)
        {
            if (_fileSystemWatcher != null)
                return;

            _fileSystemWatcher = new FileSystemWatcher();

            _fileSystemWatcher.Path = path;
            _fileSystemWatcher.IncludeSubdirectories = true;
            /* Watch for changes in LastAccess and LastWrite times, and 
               the renaming of files or directories. */
            _fileSystemWatcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName;
            // Only watch text files.
            _fileSystemWatcher.Filter = filter;

            // Add event handlers.
            _fileSystemWatcher.Changed += new FileSystemEventHandler(OnChanged);
            _fileSystemWatcher.Created += new FileSystemEventHandler(OnChanged);
            _fileSystemWatcher.Deleted += new FileSystemEventHandler(OnDeleted);

            // Begin watching.
            _fileSystemWatcher.EnableRaisingEvents = true;
        }

        private void OnChanged(object source, FileSystemEventArgs e)
        {

            loadCoverage(e.FullPath);

            //var covergeDetails = Loaders.CoverletLoader.Load(e.FullPath);
            //
            //foreach (var cd in covergeDetails)
            //    _coverage.AddUpdateCoverage(cd.SourceFile, cd.CoveredFile, cd.LineVisits);

            TagsChanged?.Invoke(this, new SnapshotSpanEventArgs(new SnapshotSpan(_buffer.CurrentSnapshot, 0, _buffer.CurrentSnapshot.Length)));
        }

        private void OnDeleted(object source, FileSystemEventArgs e)
        {
            //var ts = new Testing();
            //ts.CoverMe();

            _coverage.RemoveCoverage(e.FullPath);

            TagsChanged?.Invoke(this, new SnapshotSpanEventArgs(new SnapshotSpan(_buffer.CurrentSnapshot, 0, _buffer.CurrentSnapshot.Length)));
        }

    }
}