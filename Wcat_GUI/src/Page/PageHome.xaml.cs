using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using Wcat;
using Wcat.PageAction;
using Wcat.Stream;
using Wcat_GUI.Exceptions;

namespace Wcat_GUI
{
    /// <summary>
    /// PageHome.xaml 的互動邏輯
    /// </summary>
    public partial class PageHome : UserControl
    {
        public ICollectionView cardsView1 { get; set; }
        public ICollectionView cardsView2 { get; set; }
        public ICollectionView cardsView3 { get; set; }
        public ICollectionView cardsView4 { get; set; }
        public ICollectionView weaponsView { get; set; }
        public ObservableCollection<CardShareData.Card> cards { get; set; }
        public ObservableCollection<WeaponShareData.Weapon> weapons { get; set; }

        private ExceptionHandler mainHandler;
        private CustomWriter mainWriter;

        public PageHome()
        {
            cards = new ObservableCollection<CardShareData.Card>();
            weapons = new ObservableCollection<WeaponShareData.Weapon>();

            InitDeckList();

            cardsView1 = new CollectionViewSource() { Source = cards }.View;
            cardsView2 = new CollectionViewSource() { Source = cards }.View;
            cardsView3 = new CollectionViewSource() { Source = cards }.View;
            cardsView4 = new CollectionViewSource() { Source = cards }.View;
            weaponsView = new CollectionViewSource() { Source = weapons }.View;

            InitializeComponent();

            UpdateLocalTroopInfo();

            Buffers.pageHomeMsgs.CollectionChanged += OnCollectionChanged;

            textUserName.Text = UserInfo.i.userName;
            textMaxCost.Text = UserData.maxCost.ToString();

            mainWriter = new CustomWriter(terminal);
            mainHandler = new ExceptionHandler(mainWriter);
            InitUserProperties();
        }
        private void InitUserProperties()
        {
            var tmp = PageHomeAction.GetUserProperties();
            textUserJewel.Text = $"{tmp.jewel}";
            textUserMoney.Text = $"{tmp.gold}";
            textUserSoul.Text = $"{tmp.soul}";
        }
        private void UpdateLocalTroopInfo()
        {
            var unit1 = UserData.deck[0];
            var unit2 = UserData.deck[1];
            var unit3 = UserData.deck[2];
            var unit4 = UserData.deck[3];

            var txt1 = unit1 == null ? "無" : UserData.unitToCard[unit1.ucId].cardSimpleInfo;
            var txt2 = unit2 == null ? "無" : UserData.unitToCard[unit2.ucId].cardSimpleInfo;
            var txt3 = unit3 == null ? "無" : UserData.unitToCard[unit3.ucId].cardSimpleInfo;
            var txt4 = unit4 == null ? "無" : UserData.unitToCard[unit4.ucId].cardSimpleInfo;

            Dispatcher.Invoke(() =>
            {
                troop1Name.Text = txt1;
                troop2Name.Text = txt2;
                troop3Name.Text = txt3;
                troop4Name.Text = txt4;
            });
        }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (string str in e.NewItems)
                {
                    if (str == "UNCHECK_ENHANCE_BTN")
                    {
                    }
                    else if (str == "UPDATE_LOCAL_DECK")
                    {
                        UpdateLocalTroopInfo();
                    }
                    else if (str == "REFRESH_DECK_LIST")
                    {
                        Dispatcher.Invoke(() =>
                        {
                            InitDeckList();
                        });
                    }
                    else if (str == "REFRESH_WEAPON_LIST")
                    {
                        Dispatcher.Invoke(() =>
                        {
                        });
                    }
                    else
                    {
                        mainWriter.WriteLine($"{str}");
                    }
                    Buffers.pageHomeMsgs.Remove(str);
                }
            }
        }

        public void InitDeckList()
        {
            if (cards?.Count == 0)
            {
                var defaultCard = new CardShareData.Card()
                {
                    cardInfo = "不選擇",
                    cId = -1
                };

                cards.Add(defaultCard);
            }

            if (UserData.unitToCard?.Count > 0)
            {
                foreach (var card in UserData.unitToCard.Values)
                {
                    bool isSet = false;
                    for (int i = 0; i < cards.Count; ++i)
                    {
                        if (cards[i].ucId == card.ucId)
                        {
                            cards[i] = card;
                            isSet = true;
                            break;
                        }
                    }
                    if (!isSet) cards.Add(card);
                }
            }
        }

        private void BtnRefreshClick(object sender, EventArgs e)
        {
            InitUserProperties();
            btnUpdate.IsEnabled = false;
            btnRefresh.IsEnabled = false;

            new Thread(() =>
            {
                mainHandler.GlobalTryCatch(() =>
                {
                    PageHomeAction.StartUpdateDeck();
                    UpdateLocalTroopInfo();


                    mainWriter.WriteLine("載入資料...");

                    Dispatcher.Invoke(() =>
                    {
                        InitDeckList();
                    });

                    mainWriter.WriteLine("載入完成!");
                });

                Dispatcher.Invoke(() =>
                {
                    btnUpdate.IsEnabled = true;
                    btnRefresh.IsEnabled = true;
                });
            }).Start();
        }

        private void BtnUpdateClick(object sender, EventArgs e)
        {
            InitUserProperties();
            var updateCards = new List<CardShareData.Card>();

            var card1 = (CardShareData.Card)troop1.SelectedItem;
            var card2 = (CardShareData.Card)troop2.SelectedItem;
            var card3 = (CardShareData.Card)troop3.SelectedItem;
            var card4 = (CardShareData.Card)troop4.SelectedItem;

            if (card1 != null && card1.cId != -1) updateCards.Add(card1);
            if (card2 != null && card2.cId != -1) updateCards.Add(card2);
            if (card3 != null && card3.cId != -1) updateCards.Add(card3);
            if (card4 != null && card4.cId != -1) updateCards.Add(card4);

            if (updateCards.Count == 0) return;

            btnUpdate.IsEnabled = false;
            btnRefresh.IsEnabled = false;

            new Thread(() =>
            {
                mainHandler.GlobalTryCatch(() =>
                {
                    PageHomeAction.StartUpdateTroop(updateCards);
                });

                Dispatcher.Invoke(() =>
                {
                    btnUpdate.IsEnabled = true;
                    btnRefresh.IsEnabled = true;
                });
            }).Start();
        }

        private void OnTextChanged1(object sender, TextChangedEventArgs e)
        {
            var sb = (TextBox)sender;
            var viewSource = cardsView1;

            if (loveFilter.IsChecked ?? false)
            {
                ResetCardListByLove(viewSource, sb);
            }
            else
            {
                ResetCardListBySearch(viewSource, sb);
            }
        }

        private void OnTextChanged2(object sender, TextChangedEventArgs e)
        {
            var sb = (TextBox)sender;
            var viewSource = cardsView2;

            if (loveFilter.IsChecked ?? false)
            {
                ResetCardListByLove(viewSource, sb);
            }
            else
            {
                ResetCardListBySearch(viewSource, sb);
            }
        }

        private void OnTextChanged3(object sender, TextChangedEventArgs e)
        {
            var sb = (TextBox)sender;
            var viewSource = cardsView3;

            if (loveFilter.IsChecked ?? false)
            {
                ResetCardListByLove(viewSource, sb);
            }
            else
            {
                ResetCardListBySearch(viewSource, sb);
            }
        }

        private void OnTextChanged4(object sender, TextChangedEventArgs e)
        {
            var sb = (TextBox)sender;
            var viewSource = cardsView4;

            if (loveFilter.IsChecked ?? false)
            {
                ResetCardListByLove(viewSource, sb);
            }
            else
            {
                ResetCardListBySearch(viewSource, sb);
            }
        }

        private void OnPreviewTextInput1(object sender, TextCompositionEventArgs e)
        {
            troop1.IsDropDownOpen = true;
        }

        private void OnPreviewTextInput2(object sender, TextCompositionEventArgs e)
        {
            troop2.IsDropDownOpen = true;
        }

        private void OnPreviewTextInput3(object sender, TextCompositionEventArgs e)
        {
            troop3.IsDropDownOpen = true;
        }

        private void OnPreviewTextInput4(object sender, TextCompositionEventArgs e)
        {
            troop4.IsDropDownOpen = true;
        }

        private void terminal_TextChanged(object sender, TextChangedEventArgs e)
        {
            ((TextBox)sender).ScrollToEnd();
        }

        private void CityCrystalBtnClick(object sender, RoutedEventArgs e)
        {
            PageHomeAction.StartGetDailyCityJewel();
            InitUserProperties();
        }

        private void LoginBtnClick(object sender, RoutedEventArgs e)
        {
            PageHomeAction.StartGetDailyLoginJewel();
            InitUserProperties();
        }

        private void loveFilter_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            CheckBox cb = sender as CheckBox;

            if (cb.IsChecked ?? false)
            {
                ResetCardListByLove(cardsView1, search1);
                ResetCardListByLove(cardsView2, search2);
                ResetCardListByLove(cardsView3, search3);
                ResetCardListByLove(cardsView4, search4);
            }
            else
            {
                ResetCardListBySearch(cardsView1, search1);
                ResetCardListBySearch(cardsView2, search2);
                ResetCardListBySearch(cardsView3, search3);
                ResetCardListBySearch(cardsView4, search4);
            }
        }

        private void ResetCardListBySearch(ICollectionView viewSource, TextBox sb)
        {
            if (String.IsNullOrEmpty(sb.Text))
            {
                if (viewSource.Filter != null) viewSource.Filter = null;
                return;
            }
            else
            {
                viewSource.Filter = (x =>
                {
                    if (((CardShareData.Card)x).cardInfo.ToLower().Contains(sb.Text.ToLower())) return true;
                    else return false;
                });
            }
        }

        Predicate<object> loveFilterFuc = (x =>
        {
            var nx = ((CardShareData.Card)x);
            if (nx.heroFlag != 0) return false;
            else if (nx.cId == -1 || nx.loveLevel < nx.loveMaxLevel) return true;
            else return false;
        });

        private void ResetCardListByLove(ICollectionView viewSource, TextBox sb)
        {
            if (String.IsNullOrEmpty(sb.Text))
            {
                if (viewSource.Filter != loveFilterFuc) viewSource.Filter = loveFilterFuc;
                return;
            }
            else
            {
                viewSource.Filter = (x =>
                {
                    var nx = ((CardShareData.Card)x);
                    if (nx.heroFlag != 0) return false;
                    else if ((nx.cId == -1 || nx.loveLevel < nx.loveMaxLevel) && nx.cardInfo.ToLower().Contains(sb.Text.ToLower())) return true;
                    else return false;
                });
            }
        }
        private void BtnClearTerminalClick(object sender, EventArgs e)
        {
            terminal.Text = string.Empty;
            mainWriter.Clear();
            PageItem.ItemTerminals_Clear();
            PagePresent.PresentTerminals_Clear();
            PageQuest.QuestTerminals_Clear();
            PageCity.CityTerminals_Clear();
            WeaponSkillWindow.WeaponSkillWindowTerminals_Clear();
            DebugWindow.DebugWindow_Clear();
        }

    }
}
