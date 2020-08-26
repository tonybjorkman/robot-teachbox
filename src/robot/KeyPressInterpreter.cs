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

    public class CmdMsg
    {

        public CmdMsg(Command cmd, Vector3 v){
            Type = cmd;
            Vector = v;
        }

        public CmdMsg(Command cmd, PolarPosition p)
        {
            Type = cmd;
            Position = p;
        }

        public Command Type;
        public Vector3 Vector;

        //this member should be used instead of Vector and after a while Vector should be deprecated
        public PolarPosition Position;
        

        public bool isNone(){
            return Type==Command.None;
        }

    }

    /// <summary>
    /// Responsible for converting a user input(a keyboard-input) into either a change in ProgramSettings or 
    /// a robot movement command.
    /// 
    /// Contains state for features which are toggled. 
    /// 
    /// </summary>
    public class KeyPressInterpreter : IDisposable
    {
        private Settings ProgSettings;
        //private RobotSender _robotSender;

        public KeyPressInterpreter(Settings s){
            this.ProgSettings = s;
            //this._robotSender = new RobotSender(this.ProgSettings.port);
        }

        public CmdMsg? processKey(Key myKey){
            myKey = NormalizeDigits(myKey);//TODO:
            CmdMsg? msg = null;

            //Prog contains state which dictates how to interpret msgs
            if (ProgSettings.CurrentMoveType.Type == Command.MoveXYZ)
            {
                msg = GetXYZCmdMsgFromKey(myKey);
            } else if(ProgSettings.CurrentMoveType.Type == Command.MoveAngle)
            {
                msg = GetAngleCommandFromKey(myKey);
            }
            msg ??= StandardCommandKeys(myKey);


            if (msg == null){ //The key has not triggered any robot commands but may still be valid key for settings
                ChangeSettingsWithKey(myKey);
            } else {
                return (CmdMsg)msg; //return the robot command.
            }

            return null;
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
                    ProgSettings.SetMovType(Command.MoveXYZ);
                    break;
                case Key.A:
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
            //nothing to dispose so far
        }
    }
}