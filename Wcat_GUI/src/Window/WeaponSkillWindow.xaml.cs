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

namespace Wcat_GUI
{
    /// <summary>
    /// WeaponSkillWindow.xaml 的互動邏輯
    /// </summary>
    public partial class WeaponSkillWindow : Window
    {
        public WeaponSkillWindow()
        {
            InitializeComponent();
        }


        private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }
        private void SoloQuestBtnEndClick(object sender, RoutedEventArgs e)
        {
        }
        private void SoloQuestBtnStartClick(object sender, RoutedEventArgs e)
        {
        }
    }
}
