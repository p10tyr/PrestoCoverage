namespace PrestoCoverage.Models
{
    public class PrestoConfiguration
    {
        public PrestoConfiguration()
        {
            IsJsonConfigDriven = false;
        }

        public bool IsJsonConfigDriven { get; private set; }

        public bool ClearOnBuild { get; set; }
        public GlyphColoursOptions Colours { get; set; }
        public WatchFolderOptions WatchFolder { get; set; }

        public class GlyphColoursOptions
        {
            public string Covered { get; set; }
            public string Partial { get; set; }
            public string Uncovered { get; set; }
        }
        public class WatchFolderOptions
        {
            public bool IsEnabled { get; set; }
            public string Path { get; set; }
            public string Filter { get; set; }
        }

    }
}
