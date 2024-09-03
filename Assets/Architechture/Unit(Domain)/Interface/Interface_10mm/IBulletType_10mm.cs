public interface IBulletType_10mm : IBulletType
{
    //表記は大文字がHigh/Low、小文字がdamage/penetration/costの略　貫通力とダメージについて表記している
    public enum BulletType_10mm
    {
        Bullet_10mm_Normal,
        Bullet_10mm_HdLpHc,
        Bullet_10mm_LdHpHc,
        Bullet_10mm_LdLpLc
    }
}
