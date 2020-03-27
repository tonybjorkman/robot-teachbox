using System;
using static System.Console;
using System.Numerics;
namespace console_jogger
{
    public enum Command
    {
        MoveXYZ,
        MoveAngle
    }

    public class CmdMsg
    {

        public CmdMsg(Command cmd, Vector3 v, int length){
            Type = cmd;
            Vector = v;
            Length = length;
        }

        public Command Type;
        public Vector3 Vector;

        public int Length;
    }

    public class CommandInterpreter : IDisposable
    {
        private Settings ProgSettings;
        


        private delegate CmdMsg KeyMapperDelegate(ConsoleKey k);
        private KeyMapperDelegate KeyMapper;
        private RobotSender _robotSender;

        public CommandInterpreter(Settings s){
            this.ProgSettings = s;
            this._robotSender = new RobotSender();
            KeyMapper = GetXYZCmdMsgFromKey;
        }

        public void processKey(ConsoleKey myKey){
            CmdMsg msg = null;
            msg = KeyMapper(myKey);
            if(msg == null){
                ChangeSettingsWithKey(myKey);
            } else {
                _robotSender.ProcessCommand(msg);
            }
        }



        private CmdMsg GetMovement(Delegate keymap, ConsoleKey key)
        {
            return null;
        }

        private CmdMsg GetXYZCmdMsgFromKey(ConsoleKey myKey){
                var stp = ProgSettings.GetCurrentStep();
                return myKey switch
                {
                    ConsoleKey.D4 => new CmdMsg(Command.MoveXYZ, new Vector3(0,1,0),stp),
                    ConsoleKey.D2 => new CmdMsg(Command.MoveXYZ, new Vector3(0,-1,0),stp),
                    ConsoleKey.D3 => new CmdMsg(Command.MoveXYZ, new Vector3(1,0,0),stp),
                    _ => null
                };
        }

        private CmdMsg GetAngleCommandFromKey(ConsoleKey myKey){
                var stp = ProgSettings.GetCurrentStep();
                return myKey switch
                {
                    ConsoleKey.D1 => new CmdMsg(Command.MoveAngle, new Vector3(1,1,0),stp),
                    ConsoleKey.D2 => new CmdMsg(Command.MoveAngle, new Vector3(1,-1,0),stp),
                    ConsoleKey.D3 => new CmdMsg(Command.MoveAngle, new Vector3(2,1,0),stp),
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