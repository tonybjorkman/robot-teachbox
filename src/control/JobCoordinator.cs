using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace robot_teachbox.src.control
{
    public enum State { IDLE,GRIPPING,HOLDING_BOTTLE,POURING,POURING_ADDITIONAL,RELEASING}



    public static class MyListExtension
    {
        public static T Dequeue<T>(this List<T> list)
        {
            var item = list.First();
            if (item != null)
            {
                list.Remove(item);
            }
            return item;
        }

        public static JobTask DequeueItem(this List<JobTask> list, JobTask searchItem)
        {
            
            var item = list.Find(x => x.BottleId == searchItem.BottleId);
            if (item != null)
            {
                list.Remove(item);
            }
            return item;
        }
    }



    public class JobTask
    {
        public int BottleId { get; set; }
        public int GlassIndex;
        public int Amount;

    }

    //contains all the tasks needed to finish a product
    public class Job
    {
        public List<JobTask> Tasks { get; set; }

    }

    //a single task within a job. Task is putting Amount of BottleId in GlassIndex.


    public class JobCoordinator
    {
        private State CurrentState = State.IDLE;
        private Job CurrentJob;
        private List<Job> jobs = new List<Job>();

        private Job GetCurrentJob()
        {
            if (CurrentJob != null)
            {
                return CurrentJob;
            } else //start with next job
            {
                var j = jobs.Dequeue();
                CurrentJob = j;
                return j;
            }   
        }


        public List<JobTask> GetNextTasks() {
            var taskList = new List<JobTask>();
            var currJob = GetCurrentJob();
            if(currJob != null)
            {
                var task = currJob.Tasks.Dequeue();
                taskList.Add(task);
                foreach(var j in jobs)
                {
                    taskList.Add(j.Tasks.DequeueItem(task));
                }
            }
            return taskList;
        }

        /// <summary>
        /// Returns all tasks which are deemed similar and it also pops those task
        /// from the job they belong to so they dont get done twice.
        /// </summary>
        /// <param name="task">Task to search for</param>
        /// <returns></returns>
        private JobTask GetCommonTasks(JobTask task)
        {
            throw new NotImplementedException();
        }

        public void GetJobTask()
        {

        }

    }
}
