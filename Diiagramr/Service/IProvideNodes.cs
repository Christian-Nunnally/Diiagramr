using Diiagramr.Model;
using Diiagramr.ViewModel.Diagram;
using System.Collections.Generic;

namespace Diiagramr.Service
{
    public interface IProvideNodes
    {
        void RegisterNode(AbstractNodeViewModel abstractNodeViewModel);

        AbstractNodeViewModel LoadNodeViewModelFromNode(DiagramNode node);

        AbstractNodeViewModel CreateNodeViewModelFromName(string name);

        IEnumerable<AbstractNodeViewModel> GetRegisteredNodes();
    }
}
