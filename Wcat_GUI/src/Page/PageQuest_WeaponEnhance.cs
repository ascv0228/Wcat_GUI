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
using Wcat;
using Wcat.PageAction;
using Wcat.Action;
using Wcat.Stream;
using Wcat_GUI.Exceptions;

namespace Wcat_GUI
{
    /// <summary>
    /// PageQuest.xaml 的互動邏輯
    /// </summary>
    public partial class PageQuest
    {
        private ExceptionHandler WeaponEnhanceHandler;
        private static CustomWriter WeaponEnhanceWriter;

        private void OnCollectionChanged_WeaponEnhance(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (string str in e.NewItems)
                {
                    WeaponEnhanceWriter.WriteLine($"{str}");
                    Buffers.pageWeaponEnhanceMsgs.Remove(str);
                }
            }
        }

        private void WeaponEnhanceBtnEndClick(object sender, RoutedEventArgs e)
        {
            WeaponEnhanceThread?.Interrupt();
        }

        private Thread WeaponEnhanceThread;

        private void AutoWeaponEnhanceClick(object sender, RoutedEventArgs e)
        {
            if (WeaponEnhanceThread?.IsAlive ?? false) return;

            autoEnhanceWeapon.IsEnabled = false;
            WeaponEnhancebtnEnd.IsEnabled = true;

            string paramStr = WeaponEnhanceparamText.Text;
            var targets = new List<WeaponEnhanceAction.Property>();

            var spStr = paramStr.Split(";");
            foreach (var str in spStr)
            {
                var pt = new WeaponEnhanceAction.Property();

                if (!string.IsNullOrEmpty(str))
                {
                    try
                    {
                        string[] param = str.Split(",");
                        for (int i = 0; i < param.Length; ++i)
                        {
                            if (param[i].Trim() == "") continue;

                            switch (i)
                            {
                                case 0:
                                    pt.atk = Math.Min(8, int.Parse(param[i]));
                                    break;
                                case 1:
                                    pt.def = Math.Min(8, int.Parse(param[i]));
                                    break;
                                case 2:
                                    pt.hit = Math.Min(8, int.Parse(param[i]));
                                    break;
                                case 3:
                                    pt.elem = Math.Min(8, int.Parse(param[i]));
                                    break;
                            }
                        }
                    }
                    catch (Exception)
                    {
                        //mainTerminal.AppendText($"解析條件參數失敗!!\n");
                    }
                }
                targets.Add(pt);
            }

            foreach (var target in targets)
            {
                WeaponEnhanceWriter.WriteLine($"目標 => 攻擊:{target.atk}, 防禦:{target.def}, 會心:{target.hit} 屬傷:{target.elem}");
            }
            var AutoRecoveryChecked = WeaponEnhanceAutoRecovery.IsChecked ?? false;

            WeaponEnhanceThread = new Thread(() =>
            {
                WeaponEnhanceHandler.GlobalTryCatch(() =>
                {
                    var res = TroopAction.UpdateDeck(UserData.deck).result;
                    TroopAction.UpdateWeaponMapTable(res.weapons);
                    Buffers.pageHomeMsgs.Add("REFRESH_WEAPON_LIST");

                    var questMap = new Dictionary<int, QuestShareData.Quest>();
                    var pointMap = new Dictionary<int, int>();
                    var eventList = QuestAction.GetQuestEventList();
                    foreach (var events in eventList.result.events)
                    {
                        if (events.isWeaponEnhance == 1)
                        {
                            pointMap[events.locationId] = events.userActionPointInfo.actionPoint;
                            foreach (var group in events.weaponEnhanceGroup)
                            {
                                foreach (var id in group.weaponTypes)
                                {
                                    if (!questMap.ContainsKey(id))
                                    {
                                        questMap[id] = eventList.result.quests.FirstOrDefault(x => x.questId == group.questIds[0]);
                                    }
                                }
                            }
                        }
                    }

                    foreach (var infos in UserData.uwidToWeapon.Values)
                    {
                        if (infos.isExcludeEnhance == 1) continue;
                        if (infos.weaponEnhanceParams != null)
                        {
                            bool flag = false;
                            var wep = infos.weaponEnhanceParams;
                            foreach (var target in targets)
                            {
                                if (wep.atk.rank >= target.atk && wep.def.rank >= target.def && wep.hit.rank >= target.hit &&
                                  (wep.elemAtk == null || wep.elemAtk.rank >= target.elem))
                                {
                                    flag = true;
                                    break;
                                }
                            }
                            if (flag) continue;
                        }

                        questMap.TryGetValue(infos.category, out var quest);
                        if (quest == null) continue;

                        WeaponEnhanceWriter.WriteLine($"淬鍊{infos.name}");

                        int counter = 0;
                        bool status;
                        do
                        {
                            status = false;
                            try
                            {
                                WeaponEnhanceWriter.WriteLine($"第{++counter}次淬鍊");

                                UserData.selectedWeapon = infos;
                                status = WeaponEnhanceAction.SendWeaponEnhanceQuest(quest, injectInfo, targets);
                            }
                            catch (NullReferenceException)
                            {
                                WeaponEnhanceWriter.WriteLine($"淬鍊{infos.name}時發生錯誤");
                            }
                            catch (ArgumentNullException)
                            {
                                WeaponEnhanceWriter.WriteLine($"淬鍊{infos.name}時發生錯誤");
                            }
                            catch (ExceptionsData.GenerateException ex)
                            {
                                WeaponEnhanceWriter.WriteLine($"淬鍊{infos.name}時取得關卡失敗: {ex.Message}");
                                if(ex.Message == "ERR_UNKNOWN")
                                {
                                    if(AutoRecoveryChecked)
                                    {
                                        if (WeaponEnhanceAction.RecoveryItem(quest.locationId, quest.hard, 6, true))
                                        {
                                            WeaponEnhanceWriter.WriteLine($"符文驅動器補充完成");
                                        }
                                    }
                                    else
                                    {
                                        WeaponEnhanceEnd();
                                    }
                                }
                            }
                            catch (ExceptionsData.CompleteException ex)
                            {
                                WeaponEnhanceWriter.WriteLine($"淬鍊{infos.name}時完成關卡失敗: {ex.Message}");
                            }
                            catch (ExceptionsData.UpdateDeckException ex)
                            {
                                WeaponEnhanceWriter.WriteLine($"淬鍊{infos.name}時更新隊伍失敗: {ex.Message}");
                            }
                        } while (!status);

                        WeaponEnhanceWriter.WriteLine($"淬鍊{infos.name}完畢");
                    }
                });

                WeaponEnhanceWriter.WriteLine($"淬鍊結束!");

                Dispatcher.Invoke(() =>
                {
                    autoEnhanceWeapon.IsEnabled = true;
                    WeaponEnhancebtnEnd.IsEnabled = false;
                });
            });

            WeaponEnhanceThread.Start();
        }


        private void WeaponEnhanceEnd()
        {
            WeaponEnhanceThread?.Interrupt();
        }

    }

}
