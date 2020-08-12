using System;

namespace console_jogger
{
    public class PolarPosition
    {
        public double Distance 
        {
            get 
            {
                return Math.Sqrt(x*x+y*y);
            } 
            set 
            {
                y = Math.Sin(2*Math.PI*Angle/360)*Distance;
                x = Math.Cos(2*Math.PI*Angle/360)*Distance;
            }
        }
        public double Height{get {return z;} set { z = value;}}
        public double Angle
        {
            get 
            {
                return Math.Atan(y/x)*360/(Math.PI*2);
            }
            set 
            {
                y = Math.Sin(2*Math.PI*value/360)*Distance;
                x = Math.Cos(2*Math.PI*value/360)*Distance;   
            }
        }
        //roll-joint relative the robots own position. Normally it relates to global position which is bad for pouring use case.
        double RelRoll;
        double Pitch;

        public double x;
        public double y;
        public double z;

        public void SetPolarPos(double _distance, double _angle){
                y = Math.Sin(2*Math.PI*_angle/360)*_distance;
                x = Math.Cos(2*Math.PI*_angle/360)*_distance;
        }

        protected double GetDouble(string msg, int min, int max, int _default){
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

        public virtual void InputValues(){
                var _angle = GetDouble("waist angle",-100,100,0);
                var _distance = GetDouble("origin distance in mm", 300,800,600);
                SetPolarPos(_distance,_angle);
                Height = GetDouble("height in mm",100,600,400);
                Pitch = GetDouble("pitch angle",-90,90,0);
                RelRoll = GetDouble("roll angle",-120,120,0);  

        }

        public string ToMelfaPosString(){
                string output = $"{x:F2},{y:F2},{z:F2},{-Angle-90:F2},{Pitch+90:F2}"; //Angle thing makes the origin for robot and not relative global(rotating with waist! dont want that)
                return output;
        }

    }
}