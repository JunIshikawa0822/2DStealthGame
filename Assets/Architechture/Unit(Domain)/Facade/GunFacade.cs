using System.Collections.Generic;
using UnityEngine;
public class GunFacade
{
    //Dictionary<int, GunCategory> _gunCategoriesDic;

    private List<IGunFactory> _gunFactories;
    private List<GunCategory> categories;

    public GunFacade(List<IGunFactory> gunFactories)
    {
        _gunFactories = gunFactories;
        categories = new List<GunCategory>
        {
            new GunCategory("Handgun", _gunFactories[0]),
        };
    }

    public AGun GetGunInstance(IGunData gunData)
    {
        AGun gun = categories[0].GetInstance(gunData);

        switch(gunData)
        {
            case Handgun_Data handgunData: gun = categories[0].GetInstance(gunData);
            break;
        }

        gun.gameObject.SetActive(true);
        return gun;
    }

    public void ReturnGunInstance(AGun gun)
    {
        gun.gameObject.SetActive(false);
        switch(gun.GunData)
        {
            case Handgun_Data handgunData: categories[0].ReturnToList(gun);
            break;
        }
    }
}
