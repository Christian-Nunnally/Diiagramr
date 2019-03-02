using DiiagramrAPI.Model;
using Stylet;
using System;
using System.Windows;

namespace DiiagramrAPI.Service.Interfaces
{
    public interface IScreenDisplayService : IService
    {
        void ShowScreen(IScreen screen);
    }
}
