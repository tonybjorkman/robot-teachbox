using System;

namespace robot_teachbox
{

    /// <summary>
    /// Takes a Distance and Angle as input and stores the position as a X,Y-coordinates
    /// </summary>
    public class PolarPosition
    {
        public PolarPosition()
        {
            //just some default values to help out
            Name = "unnamed";
            Height = 200;
            Distance = 300;
        }
        private double _distance;
        /// <summary>
        /// Distance set from origin (robot base)
        /// </summary>

        public string Name { get; set; }
        public double Distance 
        {
            get 
            {
                return _distance;
            } 
            set 
            {
                _distance = value;
                UpdateInternalXY();
            }
        }

        public double GetCalculatedDistance()
        {
            return Math.Sqrt(x * x + y * y);
        }

        public double GetCalculatedAngle()
        {
            return Math.Atan(y / x) * 360 / (Math.PI * 2);
        }

        public void UpdateInternalXY()
        {
            SetPolarPos(Distance, Angle);
        }

        public double Height{get {return z;} set { z = value;}}
        public double ModelAngle { get; set; }
        public double ModelDistance { get; set; }

        private double _angle;
        public double Angle
        {
            get 
            {
                return _angle;
            }
            set 
            {
                _angle = value;
                UpdateInternalXY();
            }
        }
        //roll-joint relative the robots own position. Normally it relates to global position which is bad for pouring use case.
        public double RelRoll { get; set; }
        public double Pitch { get; set; }

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