using System;
using System.Linq;
using System.Numerics;

namespace robot_teachbox
{
    public class Circle3D: PolarPosition
    {
        public double Radius { get; set; }
        public double StartAngle { get; set; }
        public double StopAngle { get; set; }
        public Boolean Reverse { get; set; }

        private readonly double TRUE_ROLL_BOTTLE_STRAIGHT_OFFSET = 47;

        /// <summary>
        /// Pours around the centerpoint defined by the PolarPosition it inherits.
        /// So with StartAngle 0 it starts a full radius below the centerpoint and the Bottle points upwards. With -90 degree angle it will 
        /// start with the bottle horizontally and to the right side of where it pours (as seen from the robots POV).
        /// </summary>
        public Circle3D() :base()
        {
            //just some nice values to start out with
            Radius = 100;
            StartAngle = 0;
            StopAngle = 120;
        }

        public Circle3D(string where):base(where)
        {
            Radius = 100;
        }

        public override void UpdatePosition(PolarPosition rowPos)
        {
            base.UpdatePosition(rowPos);
            StartAngle = TrueRoll + TRUE_ROLL_BOTTLE_STRAIGHT_OFFSET;
            StopAngle = StartAngle - 30; //tilt 30* as standard.
        }


        private readonly double ANGLE_CORRECTION = 41; //the angle of zero should hold a bottle straight and motorside up.

        public string[] GetCirclePositionStrings()
        {
            var angles = new String[]{ GetPositionAtAngle(StartAngle),
                GetPositionAtAngle((StartAngle+StopAngle)/2),
                GetPositionAtAngle(StopAngle)};

            if (Reverse)
            {
                angles = angles.Reverse().ToArray();
            }
            return angles;
        }

        private Vector3 Get3DPerpendicularToolOffsetByAngle(double angle)
        {
            var xyz = new Vector3();
            xyz.Z = (float)(z + Radius * Math.Sin(((angle - 90) / 360) * 2 * Math.PI));
            xyz.X = (float)(x - Radius * Math.Sin(2 * Math.PI * Angle / 360) * Math.Cos(2 * Math.PI * (angle - 90) / 360));
            xyz.Y= (float)(y + Radius * Math.Cos(2 * Math.PI * Angle / 360) * Math.Cos(2 * Math.PI * (angle - 90) / 360));
            return xyz;
        }

        public string GetPositionAtAngle(double angle){
            //angle 0 = -90 in the unit circle representation.
            
            var tz = z+Radius*Math.Sin(((angle-90)/360)*2*Math.PI);
            var tx = x-Radius*Math.Sin(2*Math.PI*Angle/360)*Math.Cos(2*Math.PI*(angle - 90)/360);
            var ty = y+Radius*Math.Cos(2*Math.PI*Angle/360)*Math.Cos(2*Math.PI*(angle - 90)/360);

            var currAngle = GetCalculatedAngle(tx, ty);

            string output = $"{tx:F2},{ty:F2},{tz:F2},{angle-currAngle+ ANGLE_CORRECTION - 90:F2},{90:F2}"; //Angle thing makes the origin for robot and not relative global(rotating with waist! dont want that)

            return output;
        }

        public PolarPosition GetPositionAtStartAngle()
        {
            return new PolarPosition(GetPositionAtAngle(StartAngle));
        }
        /// <summary>
        /// Used in conjunction to position-creation if the current tool-position should
        /// be on the circle arc instead of in the middle of it.
        /// The position will be placed on the startpoint of the circlearc.
        /// </summary>
        public void OffsetRotationCenterToTip()
        {
            var offsetVector = Get3DPerpendicularToolOffsetByAngle(StartAngle - 180); //get the opposite of the (start angle position relative rotation center)
            x = offsetVector.X;
            y = offsetVector.Y;
            z = offsetVector.Z;
            

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