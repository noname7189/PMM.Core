using PMM.Core.EntityClass;
using System;

namespace PMM.Core.Interface
{
    public interface ISignalGenerator<S> where S : BaseSignal
    {
        public List<S> GetOnlineSignals();
        public List<S> GenerateSignalsDuringSystemOff(int startIndex);
        public Task FinalizeFinishedSignals(List<S> finishedSignals);
        public List<S> TryTerminateResidualSignals();
        public S? AddOnlineSignal();

        public void InitializeGeneratedSignals(List<S> generatedSignals);
    }
}
