using System.Collections.Generic;
public class GunFacade
{
    Dictionary<IGunData.CaliberTypes, IGunFactory> _gunFactoriesDic;

    public GunFacade(Dictionary<IGunData.CaliberTypes, IGunFactory> gunFactoriesDic)
    {
        _gunFactoriesDic = gunFactoriesDic;
    }

    public IGun GunInstantiate(IGunData gunData)
    {
        IGun gun = null;

        if(gunData.CaliberType == IGunData.CaliberTypes._10mm) gun =  _gunFactoriesDic[IGunData.CaliberTypes._10mm].GunInstantiate(gunData);
        if(gunData.CaliberType == IGunData.CaliberTypes._5_56mm) gun = _gunFactoriesDic[IGunData.CaliberTypes._5_56mm].GunInstantiate(gunData);
        if(gunData.CaliberType == IGunData.CaliberTypes._7_62mm) gun = _gunFactoriesDic[IGunData.CaliberTypes._7_62mm].GunInstantiate(gunData);

        gun.Reload(new Entity_Magazine(0, 0));
        return gun;
    }
}
