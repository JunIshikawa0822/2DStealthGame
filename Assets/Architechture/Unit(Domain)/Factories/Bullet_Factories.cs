using System;
using System.Collections.Generic;

public class Bullet_Factories : IBulletFactories
{
    private Dictionary<Type, IFactory<ABullet>> _factoriesDic;

    public Bullet_Factories(Dictionary<Type, IFactory<ABullet>> factories)
    {
        this._factoriesDic = factories;
    }

    public IFactory<ABullet> BulletFactory(Type type)
    {
        Type dicType = type;

        if(!_factoriesDic.ContainsKey(dicType))
        {

            foreach(Type t in _factoriesDic.Keys)
            {
                if(t == null)
                {
                    continue;
                }

                dicType = t;
                break;
            }
        }

        return _factoriesDic[dicType];
    }
}
