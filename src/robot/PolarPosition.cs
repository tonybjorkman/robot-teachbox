using System;
using System.Configuration;
using System.Text.RegularExpressions;

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


        // Where string = +10,+380.00,300.00,+50.00,+40,R,A,C

        public PolarPosition(string where)
        {
            where = where.Replace(" ", "");
            var elements = where.Split(",");
            
            x = Double.Parse(elements[0]);
            y = Double.Parse(elements[1]);
            z =  Double.Parse(elements[2]);
            _angle = GetCalculatedAngle();
            TrueRoll = Double.Parse(elements[3])+_angle;
            Pitch = Double.Parse(elements[4]);
            _distance = GetCalculatedDistance();
            
        }

        private double _distance;
        /// <summary>
        /// Distance set from origin (robot base)
        /// </summary>

        public string Name { get; set; }

        internal void OffsetHeight(double gripHeight)
        {
            z = z + gripHeight;
        }

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

        private double GetCalculatedAngle(double x, double y)
        {
            return Math.Atan(y / x) * 360 / (Math.PI * 2);
        }

        public void UpdateInternalXY()
        {
            SetPolarPos(Distance, Angle);
        }

        public double Height{get {return z;} set { z = value;}}
        public double ModelAngle; //Stores the actual angle which calc x+y
        public double ModelDistance; //stores actual distance which calc x+y

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

        public void OffsetAngle(double angle)
        {
            Angle = Angle + angle;
        }

        //roll-joint relative the robots own position. Normally it relates to global position which is bad for pouring use case.
        public double TrueRoll { get; set; }
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
                TrueRoll = GetDouble("roll angle",-120,120,0);  

        }

        //from my datamodel to the melfa.
        public string ToMelfaPosString(){
            var whereRoll = TrueRoll -_angle;
                                                        //Roll        pitch
                string output = $"{x:F2},{y:F2},{z:F2},{whereRoll:F2},{Pitch:F2}"; //Angle thing makes the origin for robot and not relative global(rotating with waist! dont want that)
                return output;
        }

        internal void UpdatePosition(PolarPosition rowPos)
        {
            x = rowPos.x;
            y = rowPos.y;
            z = rowPos.z;
            _angle = GetCalculatedAngle();
            Pitch = rowPos.Pitch;
            TrueRoll = rowPos.TrueRoll;
        }
    }
}