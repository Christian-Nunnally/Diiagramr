using Diiagramr.Model;
using Diiagramr.ViewModel.Diagram;
using System.Collections.Generic;

namespace Diiagramr.Service
{
    public interface IProvideNodes
    {
        void RegisterNode(AbstractNodeViewModel node);

        AbstractNodeViewModel LoadNodeViewModelFromNode(NodeModel node);

        AbstractNodeViewModel CreateNodeViewModelFromName(string typeFullName);

        IEnumerable<AbstractNodeViewModel> GetRegisteredNodes();
    }
}
