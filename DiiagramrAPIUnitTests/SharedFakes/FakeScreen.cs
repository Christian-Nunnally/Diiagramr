using DiiagramrAPI.Application;
using System;

namespace DiiagramrAPIUnitTests.SharedFakes
{
    public class FakeViewModel : ViewModel, IUserInputBeforeClosedRequest
    {
        public event Action<Action> ContinueIfCanCloseAction;

        public void ContinueIfCanClose(Action continuation)
        {
            ContinueIfCanCloseAction?.Invoke(continuation);
        }
    }
}