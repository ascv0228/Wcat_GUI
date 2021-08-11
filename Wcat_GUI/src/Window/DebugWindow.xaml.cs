using System.Windows;

namespace Wcat_GUI
{
    /// <summary>
    /// DebugWindow.xaml 的互動邏輯
    /// </summary>
    public partial class DebugWindow : Window
    {
        public DebugWindow()
        {
            InitializeComponent();
        }

        private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }
    }
}
