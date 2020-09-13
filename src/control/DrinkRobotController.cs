using robot_teachbox.src.inventory;
using robot_teachbox.src.robot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace robot_teachbox.src.control
{
    public class DrinkRobotController
    {

        JobCoordinator _jobCoordinator;
        Inventory _inventory;
        WorldPositions _worldPositions;
        RobotSender RobotSend;

        public DrinkRobotController(JobCoordinator jc,Inventory inventory,WorldPositions worldpos, RobotSender robotSend)
        {
            _jobCoordinator = jc;
            _inventory = inventory;
            _worldPositions = worldpos;
            RobotSend = robotSend;
        }

        public void Setup()
        {
        }

        public void RunJobs()
        {
            var taskCounter = 0;
            while (_jobCoordinator.HasJobs())
            {
                Debug.WriteLine($"run{taskCounter++}");
                var nextTasks = _jobCoordinator.GetNextTasks();


                if (nextTasks.Count() > 1)
                {
                    Debug.WriteLine("Multitask");
                } else
                {
                    Debug.WriteLine("Singletask");
                }
                var singleTask = nextTasks.First();
                //first retrieve bottle
                var bottle = _inventory.GetBottle(singleTask.BottleTypeId);
                var bottlePosition = _worldPositions.GetBottlePosition(bottle);
                bottlePosition.Grab = true;
                Debug.WriteLine("Get bottle at:"+bottlePosition.ToMelfaPosString());
                RobotSend.ProcessCommand(new CmdMsg(Command.QueryPolar, (PolarPosition)bottlePosition.Clone()));

                //Send grab-operation

                foreach (var task in nextTasks)
                {
                    Debug.WriteLine("task:" + task.ToString());
                    var pourPosition = _worldPositions.GetDrinkSlotPosition(task.GlassIndex);
                    Debug.WriteLine("Pour glass at:" + pourPosition.ToMelfaPosString());
                    //send pour operation
                    pourPosition.StartAngle = -45;
                    pourPosition.StopAngle = -120;
                    RobotSend.ProcessCommand(new CmdMsg(Command.QueryPour, (PolarPosition)pourPosition.Clone()));
                    pourPosition.Reverse = true;

                    //wait until amount

                    //send un-pour operation
                    RobotSend.ProcessCommand(new CmdMsg(Command.QueryPour, (PolarPosition)pourPosition));

                }
                Debug.WriteLine("Return bottle to:" + bottlePosition.ToMelfaPosString());
                //send release operation
                bottlePosition.Grab = false;
                RobotSend.ProcessCommand(new CmdMsg(Command.QueryPolar, (PolarPosition)bottlePosition));


            }
        }

    }
}
