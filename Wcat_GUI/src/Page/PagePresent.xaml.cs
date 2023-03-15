using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Wcat.PageAction;
using Wcat.Stream;
using Wcat_GUI.Exceptions;

namespace Wcat_GUI
{
    /// <summary>
    /// PagePresent.xaml 的互動邏輯
    /// </summary>
    public partial class PagePresent : UserControl
    {
        private class Setting
        {
            public bool? getAllChecked;
            public bool? getNormalWeaponChecked;
            public bool? getCombineWeaponChecked;
            public bool? getExchangeWeaponChecked;
            public bool? getAccessoryChecked;
            public bool? getLevelRuneChecked;
            public bool? getWeaponRuneChecked;
            public bool? getSpecialRuneChecked;
            public bool? getOtherRuneChecked;
            public bool? getTransferBeadChecked;
            public bool? getGemChecked;
            public bool? getCoinChecked;
            public bool? getSoulChecked;
            public bool? getUnitChecked;
            public bool? getBuildChecked;
            public bool? getFragmentChecked;
            public bool? getStickerChecked;
            public bool? getChenghaoChecked;
            public bool? getOtherChecked;
            public bool? getRainbowruneChecked;

            public bool? sellAllChecked;
            public bool? sellNormalWeaponChecked;
            public bool? sellCombineWeaponChecked;
            public bool? sellExchangeWeaponChecked;
            public bool? sellAccessoryChecked;
            public bool? sellLevelRuneChecked;
            public bool? sellWeaponRuneChecked;
            public bool? sellOtherRuneChecked;
        }

        private ExceptionHandler mainHandler;
        private static CustomWriter mainWriter;

