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
using System.Threading;
using System.Globalization;
using System.IO.Ports;

namespace robot_teachbox
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static CommandInterpreter CmdI;
        static Settings ProgSettings;


        public MainWindow()
        {

            InitializeComponent();

            Console.WriteLine("App started");
            const string culture = "en-US";
            CultureInfo ci = CultureInfo.GetCultureInfo(culture);
            Thread.CurrentThread.CurrentCulture = ci;
            Console.SetOut(new view.ControlWriter(textBox));
            ProgSettings = new Settings(new MyScreen());
            Console.WriteLine("App started");

            var ports = SerialPort.GetPortNames();

            foreach(string port in ports)
            {
                comboBox.Items.Add(port);
            }
            if (ports.Count() > 0)
            {
                comboBox.SelectedIndex = 0;
            }
            this.DataContext = ProgSettings;
            InitializeControls();
        }

        private void InitializeControls()
        {
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            this.button.IsEnabled = false;
            Run();
        }

        

        private void standard_move_key_click(object sender, RoutedEventArgs e)
        {
            var btnPress = sender as Button;
            string key = btnPress.Name.Replace("btn_key", "");
            Key parsedKey;
            try
            {
                Enum.TryParse(key, out parsedKey);
                ProcessKeyActivation(parsedKey);
                this.button.IsEnabled = false;
                Console.WriteLine("got:" + parsedKey.ToString());
            } catch (ArgumentException ex)
            {
                Console.WriteLine(ex);
            }
            
        }


        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            ProcessKeyActivation(e.Key);

        }

        private void ProcessKeyActivation(Key k)
        {
            switch (k)
            {
                case Key.A:
                    ChangeMovementType(Command.MoveAngle);
                    break;
                case Key.X:
                    ChangeMovementType(Command.MoveXYZ);
                    break;
                default:
                    CmdI.processKey(k);
                    break;
            }
        }

        private static void Run()
        {
            CmdI = new CommandInterpreter(ProgSettings);
        }

        private void comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                var port = comboBox.SelectedItem.ToString();
                Console.WriteLine("Selected port:" + port);
                ProgSettings.SetPort(port);
            } catch (NullReferenceException exp)
            {
                ;
            }
            
        }

        private void ChangeMovementType(Command cmd)
        {
            if(cmd == Command.MoveAngle)
            {
                this.radio1.IsChecked = false;
                this.radio2.IsChecked = true;
                CmdI.processKey(Key.A);
                setViewMovementBtnLabels(cmd);
            } else if(cmd == Command.MoveXYZ)
            {
                CmdI.processKey(Key.X);
                this.radio1.IsChecked = true;
                this.radio2.IsChecked = false;
                setViewMovementBtnLabels(cmd);
            }
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton r = sender as RadioButton;
            if (ProgSettings != null)
            {
                if ((bool)this.radio1.IsChecked)
                {
                    CmdI.processKey(Key.X);
                    setViewMovementBtnLabels(Command.MoveXYZ);
                } else if ((bool)this.radio2.IsChecked)
                {
                    CmdI.processKey(Key.A);
                    setViewMovementBtnLabels(Command.MoveAngle);
                }
            }
        }

        Dictionary<string, string> numpadNameMapXYZ = new Dictionary<string, string> { 
        { "btn_keyNumPad0", "-Z[0]" },
        { "btn_keyNumPad2", "+X[2]" },
        { "btn_keyNumPad4", "-Y[4]" },
        { "btn_keyNumPad5", "+Z[5]" },
        { "btn_keyNumPad6", "+Y[6]" },
        { "btn_keyNumPad8", "-X[8]" }};

        Dictionary<string, string> numpadNameMapAngle = new Dictionary<string, string> {
        { "btn_keyNumPad0", "-Elbow[0]" },
        { "btn_keyNumPad2", "+Elbow[2]" },
        { "btn_keyNumPad4", "-Waist[4]" },
        { "btn_keyNumPad5", "+Shoulder[5]" },
        { "btn_keyNumPad6", "+Waist[6]" },
        { "btn_keyNumPad8", "-Shoulder[8]" }};


        private void setViewMovementBtnLabels(Command movType)
        {
            Dictionary<string, string> btnNames=null;
            if(movType == Command.MoveAngle)
            {
                btnNames = numpadNameMapAngle;
            } else if (movType == Command.MoveXYZ)
            {
                btnNames = numpadNameMapXYZ;
            }

            var btns = numpadGrid.Children;
            foreach(var element in btns)
            {
                var btn = element as Button;
                btn.Content = btnNames[btn.Name];
            }
        }
    }
}
