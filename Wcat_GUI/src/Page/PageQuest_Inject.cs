using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Text.RegularExpressions;
using Wcat.Action;
using Wcat.PageAction;
using Wcat.Stream;
using Wcat_GUI.Exceptions;

namespace Wcat_GUI
{
    /// <summary>
    /// PageQuest.xaml 的互動邏輯
    /// </summary>
    public partial class PageQuest : UserControl
    {

        public static List<PageInjectionAction.ItemMap> injectItems = new List<PageInjectionAction.ItemMap>();

        private CustomWriter powerWriter;
        private CustomWriter infoWriter;

        public static PageInjectionAction.Power injectInfo = new PageInjectionAction.Power()
        {
            coinPower = 800000,
            soulPower = 80000,
            itemPower = 1,
            destroyMonsterPower = 1,
            spTreasurePower = 1,
            passiveItemPower = 1,
            weaponPower = 1,
            ornamentPower = 1,
            paramBoxPower = 1,
            cardPower = 1,
            randomDropAccessoryGroupPower = 1
        };

        public void CloseInjectAction()
        {
            Directory.CreateDirectory("config");
            File.WriteAllText("config/InjectionSetting.dat", JsonConvert.SerializeObject(injectInfo));
        }

        private void RestoreInjectSetting()
        {
            if (File.Exists("config/InjectionSetting.dat"))
            {
                var setting = JsonConvert.DeserializeObject<PageInjectionAction.Power>(File.ReadAllText("config/InjectionSetting.dat"));
                if (setting != null)
                {
                    injectInfo = setting;
                }
            }
        }

        private void ShowPowerInfo()
        {
            powerWriter.Clear();
            powerWriter.WriteLine($"金幣數量: {injectInfo.coinPower}");
            powerWriter.WriteLine($"魂石數量: {injectInfo.soulPower}");
            powerWriter.WriteLine($"符文倍率: {injectInfo.itemPower}");
            powerWriter.WriteLine($"殺敵倍率: {injectInfo.destroyMonsterPower}");
            powerWriter.WriteLine($"Treasure倍率: {injectInfo.spTreasurePower}");
            powerWriter.WriteLine($"PassiveItem倍率: {injectInfo.passiveItemPower}");
            powerWriter.WriteLine($"武器倍率: {injectInfo.weaponPower}");
            powerWriter.WriteLine($"裝飾物倍率: {injectInfo.ornamentPower}");
            powerWriter.WriteLine($"卡片倍率: {injectInfo.cardPower}");
            powerWriter.WriteLine($"隨機飾品倍率: {injectInfo.randomDropAccessoryGroupPower}");
            powerWriter.WriteLine($"ParamBox倍率: {injectInfo.paramBoxPower}");
        }

        private void BtnApply_InjectClick(object sender, RoutedEventArgs e)
        {
            try
            {
                string[] powers = InjectpowerText.Text.Split(",");
                for (int i = 0; i < powers.Length; ++i)
                {
                    if (powers[i] == "") continue;

                    switch (i)
                    {
                        case 0:
                            injectInfo.coinPower = Math.Min(int.Parse(powers[i]), 800000);
                            break;
                        case 1:
                            injectInfo.soulPower = Math.Min(int.Parse(powers[i]), 80000);
                            break;
                        case 2:
                            injectInfo.itemPower = Math.Min(int.Parse(powers[i]), 1000);
                            break;
                        case 3:
                            injectInfo.destroyMonsterPower = Math.Min(int.Parse(powers[i]), 999);
                            break;
                        case 4:
                            injectInfo.spTreasurePower = Math.Min(int.Parse(powers[i]), 1000);
                            break;
                        case 5:
                            injectInfo.passiveItemPower = Math.Min(int.Parse(powers[i]), 1000);
                            break;
                        case 6:
                            injectInfo.weaponPower = Math.Min(int.Parse(powers[i]), 100);
                            break;
                        case 7:
                            injectInfo.ornamentPower = Math.Min(int.Parse(powers[i]), 100);
                            break;
                        case 8:
                            injectInfo.cardPower = Math.Min(int.Parse(powers[i]), 100);
                            break;
                        case 9:
                            injectInfo.randomDropAccessoryGroupPower = Math.Min(int.Parse(powers[i]), 100);
                            break;
                        case 10:
                            injectInfo.paramBoxPower = Math.Min(int.Parse(powers[i]), 1000);
                            break;
                    }
                }
                ShowPowerInfo();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private void OnPowerPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = Regex.IsMatch(e.Text, "[^0-9,]+");
        }

        private void OnItemPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            InjectitemList.IsDropDownOpen = true;
        }

