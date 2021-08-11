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


namespace Wcat_GUI
{
    /// <summary>
    /// PageItem.xaml 的互動邏輯
    /// </summary>
    public partial class PageItem : UserControl
    {
        private Thread ItemCardThread;
        private class ItemCardSetting
        {
            public bool? ItemCardLv150FilterChecked;
            public bool? ItemCardskillFilterChecked;
            public bool? ItemCardExceedFilterChecked;
            public bool? ItemCardnoCCFilterChecked;
            public bool? ItemCardyesCCFilterChecked;
            public bool? ItemCardUnlockFilterChecked;
            public bool? ItemCardLockFilterChecked;
            public bool? ItemCardUnlockEXFilterChecked;
        }

        public void CloseItemCardAction()
        {
            Directory.CreateDirectory("config");
            File.WriteAllText("config/ItemCardSetting.dat", JsonConvert.SerializeObject(new ItemCardSetting()
            {
                ItemCardLv150FilterChecked = ItemCardLv150Filter.IsChecked,
                ItemCardskillFilterChecked = ItemCardskillFilter.IsChecked,
                ItemCardExceedFilterChecked = ItemCardExceedFilter.IsChecked,
                ItemCardnoCCFilterChecked = ItemCardnoCCFilter.IsChecked,
                ItemCardyesCCFilterChecked = ItemCardyesCCFilter.IsChecked,
                ItemCardUnlockFilterChecked = ItemCardUnlockFilter.IsChecked,
                ItemCardLockFilterChecked = ItemCardLockFilter.IsChecked,
                ItemCardUnlockEXFilterChecked = ItemCardUnlockEXFilter.IsChecked,
            }));
        }

        private void RestoreItemCardSetting()
        {
            if (File.Exists("config/ItemCardSetting.dat"))
            {
                var setting = JsonConvert.DeserializeObject<ItemCardSetting>(File.ReadAllText("config/ItemCardSetting.dat"));
                if (setting != null)
                {
                    ItemCardLv150Filter.IsChecked = setting.ItemCardLv150FilterChecked ?? false;
                    ItemCardskillFilter.IsChecked = setting.ItemCardskillFilterChecked ?? false;
                    ItemCardExceedFilter.IsChecked = setting.ItemCardExceedFilterChecked ?? false;
                    ItemCardnoCCFilter.IsChecked = setting.ItemCardnoCCFilterChecked ?? false;
                    ItemCardyesCCFilter.IsChecked = setting.ItemCardyesCCFilterChecked ?? false;
                    ItemCardUnlockFilter.IsChecked = setting.ItemCardUnlockFilterChecked ?? false;
                    ItemCardLockFilter.IsChecked = setting.ItemCardLockFilterChecked ?? false;
                    ItemCardUnlockEXFilter.IsChecked = setting.ItemCardUnlockEXFilterChecked ?? false;
                }
            }
        }

        public void InitDeckList()
        {
            if (cards?.Count == 0)
            {
                var defaultCard = new DeckShareData.Card()
                {
                    cardInfo = "不選擇",
                    cId = -1
                };

                cards.Add(defaultCard);
            }

            if (UserData.unitToCard?.Count > 0)
            {
                foreach (var card in UserData.unitToCard.Values)
                {
                    bool isSet = false;
                    for (int i = 0; i < cards.Count; ++i)
                    {
                        if (cards[i].ucId == card.ucId)
                        {
                            cards[i] = card;
                            isSet = true;
                            break;
                        }
                    }
                    if (!isSet) cards.Add(card);
                }
            }
        }

        public List<bool?> GetItemCardFilterList()
        {
            var re = new List<bool?>();
            re.Add(ItemCardLv150Filter.IsChecked);
            re.Add(ItemCardskillFilter.IsChecked);
            re.Add(ItemCardExceedFilter.IsChecked);
            re.Add(ItemCardnoCCFilter.IsChecked);
            re.Add(ItemCardyesCCFilter.IsChecked);
            re.Add(ItemCardUnlockFilter.IsChecked);
            re.Add(ItemCardLockFilter.IsChecked);
            re.Add(ItemCardUnlockEXFilter.IsChecked);
            return re;
        }

        private void GetItemCardFilters_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            CheckBox cb = sender as CheckBox;
            foreach(var tmp in GetItemCardFilterList())
            {
                Buffers.pageItemCardMsgs.Add($"{tmp}");
            }
        }

