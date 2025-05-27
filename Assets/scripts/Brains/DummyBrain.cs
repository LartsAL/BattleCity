using System.Collections.Generic;
using System.Linq;
using Interfaces;
using UnityEngine;

namespace Brains
{
    public class DummyBrain : IBrain
    {
        private readonly SortedDictionary<int, IGoal> _goals = new ();
        
        public void AddGoal(int weight, IGoal goal)
        {
            Debug.Log($"Adding goal {goal.GetType().Name} with weight {weight}");
            _goals.Add(weight, goal);
        }

        public void Think()
        {
            var currentGoal = _goals.FirstOrDefault(g => g.Value.IsAvailable()).Value;
            currentGoal?.Execute();
        }
    }
}