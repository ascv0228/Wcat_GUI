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
    /// PageCity.xaml 的互動邏輯
    /// </summary>
    public partial class PageCity
    {
        private Thread CatShopThread;

        private void CatShopBtnEndClick(object sender, RoutedEventArgs e)
        {
            CatShopThread?.Interrupt();
        }
        private void CatShopListBtnClick(object sender, RoutedEventArgs e)
        {
            if (CatShopThread?.IsAlive ?? false)
            {
                return;
            }
            else
            {
                CatShopWriter.WriteLine("開始顯示所有貓鋪");
                /**************************************/
                bool Filter = CatShopFilter_OnlyShowCanExchange.IsChecked ?? false;
                CatShopAllListbtn.IsEnabled = false;
                /**************************************/

                CatShopThread = new Thread(() =>
                {
                    CatShopHandler.GlobalTryCatch(() =>
                    {
                        CatShopAction.CatShopAllInfo(Filter);
                    });

                    Dispatcher.Invoke(() =>
                    {
                        CatShopAllListbtn.IsEnabled = true;
                        CatShopWriter.WriteLine("顯示所有貓鋪 結束");
                    });

                });
                CatShopThread.Start();
            }
        }
    }
}