using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.TestWindow.Extensibility;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;

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

        public IEnumerable<ITestContainer> TestContainers => null;

        public event EventHandler TestContainersUpdated;

        private void OperationState_StateChanged(object sender, OperationStateChangedEventArgs e)
        {
            if (e.State == TestOperationStates.TestExecutionFinished)
            {
                Settings.OnTestExecutionFinished(this, e);
            }
        }
    }
}
