using System;
using System.Numerics;
using System.Text;
using System.Threading;

namespace robot_teachbox
{
    public class RobotSender: IDisposable
    {
        private SerialCom Serial;

        public RobotSender(string port){
            this.Serial = new SerialCom(port);
            this.Serial.StartReadThread();
            /*var ports = this.Serial.GetAllPorts();

            foreach (var port in ports){
                System.Console.WriteLine($"port:{port}");
            }*/
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

        /// <summary>
        /// Get the command used for powering up the gripper in either Close or Open operation
        /// 
        /// </summary>
        /// <param name="v">for X=1 Opens the Grip, X=0 Closes the grip</param>
        /// <returns>Melfa string command</returns>
        private string GripperAction(Vector3 v){
            if (Math.Round(v.X) == 1)
            return "RN 10,35,2";
            else if (Math.Round(v.X) == 0)
            return "RN 40,70,2";
            else
            return "";
        }

        /// <summary>
        /// Grabs object with the gripper starting as open at the supplied position and then moving depth mm in the tool direction.
        /// </summary>
        /// <param name="pos">Position before it moves forward and grips</param>
        /// <param name="depth">The distance it moves forward to grip</param>
        public void GrabPolar(PolarPosition pos, double depth){
            MovePosition(pos.ToMelfaPosString());
            //Thread.Sleep(1300);
            ProcessCommand(new CmdMsg(Command.Gripper, new Vector3(1)));
            Thread.Sleep(1300);
            var grabPos = (PolarPosition) pos.Clone();
            ProcessCommand(new CmdMsg(Command.ToolStraight, new Vector3((float)depth)));
            //Thread.Sleep(3000);
            //Thread.Sleep(1300);
            ProcessCommand(new CmdMsg(Command.Gripper, new Vector3(0)));
            Thread.Sleep(1300);
            ProcessCommand(new CmdMsg(Command.ToolStraight, new Vector3((float)-depth)));
        }

        /// <summary>
        /// Writes a position into the robot controller memory. This is needed for some of the movement
        /// operations such as moving in arcs.
        /// </summary>
        /// <param name="pos_inx">Memory index in the Controller</param>
        /// <param name="position">5 axis-position</param>
        public void WritePositionToController(int pos_inx, string position){
            Serial.WriteLine($"PD {pos_inx},{position},R,A");
        }

        /// <summary>
        /// Moves along the circumference of a pre-defined 3D-circle and keeps the roll-angle pointing towards center.
        /// </summary>
        /// <param name="circle"></param>
        public void MoveAroundCirclePoint(Circle3D circle){
            WritePositionToController(20,circle.GetPositionAtAngle(circle.start_angle));
            WritePositionToController(21,circle.GetPositionAtAngle((circle.start_angle+circle.stop_angle)/2));
            WritePositionToController(22,circle.GetPositionAtAngle(circle.stop_angle));
            Serial.WriteLine($"MR {20},{21},{22}");
        }

        /// <summary>
        /// Asks for user input to determine movement of tooltip in arc and then makes the move as a single circular move.
        /// </summary>
        /// <returns></returns>
        public string QueryPour(){
            var circle = new Circle3D();
            circle.InputValues();
            MoveAroundCirclePoint(circle);
            
            return ""; //All commands already sent, do no more
        }

        /// <summary>
        /// Pours liquid from container.
        ///  Starts by rotating roll intil startangle is reached which occurs at the same time as target X,Y is reached. 
        /// Raises the Z-pos at tooltip at the same time as it rotates the roll such that the tip of the
        /// container will not move in Z-pos.
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="angle"></param>
        /// <param name="duration_millis"></param>




        /// <summary>
        /// Degrees is anti-clockwise, 0* is straight ahead
        /// </summary>
        /// <returns></returns>
        public string QueryPolarGrab(){
                var pos = new PolarPosition();
                pos.InputValues();
                GrabPolar(pos,120);
                return "";
        }


        public void MovePosition(string melfaString){
                Serial.WriteLine("MP "+melfaString);
        }

        public string MoveToolStraight(float distance){
                Serial.WriteLine("HE 66"); //Save current position as position_index 66 as temporary pos
                Serial.WriteLine($"MTS 66,{distance:F2}");
                return ""; //if this is called by process command, let it send empty string.
        }

        public void ProcessCommand(CmdMsg msg){

             var melfaString = msg.Type switch {
                Command.MoveXYZ => String.Format("DS {0},{1},{2}",
                                        msg.Vector.X,
                                        msg.Vector.Y,
                                        msg.Vector.Z),
                
                Command.MoveAngle => String.Format("DJ {0},{1}",
                                        msg.Vector.X,
                                        msg.Vector.Y
                                        ),
                Command.ToolStraight => MoveToolStraight(msg.Vector.X),
                Command.Where => "WH",
                Command.Gripper => GripperAction((Vector3)msg.Vector),
                Command.QueryPolar => QueryPolarGrab(),
                Command.QueryPour => QueryPour(),
                _  => null
            };


            System.Console.WriteLine($"Processing message: {msg.Type} Vector: {msg.Vector.ToString()}");
            Serial.WriteLine(melfaString);
        }

    }
}