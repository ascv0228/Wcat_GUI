using System;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using Wcat;

namespace Wcat_GUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private PageQuest pageQuest;
        private PageItem pageItem;
        // private PageCoopQuest pageCoopQuest;
        private PageLogin pageLogin;
        private PagePresent pagePresent;
        // private PageBuilding pageBuilding;
        // private PageInjection pageInjection;
        private PageHome pageHome;
        private PageCity pageCity;
        private DebugWindow debugWindow;
        public static WeaponEnhanceWindow weaponEnhanceWindow;

        public static WeaponSkillWindow weaponSkillWindow;

        public MainWindow()
        {
            debugWindow = new DebugWindow();
            weaponEnhanceWindow = new WeaponEnhanceWindow();
            weaponSkillWindow = new WeaponSkillWindow();
            Console.SetOut(new CustomWriter(debugWindow.terminal));

            InitializeComponent();

            GetGameVersionInfo();

            this.Content = pageLogin = new PageLogin();

            CheckUserLogin();

            ShowUpdateInfo();
        }

        private static void ShowUpdateInfo()
        {
            Directory.CreateDirectory("data");

            if (File.Exists("data/Load"))
            {
                var success = int.TryParse(File.ReadAllText("data/Load"), out var isFirstLoad);
                if (!success || isFirstLoad < AppVersion.i.pc)
                {
                    File.WriteAllText("data/Load", AppVersion.i.pc.ToString());
                    new UpdateInfoWindow()
                    {
                        Topmost = true
                    }.Show();
                }
            }
            else
            {
                File.WriteAllText("data/Load", AppVersion.i.pc.ToString());
                new UpdateInfoWindow()
                {
                    Topmost = true
                }.Show();
            }
        }

        private static void GetGameVersionInfo()
        {
            Directory.CreateDirectory("config");

            if (File.Exists("config/GameVersionInfo.dat"))
            {
                GameInfo.gameVersion = File.ReadAllText("config/GameVersionInfo.dat");
            }
            else
            {
                File.WriteAllText("config/GameVersionInfo.dat", GameInfo.gameVersion);
            }
        }

        public void CheckUserLogin()
        {
            new Thread(() =>
            {
                while (true)
                {
                    if (pageLogin.isLogin)
                    {
                        Dispatcher.Invoke(() =>
                        {
                            var tab = new Dragablz.TabablzControl
                            {
                                FixedHeaderCount = 9999
                            };

                            tab.Items.Add(new TabItem()
                            {
                                Header = "主頁",
                                Content = pageHome = new PageHome()
                            });
                            tab.Items.Add(new TabItem()
                            {
                                Header = "物品",
                                Content = pageItem = new PageItem()
                            });
                            tab.Items.Add(new TabItem()
                            {
                                Header = "戰鬥",
                                Content = pageQuest = new PageQuest()
                            });
                            tab.Items.Add(new TabItem()
                            {
                                Header = "禮物盒",
                                Content = pagePresent = new PagePresent()
                            });
                            tab.Items.Add(new TabItem()
                            {
                                Header = "城鎮",
                                Content = pageCity = new PageCity()
                            });/*
                            tab.Items.Add(new TabItem()
                            {
                                Header = "特殊",
                                Content = pageInjection = new PageInjection()
                            });
                            tab.Items.Add(new TabItem()
                            {
                                Header = "工具",
                                Content = pageTools = new PageTools()
                            });*/

                            this.Content = tab;
                        });
                        break;
                    }
                    Thread.Sleep(1000);
                }
            }).Start();
        }

        private void WindowClosing(object sender, CancelEventArgs e)
        {
            pageLogin?.CloseAction();
            pageItem?.CloseItemAction();
            pageQuest?.CloseQuestAction();
            pagePresent?.CloseAction();

            Environment.Exit(Environment.ExitCode);
        }

        private void WindowKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F5 && Keyboard.IsKeyDown(Key.LeftCtrl))
            {
                debugWindow.Show();
            }
        }
    }
}
