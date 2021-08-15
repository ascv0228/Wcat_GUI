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
    /// PageItem.xaml 的互動邏輯
    /// </summary>
    public static class DictionaryExtensions
    {
        public static bool AddOrUpdate(this Dictionary<string, List<string>> targetDictionary, string key, string entry)
        {
            if (!targetDictionary.ContainsKey(key))
            {
                targetDictionary.Add(key, new List<string>());
            }
            else if (targetDictionary[key].Contains(entry))
            {
                return false;
            }
            targetDictionary[key].Add(entry);
            return true;
        }
    }
    public partial class PageItem : UserControl
    {
        private Thread AllItemThread;
        public Dictionary<string, List<string>> ChangedDict = new Dictionary<string, List<string>>();
        public ICollectionView cardsView1 { get; set; }
        public ObservableCollection<DeckShareData.Card> cards { get; set; }
        public ObservableCollection<WeaponShareData.Weapon> weapons { get; set; }
        private ExceptionHandler ItemCardHandler;
        private ExceptionHandler ItemWeaponHandler;
        private ExceptionHandler ItemItemHandler;
        private ExceptionHandler ItemAccessoryHandler;
        private ExceptionHandler ItemFragmentHandler;
        private static CustomWriter ItemCardWriter;
        private static CustomWriter ItemWeaponWriter;
        private static CustomWriter ItemItemWriter;
        private static CustomWriter ItemAccessoryWriter;
        private static CustomWriter ItemFragmentWriter;

        public PageItem()
        {
            cards = new ObservableCollection<DeckShareData.Card>();
            weapons = new ObservableCollection<WeaponShareData.Weapon>();

            InitDeckList();
            InitWeaponList();
            InitializeComponent();
            RestoreItemSetting();


            ItemCardWriter = new CustomWriter(ItemCardterminal);
            ItemCardHandler = new ExceptionHandler(ItemCardWriter);
            ItemWeaponWriter = new CustomWriter(ItemWeaponterminal);
            ItemWeaponHandler = new ExceptionHandler(ItemWeaponWriter);
            ItemItemWriter = new CustomWriter(ItemItemterminal);
            ItemItemHandler = new ExceptionHandler(ItemItemWriter);
            ItemAccessoryWriter = new CustomWriter(ItemAccessoryterminal);
            ItemAccessoryHandler = new ExceptionHandler(ItemAccessoryWriter);
            ItemFragmentWriter = new CustomWriter(ItemFragmentterminal);
            ItemFragmentHandler = new ExceptionHandler(ItemFragmentWriter);

            Buffers.pageItemCardMsgs.CollectionChanged += OnCollectionChanged_Card;
            Buffers.pageItemWeaponMsgs.CollectionChanged += OnCollectionChanged_Weapon;
            Buffers.pageItemItemMsgs.CollectionChanged += OnCollectionChanged_Item;
            Buffers.pageItemAccessoryMsgs.CollectionChanged += OnCollectionChanged_Accessory;
            Buffers.pageItemFragmentMsgs.CollectionChanged += OnCollectionChanged_Fragment;

            cardsView1 = new CollectionViewSource() { Source = cards }.View;
        }
        public void RestoreItemSetting()
        {
            RestoreItemCardSetting();
            RestoreItemWeaponSetting();
            RestoreItemItemSetting();
            RestoreItemAccessorySetting();
            RestoreItemFragmentSetting();
        }
        public void CloseItemAction()
        {
            CloseItemCardAction();
            CloseItemWeaponAction();
            CloseItemItemAction();
            CloseItemAccessoryAction();
            CloseItemFragmentAction();
        }
        public static void ItemTerminals_Clear()
        {
            ItemCardWriter.Clear();
            ItemWeaponWriter.Clear();
            ItemItemWriter.Clear();
            ItemAccessoryWriter.Clear();
            ItemFragmentWriter.Clear();
        }
        private void AAAClick(object sender, EventArgs e)
        {
        }


        private void OnTextChanged1(object sender, TextChangedEventArgs e)
        {
        }
        private void OnPreviewTextInput1(object sender, TextCompositionEventArgs e)
        {

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

        private void CardBtnClick(object sender, System.Windows.RoutedEventArgs e)
        {
            PanelOpen2(cardBtn, CardPanel1, CardPanel2);
            PanelClose2(weaponBtn, weaponPanel1, weaponPanel2);
            PanelClose2(itemBtn, itemPanel1, itemPanel2);
            PanelClose2(accessoryBtn, accessoryPanel1, accessoryPanel2);
            PanelClose2(fragmentBtn, fragmentPanel1, fragmentPanel2);
        }

        private void WeaponBtnClick(object sender, RoutedEventArgs e)
        {
            PanelClose2(cardBtn, CardPanel1, CardPanel2);
            PanelOpen2(weaponBtn, weaponPanel1, weaponPanel2);
            PanelClose2(itemBtn, itemPanel1, itemPanel2);
            PanelClose2(accessoryBtn, accessoryPanel1, accessoryPanel2);
            PanelClose2(fragmentBtn, fragmentPanel1, fragmentPanel2);
        }

        private void ItemBtnClick(object sender, RoutedEventArgs e)
        {
            PanelClose2(cardBtn, CardPanel1, CardPanel2);
            PanelClose2(weaponBtn, weaponPanel1, weaponPanel2);
            PanelOpen2(itemBtn, itemPanel1, itemPanel2);
            PanelClose2(accessoryBtn, accessoryPanel1, accessoryPanel2);
            PanelClose2(fragmentBtn, fragmentPanel1, fragmentPanel2);
        }
        private void AccessoryBtnClick(object sender, RoutedEventArgs e)
        {
            PanelClose2(cardBtn, CardPanel1, CardPanel2);
            PanelClose2(weaponBtn, weaponPanel1, weaponPanel2);
            PanelClose2(itemBtn, itemPanel1, itemPanel2);
            PanelOpen2(accessoryBtn, accessoryPanel1, accessoryPanel2);
            PanelClose2(fragmentBtn, fragmentPanel1, fragmentPanel2);
        }
        private void FragmentBtnClick(object sender, RoutedEventArgs e)
        {
            PanelClose2(cardBtn, CardPanel1, CardPanel2);
            PanelClose2(weaponBtn, weaponPanel1, weaponPanel2);
            PanelClose2(itemBtn, itemPanel1, itemPanel2);
            PanelClose2(accessoryBtn, accessoryPanel1, accessoryPanel2);
            PanelOpen2(fragmentBtn, fragmentPanel1, fragmentPanel2);
        }
        private void OnCollectionChanged_Card(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (string str in e.NewItems)
                {
                    ItemCardWriter.WriteLine($"{str}");
                    Buffers.pageItemCardMsgs.Remove(str);
                }
            }
        }
        private void OnCollectionChanged_Weapon(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (string str in e.NewItems)
                {
                    ItemWeaponWriter.WriteLine($"{str}");
                    Buffers.pageItemWeaponMsgs.Remove(str);
                }
            }
        }
        private void OnCollectionChanged_Item(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (string str in e.NewItems)
                {
                    ItemItemWriter.WriteLine($"{str}");
                    Buffers.pageItemItemMsgs.Remove(str);
                }
            }
        }
        private void OnCollectionChanged_Accessory(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (string str in e.NewItems)
                {
                    ItemAccessoryWriter.WriteLine($"{str}");
                    Buffers.pageItemAccessoryMsgs.Remove(str);
                }
            }
        }
        private void OnCollectionChanged_Fragment(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (string str in e.NewItems)
                {
                    ItemFragmentWriter.WriteLine($"{str}");
                    Buffers.pageItemFragmentMsgs.Remove(str);
                }
            }
        }



        private void PanelClose1(Button Btn, Grid Panel)
        {
            Btn.IsEnabled = true;
            Btn.Background = Brushes.SkyBlue;
            Panel.Visibility = Visibility.Hidden;
        }
        private void PanelOpen1(Button Btn, Grid Panel)
        {
            Btn.IsEnabled = false;
            Btn.Background = Brushes.DodgerBlue;
            Panel.Visibility = Visibility.Visible;
        }

        private void BtnLockChange(Button openBtn, Button closeBtn)
        {
            //if "favorite": true
            closeBtn.Visibility = Visibility.Hidden;
            closeBtn.IsEnabled = false;

            openBtn.Visibility = Visibility.Visible;
            openBtn.IsEnabled = true;
        }

        private void LockAllItemBtn(string Page)
        {
            if (Page == "ItemCard")
            {
                autoLevelUp100.IsEnabled = false;
                autoLevelUp150.IsEnabled = false;
                autoTalkRead.IsEnabled = false;
                autoExceed.IsEnabled = false;
                autoSkillLevelUp.IsEnabled = false;
                autoTransfer.IsEnabled = false;
                autoUnlockEX.IsEnabled = false;
                autoLoveUpToMax.IsEnabled = false;
                ACardLevelUp150.IsEnabled = false;
                ACardExceed.IsEnabled = false;
                ACardUnlockEX.IsEnabled = false;
                ACardTransfer.IsEnabled = false;
            }
            else if (Page == "ItemWeapon")
            {
                autoCombine.IsEnabled = false;
                autoCompoWeapon.IsEnabled = false;
                WeaponExchange.IsEnabled = false;
                WeaponSkillExchange.IsEnabled = false;
                autoLevelUp100.IsEnabled = false;
                AWeaponStorage.IsEnabled = false;
                AWeaponUnstorage.IsEnabled = false;
                AWeaponSkill.IsEnabled = false;
            }

            else if (Page == "ItemItem")
            {
                AItemUse.IsEnabled = false;
            }

            else if (Page == "ItemAccessory")
            {
                AAccessorySell.IsEnabled = false;
            }

            else if (Page == "ItemFragment")
            {
                AFragmentLevelUp.IsEnabled = false;
            }
        }

        private void UnLockAllItemBtn()
        {
            autoLevelUp100.IsEnabled = true;
            autoLevelUp150.IsEnabled = true;
            autoTalkRead.IsEnabled = true;
            autoExceed.IsEnabled = true;
            autoSkillLevelUp.IsEnabled = true;
            autoTransfer.IsEnabled = true;
            autoUnlockEX.IsEnabled = true;
            autoLoveUpToMax.IsEnabled = true;
            ACardLevelUp150.IsEnabled = true;
            ACardExceed.IsEnabled = true;
            ACardUnlockEX.IsEnabled = true;
            ACardTransfer.IsEnabled = true;

            autoCombine.IsEnabled = true;
            autoCompoWeapon.IsEnabled = true;
            WeaponExchange.IsEnabled = true;
            WeaponSkillExchange.IsEnabled = true;
            autoLevelUp100.IsEnabled = true;
            AWeaponStorage.IsEnabled = true;
            AWeaponUnstorage.IsEnabled = true;
            AWeaponSkill.IsEnabled = true;

            AItemUse.IsEnabled = true;

            AAccessorySell.IsEnabled = true;

            AFragmentLevelUp.IsEnabled = true;
        }
        public void CheckInChangedDict(string Key, string Value)
        {
            if (ChangedDict.AddOrUpdate(Key, Value))
                return;
            ChangedDict = new Dictionary<string, List<string>>();
            AllItemAction.SetAllItemList();
        }
        private void ItemBtnRefreshClick(object sender, EventArgs e)
        {
            ItemCardWriter.WriteLine("重置中...");
            ItemWeaponWriter.WriteLine("重置中...");
            ItemItemWriter.WriteLine("重置中...");
            ItemAccessoryWriter.WriteLine("重置中...");
            ItemFragmentWriter.WriteLine("重置中...");
            /**************************************/
            ItembtnRefresh.IsEnabled = false;
            /**************************************/

            AllItemThread = new Thread(() =>
            {
                ItemFragmentHandler.GlobalTryCatch(() =>
                {
                    AllItemAction.SetAllItemList();
                });

                Dispatcher.Invoke(() =>
                {
                    ItemCardWriter.WriteLine("重置完成");
                    ItemWeaponWriter.WriteLine("重置完成");
                    ItemItemWriter.WriteLine("重置完成");
                    ItemAccessoryWriter.WriteLine("重置完成");
                    ItemFragmentWriter.WriteLine("重置完成");
                    ItembtnRefresh.IsEnabled = true;
                });

            });
            AllItemThread.Start();
        }
    }
}
