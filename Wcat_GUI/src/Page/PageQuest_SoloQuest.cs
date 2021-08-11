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
        private class SoloQuestSetting
        {
            public bool? mainQuestChecked;
            public bool? eventQuestChecked;
            public bool? easyQuestChecked;
            public bool? normalQuestChecked;
            public bool? hardQuestChecked;
            public bool? repeatQuestChecked;
            public bool? loopQuestChecked;
            public bool? autoUpdateTroopChecked;
            public bool? autoUpdateTroopByLoveChecked;
            public bool? autoExchangeGoldChecked;
            public bool? autoExchangeSoulChecked;
        }

        private Dictionary<int, PageQuestAction.SelectionData> selectedSoloQuests;// = new Dictionary<int, QuestData>();
        private List<QuestListResponseData.Event> Soloevents = new List<QuestListResponseData.Event>();
        private Dictionary<int, List<QuestShareData.Quest>> tmpSoloQuests = new Dictionary<int, List<QuestShareData.Quest>>();



        private ExceptionHandler SoloQuestHandler;
        private static CustomWriter SoloQuestWriter;



        private void OnCollectionChanged_SoloQuest(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (string str in e.NewItems)
                {
                    SoloQuestWriter.WriteLine($"{str}");
                    Buffers.pageQuestMsgs.Remove(str);
                }
            }
        }

        public void CloseSoloQuestAction()
        {
            Directory.CreateDirectory("config");
            File.WriteAllText("config/QuestSetting.dat", JsonConvert.SerializeObject(new SoloQuestSetting()
            {
                mainQuestChecked = mainSoloQuest.IsChecked,
                eventQuestChecked = eventSoloQuest.IsChecked,
                easyQuestChecked = easySoloQuest.IsChecked,
                normalQuestChecked = normalSoloQuest.IsChecked,
                hardQuestChecked = hardSoloQuest.IsChecked,
                repeatQuestChecked = repeatSoloQuest.IsChecked,
                loopQuestChecked = loopSoloQuest.IsChecked,
                autoUpdateTroopChecked = autoUpdateTroop_SoloQuest.IsChecked,
                autoUpdateTroopByLoveChecked = autoUpdateTroopByLove_SoloQuest.IsChecked,
                autoExchangeGoldChecked = autoExchangeGold_SoloQuest.IsChecked,
                autoExchangeSoulChecked = autoExchangeSoul_SoloQuest.IsChecked
            }));
        }

        private void RestoreSoloQuestSetting()
        {
            if (File.Exists("config/QuestSetting.dat"))
            {
                var setting = JsonConvert.DeserializeObject<SoloQuestSetting>(File.ReadAllText("config/QuestSetting.dat"));
                if (setting != null)
                {
                    mainSoloQuest.IsChecked = setting.mainQuestChecked ?? false;
                    eventSoloQuest.IsChecked = setting.eventQuestChecked ?? false;
                    easySoloQuest.IsChecked = setting.easyQuestChecked ?? false;
                    normalSoloQuest.IsChecked = setting.normalQuestChecked ?? false;
                    hardSoloQuest.IsChecked = setting.hardQuestChecked ?? false;
                    repeatSoloQuest.IsChecked = setting.repeatQuestChecked ?? false;
                    loopSoloQuest.IsChecked = setting.loopQuestChecked ?? false;
                    autoUpdateTroop_SoloQuest.IsChecked = setting.autoUpdateTroopChecked ?? false;
                    autoUpdateTroopByLove_SoloQuest.IsChecked = setting.autoUpdateTroopByLoveChecked ?? false;
                    autoExchangeGold_SoloQuest.IsChecked = setting.autoExchangeGoldChecked ?? false;
                    autoExchangeSoul_SoloQuest.IsChecked = setting.autoExchangeSoulChecked ?? false;
                }
            }
        }

        private void BtnInquirySubSoloQuestClisk(object sender, EventArgs e)
        {
            var selected = (QuestListResponseData.Event)SoloQuestList.SelectedItem;

            if (selected == null || selected.eventInfo == "ALL") return;

            btnInquirySoloQuest.IsEnabled = false;
            btnInquirySubSoloQuest.IsEnabled = false;
            modifyPanel_SoloQuest.IsEnabled = false;

            bool easy = easySoloQuest.IsChecked ?? false;
            bool normal = normalSoloQuest.IsChecked ?? false;
            bool hard = hardSoloQuest.IsChecked ?? false;
            bool repeat = repeatSoloQuest.IsChecked ?? false;

            new Thread(() =>
            {
                SoloQuestHandler.GlobalTryCatch(() =>
                {
                    SoloQuestWriter.WriteLine($"開始查詢");

                    var evt = selected;
                    var qsts = QuestAction.GetQuestModes(evt.areaId == 0 ? QuestAction.GetQuestEventList().result : null, evt, easy, normal, hard, repeat);
                    tmpSoloQuests[evt.locationId] = qsts;

                    Dispatcher.Invoke(() =>
                    {
                        subSoloQuestList.Items.Clear();
                        if (qsts.Count > 0)
                        {
                            subSoloQuestList.Items.Add(new QuestShareData.Quest() { questInfo = "ALL" });
                            subSoloQuestList.SelectedIndex = 0;
                            foreach (var qst in qsts)
                            {
                                subSoloQuestList.Items.Add(qst);
                            }
                        }
                    });

                    SoloQuestWriter.WriteLine($"查詢完畢");
                });

                Dispatcher.Invoke(() =>
                {
                    btnInquirySoloQuest.IsEnabled = true;
                    btnInquirySubSoloQuest.IsEnabled = true;
                    modifyPanel_SoloQuest.IsEnabled = true;
                });
            }).Start();
        }

        private void BtnInquirySoloQuestClick(object sender, EventArgs e)
        {
            btnInquirySoloQuest.IsEnabled = false;
            btnInquirySubSoloQuest.IsEnabled = false;
            modifyPanel_SoloQuest.IsEnabled = false;

            bool getMain = mainSoloQuest.IsChecked ?? false;
            bool getEvent = eventSoloQuest.IsChecked ?? false;
            bool easy = easySoloQuest.IsChecked ?? false;
            bool normal = normalSoloQuest.IsChecked ?? false;
            bool hard = hardSoloQuest.IsChecked ?? false;
            bool repeat = repeatSoloQuest.IsChecked ?? false;

            new Thread(() =>
            {
                SoloQuestHandler.GlobalTryCatch(() =>
                {
                    SoloQuestWriter.WriteLine($"開始查詢");

                    var tmpEvts = new List<QuestListResponseData.Event>();
                    if (getMain)
                    {
                        var normalAreas = QuestAction.GetNormalAreas();
                        for (int i = 0; i < normalAreas.Count; ++i)
                        {
                            if (repeat || normalAreas[i].modeDataArray.Any(x => x.percent < 100))
                            {
                                var evt = new QuestListResponseData.Event();
                                evt.locationId = evt.areaId = normalAreas[i].areaId;
                                evt.name = normalAreas[i].name;
                                tmpEvts.Add(evt);
                            }
                        }
                    }

                    if (getEvent)
                    {
                        var res = QuestAction.GetQuestEventList();
                        tmpEvts.AddRange(res.result.events.Where(x =>
                        {

                            bool flag;

                            if (x.areaId != 0)
                            {
                                flag = x.subMissionComplete != 1;
                            }
                            else
                            {
                                var qsts = QuestAction.GetQuestModes(res.result, x, easy, normal, hard, repeat);
                                tmpSoloQuests[x.locationId] = qsts;
                                flag = x.subMissionComplete != null ? x.subMissionComplete != 1 : qsts.Count > 0;
                            }

                            return repeat || flag;
                        }).OrderBy(x => x.restTime));
                    }

                    Soloevents = tmpEvts;

                    Dispatcher.Invoke(() =>
                    {
                        SoloQuestList.Items.Clear();
                        if (Soloevents.Count > 0)
                        {
                            SoloQuestList.Items.Add(new QuestListResponseData.Event() { eventInfo = "ALL" });
                            SoloQuestList.SelectedIndex = 0;
                            foreach (var evt in Soloevents)
                            {
                                SoloQuestList.Items.Add(evt);
                            }
                        }
                    });

                    SoloQuestWriter.WriteLine($"查詢完畢");
                });

                Dispatcher.Invoke(() =>
                {
                    btnInquirySoloQuest.IsEnabled = true;
                    btnInquirySubSoloQuest.IsEnabled = true;
                    modifyPanel_SoloQuest.IsEnabled = true;
                });

            }).Start();
        }

        private void OnSoloQuestListChange(object sender, SelectionChangedEventArgs e)
        {
            subSoloQuestList.Items.Clear();

            var selected = (QuestListResponseData.Event)SoloQuestList.SelectedItem;
            if (selected == null || selected.eventInfo == "ALL") return;

            tmpSoloQuests.TryGetValue(selected.locationId, out List<QuestShareData.Quest> qsts);
            if (qsts != null && qsts.Count > 0)
            {
                subSoloQuestList.Items.Add(new QuestShareData.Quest() { questInfo = "ALL" });
                subSoloQuestList.SelectedIndex = 0;
                foreach (var qst in qsts)
                {
                    subSoloQuestList.Items.Add(qst);
                }
            }
        }

        private void BtnRemoveSoloQuestClick(object sender, EventArgs e)
        {
            btnAddSoloQuest.IsEnabled = false;
            btnRemoveSoloQuest.IsEnabled = false;

            var selected = (QuestListResponseData.Event)SoloQuestList.SelectedItem;
            var subSelected = (QuestShareData.Quest)subSoloQuestList.SelectedItem;

            if (selected == null || selected.eventInfo == "ALL")
            {
                if (selectedSoloQuests != null)
                {
                    SoloQuestWriter.WriteLine($"移除中");
                    selectedSoloQuests = null;
                    SoloQuestWriter.WriteLine($"已移除全部關卡");

                    ShowSelectedSoloQuestInfo();
                }
            }
            else
            {
                int evtId = selected.locationId;
                if (subSelected == null || subSelected.questInfo == "ALL")
                {
                    if (selectedSoloQuests != null && selectedSoloQuests.ContainsKey(evtId))
                    {
                        SoloQuestWriter.WriteLine($"移除中");

                        if (selectedSoloQuests.Count == 1)
                        {
                            selectedSoloQuests = null;
                        }
                        else
                        {
                            selectedSoloQuests.Remove(evtId);
                        }

                        SoloQuestWriter.WriteLine($"已移除 {selected.name} 全部子關卡");

                        ShowSelectedSoloQuestInfo();
                    }
                }
                else
                {
                    if (selectedSoloQuests != null && selectedSoloQuests.ContainsKey(evtId))
                    {
                        var qst = selectedSoloQuests[evtId].qsts.FirstOrDefault(x => x.questId == subSelected.questId);
                        if (qst != null)
                        {
                            SoloQuestWriter.WriteLine($"移除中");

                            if (selectedSoloQuests[evtId].qsts.Count == 1)
                            {
                                if (selectedSoloQuests.Count == 1)
                                {
                                    selectedSoloQuests = null;
                                }
                                else
                                {
                                    selectedSoloQuests.Remove(evtId);
                                }

                            }
                            else
                            {
                                selectedSoloQuests[evtId].qsts.Remove(subSelected);
                            }

                            SoloQuestWriter.WriteLine($"已移除 關卡 {subSelected.name}");

                            ShowSelectedSoloQuestInfo();
                        }
                    }
                }
            }

            btnAddSoloQuest.IsEnabled = true;
            btnRemoveSoloQuest.IsEnabled = true;
        }

        private void BtnAddSoloQuestClick(object sender, EventArgs e)
        {
            btnAddSoloQuest.IsEnabled = false;
            btnRemoveSoloQuest.IsEnabled = false;

            var selected = (QuestListResponseData.Event)SoloQuestList.SelectedItem;
            var subSelected = (QuestShareData.Quest)subSoloQuestList.SelectedItem;

            if (selected == null || selected.eventInfo == "ALL")
            {
                if (selectedSoloQuests?.Count != 0)
                {
                    SoloQuestWriter.WriteLine($"加入中");
                    selectedSoloQuests = new Dictionary<int, PageQuestAction.SelectionData>();
                    SoloQuestWriter.WriteLine($"已加入全部關卡");

                    ShowSelectedSoloQuestInfo();
                }
            }
            else if (selectedSoloQuests?.Count != 0)
            {
                int evtId = selected.locationId;
                if (subSelected == null || subSelected.questInfo == "ALL")
                {
                    if (selectedSoloQuests == null) selectedSoloQuests = new Dictionary<int, PageQuestAction.SelectionData>();
                    if (!selectedSoloQuests.ContainsKey(evtId))
                    {
                        selectedSoloQuests[evtId] = new PageQuestAction.SelectionData();
                    }

                    if (selectedSoloQuests[evtId]?.qsts?.Count != 0)
                    {
                        SoloQuestWriter.WriteLine($"加入中");
                        var qd = selectedSoloQuests[evtId];
                        qd.evt ??= selected;
                        qd.qsts = new List<QuestShareData.Quest>();
                        SoloQuestWriter.WriteLine($"已加入 {selected.name} 的全部子關卡");

                        ShowSelectedSoloQuestInfo();
                    }
                }
                else
                {
                    selectedSoloQuests ??= new Dictionary<int, PageQuestAction.SelectionData>();
                    if (!selectedSoloQuests.ContainsKey(evtId))
                    {
                        selectedSoloQuests[evtId] = new PageQuestAction.SelectionData();
                    }

                    if (selectedSoloQuests[evtId]?.qsts?.Count != 0)
                    {
                        var qd = selectedSoloQuests[evtId];
                        qd.evt ??= selected;
                        qd.qsts ??= new List<QuestShareData.Quest>();
                        if (!qd.qsts.Any(x => x.questId == subSelected.questId))
                        {
                            SoloQuestWriter.WriteLine($"加入中");
                            qd.qsts.Add(subSelected);
                            SoloQuestWriter.WriteLine($"已加入 {subSelected.questInfo}");

                            ShowSelectedSoloQuestInfo();
                        }
                    }
                }
            }

            btnAddSoloQuest.IsEnabled = true;
            btnRemoveSoloQuest.IsEnabled = true;
        }

        private void SoloQuestBtnEndClick(object sender, EventArgs e)
        {
            SoloQuestThread?.Interrupt();
        }

        private Thread SoloQuestThread;
        private void SoloQuestBtnStartClick(object sender, EventArgs e)
        {
            if (selectedSoloQuests == null) return;

            SoloQuestbtnStart.IsEnabled = false;
            SoloQuestbtnEnd.IsEnabled = true;
            btnAddSoloQuest.IsEnabled = false;
            btnRemoveSoloQuest.IsEnabled = false;

            var option = new PageQuestAction.SendOption()
            {
                loop = loopSoloQuest.IsChecked ?? false,
                repeat = repeatSoloQuest.IsChecked ?? false,
                autoUpdateTroop = autoUpdateTroop_SoloQuest.IsChecked ?? false,
                autoUpdateTroopByLove = autoUpdateTroopByLove_SoloQuest.IsChecked ?? false,
                autoExchangeGold = autoExchangeGold_SoloQuest.IsChecked ?? false,
                autoExchangeSoul = autoExchangeSoul_SoloQuest.IsChecked ?? false,
                easy = easySoloQuest.IsChecked ?? false,
                normal = normalSoloQuest.IsChecked ?? false,
                hard = hardSoloQuest.IsChecked ?? false,
                power = new PageInjectionAction.Power(injectInfo),
                mainQuest = mainSoloQuest.IsChecked ?? false,
                eventQuest = eventSoloQuest.IsChecked ?? false,
                powerItems = new List<PageInjectionAction.ItemMap>(injectItems)
            };

            SoloQuestThread = new Thread(() =>
            {
                SoloQuestHandler.GlobalTryCatch(() =>
                {
                    PageQuestAction.StartSweep(selectedSoloQuests, option);
                });

                SoloQuestWriter.WriteLine($"結束掃蕩");

                Dispatcher.Invoke(() =>
                {
                    SoloQuestbtnStart.IsEnabled = true;
                    SoloQuestbtnEnd.IsEnabled = false;
                    btnAddSoloQuest.IsEnabled = true;
                    btnRemoveSoloQuest.IsEnabled = true;
                });
            });
            SoloQuestThread.Start();
        }

        private void OnSoloQuestTextChanged(object sender, TextChangedEventArgs e)
        {
            var sb = (TextBox)sender;
            var viewSource = SoloQuestList.Items;
            if (String.IsNullOrEmpty(sb.Text))
            {
                if (viewSource.Filter != null) viewSource.Filter = null;
                return;
            }
            else
            {
                viewSource.Filter = (x =>
                {
                    if (((QuestListResponseData.Event)x).eventInfo.ToLower().Contains(sb.Text.ToLower())) return true;
                    else return false;
                });
            }
        }

        private void OnSubSoloQuestTextChanged(object sender, TextChangedEventArgs e)
        {
            var sb = (TextBox)sender;
            var viewSource = subSoloQuestList.Items;
            if (String.IsNullOrEmpty(sb.Text))
            {
                if (viewSource.Filter != null) viewSource.Filter = null;
                return;
            }
            else
            {
                viewSource.Filter = (x =>
                {
                    if (((QuestShareData.Quest)x).questInfo.ToLower().Contains(sb.Text.ToLower())) return true;
                    else return false;
                });
            }
        }

        private void OnSoloQuestPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            SoloQuestList.IsDropDownOpen = true;
        }

        private void OnSubSoloQuestPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            subSoloQuestList.IsDropDownOpen = true;
        }

        private void ShowSelectedSoloQuestInfo()
        {
            String s = "";
            if (selectedSoloQuests != null)
            {
                if (selectedSoloQuests.Count == 0)
                {
                    s += "ALL\n\n";
                }
                else
                {
                    foreach (var qd in selectedSoloQuests.Values)
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
            subSoloQuestTerminal.Text = s;

        }

        private void SoloQuestTerminal_KeyUp(object sender, KeyEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl) && e.Key == Key.D)
            {
                ((TextBox)sender).Text = "";
            }
        }

        private void SoloQuestToggleButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if ((sender as ToggleButton).IsChecked ?? false)
            {
                SoloQuestTerminal.Visibility = Visibility.Visible;
                subSoloQuestTerminal.Visibility = Visibility.Hidden;
                SoloQuestTerminalTitle.Content = "掃蕩訊息";
            }
            else
            {
                subSoloQuestTerminal.Visibility = Visibility.Visible;
                SoloQuestTerminal.Visibility = Visibility.Hidden;
                SoloQuestTerminalTitle.Content = "已選關卡";
            }
        }
    }
}
