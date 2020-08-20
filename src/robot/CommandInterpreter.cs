using System;
using static System.Console;
using System.Numerics;
using System.Windows.Input;

namespace robot_teachbox
{
    public enum Command
    {
        MoveXYZ,
        MoveAngle,
        Where,
        Gripper,
        QueryPolar,
        QueryPour,
        ToolStraight, //moves the tool tip in the same direction as its pointing to
        None
    }

    public struct CmdMsg
    {

        public CmdMsg(Command cmd, Vector3 v){
            Type = cmd;
            Vector = v;
        }

        public Command Type;
        public Vector3 Vector;

        public bool isNone(){
            return Type==Command.None;
        }

    }


    public class CommandInterpreter : IDisposable
    {
        private Settings ProgSettings;
        private delegate CmdMsg? KeyMapperDelegate(Key k);
        private KeyMapperDelegate KeyMapper;
        private RobotSender _robotSender;

        public CommandInterpreter(Settings s){
            this.ProgSettings = s;
            this._robotSender = new RobotSender(this.ProgSettings.port);
            KeyMapper = GetXYZCmdMsgFromKey;
        }

        public void processKey(Key myKey){
            myKey = NormalizeDigits(myKey);//TODO:

            CmdMsg? msg = KeyMapper(myKey);
            msg ??= StandardCommandKeys(myKey);
            if(msg == null){ //The key has not triggered any robot commands but may still be valid key for settings
                ChangeSettingsWithKey(myKey);
            } else {
                _robotSender.ProcessCommand((CmdMsg)msg);
            }
        }

        private CmdMsg? StandardCommandKeys(Key myKey){
            return myKey switch
                {
                    Key.W => new CmdMsg(Command.Where, new Vector3()),
                    Key.O => new CmdMsg(Command.Gripper, new Vector3(1)),
                    Key.C => new CmdMsg(Command.Gripper, new Vector3(0)),
                    Key.Q => new CmdMsg(Command.QueryPolar, new Vector3(0)),
                    Key.P => new CmdMsg(Command.QueryPour, new Vector3(0)),
                    _ => null
                };
        }
        private CmdMsg? GetXYZCmdMsgFromKey(Key myKey){
                var stp = ProgSettings.GetCurrentStep();
                return myKey switch
                {
                    Key.D2 => new CmdMsg(Command.MoveXYZ, new Vector3(1,0,0)*stp),
                    Key.D8 => new CmdMsg(Command.MoveXYZ, new Vector3(-1,0,0)*stp),
                    Key.D6 => new CmdMsg(Command.MoveXYZ, new Vector3(0,1,0)*stp),
                    Key.D4 => new CmdMsg(Command.MoveXYZ, new Vector3(0,-1,0)*stp),
                    Key.D5 => new CmdMsg(Command.MoveXYZ, new Vector3(0,0,1)*stp),
                    Key.D0 => new CmdMsg(Command.MoveXYZ, new Vector3(0,0,-1)*stp),
                    Key.Down => new CmdMsg(Command.ToolStraight, new Vector3(1*stp,0,0)),
                    Key.Up => new CmdMsg(Command.ToolStraight, new Vector3(-1*stp,0,0)),
                    _ => null
                };
        }

        private Key NormalizeDigits(Key myKey)
        {
            if(myKey <= Key.NumPad9 && myKey >= Key.NumPad0)
            {
                var normalized = myKey - 40;//turns numpad  to digit 
                return normalized;
            }
            return myKey;
        }

        private CmdMsg? GetAngleCommandFromKey(Key myKey){
                var stp = ProgSettings.GetCurrentStep();
                return myKey switch
                {
                    Key.D6 => new CmdMsg(Command.MoveAngle, new Vector3(1,1*stp,0)),
                    Key.D4 => new CmdMsg(Command.MoveAngle, new Vector3(1,-1*stp,0)),
                    Key.D5 => new CmdMsg(Command.MoveAngle, new Vector3(2,1*stp,0)),
                    Key.D8 => new CmdMsg(Command.MoveAngle, new Vector3(2,-1*stp,0)),
                    Key.D0 => new CmdMsg(Command.MoveAngle, new Vector3(3,1*stp,0)),
                    Key.D2 => new CmdMsg(Command.MoveAngle, new Vector3(3,-1*stp,0)),
                    Key.Down => new CmdMsg(Command.MoveAngle, new Vector3(4,1*stp,0)),
                    Key.Up => new CmdMsg(Command.MoveAngle, new Vector3(4,-1*stp,0)),
                    Key.Left => new CmdMsg(Command.MoveAngle, new Vector3(5,1*stp,0)),
                    Key.Right => new CmdMsg(Command.MoveAngle, new Vector3(5,-1*stp,0)),

                    _ => null
                };
        }

        private void ChangeSettingsWithKey(Key myKey){
            switch (myKey)
            {
                case Key.OemPlus:
                    ProgSettings.Inc();
                    break;
                case Key.OemMinus:
                    ProgSettings.Dec();
                    break;
                case Key.X:
                    KeyMapper = GetXYZCmdMsgFromKey;
                    ProgSettings.SetMovType(Command.MoveXYZ);
                    break;
                case Key.A:
                    KeyMapper = GetAngleCommandFromKey;
                    ProgSettings.SetMovType(Command.MoveAngle);
                    break;
                case Key.H:
                    ProgSettings.view.PrintHelp();
                    break;
                default:
                    if (!(myKey == Key.Escape))
                    {
                        Console.WriteLine(myKey.ToString() + " is not a valid key. Press 'h' for help.");
                    }
                    break;
            }
        }

        public void Dispose()
        {
            this._robotSender.Dispose();
        }
    }
}