        // btnEndAction 
        private void AutoLevelUp100Click(object sender, EventArgs e)
        {
            // ItemCardWriter.WriteLine(autoLevelUp100.Background.ToString());
            if ($"{autoLevelUp100.Background}" == "#FFCBA9E5")
            {
                ItemCardWriter.WriteLine("停止升級全角色至LV100");
                ItemCardThread?.Interrupt();
            }
            else if (ItemCardThread?.IsAlive ?? false)
            {
                return;
            }
            else if (autoLevelUp100.Background.ToString() == "#FF2196F3")
            {
                ItemCardWriter.WriteLine("開始升級全角色至LV100");
                /**************************************/
                LockAllItemBtn("ItemCard");
                autoLevelUp100.IsEnabled = true;
                autoLevelUp100.Background = (Brush)new BrushConverter().ConvertFromString("#FFCBA9E5");
                autoLevelUp100.Content = "停止";
                /**************************************/

                ItemCardThread = new Thread(() =>
                {
                    ItemCardHandler.GlobalTryCatch(() =>
                    {
                        if (AllItemAction.ItemCards == null)
                        {
                            AllItemAction.SetCardWeaponList();
                        }
                        Buffers.pageHomeMsgs.Add("REFRESH_DECK_LIST");

                        foreach (var card in AllItemAction.ItemCards)
                        {
                            if (card.lv >= 100) continue;

                            try
                            {
                                var flag = CardAction.LearnSkillsToMax(card.ucId);

                                if (flag)
                                {
                                    ItemCardWriter.WriteLine($"升級{card.name}成功");
                                }
                                else
                                {
                                    ItemCardWriter.WriteLine($"升級{card.name}失敗");
                                }
                            }
                            catch (NullReferenceException)
                            {
                                ItemCardWriter.WriteLine($"升級{card.name}時發生錯誤");
                            }
                            catch (ArgumentNullException)
                            {
                                ItemCardWriter.WriteLine($"升級{card.name}時發生錯誤");
                            }
                        }
                    });

                    Dispatcher.Invoke(() =>
                    {
                        UnLockAllItemBtn();
                        autoLevelUp100.Background = (Brush)new BrushConverter().ConvertFromString("#FF2196F3");
                        autoLevelUp100.Content = "升級100";
                    });
                });

                ItemCardThread.Start();
            }
            else
            {
                ItemCardWriter.WriteLine($"這是{autoLevelUp100.Background}");
            }
        }

        private void AutoLevelUp150Click(object sender, EventArgs e)
        {
            // ItemCardWriter.WriteLine(autoLevelUp150.Background.ToString());
            if ($"{autoLevelUp150.Background}" == "#FFCBA9E5")
            {
                ItemCardWriter.WriteLine("停止升級全角色至LV150");
                ItemCardThread?.Interrupt();
            }
            else if (ItemCardThread?.IsAlive ?? false)
            {
                return;
            }
            else if (autoLevelUp150.Background.ToString() == "#FF2196F3")
            {
                ItemCardWriter.WriteLine("開始升級全角色至LV150");
                /**************************************/
                LockAllItemBtn("ItemCard");
                autoLevelUp150.IsEnabled = true;
                autoLevelUp150.Background = (Brush)new BrushConverter().ConvertFromString("#FFCBA9E5");
                autoLevelUp150.Content = "停止";
                /**************************************/

                ItemCardThread = new Thread(() =>
                {
                    ItemCardHandler.GlobalTryCatch(() =>
                    {
                        if (AllItemAction.ItemCards == null)
                        {
                            AllItemAction.SetCardWeaponList();
                        }
                        foreach (var card in AllItemAction.ItemCards)
                        {
                            if (card.lv < 100 || card.exceedMaxCnt != 8 || card.lv >= 150) continue;

                            try
                            {
                                var flag = CardAction.CardHighLevelUPToMax(card.ucId);
                                if (!flag)
                                {
                                    var presents = PresentAction.GetPresentListById(0, PresentListRequestData.Param.otherRunes);
                                    PresentAction.ReceivePresent(presents);
                                    flag = CardAction.CardHighLevelUPToMax(card.ucId);
                                }

                                if (flag)
                                {
                                    ItemCardWriter.WriteLine($"升級{card.name}到150成功");
                                }
                                else
                                {
                                    ItemCardWriter.WriteLine($"升級{card.name}到150失敗");
                                }
                            }
                            catch (NullReferenceException)
                            {
                                ItemCardWriter.WriteLine($"升級{card.name}時發生錯誤");
                            }
                            catch (ArgumentNullException)
                            {
                                ItemCardWriter.WriteLine($"升級{card.name}時發生錯誤");
                            }
                        }
                    });

                    Dispatcher.Invoke(() =>
                    {
                        UnLockAllItemBtn();
                        autoLevelUp150.Background = (Brush)new BrushConverter().ConvertFromString("#FF2196F3");
                        autoLevelUp150.Content = "升級150";
                    });
                });

                ItemCardThread.Start();
            }
            else
            {
                ItemCardWriter.WriteLine($"這是{autoLevelUp150.Background}");
            }
        }

