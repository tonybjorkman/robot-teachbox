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


        private static PolarPosition ZeroBottle = new PolarPosition("+463, -476.00, 193.00, -92.00, +90, R, A, C");
        private static PolarPosition ZeroDrinkSlot = new PolarPosition("+660, -78.00, 392.00, -90.00, +90, R, A, C");

        private PositionGrab GetBottleOrigin()
        {
            var pos = new PositionGrab();
            pos.UpdatePosition(ZeroBottle);
            pos.TrueRoll = 0;
            return pos;
        }

        private Circle3D GetDrinkSlotOrigin()
        {
            var pos = new Circle3D();
            pos.UpdatePosition(ZeroDrinkSlot);
            return pos;
        }

        /// <summary>
        /// Converts between bottle inx and PolarPosition
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public PositionGrab GetBottlePosition(Bottle bottle)
        {
            var bottlePos = GetBottleOrigin();
            bottlePos.GrabLength = 100;
            bottlePos.OffsetAngle(bottle.SlotPosIndex*BOTTLE_DEGREE_INTERVAL);
            bottlePos.OffsetHeight(bottle.Type.GripHeight);
            return bottlePos;
        }

        /// <summary>
        /// Converts between drinkslot inx and PolarPosition
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Circle3D GetDrinkSlotPosition(int slot)
        {
            var drinkPos = GetDrinkSlotOrigin();
            drinkPos.OffsetAngle(slot*DRINKSLOT_DEGREE_INTERVAL);
            return drinkPos;
        }

    }
}
