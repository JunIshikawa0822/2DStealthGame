using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGunFactory
{
    public AGun GunInstantiate(IGunData customData);
}
