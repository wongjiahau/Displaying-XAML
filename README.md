
# Displaying-XAML
This library is for display the XAML code of theme library for WPF (e.g. MaterialDesignInXamlToolkit/MahApps)

## Why is this library written ?
Because the demo app for the theme library I'm using ([MaterialDesignInXamlToolkit](https://github.com/ButchersBoy/MaterialDesignInXamlToolkit)) is too hard to use.   
For example, when I want to use a control in the demo, I have to search through its GitHub repo for the code. Obviously, this is a pain, therefore I wrote this library, so that the code can be displayed besides each control.

## Demo
![newdemo](https://user-images.githubusercontent.com/23183656/30123252-f3db094c-9363-11e7-9ae8-911789b6ae08.gif)

## How to use this library ? 
### 1. Install from nuget 
`Install-Package XamlDisplayerPackage -Version 1.0.2 `

### 2. Call the XamlDisplayerPanel initializer at the app startup 
```C#
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
```
- `source` determine where you want to load the XAML source file (either from local(your computer) or remote(e.g. GitHub)
- `defaultLocalPath` determine where the files shall be loaded by default if you set `source` as `LoadFromLocal`
- `defaultRemotePath` determine where the files shall be loaded by default if you set `source` as `LoadFromRemote`
- `attributesToBeRemoved` specifies the attributes that should be filetered out when displaying the code for each control
_Refer [here](https://msdn.microsoft.com/en-us/library/system.windows.application.startup(v=vs.110).aspx) to learn more about App_OnStartup method_


### 3. At the place where all the pages are hosted (usually MainWindow.xaml), place in the XamlDisplayerHost
```xml
<Window x:Class="DisplayXamlDemo.MainWindow">    
        <codeDisplayer:XamlDisplayerHost x:Name="XamlDisplayerHost"/>        
</Window>
```
The XamlDisplayerHost is actually a Frame, so you can call the `Navigate` method using it.

### 4. For each of the page that is going to be navigated, surround all the controls with XamlDisplayerPanel
```xml
<UserControl x:Class="XamlDisplayerDemo.Page1">  
    <codeDisplayer:XamlDisplayerPanel>
        <!--All the controls here-->
    </codeDisplayer:XamlDisplayerPanel>                    
</UserControl>     
```
_Note : The XamlDisplayerPanel can be placed on any level of depth_

### 5. Navigate to each pages by calling Navigate method of XamlDisplayerHost
```C#
        public MainWindow() {
            InitializeComponent();
            XamlDisplayerHost.Navigate(new Page1());
        }
```
_Actually, you can also use binding to bind its `Content`, because `XamlDisplayerHost` is inherited directly from `Frame`_


