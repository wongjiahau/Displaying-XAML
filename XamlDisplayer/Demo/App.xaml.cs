using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using CodeDisplayer;

namespace DisplayXamlDemo
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void App_OnStartup(object sender, StartupEventArgs e) {
            XamlDisplayerPanel.Initialize(
                source:
                XamlDisplayerPanel.SourceEnum.LoadFromLocal ,
                defaultLocalPath:
                @"C:\Users\User\Source\Repos\Displaying-XAML\XamlDisplayer\Demo\" ,
                defaultRemotePath:
                "https://raw.githubusercontent.com/wongjiahau/Displaying-XAML/master/XamlDisplayer/Demo/" ,
                attributesToBeRemoved:
                new List<string>()
                {
                    "xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\"" ,
                    "xmlns:materialDesign=\"http://materialdesigninxaml.net/winfx/xaml/themes\"" ,
                    "xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\""
                }
            );

        }
    }
}
