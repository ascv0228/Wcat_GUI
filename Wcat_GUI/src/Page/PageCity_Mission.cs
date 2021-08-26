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
    /// PageCity.xaml 的互動邏輯
    /// </summary>
    public partial class PageCity
    {
        private Thread MissionThread;

        private void MissionAllListBtnClick(object sender, RoutedEventArgs e)
        {
            if (MissionThread?.IsAlive ?? false)
            {
                return;
            }
            else
            {
                MissionWriter.WriteLine("開始顯示所有任務");
                /**************************************/
                MissionAllListbtn.IsEnabled = false;
                /**************************************/

                MissionThread = new Thread(() =>
                {
                    MissionHandler.GlobalTryCatch(() =>
                    {
                        MissionAction.PrintAllMission();
                    });

                    Dispatcher.Invoke(() =>
                    {
                        MissionAllListbtn.IsEnabled = true;
                        MissionWriter.WriteLine("顯示所有任務 結束");
                    });

                });
                MissionThread.Start();
            }
        }
        private void MissionCompleteBtnClick(object sender, RoutedEventArgs e)
        {
            if (MissionThread?.IsAlive ?? false)
            {
                return;
            }
            else
            {
                MissionWriter.WriteLine("開始領取完成任務");
                /**************************************/
                MissionCompletebtn.IsEnabled = false;
                /**************************************/

                MissionThread = new Thread(() =>
                {
                    MissionHandler.GlobalTryCatch(() =>
                    {
                        MissionAction.CompleteAllMission();
                    });

                    Dispatcher.Invoke(() =>
                    {
                        MissionCompletebtn.IsEnabled = true;
                        MissionWriter.WriteLine("完成任務 結束");
                    });

                });
                MissionThread.Start();
            }
        }
        private void MusicAllListBtnClick(object sender, RoutedEventArgs e)
        {
            if (MissionThread?.IsAlive ?? false)
            {
                return;
            }
            else
            {
                MissionWriter.WriteLine("開始顯示未完成音樂");
                /**************************************/
                MusicAllListbtn.IsEnabled = false;
                /**************************************/

                MissionThread = new Thread(() =>
                {
                    MissionHandler.GlobalTryCatch(() =>
                    {
                        MusicAction.GetUnlockMusicList(false);
                    });

                    Dispatcher.Invoke(() =>
                    {
                        MusicAllListbtn.IsEnabled = true;
                        MissionWriter.WriteLine("顯示未完成音樂 結束");
                    });

                });
                MissionThread.Start();
            }
        }
        private void MusicCompleteBtnClick(object sender, RoutedEventArgs e)
        {
            if (MissionThread?.IsAlive ?? false)
            {
                return;
            }
            else
            {
                MissionWriter.WriteLine("開始解放未完成音樂");
                /**************************************/
                MusicCompletebtn.IsEnabled = false;
                /**************************************/

                MissionThread = new Thread(() =>
                {
                    MissionHandler.GlobalTryCatch(() =>
                    {
                        MusicAction.CompleteMusic();
                    });

                    Dispatcher.Invoke(() =>
                    {
                        MusicCompletebtn.IsEnabled = true;
                        MissionWriter.WriteLine("解放未完成音樂 結束");
                    });

                });
                MissionThread.Start();
            }
        }
        private void MissionBtnEndClick(object sender, RoutedEventArgs e)
        {
            MissionThread?.Interrupt();
        }
    }
}