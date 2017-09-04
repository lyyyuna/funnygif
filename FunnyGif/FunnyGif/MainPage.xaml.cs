using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Popups;
using Microsoft.Toolkit.Uwp.UI.Controls;
using Microsoft.Toolkit.Uwp.UI;

// https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x804 上介绍了“空白页”项模板

namespace FunnyGif
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private string currentLabel = "";

        public MainPage()
        {
            this.InitializeComponent();

            contentFrame.Navigate(typeof(Views.Album));
            currentLabel = "搞笑内涵图";
            ImageCache.Instance.CacheDuration = TimeSpan.FromHours(48);
            ImageCache.Instance.MaxMemoryCacheCount = 200;
        }

        private void HamburgerMenu_OnItemClick(object sender, ItemClickEventArgs e)
        {
            var menuItem = e.ClickedItem as HamburgerMenuItem;

            if (menuItem.Label == "搞笑内涵图" && currentLabel != "搞笑内涵图")
            {
                contentFrame.Navigate(typeof(Views.Album));
                currentLabel = "搞笑内涵图";
            }
            if (menuItem.Label == "美图欣赏" && currentLabel != "美图欣赏")
            {
                contentFrame.Navigate(typeof(Views.Gank));
                currentLabel = "美图欣赏";
            }
        }

        private void HamburgerMenu_OnOptionsItemClick(object sender, ItemClickEventArgs e)
        {
            contentFrame.Navigate(typeof(Views.About));
        }
    }
}
