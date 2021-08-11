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
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Controls.Primitives;

namespace Wcat_GUI
{
    /// <summary>
    /// PageItem.xaml 的互動邏輯
    /// </summary>
    public partial class PageItem
    {
        private class ItemItemSetting
        {
            public bool? ItemFilterNormalChecked;
            public bool? ItemFilterTimeChecked;
            public bool? ItemFilterBrickChecked;
            public bool? ItemFilterBagChecked;
            public bool? ItemFilterBookChecked;
        }

        public void CloseItemItemAction()
        {
            Directory.CreateDirectory("config");
            File.WriteAllText("config/ItemItemSetting.dat", JsonConvert.SerializeObject(new ItemItemSetting()
            {
                ItemFilterNormalChecked = ItemFilterNormal.IsChecked,
                ItemFilterTimeChecked = ItemFilterTime.IsChecked,
                ItemFilterBrickChecked = ItemFilterBrick.IsChecked,
                ItemFilterBagChecked = ItemFilterBag.IsChecked,
                ItemFilterBookChecked = ItemFilterBook.IsChecked,
            }));
        }

        private void RestoreItemItemSetting()
        {
            if (File.Exists("config/ItemItemSetting.dat"))
            {
                var setting = JsonConvert.DeserializeObject<ItemItemSetting>(File.ReadAllText("config/ItemItemSetting.dat"));
                if (setting != null)
                {
                    ItemFilterNormal.IsChecked = setting.ItemFilterNormalChecked ?? false;
                    ItemFilterTime.IsChecked = setting.ItemFilterTimeChecked ?? false;
                    ItemFilterBrick.IsChecked = setting.ItemFilterBrickChecked ?? false;
                    ItemFilterBag.IsChecked = setting.ItemFilterBagChecked ?? false;
                    ItemFilterBook.IsChecked = setting.ItemFilterBookChecked ?? false;
                }
            }
        }
        private void AItemUseClick(object sender, RoutedEventArgs e)
        {

        }

        private void ItemItemBtnSelectClick(object sender, RoutedEventArgs e)
        {

        }
    }
}
