using System;
using System.IO.Ports;
using System.Threading;

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
            //Thread.Sleep(3000);
            new ConsoleJogger().Run();

        }

        public void Run(){
            ConsoleKeyInfo myKey;

            do {
                myKey = Console.ReadKey();
                System.Console.WriteLine(myKey.Key.ToString());
                CmdI.processKey(myKey.Key);
            } while (myKey.Key != ConsoleKey.Escape);

        }

    }

}
