using Microsoft.CodeAnalysis.Text;
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
        private readonly Coverage _coverage;

        public CommentTagger(ITextView textView, ITextBuffer buffer)
        {
            _coverage = new Coverage();

            _textView = textView;
            _buffer = buffer;

            var filename = GetFileName(buffer);

            var doc = _textView.TextSnapshot.GetOpenDocumentInCurrentContextWithChanges();
            if (doc == null) //happens when comparing code and probably other places I have not come across yet
                return;

            doc.TryGetTextVersion(out _loadedDocVersion);

            List<LineCoverageDetails> lcd = new List<LineCoverageDetails>();

            foreach (var item in Directory.GetFiles(Settings.WatchFolder, "*coverage.json"))
            {
                foreach (var lineCoverageDetail in Loaders.CoverletLoader.Load(item))
                {
                    _coverage.AddUpdateCoverage(lineCoverageDetail.SourceFile, lineCoverageDetail.CoveredFile, lineCoverageDetail.LineVisits);
                }
            }

            CreateFileWatcher(Settings.WatchFolder);
        }


        private readonly Microsoft.CodeAnalysis.VersionStamp _loadedDocVersion;

        IEnumerable<ITagSpan<MarginCoverageTag>> ITagger<MarginCoverageTag>.GetTags(NormalizedSnapshotSpanCollection spans)
        {
            foreach (SnapshotSpan curSpan in spans)
            {
                var doc = curSpan.Snapshot.GetOpenDocumentInCurrentContextWithChanges();

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

        private FileSystemWatcher _fileSystemWatcher;

        public void CreateFileWatcher(string path)
        {
            //if (_fileSystemWatcher != null)
            //    return;

            // Create a new FileSystemWatcher and set its properties.
            _fileSystemWatcher = new FileSystemWatcher();

            _fileSystemWatcher.Path = path;
            /* Watch for changes in LastAccess and LastWrite times, and 
               the renaming of files or directories. */
            _fileSystemWatcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName;
            // Only watch text files.
            _fileSystemWatcher.Filter = "*coverage.json";

            // Add event handlers.
            _fileSystemWatcher.Changed += new FileSystemEventHandler(OnChanged);
            _fileSystemWatcher.Created += new FileSystemEventHandler(OnChanged);
            _fileSystemWatcher.Deleted += new FileSystemEventHandler(OnDeleted);

            // Begin watching.
            _fileSystemWatcher.EnableRaisingEvents = true;
        }

        // Define the event handlers.
        private void OnChanged(object source, FileSystemEventArgs e)
        {
            ////https://stackoverflow.com/questions/1764809/filesystemwatcher-changed-event-is-raised-twice
            //_fileSystemWatcher.EnableRaisingEvents = false;

            //if (e.FullPath.EndsWith("coverage.json"))
            //{
            var covergeDetails = Loaders.CoverletLoader.Load(e.FullPath);
            //Coverage.AddUpdateCoverages(covergeDetails);
            foreach (var cd in covergeDetails)
                _coverage.AddUpdateCoverage(cd.SourceFile, cd.CoveredFile, cd.LineVisits);

            TagsChanged?.Invoke(this, new SnapshotSpanEventArgs(new SnapshotSpan(_buffer.CurrentSnapshot, 0, _buffer.CurrentSnapshot.Length)));
            //}

            //_fileSystemWatcher.EnableRaisingEvents = true;
        }

        private void OnDeleted(object source, FileSystemEventArgs e)
        {
            ////https://stackoverflow.com/questions/1764809/filesystemwatcher-changed-event-is-raised-twice
            //_fileSystemWatcher.EnableRaisingEvents = false;

            //if (e.FullPath.EndsWith("coverage.json"))
            //{
            _coverage.RemoveCoverage(e.FullPath);

            TagsChanged?.Invoke(this, new SnapshotSpanEventArgs(new SnapshotSpan(_buffer.CurrentSnapshot, 0, _buffer.CurrentSnapshot.Length)));
            //}

            //_fileSystemWatcher.EnableRaisingEvents = true;
        }

    }
}