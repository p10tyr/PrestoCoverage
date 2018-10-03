using Microsoft.CodeAnalysis.Text;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;
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

        public CommentTagger(ITextView textView, ITextBuffer buffer)
        {
            _textView = textView;
            _buffer = buffer;

            var filename = GetFileName(buffer);

            var doc = _textView.TextSnapshot.GetOpenDocumentInCurrentContextWithChanges();
            doc.TryGetTextVersion(out _loadedDocVersion);

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

                Dictionary<int, int> line_visits = Loaders.CoverletLoader.GetLinesForDocument(doc.FilePath);

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

        public void CreateFileWatcher(string path)
        {
            // Create a new FileSystemWatcher and set its properties.
            FileSystemWatcher watcher = new FileSystemWatcher();
            watcher.Path = path;
            /* Watch for changes in LastAccess and LastWrite times, and 
               the renaming of files or directories. */
            watcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName;
            // Only watch text files.
            watcher.Filter = "*.*";

            // Add event handlers.
            watcher.Changed += new FileSystemEventHandler(OnChanged);
            watcher.Created += new FileSystemEventHandler(OnChanged);
            watcher.Deleted += new FileSystemEventHandler(OnChanged);
            watcher.Renamed += new RenamedEventHandler(OnRenamed);

            // Begin watching.
            watcher.EnableRaisingEvents = true;
        }

        // Define the event handlers.
        private void OnChanged(object source, FileSystemEventArgs e)
        {
            //// Specify what is done when a file is changed, created, or deleted.
            //Console.WriteLine("File: " + e.FullPath + " " + e.ChangeType);

            TagsChanged?.Invoke(this, new SnapshotSpanEventArgs(new SnapshotSpan(_buffer.CurrentSnapshot, 0, _buffer.CurrentSnapshot.Length)));
        }

        private void OnRenamed(object source, RenamedEventArgs e)
        {
            TagsChanged?.Invoke(this, new SnapshotSpanEventArgs(new SnapshotSpan(_buffer.CurrentSnapshot, 0, _buffer.CurrentSnapshot.Length)));
        }

    }
}