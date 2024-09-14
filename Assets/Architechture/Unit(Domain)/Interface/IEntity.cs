public interface IEntity
{
    public void OnSetUp(int hp);
    public void OnUpdate();
    public bool IsEntityDead();
    public void OnEntityDead();
}
