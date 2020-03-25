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

        public CmdMsg(Command cmd, Vector3 v){
            Type = cmd;
            Vector = v;
        }

        public Command Type;
        public Vector3 Vector;
    }

    public class CommandInterpreter
    {
        private Settings ProgSettings;

        private delegate CmdMsg KeyMapperDelegate(ConsoleKey k);
        private KeyMapperDelegate KeyMapper;


        public CommandInterpreter(Settings s){
            this.ProgSettings = s;
            KeyMapper = GetXYZCmdMsgFromKey;
        }
        public void processKey(ConsoleKey myKey){
            CmdMsg msg = null;
            msg = KeyMapper(myKey);
            if(msg == null){
                ChangeSettingsWithKey(myKey);
            } else {
                ProcessCommand(msg);
            }
        }

        private void ProcessCommand(CmdMsg msg){
            System.Console.WriteLine($"Processing message: {msg.Type} Vector: {msg.Vector}");
        }

        private CmdMsg GetMovement(Delegate keymap, ConsoleKey key)
        {
            return null;
        }

        private CmdMsg GetXYZCmdMsgFromKey(ConsoleKey myKey){
                return myKey switch
                {
                    ConsoleKey.D4 => new CmdMsg(Command.MoveXYZ, new Vector3(0,1,0)),
                    ConsoleKey.D2 => new CmdMsg(Command.MoveXYZ, new Vector3(0,-1,0)),
                    ConsoleKey.D3 => new CmdMsg(Command.MoveXYZ, new Vector3(1,0,0)),
                    _ => null
                };
        }

        private CmdMsg GetAngleCommandFromKey(ConsoleKey myKey){
                return myKey switch
                {
                    ConsoleKey.D1 => new CmdMsg(Command.MoveAngle, new Vector3(1,1,0)),
                    ConsoleKey.D2 => new CmdMsg(Command.MoveAngle, new Vector3(1,-1,0)),
                    ConsoleKey.D3 => new CmdMsg(Command.MoveAngle, new Vector3(2,1,0)),
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
                ProgSettings.MoveType=Command.MoveXYZ;
                break;
                case ConsoleKey.A:
                KeyMapper = GetAngleCommandFromKey;
                ProgSettings.MoveType=Command.MoveAngle;
                break;
            }
        }

    }
}