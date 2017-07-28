using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace WpfApplication1 {
    public class Control_Definition {
        public Control_Definition(string xaml) {
            Control = GetControl(xaml);
            XAML = xaml;
        }
        private Control GetControl(string rawXaml) {
            string namespaces =
                " xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml' xmlns:materialDesign='http://materialdesigninxaml.net/winfx/xaml/themes' ";
            rawXaml = rawXaml.Insert(rawXaml.IndexOf(' ') , namespaces);
            MemoryStream stream = new MemoryStream(ASCIIEncoding.UTF8.GetBytes(rawXaml));
            return (Control)XamlReader.Load(stream);

        }

        public string XAML { get; set; }
        public Control Control { get; set; }
    }
}
