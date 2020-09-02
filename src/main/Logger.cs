using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Threading;

namespace robot_teachbox.src.main
{
    class Logger
    {

        private BlockingCollection<string> _queue = new BlockingCollection<string>();
        private static readonly Logger instance = new Logger();
        private Control TextControl;

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
        private Action EmptyDelegate = delegate () { };

        public void PrintWithCurrentThread(string msg)
        {
            Console.WriteLine(msg);
            if (TextControl != null)
            {
                Refresh();
            }
        }

        public void SetControl(Control c)
        {
            TextControl = c;
        }
        //Will put the refresh of the textcontrol at high priority.
        //Used especially when printing things just prior to application close.
        public void Refresh()
        {
            TextControl.Dispatcher.Invoke(DispatcherPriority.Render, EmptyDelegate);
        }

        public int GetLength()
        {
            return _queue.Count;
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
