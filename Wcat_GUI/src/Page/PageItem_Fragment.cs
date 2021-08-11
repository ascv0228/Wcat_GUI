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
using Wcat.Action;
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
        private Thread ItemFragmentThread;
        private class ItemFragmentSetting
        {
            public bool? FragmentFilterStar1Checked;
            public bool? FragmentFilterStar2Checked;
            public bool? FragmentFilterStar3Checked;
            public bool? FragmentFilterStar4Checked;
            public bool? FragmentFilterLockChecked;
            public bool? FragmentFilterUnLockChecked;
            public bool? FragmentFilterLvMaxChecked;
            public bool? FragmentFilterUnLvMaxChecked;
            public bool? FragmentFilterSpecialRuneChecked;
        }

        public void CloseItemFragmentAction()
        {
            Directory.CreateDirectory("config");
            File.WriteAllText("config/ItemFragmentSetting.dat", JsonConvert.SerializeObject(new ItemFragmentSetting()
            {
                FragmentFilterStar1Checked = FragmentFilterStar1.IsChecked,
                FragmentFilterStar2Checked = FragmentFilterStar2.IsChecked,
                FragmentFilterStar3Checked = FragmentFilterStar3.IsChecked,
                FragmentFilterStar4Checked = FragmentFilterStar4.IsChecked,
                FragmentFilterLockChecked  = FragmentFilterLock.IsChecked,
                FragmentFilterUnLockChecked =FragmentFilterUnLock.IsChecked,
                FragmentFilterLvMaxChecked = FragmentFilterLvMax.IsChecked,
                FragmentFilterUnLvMaxChecked = FragmentFilterUnLvMax.IsChecked,
                FragmentFilterSpecialRuneChecked = FragmentFilterSpecialRune.IsChecked,
            }));
        }

        private void RestoreItemFragmentSetting()
        {
            if (File.Exists("config/ItemFragmentSetting.dat"))
            {
                var setting = JsonConvert.DeserializeObject<ItemFragmentSetting>(File.ReadAllText("config/ItemFragmentSetting.dat"));
                if (setting != null)
                {
                    FragmentFilterStar1.IsChecked = setting.FragmentFilterStar1Checked ?? false;
                    FragmentFilterStar2.IsChecked = setting.FragmentFilterStar2Checked ?? false;
                    FragmentFilterStar3.IsChecked = setting.FragmentFilterStar3Checked ?? false;
                    FragmentFilterStar4.IsChecked = setting.FragmentFilterStar4Checked ?? false;
                    FragmentFilterLock.IsChecked = setting.FragmentFilterLockChecked ?? false;
                    FragmentFilterUnLock.IsChecked = setting.FragmentFilterUnLockChecked ?? false;
                    FragmentFilterLvMax.IsChecked = setting.FragmentFilterLvMaxChecked ?? false;
                    FragmentFilterUnLvMax.IsChecked = setting.FragmentFilterUnLvMaxChecked ?? false;
                    FragmentFilterSpecialRune.IsChecked = setting.FragmentFilterSpecialRuneChecked ?? false;
                }
            }
        }

        private void AutoCompoFragmentClick(object sender, EventArgs e)
        {            // ItemFragmentWriter.WriteLine(autoCompoFragment.Background.ToString());
            if ($"{autoCompoFragment.Background}" == "#FFCBA9E5")
            {
                ItemFragmentWriter.WriteLine("停止石板強化");
                ItemFragmentThread?.Interrupt();
                if (ItemFragmentThread != null)
                {
                    ItemFragmentThread.Interrupt();
                }
            }
            else if (ItemFragmentThread?.IsAlive ?? false)
            {
                return;
            }
            else if (autoCompoFragment.Background.ToString() == "#FF2196F3")
            {
                ItemFragmentWriter.WriteLine("開始石板強化");
                /**************************************/
                autoCompoFragment.IsEnabled = true;
                autoCompoFragment.Background = (Brush)new BrushConverter().ConvertFromString("#FFCBA9E5");
                autoCompoFragment.Content = "停止";
                /**************************************/

                ItemFragmentThread = new Thread(() =>
                {
                    ItemFragmentHandler.GlobalTryCatch(() =>
                    {
                        if (AllItemAction.ItemFragments == null)
                        {
                            AllItemAction.SetAllItemList();
                        }/*
                        FragmentAction.ExecAllFragmentLearnSkill(FragmentFilterSpecialRune.IsChecked);*/
                        var FragmentAllList = FragmentAction.GetFragmentAllList(false);
                        if (FragmentAllList == null)
                        {
                            return;
                        }
                        foreach (var elem in FragmentAllList)
                        {
                            ItemFragmentWriter.WriteLine($"升級{elem.name}");
                            if (FragmentAction.ExecSingleFragmentLearnSkill(elem.ufId))
                            {
                                ItemFragmentWriter.WriteLine($"升級{elem.name}完成");
                            }
                            else
                            {
                                ItemFragmentWriter.WriteLine($"升級{elem.name}失敗");
                                if (FragmentFilterSpecialRune.IsChecked ?? false)
                                {
                                    foreach (var elem2 in FragmentAction.GetFragmentSpecialCostItemList(elem.ufId))
                                    {
                                        ItemFragmentWriter.WriteLine($"道具ID = {elem2.iId}, Name = {elem2.name}, Num = {elem2.num}");
                                    }
                                }
                                else
                                {
                                    foreach (var elem2 in FragmentAction.GetFragmentCostItemList(elem.ufId))
                                    {
                                        ItemFragmentWriter.WriteLine($"道具ID = {elem2.iId}, Name = {elem2.name}, Num = {elem2.num}");
                                    }

                                }
                            }
                        }
                    });

                    Dispatcher.Invoke(() =>
                    {
                        autoCompoFragment.Background = (Brush)new BrushConverter().ConvertFromString("#FF2196F3");
                        autoCompoFragment.Content = "石板強化";
                        //AllItemAction.SetAllItemList();
                    });

                });
                ItemFragmentThread.Start();
            }
            else
            {
                ItemFragmentWriter.WriteLine($"這是{autoCompoFragment.Background}");
            }
        }
        private void AFragmentLockClick(object sender, EventArgs e)
        {
            //if "favorite": true
            BtnLockChange(AFragmentUnLock, AFragmentLock);
        }

        private void AFragmentUnLockClick(object sender, EventArgs e)
        {
            //if "favorite": true
            BtnLockChange(AFragmentLock, AFragmentUnLock);
        }

        private void AFragmentLevelUpClick(object sender, EventArgs e)
        {
            //if lv < lvMax
        }

        private void ItemFragmentBtnSelectClick(object sender, RoutedEventArgs e)
        {

        }
    }
}
