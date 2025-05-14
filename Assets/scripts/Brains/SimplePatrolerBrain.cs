using Goals;
using UnityEngine;

namespace Brains
{
    public class SimplePatrolBrain : DummyBrain
    {
        public SimplePatrolBrain(GameObject relatedObject)
        {
            AddGoal(0, new AttackPlayerGoal(relatedObject));
            AddGoal(1, new RandomMoveGoal(relatedObject));
        }
    }
}