public interface IGun<T>
{
    public void OnSetUp<T>(IObjectPool<T> objectPool) where T : ABullet, IPooledObject<T>;
}
