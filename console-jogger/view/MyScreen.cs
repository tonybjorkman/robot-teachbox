using System;
using System.IO;

namespace console_jogger
{
    public class MyScreen
    {
        public MyScreen(){
            System.Console.WriteLine("Screen initialized");
        }

        public void SetAngleValue(int angle){
            System.Console.WriteLine($"Angle updated to {angle}");
        }
        
        public void SetXYZValue(int xyz){
            System.Console.WriteLine($"XYZ updated to {xyz}");
        }

        public void UpdateMoveType(Command type){
            System.Console.WriteLine($"Movetype updated to {type.ToString()}");
        }

        internal void PrintHelp()
        {
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "README.md");
            if (!File.Exists(path))
            {
                path = Path.Combine(path, @"..\..\..\..\..\README.md"); //if running under development, go to project root.
            }
            try
            {
                string readmeData = System.IO.File.ReadAllText(path);
                System.Console.WriteLine(readmeData);
            } catch (FileNotFoundException e)
            {
                System.Console.WriteLine("Could not open help. /n" + e.Message);
            }
            
        }
    }
}