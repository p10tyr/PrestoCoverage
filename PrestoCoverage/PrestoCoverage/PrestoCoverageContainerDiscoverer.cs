using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.TestWindow.Extensibility;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace PrestoCoverage
{



    //[Export(typeof(IRunSettingsConfigurationInfo))]
    //[Export(typeof(PrestoCoverageContainerDiscovererer))]
    //internal class PrestoCoverageContainerDiscovererer
    //{

    //}


    [Export(typeof(ITestContainerDiscoverer))]
    [Export(typeof(PrestoCoverageContainerDiscoverer))]
    internal class PrestoCoverageContainerDiscoverer : ITestContainerDiscoverer
    {
        private readonly IServiceProvider _serviceProvider;

        [ImportingConstructor]
        internal PrestoCoverageContainerDiscoverer(
            [Import(typeof(SVsServiceProvider))]IServiceProvider serviceProvider,
            [Import(typeof(IOperationState))]IOperationState operationState)
        {
            _serviceProvider = serviceProvider;
            operationState.StateChanged += OperationState_StateChanged;

            //var componentModel = (IComponentModel)_serviceProvider.GetService(typeof(SComponentModel));
            //var _TestsService = componentModel.GetService<Microsoft.VisualStudio.TestWindow.Extensibility.ITestsService>();
            //
            //var GetTestTask = _TestsService.GetTests();


        }

        public Uri ExecutorUri => new Uri("executor://PrestoCoverageExecutor/v1");

        public IEnumerable<ITestContainer> TestContainers => null;

        public event EventHandler TestContainersUpdated;

        private void OperationState_StateChanged(object sender, OperationStateChangedEventArgs e)
        {

            //var componentModel = (IComponentModel)_serviceProvider.GetService(typeof(SComponentModel));
            //var _TestsService = componentModel.GetService<Microsoft.VisualStudio.TestWindow.Extensibility.ITestsService>();

            switch (e.State)
            {
                case TestOperationStates.None:
                    break;
                case TestOperationStates.Starting:
                    break;
                case TestOperationStates.Started:
                    break;
                case TestOperationStates.Cancel:
                    break;
                case TestOperationStates.Canceling:
                    break;
                case TestOperationStates.Finished:
                    break;
                case TestOperationStates.Canceled:
                    break;
                case TestOperationStates.Discovery:
                    break;
                case TestOperationStates.TestExecution:
                    break;
                case TestOperationStates.ChangeDetection:
                    break;
                case TestOperationStates.PlaylistRefresh:
                    break;
                case TestOperationStates.ChangeDetectionStarting:
                    break;
                case TestOperationStates.ChangeDetectionFinished:
                    break;
                case TestOperationStates.DiscoveryStarting:
                    break;
                case TestOperationStates.DiscoveryStarted:
                    break;
                case TestOperationStates.DiscoveryFinished:
                    break;
                case TestOperationStates.DiscoveryCanceled:
                    break;
                case TestOperationStates.TestExecutionStarting:
                    Settings.OnTestExecutionStarting(this, e);
                    break;
                case TestOperationStates.TestExecutionStarted:
                    break;
                case TestOperationStates.TestExecutionCanceling:
                    break;
                case TestOperationStates.TestExecutionFinished:
                    Settings.OnTestExecutionFinished(this, e);
                    break;
                case TestOperationStates.TestExecutionCancelAndFinished:

                    break;
                case TestOperationStates.PlaylistRefreshStarting:
                    break;
                case TestOperationStates.PlaylistRefreshFinished:
                    break;
                case TestOperationStates.OperationSetStarted:
                    break;
                case TestOperationStates.OperationSetFinished:
                    break;
                default:
                    break;
            }

        }
    }
}
