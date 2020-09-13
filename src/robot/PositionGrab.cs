using System;
using System.Collections.Generic;
using System.Text;

namespace robot_teachbox.src.robot
{
    public class PositionGrab : PolarPosition
    {

        public double GrabLength { get; set; }

        //Tells if the operation is grabbing or releasing grip
        public bool Grab { get; set; }

        public PositionGrab()
        {
        }


    }
}
