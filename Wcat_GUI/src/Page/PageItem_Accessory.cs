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
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Controls.Primitives;

namespace Wcat_GUI
{
    /// <summary>
    /// PageItem.xaml 的互動邏輯
    /// </summary>
    public partial class PageItem
    {
        private class ItemAccessorySetting
        {
            public bool? AccessoryFilterStar1Checked;
            public bool? AccessoryFilterStar2Checked;
            public bool? AccessoryFilterStar3Checked;
            public bool? AccessoryFilterStar4Checked;
            public bool? AccessoryFilterSkill1Checked;
            public bool? AccessoryFilterSkill2Checked;
            public bool? AccessoryFilterSkill3Checked;
            public bool? AccessoryFilterSkill4Checked;
            public bool? AccessoryFilterSkill5Checked;
            public bool? AccessoryFilterSkill6Checked;
            public bool? AccessoryFilterSkill7Checked;
            public bool? AccessoryFilterSkill8Checked;
            public bool? AccessoryFilterSkill99Checked;
            public bool? AccessoryFilterSkill100Checked;
            public bool? AccessoryFilterLockChecked;
            public bool? AccessoryFilterUnLockChecked;
        }

        public void CloseItemAccessoryAction()
        {
            Directory.CreateDirectory("config");
            File.WriteAllText("config/ItemAccessorySetting.dat", JsonConvert.SerializeObject(new ItemAccessorySetting()
            {
                AccessoryFilterStar1Checked = AccessoryFilterStar1.IsChecked,
                AccessoryFilterStar2Checked = AccessoryFilterStar2.IsChecked,
                AccessoryFilterStar3Checked = AccessoryFilterStar3.IsChecked,
                AccessoryFilterStar4Checked = AccessoryFilterStar4.IsChecked,
                AccessoryFilterSkill1Checked = AccessoryFilterSkill1.IsChecked,
                AccessoryFilterSkill2Checked = AccessoryFilterSkill2.IsChecked,
                AccessoryFilterSkill3Checked = AccessoryFilterSkill3.IsChecked,
                AccessoryFilterSkill4Checked = AccessoryFilterSkill4.IsChecked,
                AccessoryFilterSkill5Checked = AccessoryFilterSkill5.IsChecked,
                AccessoryFilterSkill6Checked = AccessoryFilterSkill6.IsChecked,
                AccessoryFilterSkill7Checked = AccessoryFilterSkill7.IsChecked,
                AccessoryFilterSkill8Checked = AccessoryFilterSkill8.IsChecked,
                AccessoryFilterSkill99Checked = AccessoryFilterSkill99.IsChecked,
                AccessoryFilterSkill100Checked = AccessoryFilterSkill100.IsChecked,
                AccessoryFilterLockChecked = AccessoryFilterLock.IsChecked,
                AccessoryFilterUnLockChecked = AccessoryFilterUnLock.IsChecked,
            }));
        }

        private void RestoreItemAccessorySetting()
        {
            if (File.Exists("config/ItemAccessorySetting.dat"))
            {
                var setting = JsonConvert.DeserializeObject<ItemAccessorySetting>(File.ReadAllText("config/ItemAccessorySetting.dat"));
                if (setting != null)
                {
                    AccessoryFilterStar1.IsChecked = setting.AccessoryFilterStar1Checked ?? false;
                    AccessoryFilterStar2.IsChecked = setting.AccessoryFilterStar2Checked ?? false;
                    AccessoryFilterStar3.IsChecked = setting.AccessoryFilterStar3Checked ?? false;
                    AccessoryFilterStar4.IsChecked = setting.AccessoryFilterStar4Checked ?? false;
                    AccessoryFilterSkill1.IsChecked = setting.AccessoryFilterSkill1Checked ?? false;
                    AccessoryFilterSkill2.IsChecked = setting.AccessoryFilterSkill2Checked ?? false;
                    AccessoryFilterSkill3.IsChecked = setting.AccessoryFilterSkill3Checked ?? false;
                    AccessoryFilterSkill4.IsChecked = setting.AccessoryFilterSkill4Checked ?? false;
                    AccessoryFilterSkill5.IsChecked = setting.AccessoryFilterSkill5Checked ?? false;
                    AccessoryFilterSkill6.IsChecked = setting.AccessoryFilterSkill6Checked ?? false;
                    AccessoryFilterSkill7.IsChecked = setting.AccessoryFilterSkill7Checked ?? false;
                    AccessoryFilterSkill8.IsChecked = setting.AccessoryFilterSkill8Checked ?? false;
                    AccessoryFilterSkill99.IsChecked = setting.AccessoryFilterSkill99Checked ?? false;
                    AccessoryFilterSkill100.IsChecked = setting.AccessoryFilterSkill100Checked ?? false;
                    AccessoryFilterLock.IsChecked = setting.AccessoryFilterLockChecked ?? false;
                    AccessoryFilterUnLock.IsChecked = setting.AccessoryFilterUnLockChecked ?? false;
                }
            }
        }

        private void accessoryweaponHouseBtnClick(object sender, EventArgs e)
        {
            //PanelOpen1(accessoryweaponHouseBtn, Grid Panel1);
            //PanelClose1(accessorywareHouseBtn, Grid Panel1) ;
        }

        private void accessorywareHouseBtnClick(object sender, EventArgs e)
        {
            //PanelOpen1(accessorywareHouseBtn, Grid Panel1) ;
            //PanelClose1(accessoryweaponHouseBtn, Grid Panel1);
        }

        private void AAccessoryLockClick(object sender, EventArgs e)
        {
            //if "favorite": true
            BtnLockChange(AAccessoryUnLock, AAccessoryLock);
        }

        private void AAccessoryUnLockClick(object sender, EventArgs e)
        {
            //if "favorite": true
            BtnLockChange(AAccessoryLock, AAccessoryUnLock);
        }

        private void AAccessorySellClick(object sender, EventArgs e)
        {
        }

        private void ItemAccessoryBtnSelectClick(object sender, RoutedEventArgs e)
        {

        }
    }
}
