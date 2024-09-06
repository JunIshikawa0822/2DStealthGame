using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour, IEntity
{
    protected List<IItem> items;

    public virtual void OnSetUp()
    {

    }

    public virtual void OnUpdate()
    {
        
    }
}
