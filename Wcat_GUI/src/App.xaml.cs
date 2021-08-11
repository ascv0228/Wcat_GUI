using System.Windows;
using System;
using System.Runtime;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using System.Threading;

namespace Wcat_GUI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        //private static Logger _logger = LogManager.GetCurrentClassLogger();
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            AppDomain.CurrentDomain.UnhandledException += (s, ex) =>
            {
                //LogUnhandledException((Exception)ex.ExceptionObject,
                //"AppDomain.CurrentDomain.UnhandledException");
                
            };

            Application.Current.DispatcherUnhandledException += (s, e) =>
            {
                e.Handled = true;
            };

            Dispatcher.UnhandledException += (s, ex) =>
            {
                //LogUnhandledException(ex.Exception,
                //"Application.Current.DispatcherUnhandledException");
                ex.Handled = true;
            };

            TaskScheduler.UnobservedTaskException += (s, ex) =>
            {
                //LogUnhandledException(ex.Exception,
                //"TaskScheduler.UnobservedTaskException");
                ex.SetObserved();
            };
        }

        static void MyHandler(object sender, UnhandledExceptionEventArgs args)
        {
            Exception e = (Exception)args.ExceptionObject;
           
            //Console.WriteLine("MyHandler caught : " + e.Message);
            //Console.WriteLine("Runtime terminating: {0}", args.IsTerminating);
        }
        void OnDispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            //string errorMessage = string.Format("An unhandled exception occurred: {0}", e.Exception.Message);
           // MessageBox.Show(errorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            // OR whatever you want like logging etc. MessageBox it's just example
            // for quick debugging etc.
            e.Handled = true;
        }

        private void LogUnhandledException(Exception e, string @event)
        {
            // Log Error here
            //e.Handled = true; //Doesn't work
        }
    }
}
