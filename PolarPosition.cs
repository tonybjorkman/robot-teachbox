using System;

namespace console_jogger
{
    public class PolarPosition
    {
        public double Distance {get;set;}
        double Height;
        double Angle;
        //roll-joint relative the robots own position. Normally it relates to global position which is bad for pouring use case.
        double RelRoll;
        double Pitch;

        private double GetDouble(string msg, int min, int max, int _default){
                var succeed =false;
                double result;
                do{
                System.Console.Write($"Set {msg}({min},{max} or Enter for {_default}): ");
                var input = Console.ReadLine();
                    if (input==""){
                        return _default;
                    }
                succeed = double.TryParse(input, out result);
                } while (!succeed || result < min || result > max);
                return result;

        }

        public object Clone(){
            return this.MemberwiseClone();
        }

        public void InputValues(){
                Angle = GetDouble("waist angle",-100,100,0);
                
                Distance = GetDouble("origin distance in mm", 300,800,600);
                Height = GetDouble("height in mm",100,600,400);
                Pitch = GetDouble("pitch angle",-90,90,0);
                RelRoll = GetDouble("roll angle",-120,120,0);    
        }

        public string ToMelfaPosString(){
            double x,y,z;
                y = Math.Sin(2*Math.PI*Angle/360)*Distance;
                x = Math.Cos(2*Math.PI*Angle/360)*Distance;
                z = Height; 
                
                string output = $"{x:F2},{y:F2},{z:F2},{-Angle-90:F2},{Pitch:F2}";
                return output;

        }

    }
}