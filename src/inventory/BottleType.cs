using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace robot_teachbox.src.inventory
{
    public class BottleType
    {

        public int Id;
        public string Name;
        public double GripHeight;

        public BottleType(string name, int gripHeight, int id)
        {
            Id = id;
            Name = name;
            GripHeight = gripHeight;
        }



    }
}
