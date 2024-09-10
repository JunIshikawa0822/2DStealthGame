using System.Collections;
using System.Collections.Generic;
using System;
public class CaliberTypeWrapper : IBulletCaliberType
{
    private Enum caliberType;

    public CaliberTypeWrapper(Enum caliberType)
    {
        this.caliberType = caliberType;
    }

    public override bool Equals(object obj)
    {
        if (obj is CaliberTypeWrapper other)
        {
            return caliberType.Equals(other.caliberType);
        }
        return false;
    }

    public override int GetHashCode()
    {
        return caliberType.GetHashCode();
    }
}
