using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Microsoft.CodeAnalysis.Text;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;

namespace PrestoCoverage
{
    public class MarginCoverage
    {

        [Export(typeof(ITaggerProvider))]
        [ContentType("code")]
        [TagType(typeof(MarginCoverageTag))]
        internal class CommentTaggerProvider : ITaggerProvider
        {
            public ITagger<T> CreateTagger<T>(ITextBuffer buffer) where T : ITag
            {
                if (buffer == null)
                {
                    throw new ArgumentNullException("buffer");
                }

                return new CommentTagger() as ITagger<T>;
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

            IEnumerable<ITagSpan<MarginCoverageTag>> ITagger<MarginCoverageTag>.GetTags(NormalizedSnapshotSpanCollection spans)
            {
                foreach (SnapshotSpan curSpan in spans)
                {
                    var doc = curSpan.Snapshot.GetOpenDocumentInCurrentContextWithChanges();

                    Dictionary<int, int> lines = Loaders.CoverletLoader.GetLinesForDocument(doc.FilePath);

                    if (lines.Count < 1)
                        continue;

                    foreach (var ln in curSpan.Snapshot.Lines.Where(l => lines.Keys.Contains(l.LineNumber + 1)))
                    {
                        var coverage = lines[ln.LineNumber + 1];

                        System.Windows.Media.Brush brushColor = coverage > 0 ? System.Windows.Media.Brushes.Green : System.Windows.Media.Brushes.Red;

                        SnapshotSpan todoSpan = new SnapshotSpan(ln.Start, ln.End);
                        yield return new TagSpan<MarginCoverageTag>(todoSpan, new MarginCoverageTag(brushColor));
                    }
                }
            }

            public event EventHandler<SnapshotSpanEventArgs> TagsChanged
            {
                add { }
                remove { }
            }
        }

    }
}
