﻿using System.Windows;
using System;

namespace Wcat_GUI
{
    /// <summary>
    /// DebugWindow.xaml 的互動邏輯
    /// </summary>
    public partial class DebugWindow : Window
    {
        private static CustomWriter mainWriter;
        public DebugWindow()
        {
            InitializeComponent();
            mainWriter = new CustomWriter(terminal);
        }

        private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }
        public static void DebugWindow_Clear()
        {
            mainWriter.Clear();
        }
    }
}
