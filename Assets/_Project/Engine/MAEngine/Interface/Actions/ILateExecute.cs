namespace MAEngine
{
    public interface ILateExecute : IAction
    {
        public void LateExecute(float fixedDeltaTime);
    }
}