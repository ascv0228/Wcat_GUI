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
    public partial class PageItem
    {
        private Thread ItemWeaponThread;
        private class ItemWeaponSetting
        {
            public bool? WeaponFilterStar1Checked;
            public bool? WeaponFilterStar2Checked;
            public bool? WeaponFilterStar3Checked;
            public bool? WeaponFilterStar4Checked;
            public bool? WeaponFilterStar5Checked;
            public bool? WeaponFilterJobAllChecked;
            public bool? WeaponFilterJob1Checked;
            public bool? WeaponFilterJob2Checked;
            public bool? WeaponFilterJob3Checked;
            public bool? WeaponFilterJob4Checked;
            public bool? WeaponFilterJob5Checked;
            public bool? WeaponFilterJob6Checked;
            public bool? WeaponFilterJob7Checked;
            public bool? WeaponFilterJob8Checked;
            public bool? WeaponFilterJob9Checked;
            public bool? WeaponFilterJob10Checked;
            public bool? WeaponFilterJob11Checked;
            public bool? WeaponFilterCanEnStrengthChecked;
            public bool? WeaponFilterUnLockChecked;
        }

        public void CloseItemWeaponAction()
        {
            Directory.CreateDirectory("config");
            File.WriteAllText("config/ItemWeaponSetting.dat", JsonConvert.SerializeObject(new ItemWeaponSetting()
            {
                WeaponFilterStar1Checked = WeaponFilterStar1.IsChecked,
                WeaponFilterStar2Checked = WeaponFilterStar2.IsChecked,
                WeaponFilterStar3Checked = WeaponFilterStar3.IsChecked,
                WeaponFilterStar4Checked = WeaponFilterStar4.IsChecked,
                WeaponFilterStar5Checked = WeaponFilterStar5.IsChecked,
                WeaponFilterJobAllChecked = WeaponFilterJobAll.IsChecked,
                WeaponFilterJob1Checked = WeaponFilterJob1.IsChecked,
                WeaponFilterJob2Checked = WeaponFilterJob2.IsChecked,
                WeaponFilterJob3Checked = WeaponFilterJob3.IsChecked,
                WeaponFilterJob4Checked = WeaponFilterJob4.IsChecked,
                WeaponFilterJob5Checked = WeaponFilterJob5.IsChecked,
                WeaponFilterJob6Checked = WeaponFilterJob6.IsChecked,
                WeaponFilterJob7Checked = WeaponFilterJob7.IsChecked,
                WeaponFilterJob8Checked = WeaponFilterJob8.IsChecked,
                WeaponFilterJob9Checked = WeaponFilterJob9.IsChecked,
                WeaponFilterJob10Checked = WeaponFilterJob10.IsChecked,
                WeaponFilterJob11Checked = WeaponFilterJob11.IsChecked,
                WeaponFilterCanEnStrengthChecked = WeaponFilterCanEnStrength.IsChecked,
                WeaponFilterUnLockChecked = WeaponFilterUnLock.IsChecked,
            }));
        }

        private void RestoreItemWeaponSetting()
        {
            if (File.Exists("config/ItemWeaponSetting.dat"))
            {
                var setting = JsonConvert.DeserializeObject<ItemWeaponSetting>(File.ReadAllText("config/ItemWeaponSetting.dat"));
                if (setting != null)
                {
                    WeaponFilterStar1.IsChecked = setting.WeaponFilterStar1Checked ?? false;
                    WeaponFilterStar2.IsChecked = setting.WeaponFilterStar2Checked ?? false;
                    WeaponFilterStar3.IsChecked = setting.WeaponFilterStar3Checked ?? false;
                    WeaponFilterStar4.IsChecked = setting.WeaponFilterStar4Checked ?? false;
                    WeaponFilterStar5.IsChecked = setting.WeaponFilterStar5Checked ?? false;
                    WeaponFilterJobAll.IsChecked = setting.WeaponFilterJobAllChecked ?? false;
                    WeaponFilterJob1.IsChecked = setting.WeaponFilterJob1Checked ?? false;
                    WeaponFilterJob2.IsChecked = setting.WeaponFilterJob2Checked ?? false;
                    WeaponFilterJob3.IsChecked = setting.WeaponFilterJob3Checked ?? false;
                    WeaponFilterJob4.IsChecked = setting.WeaponFilterJob4Checked ?? false;
                    WeaponFilterJob5.IsChecked = setting.WeaponFilterJob5Checked ?? false;
                    WeaponFilterJob6.IsChecked = setting.WeaponFilterJob6Checked ?? false;
                    WeaponFilterJob7.IsChecked = setting.WeaponFilterJob7Checked ?? false;
                    WeaponFilterJob8.IsChecked = setting.WeaponFilterJob8Checked ?? false;
                    WeaponFilterJob9.IsChecked = setting.WeaponFilterJob9Checked ?? false;
                    WeaponFilterJob10.IsChecked = setting.WeaponFilterJob10Checked ?? false;
                    WeaponFilterJob11.IsChecked = setting.WeaponFilterJob11Checked ?? false;
                    WeaponFilterCanEnStrength.IsChecked = setting.WeaponFilterCanEnStrengthChecked ?? false;
                    WeaponFilterUnLock.IsChecked = setting.WeaponFilterUnLockChecked ?? false;
                }
            }
        }

        public void InitWeaponList()
        {
            if (weapons?.Count == 0)
            {
                var defaultWeapon = new DeckShareData.Weapon()
                {
                    weaponInfo = "不選擇",
                    wId = -1
                };

                weapons.Add(defaultWeapon);
            }


            if (UserData.uwidToWeapon?.Count > 0)
            {
                var wps = new List<int>() { 0 };
                foreach (var weapon in UserData.uwidToWeapon.Values)
                {
                    bool isSet = false;
                    for (int i = 0; i < weapons.Count; ++i)
                    {
                        if (weapons[i].uwId == weapon.uwId)
                        {
                            wps.Add(i);
                            weapons[i] = weapon;
                            isSet = true;
                            break;
                        }
                    }
                    if (!isSet)
                    {
                        wps.Add(weapons.Count);
                        weapons.Add(weapon);
                    }
                }

                wps.Sort();

                for (int i = weapons.Count - 1; i >= 0; --i)
                {
                    if (!wps.Contains(i)) { Console.WriteLine(i); weapons.RemoveAt(i); }
                }
            }
        }

        private void AutoCompoWeaponClick(object sender, RoutedEventArgs e)
        {
            // ItemWeaponWriter.WriteLine(autoCompoWeapon.Background.ToString());
            if ($"{autoCompoWeapon.Background}" == "#FFCBA9E5")
            {
                ItemWeaponWriter.WriteLine("停止武器強化");
                ItemWeaponThread?.Interrupt();
            }
            else if (ItemWeaponThread?.IsAlive ?? false)
            {
                return;
            }
            else if (autoCompoWeapon.Background.ToString() == "#FF2196F3")
            {
                ItemWeaponWriter.WriteLine("開始武器強化");
                /**************************************/
                LockAllItemBtn("ItemWeapon");
                autoCompoWeapon.IsEnabled = true;
                autoCompoWeapon.Background = (Brush)new BrushConverter().ConvertFromString("#FFCBA9E5");
                autoCompoWeapon.Content = "停止";
                /**************************************/

                ItemWeaponThread = new Thread(() =>
                {
                    ItemWeaponHandler.GlobalTryCatch(() =>
                    {
                        if (AllItemAction.ItemWeapons == null)
                        {
                            AllItemAction.SetCardWeaponList();
                        }
                        foreach (var infos in AllItemAction.ItemWeapons)
                        {
                            if (infos.hasEvolve <= 0 && infos.level >= infos.levelMax) continue;

                            var weapon = infos;
                            try
                            {
                                while (true)
                                {
                                    if (weapon.hasEvolve > 0)
                                    {
                                        weapon = WeaponAction.EvolveWeapon(weapon.uwId).result.weapon;
                                    }
                                    else if (weapon.level < weapon.levelMax)
                                    {
                                        if (WeaponAction.CompoWeapon(weapon.uwId, weapon.levelMax - weapon.level))
                                        {
                                            ItemWeaponWriter.WriteLine($"強化{weapon.name}成功");
                                        }
                                        else
                                        {
                                            ItemWeaponWriter.WriteLine($"強化{weapon.name}失敗");
                                        }
                                        break;
                                    }
                                }
                            }
                            catch (NullReferenceException)
                            {
                                ItemWeaponWriter.WriteLine($"強化{weapon.name}時發生錯誤");
                            }
                            catch (ArgumentNullException)
                            {
                                ItemWeaponWriter.WriteLine($"強化{weapon.name}時發生錯誤");
                            }
                        }
                    });

                    Dispatcher.Invoke(() =>
                    {
                        UnLockAllItemBtn();
                        autoCompoWeapon.Background = (Brush)new BrushConverter().ConvertFromString("#FF2196F3");
                        autoCompoWeapon.Content = "武器合成";
                        AllItemAction.SetCardWeaponList();
                    });

                });
                ItemWeaponThread.Start();
            }
            else
            {
                ItemWeaponWriter.WriteLine($"這是{autoCompoWeapon.Background}");
            }
        }
        private void AutoCombineClick(object sender, RoutedEventArgs e)
        {

            // ItemWeaponWriter.WriteLine(autoCombine.Background.ToString());
            if ($"{autoCombine.Background}" == "#FFCBA9E5")
            {
                ItemWeaponWriter.WriteLine("停止武器合成");
                ItemWeaponThread?.Interrupt();
            }
            else if (ItemWeaponThread?.IsAlive ?? false)
            {
                return;
            }
            else if (autoCombine.Background.ToString() == "#FF2196F3")
            {
                ItemWeaponWriter.WriteLine("開始武器合成");
                /**************************************/
                LockAllItemBtn("ItemWeapon");
                autoCombine.IsEnabled = true;
                autoCombine.Background = (Brush)new BrushConverter().ConvertFromString("#FFCBA9E5");
                autoCombine.Content = "停止";
                /**************************************/

                ItemWeaponThread = new Thread(() =>
                {
                    ItemWeaponHandler.GlobalTryCatch(() =>
                    {
                        if (AllItemAction.ItemWeapons == null)
                        {
                            AllItemAction.SetCardWeaponList();
                        }
                        while (true)
                        {
                            ItemWeaponWriter.WriteLine($"接收");
                            if (false)
                            {
                                var root = PresentAction.GetPresentListByIds(0, new string[] { "6001", "6002", "6003", "6004", "6005" }, false);
                                PresentAction.ReceivePresent(root);
                            }

                            WeaponAction.CombineWeapon();
                        }
                    });

                    Dispatcher.Invoke(() =>
                    {
                        UnLockAllItemBtn();
                        autoCombine.Background = (Brush)new BrushConverter().ConvertFromString("#FF2196F3");
                        autoCombine.Content = "武器合成";
                        AllItemAction.SetCardWeaponList();
                    });
                });

                ItemWeaponThread.Start();
            }
            else
            {
                ItemWeaponWriter.WriteLine($"這是{autoCombine.Background}");
            }
        }
        private void WeaponExchangeClick(object sender, EventArgs e)
        {/*
            var flag = WeaponAction.WeaponTrade("41163", new List<string> { "27051796", "27051799", "27051800", "27051801" });
            if (flag)
            {
                ItemWeaponWriter.WriteLine("武器交換A成功");
            }
            else
            {
                ItemWeaponWriter.WriteLine("武器交換A失敗");
                flag = WeaponAction.WeaponTrade("41160", new List<string> { "27051796", "27051799", "27051800", "27051801" });
                if (flag)
                {
                    ItemWeaponWriter.WriteLine("武器交換B成功");
                }
                else
                {
                    ItemWeaponWriter.WriteLine("武器交換B失敗");
                }
            }
            ItemWeaponWriter.WriteLine("武器交換結束");*/
        }
        private void WeaponSkillExchangeClick(object sender, EventArgs e)
        {
            WeaponAction.GetWeapon("27196924");
        }
        private void ItemWeaponBtnSelectClick(object sender, EventArgs e)
        {
        }
        private void DisableReceivePanel(object sender, MouseButtonEventArgs e)
        {
            foreach (var ch in WeaponRarityreceviePanel.Children)
            {
                if (ch is Grid)
                {
                    Grid gd = ch as Grid;
                    gd.IsEnabled = !gd.IsEnabled;
                }
            }
        }
        private void AWeaponLockClick(object sender, RoutedEventArgs e)
        {

        }
        private void AWeaponUnLockClick(object sender, RoutedEventArgs e)
        {

        }
        private void AWeaponEnhanceClick(object sender, RoutedEventArgs e)
        {

        }
        private void AWeaponSkillClick(object sender, RoutedEventArgs e)
        {

        }

        private void weaponHouseBtnClick(object sender, EventArgs e)
        {
        }

        private void wareHouseBtnClick(object sender, EventArgs e)
        {
        }

        private void AWeaponStorageClick(object sender, EventArgs e)
        {
        }

        private void AWeaponUnstorageClick(object sender, EventArgs e)
        {
        }

        private void WeaponSkillWindowBtnClick(object sender, EventArgs e)
        {
            MainWindow.weaponSkillWindow.Show();
            /*
            string uwId = "27196660";
            int csId1 = 30000032;
            int csId2 = 30000033;
            int csId3 = 30000132;
            ItemWeaponWriter.WriteLine("交換技能...");
            WeaponSkillWindowBtn.IsEnabled = false;

            ItemWeaponThread = new Thread(() =>
            {
                ItemWeaponHandler.GlobalTryCatch(() =>
                {
                    WeaponAction.ExecWeaponSkill(uwId, csId1, csId2, csId3,true);
                });

                Dispatcher.Invoke(() =>
                {
                    ItemWeaponWriter.WriteLine("交換技能 完成");
                    WeaponSkillWindowBtn.IsEnabled = true;
                });

            });
            ItemWeaponThread.Start();
            */
        }
        private void ItemWeaponBtnSelectClick(object sender, RoutedEventArgs e)
        {
            ItemWeaponWriter.WriteLine("得到技能ID...");
            ItemWeaponbtnSelect.IsEnabled = false;

            ItemWeaponThread = new Thread(() =>
            {
                ItemWeaponHandler.GlobalTryCatch(() =>
                {
                    if (AllItemAction.ItemWeapons == null)
                    {
                        AllItemAction.SetAllItemList();
                    }
                    WeaponAction.GetAllWeaponSkill();
                });

                Dispatcher.Invoke(() =>
                {
                    ItemWeaponWriter.WriteLine("完成");
                    ItemWeaponbtnSelect.IsEnabled = true;
                });

            });
            ItemWeaponThread.Start();
        }
        /*
         
        {
            ItemWeaponWriter.WriteLine("得到技能ID...");
            ItemWeaponbtnSelect.IsEnabled = false;

            ItemWeaponThread = new Thread(() =>
            {
            ItemWeaponHandler.GlobalTryCatch(() =>
            {
                if (AllItemAction.ItemWeapons == null)
                {
                    AllItemAction.SetAllItemList();
                }
                WeaponAction.GetAllWeaponSkill();
            });

            Dispatcher.Invoke(() =>
            {
                ItemWeaponWriter.WriteLine("完成");
                ItemWeaponbtnSelect.IsEnabled = true;
            });

        });
            ItemWeaponThread.Start();
        }
    */
    }
}
