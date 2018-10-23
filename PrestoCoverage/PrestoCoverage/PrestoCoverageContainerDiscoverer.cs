using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.TestWindow.Extensibility;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;

namespace PrestoCoverage
{

    [Export(typeof(ITestContainerDiscoverer))]
    [Export(typeof(PrestoCoverageContainerDiscoverer))]
    internal class PrestoCoverageContainerDiscoverer : ITestContainerDiscoverer
    {
        private readonly IServiceProvider _serviceProvider;

        [ImportingConstructor]
        internal PrestoCoverageContainerDiscoverer([Import(typeof(SVsServiceProvider))]IServiceProvider serviceProvider, [Import(typeof(IOperationState))]IOperationState operationState)
        {
            _serviceProvider = serviceProvider;
            operationState.StateChanged += OperationState_StateChanged;
        }

        public Uri ExecutorUri => new Uri("executor://PrestoCoverageExecutor/v1");

        public IEnumerable<ITestContainer> TestContainers => Enumerable.Empty<ITestContainer>();

        public event EventHandler TestContainersUpdated;

        private void OperationState_StateChanged(object sender, OperationStateChangedEventArgs e)
        {
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
                    PrestoCoverageCore.OnChangeDetected(this, e);
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
                    PrestoCoverageCore.OnTestExecutionStarting(this, e);
                    break;

                case TestOperationStates.TestExecutionStarted:
                    break;
                case TestOperationStates.TestExecutionCanceling:
                    break;

                case TestOperationStates.TestExecutionFinished:
                    PrestoCoverageCore.OnTestExecutionFinished(this, e);
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
