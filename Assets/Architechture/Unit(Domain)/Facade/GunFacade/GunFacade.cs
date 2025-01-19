using System.Collections.Generic;
using UnityEngine;
public class GunFacade
{
    private List<ICustomizeFactory> _gunFactories;
    private List<ICategory<AGun>> categories;
    private Transform _facadeTrans;

    public GunFacade(List<ICustomizeFactory> gunFactories, Transform facadeTrans)
    {
        _gunFactories = gunFactories;
        _facadeTrans = facadeTrans;

        categories = new List<ICategory<AGun>>();

        AddCategory("Handgun", _gunFactories[0]);
        AddCategory("Shotgun", _gunFactories[1]);
        AddCategory("Rifle", _gunFactories[2]);
    }

    private void AddCategory(string name, ICustomizeFactory gunFactory)
    {
        GameObject parent = new GameObject(name);
        parent.transform.SetParent(_facadeTrans);

        categories.Add(new GunCategory(name, gunFactory, parent.transform));
    }

    public AGun GetGunInstance(I_Data_Item data)
    {
        AGun gun = null;
        if(!(data is I_Data_Gun gunData))return null;

        switch(gunData)
        {
            case I_Data_HandGun : gun = categories[0].GetInstance(data);break;
            case I_Data_Shotgun : gun = categories[1].GetInstance(data);break;
            case I_Data_Rifle : gun = categories[2].GetInstance(data);break;

            default : return null;
        }

        gun.gameObject.SetActive(true);
        return gun;
    }

    public void ReturnGunInstance(AGun gun)
    {
        gun.gameObject.SetActive(false);

        switch(gun.Data)
        {
            case I_Data_HandGun : categories[0].Return(gun); break;
            case I_Data_Shotgun : categories[1].Return(gun); break;
            case I_Data_Rifle : categories[2].Return(gun); break;
        }
    }
}
