using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Wcat;
using Wcat.PageAction;
using Wcat.Stream;
using Wcat_GUI.Exceptions;
using Wcat.Action;

namespace Wcat_GUI
{
    /// <summary>
    /// WeaponSkillWindow.xaml 的互動邏輯
    /// </summary>
    public partial class WeaponSkillWindow : Window
    {
        private Thread WeaponSkillThread;
        private ExceptionHandler WeaponSkillHandler;
        private static CustomWriter WeaponSkillWriter;

        public WeaponSkillWindow()
        {
            InitializeComponent();
            WeaponSkillWriter = new CustomWriter(WeaponSkillWindowTerminal);
            WeaponSkillHandler = new ExceptionHandler(WeaponSkillWriter);

            Buffers.WindowWeaponSkillMsgs.CollectionChanged += OnCollectionChanged_WeaponSkill;
        }

        private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }

        private void terminal_TextChanged(object sender, TextChangedEventArgs e)
        {
            ((TextBox)sender).ScrollToEnd();
        }

        public static void WeaponSkillWindowTerminals_Clear()
        {
            WeaponSkillWriter.Clear();
        }

        private void OnCollectionChanged_WeaponSkill(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (string str in e.NewItems)
                {
                    WeaponSkillWriter.WriteLine($"{str}");
                    Buffers.WindowWeaponSkillMsgs.Remove(str);
                }
            }
        }

        private void WeaponSkillBtnEndClick(object sender, RoutedEventArgs e)
        {
            WeaponSkillThread?.Interrupt();
        }
        private void WeaponSkillBtnStartClick(object sender, RoutedEventArgs e)
        {
            string uwId = uwIdText.Text;
            int? csId1 = Tool.StringToNullableInt(csId1Text.Text);
            int? csId2 = Tool.StringToNullableInt(csId2Text.Text);
            int? csId3 = Tool.StringToNullableInt(csId3Text.Text);
            bool IsPrint = showProcess?.IsChecked ?? false;
            bool OnlyAllowMax = !(allowNotMax?.IsChecked ?? false);

            WeaponSkillWriter.WriteLine("武器技能 變換中...");
            WeaponSkillbtnStart.IsEnabled = false;
            WeaponSkillbtnEnd.IsEnabled = true;

            WeaponSkillThread = new Thread(() =>
            {
                WeaponSkillHandler.GlobalTryCatch(() =>
                {
                    WeaponAction.ExecWeaponSkill(uwId, csId1, csId2, csId3, IsPrint, OnlyAllowMax);
                });

                Dispatcher.Invoke(() =>
                {
                    WeaponSkillWriter.WriteLine("交換技能 結束");
                    WeaponSkillbtnStart.IsEnabled = true;
                    WeaponSkillbtnEnd.IsEnabled = false;
                });

            });
            WeaponSkillThread.Start();
        }
        private void AutoWeaponLockBtnClick(object sender, RoutedEventArgs e)
        {

        }

        private void AllWeaponSkillBtnClick(object sender, RoutedEventArgs e)
        {
            WeaponSkillWriter.WriteLine("得到技能ID...");
            allWeaponSkillbtn.IsEnabled = false;

            WeaponSkillThread = new Thread(() =>
            {
                WeaponSkillHandler.GlobalTryCatch(() =>
                {
                    if (AllItemAction.ItemWeapons == null)
                    {
                        AllItemAction.SetAllItemList();
                    }
                    WeaponAction.GetAllWeaponSkill();
                });

                Dispatcher.Invoke(() =>
                {
                    WeaponSkillWriter.WriteLine("得到技能 結束");
                    allWeaponSkillbtn.IsEnabled = true;
                });

            });
            WeaponSkillThread.Start();
        }
    }   
}
