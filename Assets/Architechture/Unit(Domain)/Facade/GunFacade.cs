using System.Collections.Generic;
using UnityEngine;
public class GunFacade
{
    //Dictionary<int, GunCategory> _gunCategoriesDic;

    private List<IGunFactory> _gunFactories;
    private List<GunCategory> categories;
    private Transform _facadeTrans;

    public GunFacade(List<IGunFactory> gunFactories, Transform facadeTrans)
    {
        _gunFactories = gunFactories;
        _facadeTrans = facadeTrans;

        categories = new List<GunCategory>();

        AddCategory("Handgun", _gunFactories[0]);
        AddCategory("Shotgun", _gunFactories[1]);
    }

    private void AddCategory(string name, IGunFactory gunFactory)
    {
        GameObject parent = new GameObject(name);
        parent.transform.SetParent(_facadeTrans);

        categories.Add(new GunCategory(name, gunFactory, parent.transform));
    }

    public AGun GetGunInstance(IGunData gunData)
    {
        //Debug.Log("呼ばれた");

        AGun gun = null;

        switch(gunData)
        {
            case Handgun_Data handgunData : gun = categories[0].GetInstance(gunData);break;

            case Shotgun_Data shotgunData : gun = categories[1].GetInstance(gunData);break;
        }

        gun.gameObject.SetActive(true);
        return gun;
    }

    public void ReturnGunInstance(AGun gun)
    {
        gun.gameObject.SetActive(false);
        switch(gun.GunData)
        {
            case Handgun_Data handgunData: categories[0].ReturnToList(gun); break;
            case Shotgun_Data shotgunData: categories[1].ReturnToList(gun); break;
        }
    }
}
