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
using Wcat;
using Wcat.PageAction;
using Wcat.Stream;
using Wcat_GUI.Exceptions;

namespace Wcat_GUI
{
    /// <summary>
    /// PageLogin.xaml 的互動邏輯
    /// </summary>

    public partial class PageLogin : UserControl
    {
        public ICollectionView usersInfoView { get; set; }
        public ObservableCollection<UserInfo> usersInfo { get; set; }
        public bool isLogin = false;

        private ExceptionHandler mainHandler;
        private ExceptionHandler subHandler;
        private CustomWriter mainWriter;
        private CustomWriter subWriter;

        public PageLogin()
        {
            usersInfo = LoadUsers();
            usersInfoView = new CollectionViewSource() { Source = usersInfo }.View;

            Buffers.pageLoginMainMsgs.CollectionChanged += OnMainCollectionChanged;
            Buffers.pageLoginSubMsgs.CollectionChanged += OnSubCollectionChanged;

            InitializeComponent();

            mainWriter = new CustomWriter(mainTerminal);
            subWriter = new CustomWriter(subTerminal);
            mainHandler = new ExceptionHandler(mainWriter);
            subHandler = new ExceptionHandler(subWriter);
        }

        public void OnSubCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (string str in e.NewItems)
                {
                    if (str == "ADD_NEW_USER")
                    {

                        Dispatcher.Invoke(() =>
                        {
                            usersInfo.Add(UserInfo.i);
                        });
                    }
                    else
                    {
                        subWriter.WriteLine($"{str}");
                    }
                    Buffers.pageLoginSubMsgs.Remove(str);

                }
            }
        }

        public void OnMainCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (string str in e.NewItems)
                {
                    mainWriter.WriteLine($"{str}");
                    Buffers.pageLoginMainMsgs.Remove(str);
                }
            }
        }

        public ObservableCollection<UserInfo> LoadUsers()
        {
            if (File.Exists("config/users.dat"))
            {
                string users = File.ReadAllText("config/users.dat");
                var setting = JsonConvert.DeserializeObject<ObservableCollection<UserInfo>>(users);
                return setting == null ? new ObservableCollection<UserInfo>() : setting;
            }
            return new ObservableCollection<UserInfo>();
        }

        public void CloseAction()
        {
            Directory.CreateDirectory("config");
            File.WriteAllText("config/users.dat", JsonConvert.SerializeObject(usersInfo));
        }

        private void BtnCancelAddAcount(object sender, EventArgs e)
        {
            accountThread?.Interrupt();

            btnCancelAddAcount.IsEnabled = false;
            btnStart.IsEnabled = true;
            btnAddAcount.IsEnabled = true;
        }

        Thread accountThread;

        private void BtnAddAcountClick(object sender, EventArgs e)
        {
            btnAddAcount.IsEnabled = false;
            btnStart.IsEnabled = false;
            btnCancelAddAcount.IsEnabled = true;

            bool google = isGoogle.IsChecked ?? false;
            string email = emailBox.Text;
            string password = passwordBox.Password;

            accountThread = new Thread(() =>
            {
                subHandler.GlobalTryCatch(() =>
                {
                    PageLoginAction.StartAddAccount(email, password, usersInfo, google);
                });

                Dispatcher.Invoke(() =>
                {
                    btnAddAcount.IsEnabled = true;
                    btnStart.IsEnabled = true;
                    btnCancelAddAcount.IsEnabled = false;
                });
            });
            accountThread.Start();
        }

        private void BtnRemoveClick(object sender, EventArgs e)
        {
            if (usersList.SelectedIndex < 0) return;

            usersInfo.RemoveAt(usersList.SelectedIndex);
        }

        Thread loginThread;
        private void BtnEndClick(object sender, EventArgs e)
        {
            loginThread?.Interrupt();
        }

        private void BtnStartClick(object sender, EventArgs e)
        {
            if (usersList.SelectedIndex < 0) return;

            btnStart.IsEnabled = false;
            btnEnd.IsEnabled = true;
            btnAddAcount.IsEnabled = false;

            var user = (UserInfo)usersList.SelectedItem;
            UserInfo.i = user;

            loginThread = new Thread(() =>
            {
                mainHandler.GlobalTryCatch(() =>
                {
                    PageLoginAction.StartLogin();

                    isLogin = true;

                    Thread.Sleep(1000);
                });

                Dispatcher.Invoke(() =>
                {
                    btnStart.IsEnabled = true;
                    btnEnd.IsEnabled = false;
                    btnAddAcount.IsEnabled = true;
                });
            });
            loginThread.Start();
        }

        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            var sb = (TextBox)sender;
            var viewSource = usersInfoView;
            Console.WriteLine(sb.Text);
            if (String.IsNullOrEmpty(sb.Text))
            {
                if (viewSource.Filter != null) viewSource.Filter = null;
                return;
            }
            else
            {
                viewSource.Filter = (x =>
                {
                    if (((UserInfo)x).email.ToLower().Contains(sb.Text.ToLower())) return true;
                    else return false;
                });
            }
        }

        private void OnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            usersList.IsDropDownOpen = true;
        }

        private void terminal_TextChanged(object sender, TextChangedEventArgs e)
        {
            ((TextBox)sender).ScrollToEnd();
        }

        private void MenuItem_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var dig = new OpenFileDialog();
            if (dig.ShowDialog() == true)
            {
                accountThread?.Interrupt();

                btnAddAcount.IsEnabled = false;
                btnStart.IsEnabled = false;
                btnCancelAddAcount.IsEnabled = true;

                accountThread = new Thread(() =>
                {
                    subHandler.GlobalTryCatch(() =>
                    {
                        PageLoginAction.StartAddFileAccount(usersInfo, dig.FileName);
                    });

                    Dispatcher.Invoke(() =>
                    {
                        btnAddAcount.IsEnabled = true;
                        btnStart.IsEnabled = true;
                        btnCancelAddAcount.IsEnabled = false;
                    });
                });
                accountThread.Start();
            }
        }

        UpdateWindow updateWindow = new UpdateWindow();
        private void MenuUpdate_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            updateWindow.Show();
            updateWindow.Topmost = true;
        }
    }
}
