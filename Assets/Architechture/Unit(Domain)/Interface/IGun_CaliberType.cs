public interface IGun<T> : IItem , IGun where T : IPooledObject<T> 
{
    public void OnSetUp(IObjectPool<T> objectPool);
}