        private void AutoTalkReadClick(object sender, EventArgs e)
        {
            //ItemCardWriter.WriteLine(autoTalkRead.Background.ToString());
            if ($"{autoTalkRead.Background}" == "#FFCBA9E5")
            {
                ItemCardWriter.WriteLine("停止閱讀劇情");
                ItemCardThread?.Interrupt();
            }
            else if (ItemCardThread?.IsAlive ?? false)
            {
                return;
            }
            else if (autoTalkRead.Background.ToString() == "#FF2196F3")
            {
                ItemCardWriter.WriteLine("開始閱讀劇情");
                /**************************************/
                LockAllItemBtn("ItemCard");
                autoTalkRead.IsEnabled = true;
                autoTalkRead.Background = (Brush)new BrushConverter().ConvertFromString("#FFCBA9E5");
                autoTalkRead.Content = "停止";
                /**************************************/

                ItemCardThread = new Thread(() =>
                {
                    ItemCardHandler.GlobalTryCatch(() =>
                    {
                        if (AllItemAction.ItemCards == null)
                        {
                            AllItemAction.SetCardWeaponList();
                        }
                        foreach (var card in AllItemAction.ItemCards)
                        {

                            if (card.loveLevel != 5 || (card.eventStep == 6 && card.talkEventName7Status == null) || card.eventStep == 8) continue;

                            try
                            {
                                if (CardAction.CardTalkEventToMax(card.ucId))
                                {
                                    card.talkEventName0Status = 2;
                                    card.talkEventName1Status = 2;
                                    card.talkEventName2Status = 2;
                                    card.talkEventName3Status = 2;
                                    card.talkEventName4Status = 2;
                                    card.talkEventName5Status = 2;
                                    if (card.talkEventName7Status != null)
                                    {
                                        card.talkEventName6Status = 2;
                                        card.talkEventName7Status = 2;
                                    }
                                }
                                
                            }
                            catch (NullReferenceException)
                            {
                                ItemCardWriter.WriteLine($"完成{card.name}時發生錯誤");
                            }
                            catch (ArgumentNullException)
                            {
                                ItemCardWriter.WriteLine($"完成{card.name}時發生錯誤");
                            }
                        }
                    });

                    Dispatcher.Invoke(() =>
                    {
                        UnLockAllItemBtn();
                        autoTalkRead.Background = (Brush)new BrushConverter().ConvertFromString("#FF2196F3");
                        autoTalkRead.Content = "閱讀劇情";
                    });
                });

                ItemCardThread.Start();
            }
            else
            {
                ItemCardWriter.WriteLine($"這是{autoTalkRead.Background}");
            }
        }

        private void AutoExceedClick(object sender, EventArgs e)
        {
            // ItemCardWriter.WriteLine(autoExceed.Background.ToString());
            if ($"{autoExceed.Background}" == "#FFCBA9E5")
            {
                ItemCardWriter.WriteLine("停止全角色突破界限");
                ItemCardThread?.Interrupt();
            }
            else if (ItemCardThread?.IsAlive ?? false)
            {
                return;
            }
            else if (autoExceed.Background.ToString() == "#FF2196F3")
            {
                ItemCardWriter.WriteLine("開始全角色突破界限");
                /**************************************/
                LockAllItemBtn("ItemCard");
                autoExceed.IsEnabled = true;
                autoExceed.Background = (Brush)new BrushConverter().ConvertFromString("#FFCBA9E5");
                autoExceed.Content = "停止";
                /**************************************/

                ItemCardThread = new Thread(() =>
                {
                    ItemCardHandler.GlobalTryCatch(() =>
                    {
                        if (AllItemAction.ItemCards == null)
                        {
                            AllItemAction.SetCardWeaponList();
                        }

                        foreach (var card in AllItemAction.ItemCards)
                        {
                            if (card.heroFlag != 0 || card.exceedCnt >= card.exceedMaxCnt) continue;

                            try
                            {
                                var flag = CardAction.CardExceedToMax(card.ucId);

                                if (flag)
                                {
                                    ItemCardWriter.WriteLine($"{card.name}突破界限成功");
                                }
                                else
                                {
                                    ItemCardWriter.WriteLine($"{card.name}突破界限失敗");
                                }
                            }
                            catch (NullReferenceException)
                            {
                                ItemCardWriter.WriteLine($"{card.name}突破界限時發生錯誤");
                            }
                            catch (ArgumentNullException)
                            {
                                ItemCardWriter.WriteLine($"{card.name}突破界限時發生錯誤");
                            }
                        }
                    });

                    Dispatcher.Invoke(() =>
                    {
                        UnLockAllItemBtn();
                        autoExceed.Background = (Brush)new BrushConverter().ConvertFromString("#FF2196F3");
                        autoExceed.Content = "突破界限";
                    });
                });

                ItemCardThread.Start();
            }
            else
            {
                ItemCardWriter.WriteLine($"這是{autoExceed.Background}");
            }
        }

