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
using System.Collections.ObjectModel;
using System.ComponentModel;
using robot_teachbox.src.robot;
using robot_teachbox.view;
using robot_teachbox.src.main;

namespace robot_teachbox
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// Works on top of the KeyPressHandler which is the original console-solution
    /// which primarily accepts Keypresses.
    /// </summary>
    public partial class MainWindow : Window
    {
        static KeyPressInterpreter KeyPressInt;
        static RobotSender RobotSend;
        static Settings ProgSettings;
        bool keyEventActive = true;

        public MainWindow()
        {
            InitializeComponent();

            Console.WriteLine("App started");
            const string culture = "en-US";
            CultureInfo ci = CultureInfo.GetCultureInfo(culture);
            Thread.CurrentThread.CurrentCulture = ci;
            Console.SetOut(new view.ControlWriter(textBox));
            Logger.Instance.Log("hello world");
            InitializeRobotObjects();
            InitializeControls();

            //The UI-thread must be the one to print to console since its bound to a UI-control. 
            //therefore implemented here as async code which runs for the entire life-time.
            Func<Task> x = (async () => {
                while (true)
                {
                    await Task.Delay(500);
                    Logger.Instance.Print();
                }
            });
            x();

        }



        private void InitializeControls()
        {
            var grabPositions = new ObservableCollection<PositionGrab>();
            grabPositions.Add(new PositionGrab());
            this.dataGrid.ItemsSource = (grabPositions);
            this.dataGrid.BeginEdit();

            var pourPositions = new ObservableCollection<Circle3D>();
            pourPositions.Add(new Circle3D());
            this.dataCirclePourGrid.ItemsSource = (pourPositions);

            var ports = SerialPort.GetPortNames();
            Closing += this.OnWindowClosing;

            foreach (string port in ports)
            {
                comboBox.Items.Add(port);
            }
            if (ports.Count() > 0)
            {
                comboBox.SelectedIndex = 0;
            }
        }

        private void InitializeRobotObjects()
        {
            ProgSettings = new Settings(new MyScreen());
            this.DataContext = ProgSettings;
            KeyPressInt = new KeyPressInterpreter(ProgSettings);
            RobotSend = new RobotSender();
            RobotSend.StartProcess();
            Console.WriteLine("App started, select COM-port and press connect");
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            
            this.button.IsEnabled = !RobotSend.OpenPort(ProgSettings.port);
        }

        private void Standard_move_key_click(object sender, RoutedEventArgs e)
        {
            var btnPress = sender as Button;
            string key = btnPress.Name.Replace("btn_key", "");
            Key parsedKey;
            try
            {
                Enum.TryParse(key, out parsedKey);
                var cmd = ProcessKeyActivation(parsedKey);
                if (cmd != null)
                {
                    RobotSend.Send((CmdMsg)cmd);
                }
                Console.WriteLine("got:" + parsedKey.ToString());
            } catch (ArgumentException ex)
            {
                Console.WriteLine(ex);
            }
            
        }


        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (keyEventActive)
            {
                CmdMsg? msg = ProcessKeyActivation(e.Key);
                if (msg != null)
                {
                    RobotSend.Send((CmdMsg)msg);
                }
            }
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

        private CmdMsg? ProcessKeyActivation(Key k)
        {
            switch (k)
            { // Special cases where we want to  do something in addition to the KeyPressInterpreter
                case Key.A:
                    UpdateMovementTypeView(Command.MoveAngle);
                    break;
                case Key.X:
                    UpdateMovementTypeView(Command.MoveXYZ);
                    break;
            }
            return KeyPressInt.processKey(k);
        }

        private void UpdateMovementTypeView(Command cmd)
        {
            if (cmd == Command.MoveAngle)
            {
                this.radio1.IsChecked = false;
                this.radio2.IsChecked = true;
                SetViewMovementBtnLabels(cmd);
            }
            else if (cmd == Command.MoveXYZ)
            {
                KeyPressInt.processKey(Key.X);
                this.radio1.IsChecked = true;
                this.radio2.IsChecked = false;
                SetViewMovementBtnLabels(cmd);
            }
        }

        private void MovementType_RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("radiobutton triggered");
            RadioButton r = sender as RadioButton;
            if (ProgSettings != null)
            {
                if ((bool)this.radio1.IsChecked)
                {
                    KeyPressInt.processKey(Key.X);
                    SetViewMovementBtnLabels(Command.MoveXYZ);
                }
                else if ((bool)this.radio2.IsChecked)
                {
                    KeyPressInt.processKey(Key.A);
                    SetViewMovementBtnLabels(Command.MoveAngle);
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


        private void SetViewMovementBtnLabels(Command movType)
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

        private void DataGrid_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            keyEventActive = false;
            Console.WriteLine("Keyinput disabled while editing");
        }

        private void DataGrid_LostFocus(object sender, DataGridCellEditEndingEventArgs e)
        {
            keyEventActive = true;
            Console.WriteLine("Edit finished, Keyinput enabled");
        }

        private void PolarPosRow_Button_Click(object sender, RoutedEventArgs e)
        {
            var rows = this.dataGrid.SelectedCells;
            var pos = rows.ElementAt(0).Item as PositionGrab;
            //we only care about a single selection, a button should map to only its own row.
            if (pos != null)
            {
                //RobotSend.GrabPolar(pos);
                RobotSend.Send(new CmdMsg(Command.QueryPolar,(PolarPosition)pos));
                Console.WriteLine("Grab polar"+pos.ToMelfaPosString());
            }
        }


        private void Circle3DPosRow_Button_Click(object sender, RoutedEventArgs e)
        {
            var rows = this.dataCirclePourGrid.SelectedCells;
            var pos = rows.ElementAt(0).Item as Circle3D;
            //we only care about a single selection, a button should map to only its own row.
            if (pos != null)
            {
                RobotSend.Send(new CmdMsg(Command.QueryPour, (PolarPosition)pos));
                Console.WriteLine("Pour:"+pos.ToMelfaPosString());
            }
        }

        public void OnWindowClosing(object sender, CancelEventArgs e )
        {
            RobotSend.Dispose();
        }

        //For test
        private void testButton1_Click(object sender, RoutedEventArgs e)
        {
            Logger.Instance.Log("Connected="+RobotSend.IsConnected());
        }

        //for test
        private void testButton2_Click(object sender, RoutedEventArgs e)
        {
            RobotSend.Send(KeyPressInt.processKey(Key.D8));
        }

    }
}
