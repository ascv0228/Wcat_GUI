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
using System.Windows.Threading;
using Wcat.PageAction;
using Wcat.Stream;


namespace Wcat_GUI.Exceptions
{
    public class ExceptionHandler
    {
        private CustomWriter _writer;
        
        public ExceptionHandler(CustomWriter writer)
        {
            _writer = writer;
        }

        public void GlobalTryCatch(Action action)
        {
            try
            {
                action.Invoke();
            }
            catch (ThreadInterruptedException ex)
            {
                _writer.WriteLine("已停止");
            }
            catch (Exception ex)
            {
                _writer.WriteLine("發生未知錯誤");
                Console.WriteLine(ex);
            }
        }
    }
}
