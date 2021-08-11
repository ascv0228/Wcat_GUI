using System.Windows;

namespace Wcat_GUI
{
    /// <summary>
    /// UpdateInfoWindow.xaml 的互動邏輯
    /// </summary>
    public partial class UpdateInfoWindow : Window
    {
        public UpdateInfoWindow()
        {
            InitializeComponent();

            SetText("版本 [1.0.0 / 17] 更新訊息: \n\n" +
                    "*修正自動淬鍊失敗的問題\n" +
                    "*修正單人「銀河新年2019」找不到關卡的問題");
            SetText("版本 [1.0.0 / 16] 更新訊息: \n\n" +
                    "*修正部分關卡搜尋卡死的問題");
            SetText("版本 [1.0.0 / 15] 更新訊息: \n\n" +
                    "*修復部分協力bug\n" +
                    "*修正單人「神將降臨IXA戰爭律動」無法三冠的問題\n\n" +
                    "*道具注入改為可用活動名和id等查詢\n" +
                    "*強化[自動換隊]功能");
            SetText("版本 [1.0.0 / 14] 更新訊息: \n\n" +
                    "*武器淬鍊新增多條件功能");
            SetText("版本 [1.0.0 / 13] 更新訊息: \n\n" +
                    "*修正茶雄2016的問題\n\n" +
                    "*武器淬鍊添加屬傷選項");
            SetText("版本 [1.0.0 / 12] 更新訊息: \n\n" +
                    "*修正部分關卡找不到的問題\n\n" +
                    "*新增一鍵淬鍊的功能");
            SetText("版本 [1.0.0 / 11] 更新訊息: \n\n" +
                    "*修正Boss類型關卡無法正確掃蕩的問題\n" +
                    "*修正[重複]選項的小bug\n\n" +
                    "*禮物盒功能添加新的選項");
            SetText("版本 [1.0.0 / 10] 更新訊息: \n\n" +
                    "*修正單人「吉爾貝斯達物語～野公主與義勇騎士～」找不到關卡的問題\n" +
                    "*修正單人「無限討伐任務2」無法掃蕩的問題\n\n" +
                    "*新增更新後顯示更新內容的功能");
        }

        public void SetText(string content)
        {
            if (string.IsNullOrEmpty(terminal.Text))
            {
                terminal.Text = content;
            }
            else
            {
                terminal.Text += "\n\n=====================================================\n\n" + content;
            }
        }
    }
}
