﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace robot_teachbox.src.control
{
    public enum State { IDLE,GRIPPING,HOLDING_BOTTLE,POURING,POURING_ADDITIONAL,RELEASING}



    public static class MyListExtension
    {
        public static T Dequeue<T>(this List<T> list)
        {

            T item=default(T);
            if (list != null && list.Count>0)
            {
                item = list.First();
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

        public Job()
        {
            Tasks = new List<JobTask>();
        }

        public void AddTask(JobTask task)
        {
            Tasks.Add(task);
        }

        public JobTask DequeueTask()
        {
            return Tasks.Dequeue();
        }

        public JobTask DequeueTask(JobTask task)
        {
            return Tasks.DequeueItem(task);
        }

    }

    //a single task within a job. Task is putting Amount of BottleId in GlassIndex.


    public class JobCoordinator
    {
        private State CurrentState = State.IDLE;
        private Job CurrentJob;
        private List<Job> jobs = new List<Job>();

        private Job GetCurrentJob()
        {
            if (CurrentJob != null && CurrentJob.Tasks.Count > 0)  
            {
                return CurrentJob;
            }   
            else //start with next job if prev is finished
            {
                var j = jobs.Dequeue();
                CurrentJob = j;
                return j;
            }   
        }

        

        public void AddJob(Job job)
        {
            Debug.WriteLine($"Added job {job.GetHashCode()}");

            jobs.Add(job);
        }


        //A list of jobs, 
        /// <summary>
        ///     Gets the currentJobs (if one exists) next Task (if one exist) and returns
        /// The tasks from other jobs aswell if they are equal to the currentJobs.
        /// </summary>
        /// <returns></returns>
        public List<JobTask> GetNextTasks() {
            var taskList = new List<JobTask>();
            var currJob = GetCurrentJob();
            if(currJob != null)
            {
                var task = currJob.DequeueTask();
                if (task != null)
                {
                    taskList.Add(task);
                    Debug.WriteLine($"GetNextTask was called and task {task.BottleId} added");
                    taskList.AddRange(GetCommonTasks(task));

                } 
            }
            Debug.WriteLine(" ");
            return taskList;
        }

        /// <summary>
        /// Returns all tasks which are deemed similar and it also pops those task
        /// from the job they belong to so they dont get done twice.
        /// </summary>
        /// <param name="task">Task to search for</param>
        /// <returns></returns>
        private List<JobTask> GetCommonTasks(JobTask task)
        {
            var taskList = new List<JobTask>();

            foreach (var j in jobs)
            {

                var siblingTask = j.DequeueTask(task);
                if (siblingTask != null)
                {
                    taskList.Add(siblingTask);
                    Debug.WriteLine($"found siblingtask {siblingTask.BottleId}");

                }
            }
            return taskList;
        }

        public void GetJobTask()
        {

        }

    }
}
