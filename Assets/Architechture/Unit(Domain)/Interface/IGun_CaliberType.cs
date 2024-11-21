public interface IGun<T> : IGun where T : IPooledObject<T> 
{
    public void OnSetUp(IObjectPool<T> objectPool);
}
