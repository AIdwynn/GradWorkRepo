namespace Vital.ObjectPools
{
    public interface IPoolableScript
    {
        public string Name { get; }
        public bool IsViewActive { get; }
    }
}