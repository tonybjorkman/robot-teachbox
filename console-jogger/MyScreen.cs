using System;

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

        public void updateMoveType(Command type){
            System.Console.WriteLine($"Movetype updated to {type.ToString()}");
        }

    }
}