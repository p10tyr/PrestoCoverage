using System.Collections.Generic;
using System.Windows.Controls;

namespace PrestoCoverage.Options
{
    /// <summary>
    /// Interaction logic for OptionsDialogPageControl.xaml
    /// </summary>
    public partial class OptionsDialogPageControl : UserControl
    {
        public List<System.Drawing.Color> Colours = new List<System.Drawing.Color>();

        public OptionsDialogPageControl()
        {
            InitializeComponent();

            ColourCovered.Text = GeneralSettings.Default.Glyph_CoveredColour;
            ColourPartialCovered.Text = GeneralSettings.Default.Glyph_PartialCoverColour;
            ColourUncovered.Text = GeneralSettings.Default.Glyph_UncoveredColour;

            ClearCoverageOnChangeCheckbox.IsChecked = GeneralSettings.Default.ClearCoverageOnChange;

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
    }
}
