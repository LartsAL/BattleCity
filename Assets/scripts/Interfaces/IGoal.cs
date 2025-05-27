namespace Interfaces
{
    public interface IGoal
    {
        public bool IsAvailable();

        public void Execute();
    }
}