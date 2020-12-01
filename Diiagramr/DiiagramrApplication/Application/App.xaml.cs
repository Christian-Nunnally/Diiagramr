using System;
using System.Windows;

namespace DiiagramrApplication.Application
{
    public partial class App : System.Windows.Application
    {
        public ResourceDictionary ThemeDictionary => Resources.MergedDictionaries[0];

        public void ChangeTheme(Uri uri)
        {
            ThemeDictionary.MergedDictionaries.Clear();
            ThemeDictionary.MergedDictionaries.Add(new ResourceDictionary() { Source = uri });
        }
    }
}