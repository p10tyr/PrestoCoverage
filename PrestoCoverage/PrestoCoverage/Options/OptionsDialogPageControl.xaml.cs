using System.Windows.Controls;

namespace PrestoCoverage.Options
{
    /// <summary>
    /// Interaction logic for OptionsDialogPageControl.xaml
    /// </summary>
    public partial class OptionsDialogPageControl : UserControl
    {
        //public List<System.Drawing.Color> Colours = new List<System.Drawing.Color>();

        public OptionsDialogPageControl()
        {
            InitializeComponent();

            ColourCovered.Text = PrestoCoverageCore.PrestoConfiguration.Colours.Covered;
            ColourPartialCovered.Text = PrestoCoverageCore.PrestoConfiguration.Colours.Partial;
            ColourUncovered.Text = PrestoCoverageCore.PrestoConfiguration.Colours.Uncovered;

            ClearCoverageOnChangeCheckbox.IsChecked = PrestoCoverageCore.PrestoConfiguration.ClearOnBuild;

            WatchFolderPath.Text = PrestoCoverageCore.PrestoConfiguration.WatchFolder.Path;
            WatchFolderFilter.Text = PrestoCoverageCore.PrestoConfiguration.WatchFolder.Filter;

            if (PrestoCoverageCore.PrestoConfiguration.IsJsonConfigDriven)
                LockAllControls();
        }

        private void LockAllControls()
        {
            ColourCovered.IsEnabled = false;
            ColourPartialCovered.IsEnabled = false;
            ColourUncovered.IsEnabled = false;
            ClearCoverageOnChangeCheckbox.IsEnabled = false;
            WatchFolderFilter.IsEnabled = false;
            WatchFolderPath.IsEnabled = false;

            HeaderLabel.Content = "Configuration driven by presto.config.json";
            HeaderLabel.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(0xff, 0x00, 0x00));
        }

        private void TextBox_Covered_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                var c = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(ColourCovered.Text);

                PrestoCoverageCore.Colour_Covered = new System.Windows.Media.SolidColorBrush(c);
                ColourCoveredFill.Fill = PrestoCoverageCore.Colour_Covered;

                GeneralSettings.Default.Glyph_CoveredColour = c.ToString();
                GeneralSettings.Default.Save();

                PrestoCoverageCore.reloadTaggers();
            }
            catch (System.Exception)
            {
            }
        }

        private void TextBox_Partial_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                var c = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(ColourPartialCovered.Text);

                PrestoCoverageCore.Colour_CoveredPartial = new System.Windows.Media.SolidColorBrush(c);
                ColourPartialFill.Fill = PrestoCoverageCore.Colour_CoveredPartial;

                GeneralSettings.Default.Glyph_PartialCoverColour = c.ToString();
                GeneralSettings.Default.Save();

                PrestoCoverageCore.reloadTaggers();
            }
            catch (System.Exception)
            {
            }
        }

        private void TextBox_Uncovered_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                var c = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(ColourUncovered.Text);

                PrestoCoverageCore.Colour_Uncovered = new System.Windows.Media.SolidColorBrush(c);
                ColourUncoveredFill.Fill = PrestoCoverageCore.Colour_Uncovered;

                GeneralSettings.Default.Glyph_UncoveredColour = c.ToString();
                GeneralSettings.Default.Save();

                PrestoCoverageCore.reloadTaggers();
            }
            catch (System.Exception)
            {
            }

        }


        private void CheckBox_ClearCoverageOnChange_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            GeneralSettings.Default.ClearCoverageOnChange = ClearCoverageOnChangeCheckbox.IsChecked.Value;
            GeneralSettings.Default.Save();

            PrestoCoverageCore.ClearCoverageOnChange = ClearCoverageOnChangeCheckbox.IsChecked.Value;
        }

        private void WatchFolderPath_TextChanged(object sender, TextChangedEventArgs e)
        {
            GeneralSettings.Default.WatchFolderPath = WatchFolderPath.Text;
            GeneralSettings.Default.Save();
        }

        private void WatchFolderFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            GeneralSettings.Default.WatchFolderFilter = WatchFolderFilter.Text;
            GeneralSettings.Default.Save();
        }


    }
}
