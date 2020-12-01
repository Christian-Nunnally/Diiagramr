using DiiagramrAPI.Application;
using DiiagramrModel;
using System;

namespace DiiagramrAPIUnitTests.SharedFakes
{
    public class FakeViewModel : ViewModel<FakeModel>, IUserInputBeforeClosedRequest
    {
        public event Action<Action> ContinueIfCanCloseAction;

        public void ContinueIfCanClose(Action continuation)
        {
            ContinueIfCanCloseAction?.Invoke(continuation);
        }
    }

    public class FakeModel : ModelBase
    {
    }
}