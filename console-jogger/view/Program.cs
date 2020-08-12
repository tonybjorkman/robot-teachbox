using System;
using System.IO.Ports;
using System.Threading;
using System.Windows.Input;
using System.Globalization;

namespace console_jogger
{
    class ConsoleJogger
    {
        
        private CommandInterpreter CmdI;
        public ConsoleJogger(){
            this.CmdI = new CommandInterpreter(new Settings(new MyScreen()));
 
        }

        static void Main(string[] args)
        {
            Console.WriteLine("App started");
            const string culture = "en-US";
            CultureInfo ci = CultureInfo.GetCultureInfo(culture);

            Thread.CurrentThread.CurrentCulture = ci;
            //Thread.Sleep(3000);
            new ConsoleJogger().Run();



        }


        public static void ClearKeyBuffer()
        {
            while (Console.KeyAvailable){
                var key = Console.ReadKey(true);
                System.Console.WriteLine("Key available:"+key);
            }
        }
        public void Run(){
            ConsoleKeyInfo myKey;

            do {
                //ClearKeyBuffer(); //We dont want to have a buffer with keystrokes, then the robot will keep making moves even when no key is pressed!
                myKey = Console.ReadKey(true);                
                //System.Console.WriteLine(myKey.Key.ToString());
                CmdI.processKey(myKey.Key);
            } while (myKey.Key != ConsoleKey.Escape);

            //get rid of all unmanaged resources and extra thread for serial
            CmdI.Dispose();
        }

    }

}
