using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;
using System.Windows.Threading;
using System.Security.Cryptography.X509Certificates;
using System.IO;
using System.Threading;

namespace ShortCuts
{
    public partial class MainWindow : Window
    {
        //Membervariablen
        public string directoryToTemp = @"C:\PowerOFF\";
        public string tempDatei = "timetemp.tempo";
        public string link { get; set; }
        public DateTime time = DateTime.Now;
        public string shutdownTime;
        private string _selectedEnumType;


        //Konstruktor
        public MainWindow()
        {
            InitializeComponent();
            CheckIfFileExist();
            link = directoryToTemp + tempDatei;
            //shutdownTime = DateTime.Now.Ticks.ToString();         
            
        }
        //Methoden
        private void CheckIfFileExist()
        {
            string link = directoryToTemp + tempDatei;
            FileInfo fileInfo = new FileInfo(link);
            if (fileInfo.Exists == true)
            {
                using (StreamReader sr = new StreamReader(link))
                {
                    string data = sr.ReadLine();
                    if (data == null || data.Contains(""))
                    {
                        sr.Dispose();
                        CreateFile();
                        Debug.WriteLine("if1 wird ausgeführt");
                    }
                    else
                    {
                        long savedData = long.Parse(data);

                        if (savedData <= time.Ticks)
                        {
                            Debug.WriteLine("Content time.Ticks: {0}", time.Ticks);
                            Debug.WriteLine("Content savedData: {0}", savedData);
                            sr.Dispose();
                            File.Delete(link);
                            Debug.WriteLine("Das File wird gelöscht");
                        }
                        else
                        {
                            sr.Dispose();
                            Debug.WriteLine("else1 wird ausgeführt");
                        }
                    }
                }
            }
            else
            {
                CreateFile();
                Debug.WriteLine("Else2 ausgeführt");
            }
        }
        private void CreateFile()
        {
            string link = directoryToTemp + tempDatei;
            if (!File.Exists(link))
            {
                Directory.CreateDirectory(directoryToTemp);
                using (File.Create(link))
                {
                }
            }
            //vorrübergehende Testeinheit, um null zu vermeiden wir der aktuelle Wert eingetragen.
            using (StreamWriter w = new StreamWriter(link))
            {
                w.WriteLine(time.Ticks);
                w.Dispose();
            }
        }
        private string ReadFile()
        {            
            CheckIfFileExist();
            string inhalte; 
            using (StreamReader read = new StreamReader(link))
            {
                long inhalt = long.Parse(read.ReadToEnd());
                inhalte = inhalt.ToString();
                Debug.WriteLine("Inhalt der shutdownTime: {0}", inhalt.ToString());
                read.Dispose();
                
            }
            return inhalte;
        }
        private void WriteFile(long time)
        {
            string linkw = directoryToTemp + tempDatei;
            using (StreamWriter write = new StreamWriter(linkw))
            {
                write.Write(time);
                write.Dispose();
            }
        }
        private void Herunterfahren(int minutes)
        {
            int seconds = minutes * 60;
            Process.Start("shutdown", "/s /t"+ seconds);
            DateTime Zeit = time.AddSeconds(seconds);
            string countdown = Zeit.TimeOfDay.ToString();
            Debug.WriteLine("Es wird heruntergefahren in {0}", countdown);
            WriteFile(Zeit.Ticks);
        }
        private void Abbrechen()
        {
            Process.Start("shutdown", "/a");
            Debug.WriteLine("Herunterfahren wird abgebrochen");
            lblTime.Content = "-- : -- : --";
        }
        private void Verlaengern()
        {
            long variableX;
            lblTime.Content = "-- : -- : --";
            Process.Start("shutdown", "-a");
            Debug.WriteLine("Herunterfahren abgebrochen");
            long oldShutdownTime = long.Parse(ReadFile());
            int selectedIndex = comboBox.SelectedIndex;
            Debug.WriteLine("Inhalt der selectedIndex: {0}", selectedIndex);
            //long newShutdownTime = oldShutdownTime + variableX;

            //Berechnen des neuen Shutdown-Wertes, danach subtrahieren mit dem alten.
        }
        // GUI Data Binding
        private void Button_Shutdown(object sender, RoutedEventArgs e)
        {
            //muss überarbeitet werden
            try
            {
                int minutes = int.Parse(Minuten.Text);
                Herunterfahren(minutes);
                Process.Start("shutdown", "/s /t " + minutes * 60);
                DateTime Zeit = time.AddSeconds(minutes*60);
                lblTime.Content = Zeit.TimeOfDay;
                WriteFile(long.Parse(Zeit.Ticks.ToString()));
                } catch (Exception ex)
            {
                Console.WriteLine(ex);
                Process.Start("shutdown", "/s /t 60");
                DateTime Zeit = time.AddSeconds(60);
                lblTime.Content = Zeit.TimeOfDay;
                WriteFile(long.Parse(Zeit.Ticks.ToString()));
            }          
        }
        private void Button_Reset(object sender, RoutedEventArgs e)
        {
            Abbrechen();         
        }
        private void Button_Verlaengern(object sender, RoutedEventArgs e)
        {
            Verlaengern();
        }

        public string SelectedMyEnumType
        {
            get { return _selectedEnumType; }
            set { _selectedEnumType = value;
                OnPropertyChanged(SelectedMyEnumType);
            }
        }

        public IEnumerable<string> MyEnumTypeValues
        {
            get
            {
                return Enum.GetValues(typeof(string)).Cast<string>();
            }
        }
    }
        /*
        private void LoadComboBoxData(object sender, RoutedEventArgs e)
        {
            List<int> data = new List<int>();
            data.Add(5);
            data.Add(15);
            data.Add(30);
            data.Add(45);
            data.Add(60);
            data.Add(90);
            data.Add(120);

            var comboBox = sender as ComboBox;
            comboBox.ItemsSource = data;

        }
        private void ComboBox_Verlaengerung(object sender, SelectionChangedEventArgs e)
        {
            //TODO

            var comboBox = sender as ComboBox;
        }

    }
}
        */