using robot_teachbox.src.main;
using robot_teachbox.src.robot;
using robot_teachbox.view;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq.Expressions;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace robot_teachbox
{
    public class RobotSender: IDisposable
    {
        private SerialCom Serial;
        public BlockingCollection<CmdMsg> CmdQueue { get; set; }
        private readonly CancellationTokenSource StopToken;
        private Task ConsumerLoopTask;

        public RobotSender(){

            this.CmdQueue = new BlockingCollection<CmdMsg>();
            this.StopToken = new CancellationTokenSource();
            this.Serial = new SerialCom();
        }

        public void Stop()
        {
            StopToken.Cancel();
        }

        public void Dispose()
        {
            var task = this.Serial.StopReadThread();
            Console.WriteLine("Waiting for serial readthread to close");
            if (task != null)
            {
                task.Wait();
            }
            Console.WriteLine("Thread closed");

            this.Serial.Dispose();
            this.StopToken.Cancel();
            Logger.Instance.Log("RobotSender disposed");
        }

        public bool IsConnected()
        {
            return this.Serial.IsConnected();
        }

        public bool OpenPort(string port)
        {
            try
            {
                this.Serial.OpenPort(port);
                this.Serial.StartReadThread();
                return true;
            } catch (ArgumentException)
            {
                return false;
            }
            catch (UnauthorizedAccessException)
            {
                return false;
            }
        }

        public void StartProcess()
        {
            var tok = StopToken.Token;
            ConsumerLoopTask = Task.Run(ConsumerLoop);
            Logger.Instance.Log("Process initiated start");
        }

        public Task ConsumerLoop()
        {
            Debug.WriteLine("RobotSenderWorker started");

            while(true)
            {
                try
                {
                    CmdMsg newCmd;
                    this.CmdQueue.TryTake(out newCmd, -1, StopToken.Token);
                    // process
                    Debug.WriteLine("Received Msg:" + newCmd.ToString());
                    if (Serial.IsConnected())
                    {
                        ProcessCommand(newCmd);
                    } 
                    else
                    {
                        Logger.Instance.Log($"Not connected, discarding command {newCmd.Type}");
                    }
                }
                catch (OperationCanceledException)
                {
                    break;
                }
            }
            Debug.WriteLine("RobotSenderWorker finished");
            return Task.CompletedTask;
        }

        public void Send(CmdMsg item)
        {
            Logger.Instance.Log("Added item to send queue");
            this.CmdQueue.Add(item);
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
            return "RN 10,35,2"; //Runs a program which contains start-delay-stop the relayout
            else if (Math.Round(v.X) == 0)
            return "RN 40,70,2"; //Runs a program which contains start-delay-stop the relayout
            else
            return "";
        }

        /// <summary>
        /// Manipulates an object with the gripper. Either starting as open at the supplied position and then moving depth mm in the tool direction to grip and return or release and return.
        /// Prereq: To grab, the gripper needs to be in an initialy open state.
        /// </summary>
        /// <param name="pos">Position before it moves forward and grips</param>
        public String GrabPolar(PositionGrab pos){
            MovePosition(pos.ToMelfaPosString());
            //Thread.Sleep(1300);
            
            /*if (pos.Grab) //is this a grab operation? Then open before grabbing
            {
                ProcessCommand(new CmdMsg(Command.Gripper, new Vector3(1))); //open
            }*/
            Thread.Sleep(1300);
            //await Task.Delay(1300);
            var grabPos = (PolarPosition) pos.Clone();
            ProcessCommand(new CmdMsg(Command.ToolStraight, new Vector3((float)pos.GrabLength)));
            //Thread.Sleep(3000);
            //Thread.Sleep(1300);
            if (pos.Grab) //is this a grab or release operation?
            {
                ProcessCommand(new CmdMsg(Command.Gripper, new Vector3(0))); //close
            }
            else
            {
                ProcessCommand(new CmdMsg(Command.Gripper, new Vector3(1))); //open
            }
            Thread.Sleep(1300);
            //await Task.Delay(1300);
            ProcessCommand(new CmdMsg(Command.ToolStraight, new Vector3((float)-pos.GrabLength)));
            return "";
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
            WritePositionToController(20,circle.GetPositionAtAngle(circle.StartAngle));
            WritePositionToController(21,circle.GetPositionAtAngle((circle.StartAngle+circle.StopAngle)/2));
            WritePositionToController(22,circle.GetPositionAtAngle(circle.StopAngle));
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
        /*public string QueryPolarGrab(){
                var pos = new PolarPosition();
                pos.InputValues();
                GrabPolar(pos,120);
                return "";
        }*/ //will be replaced since the console is no longer used to query for input.


        public void MovePosition(string melfaString){
                Serial.WriteLine("MP "+melfaString);
        }

        public string MoveToolStraight(float distance){
                Serial.WriteLine("HE 66"); //Save current position as position_index 66 as temporary pos
                Serial.WriteLine($"MTS 66,{distance:F2}");
                return ""; //if this is called by process command, let it send empty string.
        }

        public PolarPosition GetPosition()
        {
            string posString = Serial.WriteRead("WH");
            return new PolarPosition(posString);

        }

        public void ProcessCommand(CmdMsg msg){

            string str;
             switch (msg.Type) 
            {
                case Command.MoveXYZ: 
                    str = String.Format("DS {0},{1},{2}",
                                        msg.Vector.X,
                                        msg.Vector.Y,
                                        msg.Vector.Z);
                    Serial.WriteLine(str);
                    break;

                case Command.MoveAngle: 
                    str = String.Format("DJ {0},{1}",
                                        msg.Vector.X,
                                        msg.Vector.Y
                                        );
                    Serial.WriteLine(str);
                    break;
                case Command.ToolStraight:
                    str = MoveToolStraight(msg.Vector.X);
                    Serial.WriteLine(str);
                    break;
                case Command.Where:
                    Serial.WriteLine("WH");
                    break;
                case Command.Gripper:
                    str = GripperAction((Vector3)msg.Vector);
                    Serial.WriteLine(str);
                    break;
                case Command.QueryPolar:
                    GrabPolar((PositionGrab)msg.Position);
                    break;
                case Command.QueryPour:
                    MoveAroundCirclePoint((Circle3D)msg.Position);
                    break;
                case Command.MovePosition:
                    MovePosition(msg.Position.ToMelfaPosString());
                    break;
                default:
                    throw new NotImplementedException("No implementation to deal with this CmdMsg type");
            };

            Logger.Instance.Log($"Processing message: {msg.Type} Vector: {msg.Vector.ToString()}");
        }


    }
}