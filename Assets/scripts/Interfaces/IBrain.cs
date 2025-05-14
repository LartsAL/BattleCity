namespace Interfaces
{
    public interface IBrain
    {
        public void AddGoal(int weight, IGoal goal);

        public void Think();
    }
}