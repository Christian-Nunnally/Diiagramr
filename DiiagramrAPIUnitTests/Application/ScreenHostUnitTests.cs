using DiiagramrAPI.Application;
using DiiagramrAPIUnitTests.SharedFakes;
using System;
using Xunit;

namespace DiiagramrAPIUnitTests
{
    public class ScreenHostUnitTests : UnitTest
    {
        [Fact]
        public void ShowScreen_ActiveItemSetToScreen()
        {
            var fakeScreen = new FakeViewModel();
            var screenHost = new ScreenHost();
            screenHost.ShowScreen(fakeScreen);

            Assert.Equal(fakeScreen, screenHost.ActiveItem);
        }

        [Fact]
        public void IUserInputBeforeClosedRequestScreenOpen_ShowScreenAndRunContinuation_ActiveItemSetToScreen()
        {
            var currentlyOpenFakeScreen = new FakeViewModel();
            var fakeScreenToOpen = new FakeViewModel();
            Action continuation = () => { };
            currentlyOpenFakeScreen.ContinueIfCanCloseAction += c => continuation = c;
            var screenHost = new ScreenHost();
            screenHost.ShowScreen(currentlyOpenFakeScreen);

            screenHost.ShowScreen(fakeScreenToOpen);
            continuation();

            Assert.Equal(fakeScreenToOpen, screenHost.ActiveItem);
        }

        [Fact]
        public void IUserInputBeforeClosedRequestScreenOpen_ShowScreen_ActiveItemStillIUserInputBeforeClosedRequestScreen()
        {
            var currentlyOpenFakeScreen = new FakeViewModel();
            var fakeScreenToOpen = new FakeViewModel();
            var screenHost = new ScreenHost();
            screenHost.ShowScreen(currentlyOpenFakeScreen);

            screenHost.ShowScreen(fakeScreenToOpen);

            Assert.Equal(currentlyOpenFakeScreen, screenHost.ActiveItem);
        }

        [Fact]
        public void IUserInputBeforeClosedRequestScreenOpen_InteractivelyCloseAllScreens_ActiveItemStillIUserInputBeforeClosedRequestScreen()
        {
            var currentlyOpenFakeScreen = new FakeViewModel();
            var screenHost = new ScreenHost();
            screenHost.ShowScreen(currentlyOpenFakeScreen);

            screenHost.InteractivelyCloseAllScreens(() => { });

            Assert.Equal(currentlyOpenFakeScreen, screenHost.ActiveItem);
        }

        [Fact]
        public void IUserInputBeforeClosedRequestScreenOpen_InteractivelyCloseAllScreens_ContinuationSetsActiveItemToNull()
        {
            var currentlyOpenFakeScreen = new FakeViewModel();
            var screenHost = new ScreenHost();
            Action continuation = () => { };
            currentlyOpenFakeScreen.ContinueIfCanCloseAction += c => continuation = c;
            screenHost.ShowScreen(currentlyOpenFakeScreen);

            screenHost.InteractivelyCloseAllScreens(() => { });
            continuation();

            Assert.Null(screenHost.ActiveItem);
        }

        [Fact]
        public void IUserInputBeforeClosedRequestScreenOpen_InteractivelyCloseAllScreens_ContinuationCallsContination()
        {
            var currentlyOpenFakeScreen = new FakeViewModel();
            var screenHost = new ScreenHost();
            var continationRun = false;
            Action continuation = () => { };
            currentlyOpenFakeScreen.ContinueIfCanCloseAction += c => continuation = c;
            screenHost.ShowScreen(currentlyOpenFakeScreen);

            screenHost.InteractivelyCloseAllScreens(() => continationRun = true);
            continuation();

            Assert.True(continationRun);
        }
    }
}