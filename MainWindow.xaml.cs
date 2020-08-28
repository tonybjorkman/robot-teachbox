﻿using System;
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
using System.IO;
using System.Text.Json;

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
            /*var grabPositions = new ObservableCollection<PositionGrab>();
            grabPositions.Add(new PositionGrab());
            this.dataGrid.ItemsSource = (grabPositions);
            this.dataGrid.BeginEdit();

            var pourPositions = new ObservableCollection<Circle3D>();
            pourPositions.Add(new Circle3D());
            this.dataCirclePourGrid.ItemsSource = (pourPositions);*/
            LoadPositionFromFile();

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

        private void SavePositionsToFile()
        {
            string workingDirectory = Environment.CurrentDirectory;
            // or: Directory.GetCurrentDirectory() gives the same result
            // This will get the current PROJECT directory
            string projectDirectory = Directory.GetParent(workingDirectory).Parent.FullName;
            string fullPathGrab = System.IO.Path.Combine(projectDirectory, "grab_saved.txt");
            string fullPathCircle = System.IO.Path.Combine(projectDirectory, "circle_saved.txt");

            using (System.IO.StreamWriter file = new System.IO.StreamWriter(fullPathGrab))
            {
                var gridJson = JsonSerializer.Serialize(this.dataGrid.ItemsSource);
                file.Write(gridJson);
            }
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(fullPathCircle))
            {
                var gridJson = JsonSerializer.Serialize(this.dataCirclePourGrid.ItemsSource);
                file.Write(gridJson);
            }
        }

        //#TODO: remove duplication
        private void LoadPositionFromFile()
        {
            string workingDirectory = Environment.CurrentDirectory;
            // or: Directory.GetCurrentDirectory() gives the same result
            // This will get the current PROJECT directory
            string projectDirectory = Directory.GetParent(workingDirectory).Parent.FullName;
            string fullPathGrab = System.IO.Path.Combine(projectDirectory, "grab_saved.txt");
            string fullPathCircle = System.IO.Path.Combine(projectDirectory, "circle_saved.txt");

            using (System.IO.StreamReader file = new System.IO.StreamReader(fullPathGrab))
            {
                var gridJson = file.ReadToEnd();
                var positions = JsonSerializer.Deserialize<ObservableCollection<PositionGrab>>(gridJson);
                this.dataGrid.ItemsSource=positions;
            }
            using (System.IO.StreamReader file = new System.IO.StreamReader(fullPathCircle))
            {
                var gridJson = file.ReadToEnd();
                var positions = JsonSerializer.Deserialize<ObservableCollection<Circle3D>>(gridJson);
                this.dataCirclePourGrid.ItemsSource = positions;
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

                if (e.Key == Key.Up || e.Key == Key.Down || e.Key == Key.Left || e.Key == Key.Right)
                {
                    // dont let the arrowkeys change control focus(tab between controls)
                    e.Handled = true;
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
        { "btn_keyNumPad8", "-X[8]" },
        { "btn_keyUp", "-TStrght[▲]" },
        { "btn_keyDown", "+TStrght[▼]" },
        { "btn_keyLeft", "[◄]" },
        { "btn_keyRight", "[►]" }};

        Dictionary<string, string> numpadNameMapAngle = new Dictionary<string, string> {
        { "btn_keyNumPad0", "-Elbow[0]" },
        { "btn_keyNumPad2", "+Elbow[2]" },
        { "btn_keyNumPad4", "-Waist[4]" },
        { "btn_keyNumPad5", "+Shoulder[5]" },
        { "btn_keyNumPad6", "+Waist[6]" },
        { "btn_keyNumPad8", "-Shoulder[8]" },
        { "btn_keyUp", "-Pitch[▲]" },
        { "btn_keyDown", "+Pitch[▼]" },
        { "btn_keyLeft", "+Roll[◄]" },
        { "btn_keyRight", "-Roll[►]" }};


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
                //Only update the buttons associated with movementtype
                if (btnNames.Keys.Contains(btn.Name))
                {
                    btn.Content = btnNames[btn.Name];
                }
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
            
            //this.dataGrid.CommitEdit();
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

        public async void OnWindowClosing(object sender, CancelEventArgs e )
        {
            Logger.Instance.Log("Exiting application");
            SavePositionsToFile();
            await Task.Delay(1000); //Give the logger time to show exit.
            RobotSend.Dispose();
        }

        //For test
        private void testButton1_Click(object sender, RoutedEventArgs e)
        {
            LoadPositionFromFile();
        }

        //for test
        private void testButton2_Click(object sender, RoutedEventArgs e)
        {
            SavePositionsToFile();
        }

        private void exitClicked(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {

        }

        private void DataGrid_GotoButton_Click(object sender, RoutedEventArgs e)
        {

            Action<DataGrid> GotoSelectedGridPos = (grid) =>
            {
                //finish the edit-event and refresh items in case the
                //previous selection was editing a cell.
                grid.CommitEdit();
                grid.Items.Refresh();
                var rows = grid.SelectedCells;
                var pos = rows.ElementAt(0).Item as PolarPosition;
                if (pos != null)
                {
                    Logger.Instance.Log(pos.ToMelfaPosString());
                    var msg = new CmdMsg(Command.MovePosition, pos);
                    RobotSend.Send(msg);
                }
            };

            var btn = sender as Button;

            if (btn == null){ return; }
            else if (btn.Name == "GotoGrab")
            {
                GotoSelectedGridPos(this.dataGrid);
            } else if (btn.Name == "GotoCircle")
            {
                GotoSelectedGridPos(this.dataCirclePourGrid);
            }

        }

        private void DataGrid_SetButton_Click(object sender, RoutedEventArgs e)
        {
            Action<DataGrid> SetPosToCurrent = (grid) =>
            {
                //finish the edit-event and refresh items in case the
                //previous selection was editing a cell.
                grid.CommitEdit();
                grid.Items.Refresh();
                var rows = grid.SelectedCells;
                var robotPosNow = RobotSend.GetPosition();
                var rowPos = rows.ElementAt(0).Item as PolarPosition;
                rowPos.UpdatePosition(robotPosNow);
            };

            var btn = sender as Button;

            if (btn == null) { return; }
            else if (btn.Name == "SetGrab")
            {
                SetPosToCurrent(this.dataGrid);
            }
            else if (btn.Name == "SetCircle")
            {
                SetPosToCurrent(this.dataCirclePourGrid);
            }
        }

        private void dataCirclePourGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            Logger.Instance.Log("row eedit eneded");
        }
    }
}
