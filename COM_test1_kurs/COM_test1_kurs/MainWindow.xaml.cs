using System;
using System.Windows;
using System.Timers;
using System.IO.Ports;

namespace COM_test1_kurs
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        System.Timers.Timer aTimer;
        SerialPort currentPort;
        private delegate void updateDelegate(string txt);

        public MainWindow()
        {
            InitializeComponent();
        }

        private bool ArduinoDetected()
        {
            try
            {
                currentPort.Open();
                System.Threading.Thread.Sleep(2000);
                string ReturnMessage = currentPort.ReadLine();
                currentPort.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            bool ArduinoPortFounded = false;
            try
            {
                    currentPort = new SerialPort("COM4", 9600);
                    if (ArduinoDetected())
                    {
                        ArduinoPortFounded = true;
                      
                    }
                    else
                    {
                        ArduinoPortFounded = false;
                    }
            }
            catch { }
            if (ArduinoPortFounded == false) return;
            System.Threading.Thread.Sleep(500);
            currentPort.BaudRate = 9600;
            currentPort.DtrEnable = true;
            currentPort.ReadTimeout = 1000;
            try
            {
                currentPort.Open();
            }
            catch { }
            aTimer = new System.Timers.Timer(1000);
            aTimer.Elapsed += OnTimedEvent;
            aTimer.AutoReset = true;
            aTimer.Enabled = true;
        }

        private void OnTimedEvent(object sender, ElapsedEventArgs e)
        {
            if (!currentPort.IsOpen) return;
            try
            {
                currentPort.DiscardInBuffer();
                string StrFromPort = currentPort.ReadLine();
                lblPortData.Dispatcher.BeginInvoke(new updateDelegate(UpdateTextBox), StrFromPort);
            }
            catch(Exception exc)
            {
                lblPortData.Dispatcher.BeginInvoke(new updateDelegate(UpdateTextBox), exc.Message);
            }
        }

        private void UpdateTextBox(string txt)
        {
            lblPortData.Content = txt;
        }

        private void Windows_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            aTimer.Enabled = false;
            currentPort.Close();
        }
    }
}