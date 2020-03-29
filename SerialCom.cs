using System;
using System.Collections.Generic;
using System.IO.Ports;  
using System.Threading; 

namespace console_jogger
{
    public class SerialCom : IDisposable
    {
        private SerialPort _serialPort;
        static bool _continue=true;

        private Thread ReadThread;

        public SerialCom(){
            _serialPort = new SerialPort();
            _serialPort.BaudRate = 9600;
            _serialPort.DataBits = 8;
            _serialPort.Parity = Parity.Even;
            _serialPort.StopBits = StopBits.Two;
            _serialPort.PortName="COM1";
            _serialPort.ReadTimeout = 1000;
            _serialPort.RtsEnable = true;
            _serialPort.Open();
            System.Console.WriteLine("Com port opened");
        }

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

        public void StopReadThread(){
            _continue = false;
            ReadThread.Join();
            System.Console.WriteLine("ReadThread closed");
        }

        public void StartReadThread(){
            ReadThread = new Thread(this.Read);
            ReadThread.Start();
            System.Console.WriteLine("ReadThread started");
        }


        public void WriteLine(string msg){
            while(!_serialPort.CtsHolding){  //wait with sending until its ready.

            } //Prevents the serial output from creating a buffer of commands which makes the robot unsreponsive
            _serialPort.WriteLine(msg+"\r\n");
        }
        public void Read()
        {
            while (_continue)
            {
                try
                {
                    string message = _serialPort.ReadLine();
                    Console.WriteLine(message);
                }
                catch (TimeoutException) { 
                    //It will timeout alot but thats not a problem,
                    //we still dont want it to wait forever and 
                    //never be able to return
                }
            }
        }

        public void Dispose()
        {
            this._serialPort.Dispose();
        }
    }


}