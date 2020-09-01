using System;
namespace robot_teachbox
{
    public class Circle3D: PolarPosition
    {
        public double Radius { get; set; }

        public double StartAngle { get; set; }
        public double StopAngle { get; set; }

        public Circle3D() :base()
        {
            //just some nice values to start out with
            Radius = 100;
            StartAngle = 0;
            StopAngle = 120;
        }

        public string GetPositionAtAngle(double angle){
            //angle 0 = -90 in the unit circle representation.
            
            var tz = z+Radius*Math.Sin(((angle-90)/360)*2*Math.PI);
            var tx = x-Radius*Math.Sin(2*Math.PI*Angle/360)*Math.Cos(2*Math.PI*(angle-90)/360);
            var ty = y+Radius*Math.Cos(2*Math.PI*Angle/360)*Math.Cos(2*Math.PI*(angle-90)/360);

            string output = $"{tx:F2},{ty:F2},{tz:F2},{-Angle+angle-90:F2},{90:F2}"; //Angle thing makes the origin for robot and not relative global(rotating with waist! dont want that)

            return output;
        }

        public override void InputValues(){
            var _angle = GetDouble("waist angle",-100,100,0);
            var _distance = GetDouble("origin distance in mm", 300,800,600);
            SetPolarPos(_distance,_angle);
            Height = GetDouble("height in mm",100,600,400);
            Radius = GetDouble("roll-radius mm",50,300,100);
            StartAngle = GetDouble("start angle",0,360,0);
            StopAngle = GetDouble("stop angle",0,360,120);
            
        }

    }
}