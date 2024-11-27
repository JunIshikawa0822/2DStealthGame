
public class GunFacade
{
    Gun_10mm_CreateConcreteFactory _gun_10mm_Fac;
    Gun_7_62mm_CreateConcreteFactory _gun_7_62mm_Fac;
    Gun_5_56mm_CreateConcreteFactory _gun_5_56mm_Fac;

    public GunFacade(IGunFactory[] gunFactories)
    {
        foreach(IGunFactory factory in gunFactories)
        {
            if(factory is Gun_10mm_CreateConcreteFactory)_gun_10mm_Fac = factory as Gun_10mm_CreateConcreteFactory;
            if(factory is Gun_5_56mm_CreateConcreteFactory)_gun_5_56mm_Fac = factory as Gun_5_56mm_CreateConcreteFactory;
            if(factory is Gun_7_62mm_CreateConcreteFactory)_gun_7_62mm_Fac = factory as Gun_7_62mm_CreateConcreteFactory;
        }
    }

    public IGun InstantiateGun(IGunData gunData)
    {
        if(gunData.CaliberType == IGunData.CaliberTypes._10mm)return _gun_10mm_Fac.GunInstantiate(gunData);
        if(gunData.CaliberType == IGunData.CaliberTypes._5_56mm)return _gun_5_56mm_Fac.GunInstantiate(gunData);
        if(gunData.CaliberType == IGunData.CaliberTypes._7_62mm)return _gun_7_62mm_Fac.GunInstantiate(gunData);
        else return _gun_10mm_Fac.GunInstantiate(gunData);
    }
}
