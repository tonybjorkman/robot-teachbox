using System;
namespace robot_teachbox
{
    public class Circle3D: PolarPosition
    {
        public double radius;

        public double start_angle;
        public double stop_angle;
        public string GetPositionAtAngle(double angle){
            //angle 0 = -90 in the unit circle representation.
            
            var tz = z+radius*Math.Sin(((angle-90)/360)*2*Math.PI);
            var tx = x+radius*Math.Sin(2*Math.PI*Angle/360)*Math.Cos(2*Math.PI*(angle-90)/360);
            var ty = y+radius*Math.Cos(2*Math.PI*Angle/360)*Math.Cos(2*Math.PI*(angle-90)/360);
            
            string output = $"{tx:F2},{ty:F2},{tz:F2},{-Angle+angle-90:F2},{90:F2}"; //Angle thing makes the origin for robot and not relative global(rotating with waist! dont want that)
            return output;
        }

        public override void InputValues(){
            var _angle = GetDouble("waist angle",-100,100,0);
            var _distance = GetDouble("origin distance in mm", 300,800,600);
            SetPolarPos(_distance,_angle);
            Height = GetDouble("height in mm",100,600,400);
            radius = GetDouble("roll-radius mm",50,300,100);
            start_angle = GetDouble("start angle",0,360,0);
            stop_angle = GetDouble("stop angle",0,360,120);
            
        }

    }
}