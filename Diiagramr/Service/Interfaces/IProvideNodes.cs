using Diiagramr.Model;
using Diiagramr.ViewModel.Diagram;
using System.Collections.Generic;
using Diiagramr.Service.Interfaces;

namespace Diiagramr.Service
{
    public interface IProvideNodes
    {
        IProjectManager ProjectManager { get; set; }

        void RegisterNode(AbstractNodeViewModel node);

        AbstractNodeViewModel LoadNodeViewModelFromNode(NodeModel node);

        AbstractNodeViewModel CreateNodeViewModelFromName(string typeFullName);

        IEnumerable<AbstractNodeViewModel> GetRegisteredNodes();
    }
}
