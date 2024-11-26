using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGunFactory
{
    public IGun GunInstantiate(IGunData customData);
}
