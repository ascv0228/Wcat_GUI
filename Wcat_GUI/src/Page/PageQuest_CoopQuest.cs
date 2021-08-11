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
    public partial class PageQuest
    {

        private class CoopQuestSetting
        {
            public bool? mainQuest;
            public bool? eventQuest;
            public bool? repeatQuestChecked;
            public bool? loopQuestChecked;
            public bool? autoUpdateTroopChecked;
            public bool? autoUpdateTroopByLoveChecked;
            public bool? autoExchangeGoldChecked;
            public bool? autoExchangeSoulChecked;
        }

        private Dictionary<int, PageCoopQuestAction.SelectionData> selectedCoopQuests;
        private List<CoopQuestListResponseData.CoopList> Coopevents;
        private Dictionary<int, List<CoopQuestListResponseData.CoopQuest>> tmpCoopQuests = new Dictionary<int, List<CoopQuestListResponseData.CoopQuest>>();
        private ExceptionHandler CoopQuestHandler;
        private static CustomWriter CoopQuestWriter;

        private void OnCollectionChanged_CoopQuest(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (string str in e.NewItems)
                {
                    CoopQuestWriter.WriteLine($"{str}");
                    Buffers.pageCoopQuestMsgs.Remove(str);
                }
            }
        }

        public void CloseCoopQuestAction()
        {
            Directory.CreateDirectory("config");
            File.WriteAllText("config/CoopQuestSetting.dat", JsonConvert.SerializeObject(new CoopQuestSetting()
            {
                repeatQuestChecked = repeatCoopQuest.IsChecked,
                loopQuestChecked = loopCoopQuest.IsChecked,
                autoUpdateTroopChecked = autoUpdateTroop_CoopQuest.IsChecked,
                mainQuest = mainCoopQuest.IsChecked,
                eventQuest = eventCoopQuest.IsChecked,
                autoUpdateTroopByLoveChecked = autoUpdateTroopByLove_CoopQuest.IsChecked,
                autoExchangeGoldChecked = autoExchangeGold_CoopQuest.IsChecked,
                autoExchangeSoulChecked = autoExchangeSoul_CoopQuest.IsChecked
            }));
        }

        private void RestoreCoopQuestSetting()
        {
            if (File.Exists("config/CoopQuestSetting.dat"))
            {
                var setting = JsonConvert.DeserializeObject<CoopQuestSetting>(File.ReadAllText("config/CoopQuestSetting.dat"));
                if (setting != null)
                {
                    repeatCoopQuest.IsChecked = setting.repeatQuestChecked ?? false;
                    loopCoopQuest.IsChecked = setting.loopQuestChecked ?? false;
                    autoUpdateTroop_CoopQuest.IsChecked = setting.autoUpdateTroopChecked ?? false;
                    mainCoopQuest.IsChecked = setting.mainQuest ?? false;
                    eventCoopQuest.IsChecked = setting.eventQuest ?? false;
                    autoUpdateTroopByLove_CoopQuest.IsChecked = setting.autoUpdateTroopByLoveChecked ?? false;
                    autoExchangeGold_CoopQuest.IsChecked = setting.autoExchangeGoldChecked ?? false;
                    autoExchangeSoul_CoopQuest.IsChecked = setting.autoExchangeSoulChecked ?? false;
                }
            }
        }

        private void BtnInquirySubCoopQuestClisk(object sender, EventArgs e)
        {
            var selected = (CoopQuestListResponseData.CoopList)CoopQuestList.SelectedItem;
            if (selected == null || selected.eventInfo == "ALL") return;

            btnInquiryCoopQuest.IsEnabled = false;
            btnInquirySubCoopQuest.IsEnabled = false;
            modifyPanel_CoopQuest.IsEnabled = false;

            bool repeat = repeatCoopQuest.IsChecked ?? false;

            new Thread(() =>
            {
                CoopQuestHandler.GlobalTryCatch(() =>
                {
                    CoopQuestWriter.WriteLine("開始查詢");

                    var evt = selected;
                    var res = CoopQuestAction.GetCoopQuestList();
                    var qsts = CoopQuestAction.GetCoopQuestModes(res.result, evt, repeat).OrderBy(q => q.difficulty).ToList();
                    tmpCoopQuests[evt.locationId] = qsts;

                    Dispatcher.Invoke(() =>
                    {
                        subCoopQuestList.Items.Clear();
                        if (qsts.Count > 0)
                        {
                            subCoopQuestList.Items.Add(new CoopQuestListResponseData.CoopQuest() { questInfo = "ALL" });
                            subCoopQuestList.SelectedIndex = 0;
                            foreach (var qst in qsts)
                            {
                                subCoopQuestList.Items.Add(qst);
                            }
                        }
                    });

                    CoopQuestWriter.WriteLine("查詢完畢");
                });

                Dispatcher.Invoke(() =>
                {
                    btnInquiryCoopQuest.IsEnabled = true;
                    btnInquirySubCoopQuest.IsEnabled = true;
                    modifyPanel_CoopQuest.IsEnabled = true;
                });
            }).Start();
        }


        private void BtnInquiryCoopQuestClick(object sender, EventArgs e)
        {
            btnInquiryCoopQuest.IsEnabled = false;
            btnInquirySubCoopQuest.IsEnabled = false;
            modifyPanel_CoopQuest.IsEnabled = false;

            bool getMain = mainCoopQuest.IsChecked ?? false;
            bool getEvent = eventCoopQuest.IsChecked ?? false;
            bool repeat = repeatCoopQuest.IsChecked ?? false;

            new Thread(() =>
            {
                CoopQuestHandler.GlobalTryCatch(() =>
                {
                    CoopQuestWriter.WriteLine("開始查詢");

                    var res = CoopQuestAction.GetCoopQuestList();

                    Coopevents = new List<CoopQuestListResponseData.CoopList>();
                    if (getMain)
                    {
                        Coopevents.AddRange(res.result.coopLists.Where(x =>
                        {
                            var qsts = CoopQuestAction.GetCoopQuestModes(res.result, x, repeat).OrderBy(q => q.difficulty).ToList();
                            tmpCoopQuests[x.locationId] = qsts;

                            return qsts.Count > 0;
                        }));
                    }

                    if (getEvent)
                    {
                        Coopevents.AddRange(res.result.coopEventLists.Where(x =>
                        {
                            var qsts = CoopQuestAction.GetCoopQuestModes(res.result, x, repeat).OrderBy(q => q.difficulty).ToList();
                            tmpCoopQuests[x.locationId] = qsts;

                            return qsts.Count > 0;
                        }).OrderBy(x => x.limitTimeInfo));
                    }

                    Dispatcher.Invoke(() =>
                    {
                        CoopQuestList.Items.Clear();
                        if (Coopevents.Count > 0)
                        {
                            CoopQuestList.Items.Add(new CoopQuestListResponseData.CoopList() { eventInfo = "ALL" });
                            CoopQuestList.SelectedIndex = 0;
                            foreach (var evt in Coopevents)
                            {
                                CoopQuestList.Items.Add(evt);
                            }
                        }
                    });

                    CoopQuestWriter.WriteLine("查詢完畢");
                });

                Dispatcher.Invoke(() =>
                {
                    btnInquiryCoopQuest.IsEnabled = true;
                    btnInquirySubCoopQuest.IsEnabled = true;
                    modifyPanel_CoopQuest.IsEnabled = true;
                });
            }).Start();
        }

        private void OnCoopQuestListChange(object sender, SelectionChangedEventArgs e)
        {
            subCoopQuestList.Items.Clear();

            var selected = (CoopQuestListResponseData.CoopList)CoopQuestList.SelectedItem;
            if (selected == null || selected.eventInfo == "ALL") return;

            tmpCoopQuests.TryGetValue(selected.locationId, out var qsts);
            if (qsts != null && qsts.Count > 0)
            {
                subCoopQuestList.Items.Add(new CoopQuestListResponseData.CoopQuest() { questInfo = "ALL" });
                subCoopQuestList.SelectedIndex = 0;
                foreach (var qst in qsts)
                {
                    subCoopQuestList.Items.Add(qst);
                }
            }
        }

        private void BtnRemoveCoopQuestClick(object sender, EventArgs e)
        {
            btnAddCoopQuest.IsEnabled = false;
            btnRemoveCoopQuest.IsEnabled = false;

            var selected = (CoopQuestListResponseData.CoopList)CoopQuestList.SelectedItem;
            var subSelected = (CoopQuestListResponseData.CoopQuest)subCoopQuestList.SelectedItem;

            if (selected == null || selected.eventInfo == "ALL")
            {
                if (selectedCoopQuests != null)
                {
                    CoopQuestWriter.WriteLine("移除中");
                    selectedCoopQuests = null;
                    CoopQuestWriter.WriteLine("已移除全部關卡!");

                    ShowSelectedCoopQuestInfo();
                }
            }
            else
            {
                int evtId = selected.locationId;
                if (subSelected == null || subSelected.questInfo == "ALL")
                {
                    if (selectedCoopQuests != null && selectedCoopQuests.ContainsKey(evtId))
                    {
                        CoopQuestWriter.WriteLine("移除中");

                        if (selectedCoopQuests.Count == 1)
                        {
                            selectedCoopQuests = null;
                        }
                        else
                        {
                            selectedCoopQuests.Remove(evtId);
                        }

                        CoopQuestWriter.WriteLine($"已移除 {selected.name} 全部子關卡!");

                        ShowSelectedCoopQuestInfo();
                    }
                }
                else
                {
                    if (selectedCoopQuests != null && selectedCoopQuests.ContainsKey(evtId))
                    {
                        var qst = selectedCoopQuests[evtId].qsts.FirstOrDefault(x => x.questId == subSelected.questId);
                        if (qst != null)
                        {
                            CoopQuestWriter.WriteLine("移除中");

                            if (selectedCoopQuests[evtId].qsts.Count == 1)
                            {
                                if (selectedCoopQuests.Count == 1)
                                {
                                    selectedCoopQuests = null;
                                }
                                else
                                {
                                    selectedCoopQuests.Remove(evtId);
                                }

                            }
                            else
                            {
                                selectedCoopQuests[evtId].qsts.Remove(qst);
                            }

                            CoopQuestWriter.WriteLine($"已移除 關卡: {subSelected.name}!");

                            ShowSelectedCoopQuestInfo();
                        }
                    }
                }
            }

            btnAddCoopQuest.IsEnabled = true;
            btnRemoveCoopQuest.IsEnabled = true;
        }

        private void BtnAddCoopQuestClick(object sender, EventArgs e)
        {
            btnAddCoopQuest.IsEnabled = false;
            btnRemoveCoopQuest.IsEnabled = false;

            var selected = (CoopQuestListResponseData.CoopList)CoopQuestList.SelectedItem;
            var subSelected = (CoopQuestListResponseData.CoopQuest)subCoopQuestList.SelectedItem;

            if (selected == null || selected.eventInfo == "ALL")
            {
                if (selectedCoopQuests?.Count != 0)
                {
                    CoopQuestWriter.WriteLine("加入中");
                    selectedCoopQuests = new Dictionary<int, PageCoopQuestAction.SelectionData>();
                    CoopQuestWriter.WriteLine("已加入全部關卡!");

                    ShowSelectedCoopQuestInfo();
                }
            }
            else if (selectedCoopQuests?.Count != 0)
            {

                int evtId = selected.locationId;
                if (subSelected == null || subSelected.questInfo == "ALL")
                {
                    selectedCoopQuests ??= new Dictionary<int, PageCoopQuestAction.SelectionData>();
                    if (!selectedCoopQuests.ContainsKey(evtId))
                    {
                        selectedCoopQuests[evtId] = new PageCoopQuestAction.SelectionData();
                    }

                    if (selectedCoopQuests[evtId]?.qsts?.Count != 0)
                    {
                        CoopQuestWriter.WriteLine("加入中");
                        var qd = selectedCoopQuests[evtId];
                        qd.evt ??= selected;
                        qd.qsts = new List<CoopQuestListResponseData.CoopQuest>();
                        CoopQuestWriter.WriteLine($"已加入 {selected.name} 的全部子關卡!");

                        ShowSelectedCoopQuestInfo();
                    }
                }
                else
                {
                    selectedCoopQuests ??= new Dictionary<int, PageCoopQuestAction.SelectionData>();
                    if (!selectedCoopQuests.ContainsKey(evtId))
                    {
                        selectedCoopQuests[evtId] = new PageCoopQuestAction.SelectionData();
                    }

                    if (selectedCoopQuests[evtId]?.qsts?.Count != 0)
                    {
                        var qd = selectedCoopQuests[evtId];
                        qd.evt ??= selected;
                        qd.qsts ??= new List<CoopQuestListResponseData.CoopQuest>();
                        if (!qd.qsts.Any(x => x.questId == subSelected.questId))
                        {
                            CoopQuestWriter.WriteLine("加入中");
                            qd.qsts.Add(subSelected);
                            CoopQuestWriter.WriteLine($"已加入 {subSelected.questInfo}!");

                            ShowSelectedCoopQuestInfo();
                        }
                    }
                }
            }

            btnAddCoopQuest.IsEnabled = true;
            btnRemoveCoopQuest.IsEnabled = true;
        }
        private void CoopQuestBtnEndClick(object sender, EventArgs e)
        {
            CoopQuestThread?.Interrupt();
        }

        private Thread CoopQuestThread;
        private void CoopQuestBtnStartClick(object sender, EventArgs e)
        {
            if (selectedCoopQuests == null) return;

            CoopQuestbtnStart.IsEnabled = false;
            CoopQuestbtnEnd.IsEnabled = true;
            btnAddCoopQuest.IsEnabled = false;
            btnRemoveCoopQuest.IsEnabled = false;

            var option = new PageCoopQuestAction.SendOption()
            {
                loop = loopCoopQuest.IsChecked ?? false,
                repeat = repeatCoopQuest.IsChecked ?? false,
                autoExchangeGold = autoExchangeGold_CoopQuest.IsChecked ?? false,
                autoExchangeSoul = autoExchangeSoul_CoopQuest.IsChecked ?? false,
                autoUpdateTroop = autoUpdateTroop_CoopQuest.IsChecked ?? false,
                autoUpdateTroopByLove = autoUpdateTroopByLove_CoopQuest.IsChecked ?? false,
                power = new PageInjectionAction.Power(injectInfo),
                mainQuest = mainCoopQuest.IsChecked ?? false,
                eventQuest = eventCoopQuest.IsChecked ?? false,
                powerItems = new List<PageInjectionAction.ItemMap>(injectItems)
            };

            CoopQuestThread = new Thread(() =>
            {
                CoopQuestHandler.GlobalTryCatch(() =>
                {
                    PageCoopQuestAction.StartSweep(selectedCoopQuests, option);
                });

                CoopQuestWriter.WriteLine("結束掃蕩");

                Dispatcher.Invoke(() =>
                {
                    CoopQuestbtnStart.IsEnabled = true;
                    CoopQuestbtnEnd.IsEnabled = false;
                    btnAddCoopQuest.IsEnabled = true;
                    btnRemoveCoopQuest.IsEnabled = true;
                });
            });
            CoopQuestThread.Start();
        }

        private void OnCoopQuestTextChanged(object sender, TextChangedEventArgs e)
        {
            var sb = (TextBox)sender;
            var viewSource = CoopQuestList.Items;
            if (String.IsNullOrEmpty(sb.Text))
            {
                if (viewSource.Filter != null) viewSource.Filter = null;
                return;
            }
            else
            {
                viewSource.Filter = (x =>
                {
                    if (((CoopQuestListResponseData.CoopList)x).eventInfo.ToLower().Contains(sb.Text.ToLower())) return true;
                    else return false;
                });
            }
        }

        private void OnSubCoopQuestTextChanged(object sender, TextChangedEventArgs e)
        {
            var sb = (TextBox)sender;
            var viewSource = subCoopQuestList.Items;
            if (String.IsNullOrEmpty(sb.Text))
            {
                if (viewSource.Filter != null) viewSource.Filter = null;
                return;
            }
            else
            {
                viewSource.Filter = (x =>
                {
                    if (((CoopQuestListResponseData.CoopQuest)x).questInfo.ToLower().Contains(sb.Text.ToLower())) return true;
                    else return false;
                });
            }
        }

        private void OnCoopQuestPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            CoopQuestList.IsDropDownOpen = true;
        }

        private void OnSubCoopQuestPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            subCoopQuestList.IsDropDownOpen = true;
        }
        /*
        private void terminal_TextChanged(object sender, TextChangedEventArgs e)
        {
            ((TextBox)sender).ScrollToEnd();
        }*/


        private void ShowSelectedCoopQuestInfo()
        {
            String s = "";
            if (selectedCoopQuests != null)
            {
                if (selectedCoopQuests.Count == 0)
                {
                    s += "ALL\n\n";
                }
                else
                {
                    foreach (var qd in selectedCoopQuests.Values)
                    {
                        s += $"{qd.evt.name} - {qd.evt.locationId}\n";
                        if (qd.qsts?.Count == 0)
                        {
                            s += $"--- ALL Sub\n\n";
                        }
                        else
                        {
                            foreach (var qst in qd.qsts)
                            {
                                s += $"--- {qst.questInfo}\n";
                            }
                            s += "\n";
                        }
                    }
                }
            }
            subCoopQuestTerminal.Text = s;

        }

        private void CoopQuestTerminal_KeyUp(object sender, KeyEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl) && e.Key == Key.D)
            {
                ((TextBox)sender).Text = "";
            }
        }

        private void CoopQuestToggleButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if ((sender as ToggleButton).IsChecked ?? false)
            {
                CoopQuestTerminal.Visibility = Visibility.Visible;
                subCoopQuestTerminal.Visibility = Visibility.Hidden;
                CoopQuestTerminalTitle.Content = "掃蕩訊息";
            }
            else
            {
                subCoopQuestTerminal.Visibility = Visibility.Visible;
                CoopQuestTerminal.Visibility = Visibility.Hidden;
                CoopQuestTerminalTitle.Content = "已選關卡";
            }
        }
    }
}
