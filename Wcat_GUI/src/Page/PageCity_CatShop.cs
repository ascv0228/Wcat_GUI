using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using Wcat;
using Wcat.PageAction;
using Wcat.Stream;
using Wcat_GUI.Exceptions;
using System.Collections.Generic;
using Wcat.Action;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Controls.Primitives;

namespace Wcat_GUI
{
    /// <summary>
    /// PageCity.xaml ªº¤¬°ÊÅÞ¿è
    /// </summary>
    public partial class PageCity
    {
        private Thread CatShopThread;

        private void CatShopBtnEndClick(object sender, RoutedEventArgs e)
        {
            CatShopThread?.Interrupt();
        }
    }
}