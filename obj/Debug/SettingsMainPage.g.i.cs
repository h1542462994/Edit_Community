﻿#pragma checksum "..\..\SettingsMainPage.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "96E045229D672CBECC33D88995EA5C91"
//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.42000
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

using Edit_Community;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;
using User.UI;


namespace Edit_Community {
    
    
    /// <summary>
    /// SettingsMainPage
    /// </summary>
    public partial class SettingsMainPage : System.Windows.Controls.Page, System.Windows.Markup.IComponentConnector {
        
        
        #line 19 "..\..\SettingsMainPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal User.UI.UImageMenu UImgTheme;
        
        #line default
        #line hidden
        
        
        #line 20 "..\..\SettingsMainPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal User.UI.UImageMenu UImgExtension;
        
        #line default
        #line hidden
        
        
        #line 21 "..\..\SettingsMainPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal User.UI.UImageMenu UImgHelp;
        
        #line default
        #line hidden
        
        
        #line 22 "..\..\SettingsMainPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal User.UI.UImageMenu UImgDeveloper;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/Edit Community;component/settingsmainpage.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\SettingsMainPage.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.UImgTheme = ((User.UI.UImageMenu)(target));
            
            #line 19 "..\..\SettingsMainPage.xaml"
            this.UImgTheme.Tapped += new System.Windows.RoutedEventHandler(this.UImageMenu_Tapped);
            
            #line default
            #line hidden
            return;
            case 2:
            this.UImgExtension = ((User.UI.UImageMenu)(target));
            
            #line 20 "..\..\SettingsMainPage.xaml"
            this.UImgExtension.Tapped += new System.Windows.RoutedEventHandler(this.UImageMenu_Tapped);
            
            #line default
            #line hidden
            return;
            case 3:
            this.UImgHelp = ((User.UI.UImageMenu)(target));
            
            #line 21 "..\..\SettingsMainPage.xaml"
            this.UImgHelp.Tapped += new System.Windows.RoutedEventHandler(this.UImageMenu_Tapped);
            
            #line default
            #line hidden
            return;
            case 4:
            this.UImgDeveloper = ((User.UI.UImageMenu)(target));
            
            #line 22 "..\..\SettingsMainPage.xaml"
            this.UImgDeveloper.Tapped += new System.Windows.RoutedEventHandler(this.UImageMenu_Tapped);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}
