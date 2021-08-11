using System;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Wcat_GUI
{
    public class CustomWriter : StringWriter
    {
        private TextBox _textBox;
        private Dispatcher _dispatcher;

        public CustomWriter(TextBox textBox)
        {
            _dispatcher = textBox.Dispatcher;
            _textBox = textBox;
        }

        override
        public void WriteLine()
        {
            _dispatcher.Invoke(() =>
            {
                _textBox.AppendText("\n");
            });
        }

        override
        public void WriteLine(string str)
        {
            _dispatcher.Invoke(() =>
            {
                _textBox.AppendText($"{str}\n");
            });
        }

        override
        public void WriteLine(char[] str)
        {
            _dispatcher.Invoke(() =>
            {
                _textBox.AppendText($"{new String(str)}\n");
            });
        }

        override
        public void WriteLine(char ch)
        {
            _dispatcher.Invoke(() =>
            {
                _textBox.AppendText($"{ch}\n");
            });
        }

        override
        public void Write(string str)
        {
            _dispatcher.Invoke(() =>
            {
                _textBox.AppendText(str);
            });
        }

        override
        public void Write(char[] str)
        {
            _dispatcher.Invoke(() =>
            {
                _textBox.AppendText(new string(str));
            });
        }

        override
        public void Write(char ch)
        {
            _dispatcher.Invoke(() =>
            {
                _textBox.AppendText(ch.ToString());
            });
        }

        public void Clear()
        {
            _dispatcher.Invoke(() =>
            {
                _textBox.Text = "";
            });
        }
    }
}
