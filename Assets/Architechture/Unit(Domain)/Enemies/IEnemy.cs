
public interface IEnemy
{
    public void OnSetUp(Entity_HealthPoint enemyHP, FindOpponent find);
    public void OnMove();
    public void OnAttack();
    public void OnReload();
    public void OnHide();
    //public abstract void SearchAround();
}
