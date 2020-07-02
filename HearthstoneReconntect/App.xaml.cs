using System;
using System.Globalization;
using System.Windows;

namespace HearthstoneReconntect
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void AppStartup(object sender, StartupEventArgs e)
        {
            Application app = Current;
            app.StartupUri = new Uri("MainWindow.xaml", UriKind.RelativeOrAbsolute);
            LoadLanguage();
        }

        private void LoadLanguage()
        {
            CultureInfo cultureInfo = CultureInfo.CurrentCulture;
            ResourceDictionary langDict;
            try
            {
                langDict = LoadComponent(new Uri(string.Format(@"Resources\Language\{0}.xaml", cultureInfo.TwoLetterISOLanguageName), UriKind.Relative)) as ResourceDictionary;
                //langDict = LoadComponent(new Uri(@"Resources\Language\en.xaml", UriKind.Relative)) as ResourceDictionary;
            }
            catch 
            {
                langDict = LoadComponent(new Uri(@"Resources\Language\en.xaml", UriKind.Relative)) as ResourceDictionary;
            }
            if (langDict != null)
            {
                if (Resources.MergedDictionaries.Count > 0)
                {
                    Resources.MergedDictionaries.Clear();
                }
                Resources.MergedDictionaries.Add(langDict);
            }
        }
    }
}
