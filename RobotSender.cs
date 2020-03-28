using System;
using System.Numerics;
using System.Text;

namespace console_jogger
{
    public class RobotSender: IDisposable
    {
        private SerialCom Serial;

        public RobotSender(){
            this.Serial = new SerialCom();
            this.Serial.StartReadThread();
            var ports = this.Serial.GetAllPorts();

            foreach (var port in ports){
                System.Console.WriteLine($"port:{port}");
            }
        }

        public void Dispose()
        {
            this.Serial.StopReadThread();
            this.Serial.Dispose();
        }

        public string ByteArrayToString(char[] ba)
{
        StringBuilder hex = new StringBuilder(ba.Length * 2);
        foreach (char b in ba)
            hex.AppendFormat("{0:X}", b);
        return hex.ToString();
        }

        private string GripperAction(Vector3 v){
            if (Math.Round(v.X) == 1)
            return "RN 10,35,2";
            else if (Math.Round(v.X) == 0)
            return "RN 40,70,2";
            else
            return "";
        }

        public void ProcessCommand(CmdMsg? msg){
            
             var melfaString = msg?.Type switch {
                Command.MoveXYZ => String.Format("DS {0},{1},{2}",
                                        msg?.Vector.X,
                                        msg?.Vector.Y,
                                        msg?.Vector.Z),
                
                Command.MoveAngle => String.Format("DJ {0},{1}",
                                        msg?.Vector.X,
                                        msg?.Vector.Y
                                        ),
                Command.Where => "WH",
                Command.Gripper => GripperAction((Vector3)msg?.Vector),
                _  => null
            };


            System.Console.WriteLine($"Processing message: {msg?.Type} Vector: {msg?.Vector.ToString()}");
            Serial.WriteLine(melfaString);
        }

    }
}