        public void ShowSelectedInjectItems()
        {
            if (injectItems == null) return;

            string s = "";
            foreach (var item in injectItems)
            {
                s += $"{item.info}\n";
            }
            InjectitemTerminal.Text = s;
        }

        private void OnItemTextChanged(object sender, TextChangedEventArgs e)
        {
            var sb = (TextBox)sender;
            var viewSource = InjectitemList.Items;
            if (String.IsNullOrEmpty(sb.Text))
            {
                if (viewSource.Filter != null) viewSource.Filter = null;
                return;
            }
            else
            {
                viewSource.Filter = (x =>
                {
                    var item = ((PageInjectionAction.ItemMap)x);
                    if (item.info.ToLower().Contains(sb.Text.ToLower())) return true;
                    else return false;
                });
            }
        }

        private void BtnRemoveInjectItemClick(object sender, RoutedEventArgs e)
        {
            if (InjectitemList.SelectedItem == null) return;

            var selected = (PageInjectionAction.ItemMap)InjectitemList.SelectedItem;
            PageInjectionAction.ItemMap im;
            if ((im = injectItems?.FirstOrDefault(x => x.id == selected.id && x.type == selected.type)) != null)
            {
                injectItems.Remove(im);
                infoWriter.WriteLine($"移除: {selected.name} - {selected.type} - {selected.id}");
                ShowSelectedInjectItems();
            }
        }

        private void BtnAddInjectItemClick(object sender, RoutedEventArgs e)
        {
            if (InjectitemList.SelectedItem == null) return;
            injectItems ??= new List<PageInjectionAction.ItemMap>();
            var selected = (PageInjectionAction.ItemMap)InjectitemList.SelectedItem;
            PageInjectionAction.ItemMap im;

            if ((im = injectItems.FirstOrDefault(x => x.id == selected.id && x.type == selected.type)) == null)
            {
                injectItems.Add(selected);
                infoWriter.WriteLine($"添加: {selected.name} - {selected.type} - {selected.id}");
                ShowSelectedInjectItems();
            }
        }

        private void InjectToggleButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if ((sender as ToggleButton).IsChecked ?? false)
            {
                InjectinfoTerminal.Visibility = Visibility.Visible;
                InjectitemTerminal.Visibility = Visibility.Hidden;
                InjectterminalTitle.Content = "注入訊息";
            }
            else
            {
                InjectitemTerminal.Visibility = Visibility.Visible;
                InjectinfoTerminal.Visibility = Visibility.Hidden;
                InjectterminalTitle.Content = "已選道具";
            }
        }

        private void BtnClearInjectItemClick(object sender, RoutedEventArgs e)
        {
            PageQuest.injectItems.Clear();
            this.ShowSelectedInjectItems();
        }

        private void BtnSaveInjectItemClick(object sender, RoutedEventArgs e)
        {
            Directory.CreateDirectory("config");
            File.WriteAllText("config/InjectionData.dat", JsonConvert.SerializeObject(injectItems, Formatting.Indented));
        }

        private void BtnLoadInjectItemClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if (File.Exists("config/InjectionData.dat"))
                {
                    List<PageInjectionAction.ItemMap> list = JsonConvert.DeserializeObject<List<PageInjectionAction.ItemMap>>(File.ReadAllText("config/InjectionData.dat"));
                    if (list != null)
                    {
                        injectItems = list;
                        this.ShowSelectedInjectItems();
                        this.infoWriter.WriteLine("載入成功!");
                        return;
                    }
                }
                this.infoWriter.WriteLine("載入失敗!");
            }
            catch (Exception value)
            {
                this.infoWriter.WriteLine("載入時發生錯誤!");
                Console.WriteLine(value);
            }
        }
    }
}