        public PagePresent()
        {
            InitializeComponent();
            RestoreSetting();
            Buffers.pagePresentMsgs.CollectionChanged += OnCollectionChanged;

            mainWriter = new CustomWriter(terminal);
            mainHandler = new ExceptionHandler(mainWriter);
        }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (string str in e.NewItems)
                {
                    mainWriter.WriteLine($"{str}");
                    Buffers.pagePresentMsgs.Remove(str);
                }
            }
        }

        public void CloseAction()
        {
            Directory.CreateDirectory("config");
            File.WriteAllText("config/PresentSetting.dat", JsonConvert.SerializeObject(new Setting()
            {
                getAllChecked = getAll.IsChecked,
                getNormalWeaponChecked = getNormalWeapon.IsChecked,
                getCombineWeaponChecked = getCombineWeapon.IsChecked,
                getExchangeWeaponChecked = getExchangeWeapon.IsChecked,
                getAccessoryChecked = getAccessory.IsChecked,
                getLevelRuneChecked = getLevelRune.IsChecked,
                getWeaponRuneChecked = getWeaponRune.IsChecked,
                getSpecialRuneChecked = getSpecialRune.IsChecked,
                getOtherRuneChecked = getOtherRune.IsChecked,
                getTransferBeadChecked = getTransferBead.IsChecked,
                getGemChecked = getGem.IsChecked,
                getCoinChecked = getCoin.IsChecked,
                getSoulChecked = getSoul.IsChecked,
                getUnitChecked = getUnit.IsChecked,
                getBuildChecked = getBuild.IsChecked,
                getFragmentChecked = getFragment.IsChecked,
                getStickerChecked = getSticker.IsChecked,
                getChenghaoChecked = getChenghao.IsChecked,
                getOtherChecked = getOther.IsChecked,
                getRainbowruneChecked = getRainbowrune.IsChecked,

                sellAllChecked = sellAll.IsChecked,
                sellNormalWeaponChecked = sellNormalWeapon.IsChecked,
                sellCombineWeaponChecked = sellCombineWeapon.IsChecked,
                sellExchangeWeaponChecked = sellExchangeWeapon.IsChecked,
                sellAccessoryChecked = sellAccessory.IsChecked,
                sellLevelRuneChecked = sellLevelRune.IsChecked,
                sellWeaponRuneChecked = sellWeaponRune.IsChecked,
                sellOtherRuneChecked = sellOtherRune.IsChecked
            }));
        }

        private void RestoreSetting()
        {
            if (File.Exists("config/PresentSetting.dat"))
            {
                var setting = JsonConvert.DeserializeObject<Setting>(File.ReadAllText("config/PresentSetting.dat"));
                if (setting != null)
                {
                    getAll.IsChecked = setting.getAllChecked ?? false;
                    getNormalWeapon.IsChecked = setting.getNormalWeaponChecked ?? false;
                    getCombineWeapon.IsChecked = setting.getCombineWeaponChecked ?? false;
                    getExchangeWeapon.IsChecked = setting.getExchangeWeaponChecked ?? false;
                    getAccessory.IsChecked = setting.getAccessoryChecked ?? false;
                    getLevelRune.IsChecked = setting.getLevelRuneChecked ?? false;
                    getWeaponRune.IsChecked = setting.getWeaponRuneChecked ?? false;
                    getSpecialRune.IsChecked = setting.getSpecialRuneChecked ?? false;
                    getOtherRune.IsChecked = setting.getOtherRuneChecked ?? false;
                    getTransferBead.IsChecked = setting.getTransferBeadChecked ?? false;
                    getGem.IsChecked = setting.getGemChecked ?? false;
                    getCoin.IsChecked = setting.getCoinChecked ?? false;
                    getSoul.IsChecked = setting.getSoulChecked ?? false;
                    getUnit.IsChecked = setting.getUnitChecked ?? false;
                    getBuild.IsChecked = setting.getBuildChecked ?? false;
                    getFragment.IsChecked = setting.getFragmentChecked ?? false;
                    getSticker.IsChecked = setting.getStickerChecked ?? false;
                    getChenghao.IsChecked = setting.getChenghaoChecked ?? false;
                    getOther.IsChecked = setting.getOtherChecked ?? false;
                    getRainbowrune.IsChecked = setting.getRainbowruneChecked ?? false;

                    sellAll.IsChecked = setting.sellAllChecked ?? false;
                    sellNormalWeapon.IsChecked = setting.sellNormalWeaponChecked ?? false;
                    sellCombineWeapon.IsChecked = setting.sellCombineWeaponChecked ?? false;
                    sellExchangeWeapon.IsChecked = setting.sellExchangeWeaponChecked ?? false;
                    sellAccessory.IsChecked = setting.sellAccessoryChecked ?? false;
                    sellLevelRune.IsChecked = setting.sellLevelRuneChecked ?? false;
                    sellWeaponRune.IsChecked = setting.sellWeaponRuneChecked ?? false;
                    sellOtherRune.IsChecked = setting.sellOtherRuneChecked ?? false;
                }
            }
        }

        private void CheckAllReceivePanel(object sender, RoutedEventArgs e)
        {
            foreach (var ch in receviePanel.Children)
            {
                if (ch is Grid)
                {
                    foreach (CheckBox cb in ((Grid)ch).Children)
                    {
                        cb.IsChecked = true;
                    }
                }
            }
        }

        private void UnCheckReceivePanelAction(object sender, RoutedEventArgs e)
        {
            getAll.IsChecked = false;
        }

        private void CheckAllSellPanel(object sender, RoutedEventArgs e)
        {
            foreach (var ch in sellPanel.Children)
            {
                if (ch is Grid)
                {
                    foreach (CheckBox cb in ((Grid)ch).Children)
                    {
                        cb.IsChecked = true;
                    }
                }
            }
        }

        private List<bool> GetPanelChecked(StackPanel panel)
        {
            var checkedList = new List<bool>();
            foreach (var ch in panel.Children)
            {
                if (ch is Grid)
                {
                    foreach (CheckBox cb in ((Grid)ch).Children)
                    {
                        if (cb == getAll || cb == sellAll) continue;

                        checkedList.Add(cb.IsChecked ?? false);
                    }
                }
            }
            return checkedList;
        }

        private void UnCheckSellPanelAction(object sender, RoutedEventArgs e)
        {
            sellAll.IsChecked = false;
        }

        private void DisableReceivePanel(object sender, MouseButtonEventArgs e)
        {
            foreach (var ch in receviePanel.Children)
            {
                if (ch is Grid)
                {
                    Grid gd = ch as Grid;
                    gd.IsEnabled = !gd.IsEnabled;
                }
            }
        }

        private void DisableSellPanel(object sender, MouseButtonEventArgs e)
        {
            foreach (var ch in sellPanel.Children)
            {
                if (ch is Grid)
                {
                    Grid gd = ch as Grid;
                    gd.IsEnabled = !gd.IsEnabled;
                }
            }
        }

        Thread presentThread;

        private void BtnStartClick(object sender, EventArgs e)
        {
            List<bool> receiveChecked = GetPanelChecked(receviePanel);
            List<bool> sellChecked = GetPanelChecked(sellPanel);
            bool willReceive = getAll.IsEnabled && receiveChecked.Contains(true);
            bool willSell = sellAll.IsEnabled && sellChecked.Contains(true);

            if (!willReceive && !willSell) return;

            btnStart.IsEnabled = false;
            btnEnd.IsEnabled = true;

            presentThread = new Thread(() =>
            {
                mainHandler.GlobalTryCatch(() =>
                {
                    if (willReceive)
                    {
                        PagePresentAction.StartReceivePresents(false, receiveChecked.ToArray());
                    }

                    if (willSell)
                    {
                        PagePresentAction.StartReceivePresents(true, sellChecked.ToArray());
                    }
                });

                Dispatcher.Invoke(() =>
                {
                    btnStart.IsEnabled = true;
                    btnEnd.IsEnabled = false;
                });
            });
            presentThread.Start();
        }

        private void BtnEndClick(object sender, EventArgs e)
        {
            presentThread?.Interrupt();
        }

        private void terminal_TextChanged(object sender, TextChangedEventArgs e)
        {
            ((TextBox)sender).ScrollToEnd();
        }

        public static void PresentTerminals_Clear()
        {
            mainWriter.Clear();

        }
    }
}
