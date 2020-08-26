using robot_teachbox.src.main;
using System;
using System.Collections.Generic;
using System.IO.Ports;  
using System.Threading; 

namespace robot_teachbox
{
    public class SerialCom : IDisposable
    {
        private SerialPort _serialPort;
        static bool _continue=true;

        private Thread ReadThread;

        public SerialCom(string port){
            _serialPort = new SerialPort();
            _serialPort.BaudRate = 9600;
            _serialPort.DataBits = 8;
            _serialPort.Parity = Parity.Even;
            _serialPort.StopBits = StopBits.Two;
            _serialPort.PortName=port;
            _serialPort.ReadTimeout = 1000;
            _serialPort.RtsEnable = true;
            _serialPort.Open();
            Logger.Instance.Log("Com port opened");
        }

        public SerialCom(): this("COM6") {}

        ~SerialCom(){
            _serialPort.Close();
        }

        public List<string> GetAllPorts()
        {
            List<String> allPorts = new List<String>();
            foreach (String portName in System.IO.Ports.SerialPort.GetPortNames())
            {
                allPorts.Add(portName);
            }
            return allPorts;
        }

        /// <summary>
        /// Stop Thread used for continously reading and outputting serial in data
        /// </summary>
        public void StopReadThread(){
            _continue = false;
            ReadThread.Join();
            Logger.Instance.Log("ReadThread closed");
        }

        /// <summary>
        /// Start Thread used for continously reading and outputting serial in data
        /// </summary>
        public void StartReadThread(){
            ReadThread = new Thread(this.Read);
            ReadThread.Start();
            Logger.Instance.Log("ReadThread started");
        }


        /// <summary>
        ///  Sends string to robot via serial
        /// </summary>
        /// <param name="msg">string to send</param>
        public void WriteLine(string msg){
            while(!_serialPort.CtsHolding){  //wait with sending until its ready.
                Thread.Sleep(50);
            } //Prevents the serial output from creating a buffer of commands which makes the robot unsreponsive
            Thread.Sleep(100);
            _serialPort.WriteLine(msg+"\r\n");
        }
        private void Read()
        {
            while (_continue)
            {
                try
                {
                    string message = _serialPort.ReadLine();
                    Logger.Instance.Log(message);
                }
                catch (TimeoutException e) {
                    //It will timeout alot but thats not a problem,
                    //we still dont want it to wait forever and 
                    //never be able to return. It's timeout prevents this loop from running too fast.
                    ;
                }
            }
        }

        public void Dispose()
        {
            this._serialPort.Dispose();
        }
    }


}