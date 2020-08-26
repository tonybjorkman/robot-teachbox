using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Windows.Documents;

namespace robot_teachbox.src.main
{
    class Logger
    {

        private BlockingCollection<string> _queue = new BlockingCollection<string>();
        private static readonly Logger instance = new Logger();

        // Explicit static constructor to tell C# compiler
        // not to mark type as beforefieldinit
        static Logger()
        {
        }

        private Logger()
        {
        }

        public static Logger Instance
        {
            get
            {
                return instance;
            }
        }

        public void Log(string msg)
        {
            _queue.Add(msg);
        }

        public void Print() {
            
            string msg;
            while (_queue.TryTake(out msg))
            {
                Console.WriteLine(msg);
            }
        }



    }
}
