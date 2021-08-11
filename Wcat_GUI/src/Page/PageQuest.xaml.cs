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
        public PageQuest()
        {
            InitializeComponent();

            RestoreQuestSetting();

            /******************************/
            SoloQuestWriter = new CustomWriter(SoloQuestTerminal);
            SoloQuestHandler = new ExceptionHandler(SoloQuestWriter);

            CoopQuestWriter = new CustomWriter(CoopQuestTerminal);
            CoopQuestHandler = new ExceptionHandler(CoopQuestWriter);

            WeaponEnhanceWriter = new CustomWriter(WeaponEnhanceTerminal);
            WeaponEnhanceHandler = new ExceptionHandler(WeaponEnhanceWriter);

            powerWriter = new CustomWriter(InjectpowerTerminal);
            infoWriter = new CustomWriter(InjectinfoTerminal);
            /******************************/

            Buffers.pageQuestMsgs.CollectionChanged += OnCollectionChanged_SoloQuest;
            Buffers.pageCoopQuestMsgs.CollectionChanged += OnCollectionChanged_CoopQuest;
            Buffers.pageWeaponEnhanceMsgs.CollectionChanged += OnCollectionChanged_WeaponEnhance;



            /******************************/

            ShowSelectedSoloQuestInfo();
            /////////////////////////////////////////////////////////////////

            ShowSelectedCoopQuestInfo();
            /////////////////////////////////////////////////////////////////



            /////////////////////////////////////////////////////////////////

            PageInjectionAction.LoadItemsInfo();
            InjectitemList.ItemsSource = PageInjectionAction.itemsMapTable;


            ShowSelectedInjectItems();
            ShowPowerInfo();
        }

        public void RestoreQuestSetting()
        {
            RestoreSoloQuestSetting();
            RestoreCoopQuestSetting();
            //RestoreWeaponEnhanceSetting();
            RestoreInjectSetting();
            //RestoreExploreSetting();
        }
        public void CloseQuestAction()
        {
            CloseSoloQuestAction();
            CloseCoopQuestAction();
            //CloseWeaponEnhanceAction();
            CloseInjectAction();
            //CloseExploreAction();
        }

        public static void QuestTerminals_Clear()
        {
            SoloQuestWriter.Clear();
            CoopQuestWriter.Clear();
            WeaponEnhanceWriter.Clear();
        }
        private void PanelClose2(Button Btn, Grid Panel1, Grid Panel2)
        {
            Btn.IsEnabled = true;
            Btn.Background = Brushes.SkyBlue;
            Panel1.Visibility = Visibility.Hidden;
            Panel2.Visibility = Visibility.Hidden;
        }
        private void PanelOpen2(Button Btn, Grid Panel1, Grid Panel2)
        {
            Btn.IsEnabled = false;
            Btn.Background = Brushes.DodgerBlue;
            Panel1.Visibility = Visibility.Visible;
            Panel2.Visibility = Visibility.Visible;
        }
        private void SoloQuestBtnClick(object sender, System.Windows.RoutedEventArgs e)
        {
            PanelOpen2(SoloQuestBtn, SoloQuestPanel1, SoloQuestPanel2);
            PanelClose2(CoopQuestBtn, CoopQuestPanel1, CoopQuestPanel2);
            PanelClose2(WeaponEnhanceBtn, WeaponEnhancePanel1, WeaponEnhancePanel2);
            PanelClose2(JetTravelBtn, JetTravelPanel1, JetTravelPanel2);
            PanelClose2(ItemInjectBtn, ItemInjectPanel1, ItemInjectPanel2);
            PanelClose2(ExploreBtn, ExplorePanel1, ExplorePanel2);
        }
        private void CoopQuestBtnClick(object sender, System.Windows.RoutedEventArgs e)
        {
            PanelOpen2(CoopQuestBtn, CoopQuestPanel1, CoopQuestPanel2);
            PanelClose2(SoloQuestBtn, SoloQuestPanel1, SoloQuestPanel2);
            PanelClose2(WeaponEnhanceBtn, WeaponEnhancePanel1, WeaponEnhancePanel2);
            PanelClose2(JetTravelBtn, JetTravelPanel1, JetTravelPanel2);
            PanelClose2(ItemInjectBtn, ItemInjectPanel1, ItemInjectPanel2);
            PanelClose2(ExploreBtn, ExplorePanel1, ExplorePanel2);
        }
        private void WeaponEnhanceBtnClick(object sender, System.Windows.RoutedEventArgs e)
        {
            PanelOpen2(WeaponEnhanceBtn, WeaponEnhancePanel1, WeaponEnhancePanel2);
            PanelClose2(SoloQuestBtn, SoloQuestPanel1, SoloQuestPanel2);
            PanelClose2(CoopQuestBtn, CoopQuestPanel1, CoopQuestPanel2);
            PanelClose2(JetTravelBtn, JetTravelPanel1, JetTravelPanel2);
            PanelClose2(ItemInjectBtn, ItemInjectPanel1, ItemInjectPanel2);
            PanelClose2(ExploreBtn, ExplorePanel1, ExplorePanel2);
        }
        private void JetTravelBtnClick(object sender, System.Windows.RoutedEventArgs e)
        {
            PanelOpen2(JetTravelBtn, JetTravelPanel1, JetTravelPanel2);
            PanelClose2(SoloQuestBtn, SoloQuestPanel1, SoloQuestPanel2);
            PanelClose2(CoopQuestBtn, CoopQuestPanel1, CoopQuestPanel2);
            PanelClose2(WeaponEnhanceBtn, WeaponEnhancePanel1, WeaponEnhancePanel2);
            PanelClose2(ItemInjectBtn, ItemInjectPanel1, ItemInjectPanel2);
            PanelClose2(ExploreBtn, ExplorePanel1, ExplorePanel2);
        }
        private void ItemInjectBtnClick(object sender, System.Windows.RoutedEventArgs e)
        {
            PanelOpen2(ItemInjectBtn, ItemInjectPanel1, ItemInjectPanel2);
            PanelClose2(SoloQuestBtn, SoloQuestPanel1, SoloQuestPanel2);
            PanelClose2(CoopQuestBtn, CoopQuestPanel1, CoopQuestPanel2);
            PanelClose2(WeaponEnhanceBtn, WeaponEnhancePanel1, WeaponEnhancePanel2);
            PanelClose2(JetTravelBtn, JetTravelPanel1, JetTravelPanel2);
            PanelClose2(ExploreBtn, ExplorePanel1, ExplorePanel2);
        }
        private void ExploreBtnClick(object sender, System.Windows.RoutedEventArgs e)
        {
            PanelOpen2(ExploreBtn, ExplorePanel1, ExplorePanel2);
            PanelClose2(ItemInjectBtn, ItemInjectPanel1, ItemInjectPanel2);
            PanelClose2(SoloQuestBtn, SoloQuestPanel1, SoloQuestPanel2);
            PanelClose2(CoopQuestBtn, CoopQuestPanel1, CoopQuestPanel2);
            PanelClose2(WeaponEnhanceBtn, WeaponEnhancePanel1, WeaponEnhancePanel2);
            PanelClose2(JetTravelBtn, JetTravelPanel1, JetTravelPanel2);
        }
        private void terminal_TextChanged(object sender, TextChangedEventArgs e)
        {
            ((TextBox)sender).ScrollToEnd();
        }
    }
}
