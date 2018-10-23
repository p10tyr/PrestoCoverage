using Microsoft.VisualStudio.Shell;
using PrestoCoverage.Options;
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows;

namespace PrestoCoverage
{
    [ClassInterface(ClassInterfaceType.AutoDual)]
    [ComVisible(true)]
    [Guid("1D9ECCF3-5D2F-4112-9B25-264596873DC9")]
    public class OptionsDialogPage : UIElementDialogPage
    {
        private OptionsDialogPageControl optionsDialogControl;

        protected override UIElement Child
        {
            get { return optionsDialogControl ?? (optionsDialogControl = new OptionsDialogPageControl()); }
        }

        protected override void OnActivate(CancelEventArgs e)
        {
            base.OnActivate(e);

            //var encouraments = GetEncouragements();
            //optionsDialogControl.Encouragements = string.Join(Environment.NewLine, encouraments.AllEncouragements);
        }

        protected override void OnApply(PageApplyEventArgs args)
        {
            //if (args.ApplyBehavior == ApplyKind.Apply)
            //{
            //    string[] userEncouragments = optionsDialogControl.Encouragements.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            //    GetEncouragements().AllEncouragements = userEncouragments;
            //}

            base.OnApply(args);
        }

        //private IEncouragements GetEncouragements()
        //{
        //    var componentModel = (IComponentModel)(Site.GetService(typeof(SComponentModel)));
        //    return componentModel.DefaultExportProvider.GetExportedValue<IEncouragements>();
        //}
    }
}
