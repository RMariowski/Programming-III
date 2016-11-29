using System;
using System.Windows;
using System.Windows.Controls;

namespace Client
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public Models.Client Client { get; private set; }

        public MainWindow Window { get; set; }

        public Frame MainFrame { get; set; }

        /// <summary>
        /// Konstruktor
        /// </summary>
        public App()
        {
            Client = new Models.Client();
        }

        /// <summary>
        /// Wykonuje akcje w glownym watku WPF.
        /// </summary>
        /// <param name="action">Akcja do wykonania</param>
        public void Invoke(Action action)
        {
            MainFrame.Dispatcher.Invoke(action, null);
        }
    }
}