        private void AutoSkillLevelUpClick(object sender, EventArgs e)
        {            // ItemCardWriter.WriteLine(autoSkillLevelUp.Background.ToString());
            if ($"{autoSkillLevelUp.Background}" == "#FFCBA9E5")
            {
                ItemCardWriter.WriteLine("停止全角色技能覺醒");
                ItemCardThread?.Interrupt();
            }
            else if (ItemCardThread?.IsAlive ?? false)
            {
                return;
            }
            else if (autoSkillLevelUp.Background.ToString() == "#FF2196F3")
            {
                ItemCardWriter.WriteLine("開始全角色技能覺醒");
                /**************************************/
                LockAllItemBtn("ItemCard");
                autoSkillLevelUp.IsEnabled = true;
                autoSkillLevelUp.Background = (Brush)new BrushConverter().ConvertFromString("#FFCBA9E5");
                autoSkillLevelUp.Content = "停止";
                /**************************************/

                ItemCardThread = new Thread(() =>
                {
                    ItemCardHandler.GlobalTryCatch(() =>
                    {
                        if (AllItemAction.ItemCards == null)
                        {
                            AllItemAction.SetCardWeaponList();
                        }

                        foreach (var card in AllItemAction.ItemCards)
                        {
                            if (card.actionSkillUpgradeData == null || card.skillUpgradeStatus == 0) continue;

                            try
                            {
                                var flag = CardAction.SkillUpgradeLvUpByUcId(card.ucId);
                            }
                            catch (NullReferenceException)
                            {
                                ItemCardWriter.WriteLine($"{card.name}技能覺醒時發生錯誤");
                            }
                            catch (ArgumentNullException)
                            {
                                ItemCardWriter.WriteLine($"{card.name}技能覺醒時發生錯誤");
                            }
                        }
                    });

                    Dispatcher.Invoke(() =>
                    {
                        UnLockAllItemBtn();
                        autoSkillLevelUp.Background = (Brush)new BrushConverter().ConvertFromString("#FF2196F3");
                        autoSkillLevelUp.Content = "技能覺醒";
                    });
                });

                ItemCardThread.Start();
            }
            else
            {
                ItemCardWriter.WriteLine($"這是{autoSkillLevelUp.Background}");
            }
        }
        private void AutoTransferClick(object sender, EventArgs e)
        {
            // ItemCardWriter.WriteLine(autoTransfer.Background.ToString());
            if ($"{autoTransfer.Background}" == "#FFCBA9E5")
            {
                ItemCardWriter.WriteLine("停止自動轉職");
                ItemCardThread?.Interrupt();
            }
            else if (ItemCardThread?.IsAlive ?? false)
            {
                return;
            }
            else if (autoTransfer.Background.ToString() == "#FF2196F3")
            {
                ItemCardWriter.WriteLine("開始自動轉職");
                /**************************************/
                LockAllItemBtn("ItemCard");
                autoTransfer.IsEnabled = true;
                autoTransfer.Background = (Brush)new BrushConverter().ConvertFromString("#FFCBA9E5");
                autoTransfer.Content = "停止";
                /**************************************/

                ItemCardThread = new Thread(() =>
                {
                    ItemCardHandler.GlobalTryCatch(() =>
                    {
                        AllItemAction.SetCardWeaponList();

                        foreach (var card in AllItemAction.ItemCards)
                        {
                            if (card.advancedJobInfo.levelUpItems == null || card.lv != 100) continue;

                            try
                            {
                                var presents = PresentAction.GetPresentListById(0, PresentListRequestData.Param.soul);
                                PresentAction.ReceivePresent(presents);
                                presents = PresentAction.GetPresentListById(0, PresentListRequestData.Param.transferBeads);
                                PresentAction.ReceivePresent(presents);

                                var flag = CardAction.AdvancedJob(card.ucId);

                                if (flag)
                                {
                                    ItemCardWriter.WriteLine($"轉職{card.name}成功");
                                }
                                else
                                {
                                    ItemCardWriter.WriteLine($"轉職{card.name}失敗");
                                }
                            }
                            catch (NullReferenceException)
                            {
                                ItemCardWriter.WriteLine($"{card.name}轉職時發生錯誤");
                            }
                            catch (ArgumentNullException)
                            {
                                ItemCardWriter.WriteLine($"{card.name}轉職時發生錯誤");
                            }
                        }
                    });

                    Dispatcher.Invoke(() =>
                    {
                        UnLockAllItemBtn();
                        autoTransfer.Background = (Brush)new BrushConverter().ConvertFromString("#FF2196F3");
                        autoTransfer.Content = "自動轉職";
                    });
                });

                ItemCardThread.Start();
            }
            else
            {
                ItemCardWriter.WriteLine($"這是{autoTransfer.Background}");
            }
        }
        private void AutoUnlockEXClick(object sender, EventArgs e)
        {
            // ItemCardWriter.WriteLine(autoUnlockEX.Background.ToString());
            if ($"{autoUnlockEX.Background}" == "#FFCBA9E5")
            {
                ItemCardWriter.WriteLine("停止自動解鎖EX星盤");
                ItemCardThread?.Interrupt();
            }
            else if (ItemCardThread?.IsAlive ?? false)
            {
                return;
            }
            else if (autoUnlockEX.Background.ToString() == "#FF2196F3")
            {
                ItemCardWriter.WriteLine("開始自動解鎖EX星盤");
                /**************************************/
                LockAllItemBtn("ItemCard");
                autoUnlockEX.IsEnabled = true;
                autoUnlockEX.Background = (Brush)new BrushConverter().ConvertFromString("#FFCBA9E5");
                autoUnlockEX.Content = "停止";
                /**************************************/

                ItemCardThread = new Thread(() =>
                {
                    ItemCardHandler.GlobalTryCatch(() =>
                    {

                        if (AllItemAction.ItemCards == null)
                        {
                            AllItemAction.SetCardWeaponList();
                        }
                        foreach (var card in AllItemAction.ItemCards)
                        {
                            if (card.fSlot2 != null && card.fSlot3 != null && card.fSlot4 != null) continue;

                            try
                            {
                                var flag = CardAction.CardUnlockExFragments(card.ucId);

                                if (flag)
                                {
                                    ItemCardWriter.WriteLine($"解鎖{card.name}的EX星盤成功");

                                }
                                else
                                {
                                    ItemCardWriter.WriteLine($"解鎖{card.name}的EX星盤失敗");
                                }
                            }
                            catch (NullReferenceException)
                            {
                                ItemCardWriter.WriteLine($"{card.name}解鎖EX星盤時發生錯誤");
                            }
                            catch (ArgumentNullException)
                            {
                                ItemCardWriter.WriteLine($"{card.name}解鎖EX星盤時發生錯誤");
                            }
                        }
                    });

                    Dispatcher.Invoke(() =>
                    {
                        UnLockAllItemBtn();
                        autoUnlockEX.Background = (Brush)new BrushConverter().ConvertFromString("#FF2196F3");
                        autoUnlockEX.Content = "EX解鎖星盤";
                        AllItemAction.SetCardWeaponList();
                    });
                });

                ItemCardThread.Start();
            }
            else
            {
                ItemCardWriter.WriteLine($"這是{autoUnlockEX.Background}");
            }
        }
        private void AutoLoveUpToMaxClick(object sender, EventArgs e)
        {
        }
        private void ACardLevelUp100Click(object sender, EventArgs e)
        {
        }
        private void ACardLevelUp150Click(object sender, EventArgs e)
        {
        }
        private void ACardExceedClick(object sender, EventArgs e)
        {
        }
        private void ACardUnlockEXClick(object sender, EventArgs e)
        {
        }
        private void ACardTransferClick(object sender, EventArgs e)
        {
        }

        private void ACardLockClick(object sender, RoutedEventArgs e)
        {

        }
        private void ACardUnLockClick(object sender, RoutedEventArgs e)
        {

        }
        private void ACardExceedClick(object sender, RoutedEventArgs e)
        {

        }
        private void ItemCardBtnSelectClick(object sender, RoutedEventArgs e)
        {

        }
    }
}
