using robot_teachbox.src.main;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace robot_teachbox
{
    public class SerialCom : IDisposable
    {
        private SerialPort _serialPort;
        static bool _continue=true;

        private Task ReadThread;
        private object readLock = new object();

        public SerialCom(){
            _serialPort = new SerialPort();
            _serialPort.BaudRate = 9600;
            _serialPort.DataBits = 8;
            _serialPort.Parity = Parity.Even;
            _serialPort.StopBits = StopBits.Two;
            _serialPort.ReadTimeout = 1000;
            _serialPort.RtsEnable = true;
        }

        public void OpenPort(string port)
        {
            try
            {
                _serialPort.PortName = port;
                _serialPort.Open();
                Logger.Instance.Log($"Com port {port} opened successfully");
            } catch (UnauthorizedAccessException e)
            {
                Logger.Instance.Log(e.Message);
                throw;
            } catch (ArgumentException e)
            {
                Logger.Instance.Log(e.Message);
                throw;
            }
        }

        ~SerialCom(){
            _serialPort.Close();
        }

        public bool IsConnected()
        {
            return _serialPort.IsOpen;
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
        public Task StopReadThread(){
            if (ReadThread != null)
            {
                _continue = false;
                Logger.Instance.Log("ReadThread closed");
            }
            return ReadThread;
        }

        /// <summary>
        /// Start Thread used for continously reading and outputting serial in data
        /// </summary>
        public void StartReadThread(){
            ReadThread = Task.Run(this.Read);
            Logger.Instance.Log("ReadThread started");
        }


        /// <summary>
        ///  Sends string to robot via serial
        /// </summary>
        /// <param name="msg">string to send</param>
        public void WriteLine(string msg){

            if (!_serialPort.CtsHolding)
            {
                Logger.Instance.Log("#SERIAL: Awaiting Clear To Send from port..");
            }

            while(!_serialPort.CtsHolding){  //wait with sending until its ready.
                Thread.Sleep(50);
            } //Prevents the serial output from creating a buffer of commands which makes the robot unsreponsive
            Thread.Sleep(100);
            _serialPort.WriteLine(msg+"\r\n");
            Logger.Instance.Log("#SERIAL O: " + msg);
        }

        public string WriteRead(string msg)
        {
            string reply;
            lock (readLock)
            {
                try
                {
                    WriteLine(msg);
                    Logger.Instance.Log("WR: Awaiting read");
                    _serialPort.ReadTimeout = 20000;
                    reply = _serialPort.ReadLine();
                    _serialPort.ReadTimeout = 200;
                    Logger.Instance.Log("WR: Read " + reply);
                } catch (TimeoutException)
                {
                    reply = "";
                    Logger.Instance.Log("#SERIAL Timeout waiting for WHERE response");
                }

            }
            return reply;
        }

        private async Task Read()
        {
            while (_continue)
            {
                try
                {
                    lock (readLock)
                    {
                        string message = _serialPort.ReadLine();
                        Logger.Instance.Log("#SERIAL I: " + message);
                    }
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
            this._serialPort.Close();
            this._serialPort.Dispose();
        }
    }


}