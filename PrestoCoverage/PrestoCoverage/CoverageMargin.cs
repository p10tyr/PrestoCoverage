using Microsoft.CodeAnalysis.Text;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;
using PrestoCoverage.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;

namespace PrestoCoverage
{

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

    internal class CommentTagger : ITagger<MarginCoverageTag>, ITagReloader, IDisposable
    {
        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

        private readonly ITextView _textView;
        private readonly ITextBuffer _buffer;

        private readonly string _solutionDirectory;
        private readonly Microsoft.CodeAnalysis.VersionStamp _loadedDocVersion;


        public CommentTagger(ITextView textView, ITextBuffer buffer)
        {
            _textView = textView;
            _buffer = buffer;

            //Todo refactor this into one place
            var doc = _textView.TextSnapshot.GetOpenDocumentInCurrentContextWithChanges();
            if (doc == null) //happens when comparing code and probably other places I have not come across yet
                return;

            PrestoCoverageCore.AddTagSession(this);

            doc.TryGetTextVersion(out _loadedDocVersion);
        }

        public void ReloadTags()
        {
            TagsChanged?.Invoke(this, new SnapshotSpanEventArgs(new SnapshotSpan(_buffer.CurrentSnapshot, 0, _buffer.CurrentSnapshot.Length)));
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

                var line_visits = PrestoCoverageCore.CoverageRepository.GetDocumentCoverage(doc.FilePath);

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

        public void Dispose()
        {
            PrestoCoverageCore.RemoveTagSession(this);
        }
    }
}