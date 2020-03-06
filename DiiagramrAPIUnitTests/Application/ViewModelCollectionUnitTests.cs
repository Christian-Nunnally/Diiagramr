using DiiagramrAPI.Application;
using DiiagramrModel;
using System.Collections.ObjectModel;
using Xunit;

namespace DiiagramrAPIUnitTests
{
    public class ViewModelCollectionUnitTests : UnitTest
    {
        [Fact]
        public void TopLevelMenuWithOneChild_OpenContextMenuForTopLevelMenu_ContextMenuOpensWithCommand()
        {
            var fakeViewModel = new FakeViewModel();
            var fakeModel = new FakeModel();
            var viewModelCollection = new ViewModelCollection<FakeViewModel, FakeModel>(
                fakeViewModel,
                () => fakeViewModel.Models,
                m => new FakeViewModel());

            fakeViewModel.Models.Add(fakeModel);
        }

        private class FakeViewModel : ViewModel
        {
            public FakeViewModel(FakeModel model)
            {
            }

            public ObservableCollection<FakeModel> Models { get; set; }
        }

        private class FakeModel : ModelBase
        {
        }
    }
}