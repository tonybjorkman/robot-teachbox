using robot_teachbox.src.inventory;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace robot_teachbox.src.control
{

    /// <summary>
    /// Takes recipies and turns them into jobs
    /// A list of tasks
    /// 
    /// </summary>
    public class JobFactory
    {

        private Inventory _inventory;

        public JobFactory(Inventory inv)
        {
            _inventory = inv;
        }
        /// <summary>
        /// Creates a job (pick and pour a number of ingredients into a drink).
        /// </summary>
        /// <param name="glassInx">The slot at which the glass to pour into is placed</param>
        /// <param name="recipe">Dictionary with bottle type name as key and amount as value</param>
        /// <returns></returns>
        public Job CreateDrinkByIngredients(int glassInx, Dictionary<string,int> recipe)
        {
            var recipeTasks = new List<JobTask>();
            foreach(KeyValuePair<string,int> entry in recipe)
            {
                var bottle = _inventory.GetBottleType(entry.Key);
                if (bottle!=null && TryReserveAmount(bottle, entry.Value))
                {
                    recipeTasks.Add(new JobTask { GlassIndex = glassInx, Amount = entry.Value, BottleTypeId = bottle.Id });
                } else
                {
                    throw new ArgumentException($"Either drinkname {entry.Key} does not exist in inventory or not enough left in bottle");
                }
            }

            var job = new Job(recipeTasks);
            return job;
        }


        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private bool TryReserveAmount(object bottle, int value)
        {
            return true; //To be implemented
        }

        private Dictionary<string, int> _bottleIds = new Dictionary<string, int>
        {{ "Whiskey",0},{ "Cola",1},{ "Juice",2},{ "Rum",3}};

        private int GetBottleId(string key)
        {
            if (_bottleIds.ContainsKey(key))
            {
                return _bottleIds[key];
            } else
            {
                throw new ArgumentException($"No drink {key} in inventory");
            }
        }
    }
}
