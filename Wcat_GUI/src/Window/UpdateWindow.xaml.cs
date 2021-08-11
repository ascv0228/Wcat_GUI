using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows;

namespace Wcat_GUI
{
    /// <summary>
    /// UpdateWindow.xaml 的互動邏輯
    /// </summary>
    public partial class UpdateWindow : Window
    {
        public UpdateWindow()
        {
            InitializeComponent();

            versionInfo.Text = $"當前版本: {AppVersion.i.ver} / {AppVersion.i.pc}";
        }

        private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            updateThread?.Interrupt();
            this.Hide();
        }

        Thread updateThread;
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            checkVersionBtn.IsEnabled = false;

            updateThread = new Thread(() =>
            {
                try
                {
                    GoogleCredential cred;
                    using (var stream =
                        new FileStream("data/key.json", FileMode.Open, FileAccess.Read))
                    {
                        cred = GoogleCredential.FromStream(stream).CreateScoped(new string[] {
                            DriveService.Scope.DriveReadonly
                        });
                    }

                    var service = new DriveService(new BaseClientService.Initializer()
                    {
                        HttpClientInitializer = cred,
                    });

                    var listRequest = service.Files.List();
                    Dispatcher.Invoke(() =>
                    {
                        Title = $"版本資訊 - 連線中...";
                    });
                    listRequest.Q = "'1j0MI5SoOyfqMK5ZCocOCCmDWPQGnpLpW' in parents";

                    var files = listRequest.Execute().Files;

                    if (files != null && files.Count > 0)
                    {
                        Dispatcher.Invoke(() =>
                        {
                            Title = $"版本資訊 - 查詢中...";
                        });

                        foreach (var file in files)
                        {
                            Console.WriteLine(file.Name);
                            if (file.Name == "ver.json")
                            {
                                var res = service.Files.Get(file.Id);
                                var ms = new MemoryStream();
                                res.Download(ms);
                                var setting = JsonConvert.DeserializeObject<AppVersion>(Encoding.UTF8.GetString(ms.ToArray()));
                                if (setting != null && setting.pc > AppVersion.i.pc)
                                {
                                    Dispatcher.Invoke(() =>
                                    {
                                        Title = $"版本資訊 - 發現新版本!";
                                    });

                                    Directory.CreateDirectory("tmp");
                                    foreach (var outFile in files)
                                    {
                                        if (outFile.Name == "ver.json") continue;

                                        if (outFile.Name == "update.bat")
                                        {
                                            Dispatcher.Invoke(() =>
                                            {
                                                Title = $"版本資訊 - 更新{outFile.Name}";
                                            });

                                            ms = new MemoryStream();
                                            res = service.Files.Get(outFile.Id);
                                            res.Download(ms);
                                            File.WriteAllBytes($"{outFile.Name}", ms.ToArray());
                                        }
                                        else
                                        {
                                            Dispatcher.Invoke(() =>
                                            {
                                                Title = $"版本資訊 - 下載{outFile.Name}";
                                            });

                                            ms = new MemoryStream();
                                            res = service.Files.Get(outFile.Id);
                                            res.Download(ms);
                                            File.WriteAllBytes($"tmp/{outFile.Name}", ms.ToArray());
                                        }
                                    }

                                    Dispatcher.Invoke(() =>
                                    {
                                        Title = $"版本資訊 - 執行更新動作!";
                                    });

                                    var processInfo = new ProcessStartInfo("cmd.exe", "/c update.bat");
                                    Process.Start(processInfo).WaitForExit();
                                }
                                else
                                {
                                    Dispatcher.Invoke(() =>
                                    {
                                        Title = $"版本資訊 - 未發現新版本!";
                                    });
                                }
                                return;
                            }
                        }
                    }

                    Dispatcher.Invoke(() =>
                    {
                        Title = $"版本資訊 - 查詢失敗!";
                    });
                }
                catch (Exception ex)
                {
                    Dispatcher.Invoke(() =>
                    {
                        Title = $"版本資訊 - 發生錯誤!";
                    });
                    Console.WriteLine(ex);
                }
                finally
                {
                    Dispatcher.Invoke(() =>
                    {
                        checkVersionBtn.IsEnabled = true;
                    });
                }


            });
            updateThread.Start();
        }
    }
}
