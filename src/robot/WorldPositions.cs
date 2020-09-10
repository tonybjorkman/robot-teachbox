using robot_teachbox.src.inventory;
using System;
using System.Collections.Generic;
using System.Text;

namespace robot_teachbox.src.robot
{
    public class WorldPositions
    {
        private const double BOTTLE_DISTANCE = 400;
        private const double BOTTLE_DEGREE_INTERVAL = 30;

        private const double DRINKSLOT_DISTANCE = 400;
        private const double DRINKSLOT_DEGREE_INTERVAL = 20;


        private static PolarPosition ZeroBottle = new PolarPosition("+10, -380.00, 100.00, +90.00, +0, R, A, C");
        private static PolarPosition ZeroDrinkSlot = new PolarPosition("+20, +220.00, 200.00, +50.00, +40, R, A, C");

        private PolarPosition GetBottleOrigin()
        {
            return ZeroBottle.Clone() as PolarPosition;
        }

        private PolarPosition GetDrinkSlotOrigin()
        {
            return ZeroDrinkSlot.Clone() as PolarPosition;
        }

        /// <summary>
        /// Converts between bottle inx and PolarPosition
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public PolarPosition GetBottlePosition(Bottle bottle)
        {
            var bottlePos = GetBottleOrigin(); 
            bottlePos.OffsetAngle(bottle.SlotPosIndex*BOTTLE_DEGREE_INTERVAL);
            bottlePos.OffsetHeight(bottle.Type.GripHeight);
            return bottlePos;
        }

        /// <summary>
        /// Converts between drinkslot inx and PolarPosition
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public PolarPosition GetDrinkSlotPosition(int slot)
        {
            var drinkPos = GetDrinkSlotOrigin();
            drinkPos.OffsetAngle(slot*DRINKSLOT_DEGREE_INTERVAL);
            return drinkPos;
        }

    }
}
