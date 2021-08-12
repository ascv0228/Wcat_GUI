using System.Collections.Specialized;
using System.Windows;
using System;
using Wcat;
using Wcat.Action;
using Wcat.Stream;
using System.Threading;

namespace Wcat_GUI
{
    /// <summary>
    /// WeaponEnhanceWindow.xaml 的互動邏輯
    /// </summary>
    public partial class WeaponEnhanceWindow : Window
    {
        private bool isSet = false;

        public WeaponEnhanceWindow()
        {
            InitializeComponent();

            Buffers.WindowWeaponEnhanceMsgs.CollectionChanged += OnCollectionChanged;
        }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (string str in e.NewItems)
                {
                    if (str.StartsWith("INIT"))
                    {
                        isSet = false;
                        var sp = str.Split("$");
                        var preParam = sp[1].Split(",");
                        var newParam = sp[2].Split(",");
                        Dispatcher.Invoke(() =>
                        {
                            if (preParam.Length == 1)
                            {
                                preAtk.Text = "-1";
                                preDef.Text = "-1";
                                preHit.Text = "-1";
                            }
                            else
                            {
                                preAtk.Text = $"{preParam[0]} rank:{preParam[1]}";
                                preDef.Text = $"{preParam[2]} rank:{preParam[3]}";
                                preHit.Text = $"{preParam[4]} rank:{preParam[5]}";
                            }

                            if (newParam.Length == 1)
                            {
                                newAtk.Text = "-1";
                                newDef.Text = "-1";
                                newHit.Text = "-1";
                            }
                            else
                            {
                                newAtk.Text = $"{newParam[0]} rank:{newParam[1]}";
                                newDef.Text = $"{newParam[2]} rank:{newParam[3]}";
                                newHit.Text = $"{newParam[4]} rank:{newParam[5]}";
                            }
                        });
                    }
                    if (str.StartsWith("WEAPON_ENHANCE$"))
                    {
                        isSet = true;
                        var sp = str.Split("$");
                        var preParam = sp[1].Split(",");
                        var newParam = sp[2].Split(",");
                        Dispatcher.Invoke(() =>
                        {
                            if (preParam.Length == 1)
                            {
                                preAtk.Text = "-1";
                                preDef.Text = "-1";
                                preHit.Text = "-1";
                            }
                            else
                            {
                                preAtk.Text = $"{preParam[0]} rank:{preParam[1]}";
                                preDef.Text = $"{preParam[2]} rank:{preParam[3]}";
                                preHit.Text = $"{preParam[4]} rank:{preParam[5]}";
                            }

                            if (newParam.Length == 1)
                            {
                                newAtk.Text = "-1";
                                newDef.Text = "-1";
                                newHit.Text = "-1";
                            }
                            else
                            {
                                newAtk.Text = $"{newParam[0]} rank:{newParam[1]}";
                                newDef.Text = $"{newParam[2]} rank:{newParam[3]}";
                                newHit.Text = $"{newParam[4]} rank:{newParam[5]}";
                            }
                        });
                    }
                    Buffers.pageQuestMsgs.Remove(str);
                }
            }
        }

        private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Buffers.pageHomeMsgs.Add("UNCHECK_ENHANCE_BTN");
            Hide();
            e.Cancel = true;
        }

        private void UpdateWeaponBtnClick(object sender, RoutedEventArgs e)
        {
            if (!isSet) return;

            UpdateWeaponBtn.IsEnabled = false;

            Title = "武器淬鍊 - 強化中...";

            new Thread(() =>
            {
                try
                {
                    WeaponEnhanceAction.UpdateWeapon(true);

                    Dispatcher.Invoke(() =>
                    {
                        var atk = newAtk.Text.Split(" rank:");
                        var def = newDef.Text.Split(" rank:");
                        var hit = newHit.Text.Split(" rank:");

                        UserData.selectedWeapon.weaponEnhanceParams.atk.value = int.Parse(atk[0]);
                        UserData.selectedWeapon.weaponEnhanceParams.atk.rank = int.Parse(atk[1]);
                        UserData.selectedWeapon.weaponEnhanceParams.def.value = int.Parse(def[0]);
                        UserData.selectedWeapon.weaponEnhanceParams.def.rank = int.Parse(def[1]);
                        UserData.selectedWeapon.weaponEnhanceParams.hit.value = int.Parse(hit[0]);
                        UserData.selectedWeapon.weaponEnhanceParams.hit.rank = int.Parse(hit[1]);

                        preAtk.Text = newAtk.Text;
                        preDef.Text = newDef.Text;
                        preHit.Text = newHit.Text;

                        newAtk.Text = "-1";
                        newDef.Text = "-1";
                        newHit.Text = "-1";

                        Title = "武器淬鍊 - 強化完畢!";
                    });
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex);
                    Dispatcher.Invoke(() =>
                    {
                        Title = "武器淬鍊 - 發生錯誤!";
                    });
                } 
                finally
                {
                    isSet = false;
                    UpdateWeaponBtn.IsEnabled = true;
                }
            }).Start();
        }
    }
}
