using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Shapes;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Formatting;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;
using static PrestoCoverage.MarginCoverage;

namespace PrestoCoverage
{
    [Export(typeof(IGlyphFactoryProvider))]
    [Name("CommentGlyph")]
    [Order(Before = "VsTextMarker")]
    [ContentType("code")]
    [TagType(typeof(MarginCoverageTag))]
    internal sealed class CommentGlyphFactoryProvider : IGlyphFactoryProvider
    {
        public IGlyphFactory GetGlyphFactory(IWpfTextView view, IWpfTextViewMargin margin)
        {
            return new CommentGlyphFactory();
        }
    }

    internal class CommentGlyphFactory : IGlyphFactory
    {

        public UIElement GenerateGlyph(IWpfTextViewLine line, IGlyphTag tag)
        {
            var lineHeight = line.Height;
            var grid = new System.Windows.Controls.Grid()
            {
                Width = lineHeight,
                Height = lineHeight
            };
            grid.Children.Add(new Rectangle()
            {
                Fill = ((MarginCoverageTag)tag).BrushColor,
                Width = 2,
                Height = lineHeight - (lineHeight * 0.1),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            });

            return grid;
        }

    }
}
