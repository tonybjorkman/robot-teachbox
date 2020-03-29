using System;
using static System.Console;
using System.Numerics;
namespace console_jogger
{
    public enum Command
    {
        MoveXYZ,
        MoveAngle,
        Where,
        Gripper,
        QueryPolar,
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
        private delegate CmdMsg? KeyMapperDelegate(ConsoleKey k);
        private KeyMapperDelegate KeyMapper;
        private RobotSender _robotSender;

        public CommandInterpreter(Settings s){
            this.ProgSettings = s;
            this._robotSender = new RobotSender();
            KeyMapper = GetXYZCmdMsgFromKey;
        }

        public void processKey(ConsoleKey myKey){
            
            CmdMsg? msg = KeyMapper(myKey);
            msg ??= StandardCommandKeys(myKey);
            if(msg == null){ //The key has not triggered any robot commands but may still be valid key for settings
                ChangeSettingsWithKey(myKey);
            } else {
                _robotSender.ProcessCommand(msg);
            }
        }

        private CmdMsg? StandardCommandKeys(ConsoleKey myKey){
            return myKey switch
                {
                    ConsoleKey.W => new CmdMsg(Command.Where, new Vector3()),
                    ConsoleKey.O => new CmdMsg(Command.Gripper, new Vector3(1)),
                    ConsoleKey.C => new CmdMsg(Command.Gripper, new Vector3(0)),
                    ConsoleKey.Q => new CmdMsg(Command.QueryPolar, new Vector3(0)),
                    _ => null
                };
        }
        private CmdMsg? GetXYZCmdMsgFromKey(ConsoleKey myKey){
                var stp = ProgSettings.GetCurrentStep();
                return myKey switch
                {
                    ConsoleKey.D2 => new CmdMsg(Command.MoveXYZ, new Vector3(1,0,0)*stp),
                    ConsoleKey.D8 => new CmdMsg(Command.MoveXYZ, new Vector3(-1,0,0)*stp),
                    ConsoleKey.D6 => new CmdMsg(Command.MoveXYZ, new Vector3(0,1,0)*stp),
                    ConsoleKey.D4 => new CmdMsg(Command.MoveXYZ, new Vector3(0,-1,0)*stp),
                    ConsoleKey.D5 => new CmdMsg(Command.MoveXYZ, new Vector3(0,0,1)*stp),
                    ConsoleKey.D0 => new CmdMsg(Command.MoveXYZ, new Vector3(0,0,-1)*stp),
                    _ => null
                };
        }

        private CmdMsg? GetAngleCommandFromKey(ConsoleKey myKey){
                var stp = ProgSettings.GetCurrentStep();
                return myKey switch
                {
                    ConsoleKey.D6 => new CmdMsg(Command.MoveAngle, new Vector3(1,1*stp,0)),
                    ConsoleKey.D4 => new CmdMsg(Command.MoveAngle, new Vector3(1,-1*stp,0)),
                    ConsoleKey.D5 => new CmdMsg(Command.MoveAngle, new Vector3(2,1*stp,0)),
                    ConsoleKey.D8 => new CmdMsg(Command.MoveAngle, new Vector3(2,-1*stp,0)),
                    ConsoleKey.D0 => new CmdMsg(Command.MoveAngle, new Vector3(3,1*stp,0)),
                    ConsoleKey.D2 => new CmdMsg(Command.MoveAngle, new Vector3(3,-1*stp,0)),
                    ConsoleKey.DownArrow => new CmdMsg(Command.MoveAngle, new Vector3(4,1*stp,0)),
                    ConsoleKey.UpArrow => new CmdMsg(Command.MoveAngle, new Vector3(4,-1*stp,0)),
                    ConsoleKey.LeftArrow => new CmdMsg(Command.MoveAngle, new Vector3(5,1*stp,0)),
                    ConsoleKey.RightArrow => new CmdMsg(Command.MoveAngle, new Vector3(5,-1*stp,0)),

                    _ => null
                };
        }

        private void ChangeSettingsWithKey(ConsoleKey myKey){
            switch (myKey)
            {
                case ConsoleKey.OemPlus:
                ProgSettings.Inc();
                break;
                case ConsoleKey.OemMinus:
                ProgSettings.Dec();
                break;
                case ConsoleKey.X:
                KeyMapper = GetXYZCmdMsgFromKey;
                ProgSettings.SetMovType(Command.MoveXYZ);
                break;
                case ConsoleKey.A:
                KeyMapper = GetAngleCommandFromKey;
                ProgSettings.SetMovType(Command.MoveAngle);
                break;
            }
        }

        public void Dispose()
        {
            this._robotSender.Dispose();
        }
    }
}