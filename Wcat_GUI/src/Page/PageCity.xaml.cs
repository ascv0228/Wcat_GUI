
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

namespace Wcat_GUI
{
    /// <summary>
    /// PageCity.xaml 的互動邏輯
    /// </summary>
    public partial class PageCity : UserControl
    {
        private ExceptionHandler MissionHandler;
        private static CustomWriter MissionWriter;

        public PageCity()
        {
            InitializeComponent();

            MissionWriter = new CustomWriter(MissionTerminal);
            MissionHandler = new ExceptionHandler(MissionWriter);


            Buffers.pageCityMissionMsgs.CollectionChanged += OnCollectionChanged_Mission;
        }


        private void OnCollectionChanged_Mission(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (string str in e.NewItems)
                {
                    MissionWriter.WriteLine($"{str}");
                    Buffers.pageCityMissionMsgs.Remove(str);
                }
            }
        }

        public static void CityTerminals_Clear()
        {
            MissionWriter.Clear();
            //ItemWeaponWriter.Clear();
            //ItemItemWriter.Clear();
            //ItemAccessoryWriter.Clear();
            //ItemFragmentWriter.Clear();
        }

        private void terminal_TextChanged(object sender, TextChangedEventArgs e)
        {
            ((TextBox)sender).ScrollToEnd();
        }

        private void PanelClose2(Button Btn, Grid Panel1, Grid Panel2)
        {
            Btn.IsEnabled = true;
            Btn.Background = Brushes.SkyBlue;
            Panel1.Visibility = Visibility.Hidden;
            Panel2.Visibility = Visibility.Hidden;
        }
        private void PanelOpen2(Button Btn, Grid Panel1, Grid Panel2)
        {
            Btn.IsEnabled = false;
            Btn.Background = Brushes.DodgerBlue;
            Panel1.Visibility = Visibility.Visible;
            Panel2.Visibility = Visibility.Visible;
        }

        private void MissionBtnClick(object sender, System.Windows.RoutedEventArgs e)
        {
            PanelOpen2(MissionBtn, MissionPanel1, MissionPanel2);
            PanelClose2(BuildingBtn, BuildingPanel1, BuildingPanel2);
            PanelClose2(ShopBtn, ShopPanel1, ShopPanel2);
            PanelClose2(CatShopBtn, CatShopPanel1, CatShopPanel2);
        }

        private void BuildingBtnClick(object sender, System.Windows.RoutedEventArgs e)
        {
            PanelOpen2(BuildingBtn, BuildingPanel1, BuildingPanel2);
            PanelClose2(MissionBtn, MissionPanel1, MissionPanel2);
            PanelClose2(ShopBtn, ShopPanel1, ShopPanel2);
            PanelClose2(CatShopBtn, CatShopPanel1, CatShopPanel2);
        }

        private void ShopBtnClick(object sender, System.Windows.RoutedEventArgs e)
        {
            PanelOpen2(ShopBtn, ShopPanel1, ShopPanel2);
            PanelClose2(MissionBtn, MissionPanel1, MissionPanel2);
            PanelClose2(BuildingBtn, BuildingPanel1, BuildingPanel2);
            PanelClose2(CatShopBtn, CatShopPanel1, CatShopPanel2);
        }

        private void CatShopBtnClick(object sender, System.Windows.RoutedEventArgs e)
        {
            PanelOpen2(CatShopBtn, CatShopPanel1, CatShopPanel2);
            PanelClose2(MissionBtn, MissionPanel1, MissionPanel2);
            PanelClose2(BuildingBtn, BuildingPanel1, BuildingPanel2);
            PanelClose2(ShopBtn, ShopPanel1, ShopPanel2);
        }
    }
}
