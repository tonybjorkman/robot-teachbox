using System;
using System.Collections.Generic;
using System.Text;

namespace robot_teachbox.src.inventory
{
    public class Bottle
    {
        public BottleType Type;
        public int AmountLeft;
        public int SlotPosIndex;
        private int amount;

        public Bottle(BottleType type, int inxPos, int amount)
        {
            this.Type = type;
            this.SlotPosIndex = inxPos;
            this.AmountLeft = amount;
        }
    }
}
