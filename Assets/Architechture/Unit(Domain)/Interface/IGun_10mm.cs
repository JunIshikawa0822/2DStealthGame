using System;

public interface IGun_10mm : IGun, IBType_10mm
{
    public IFactory<ABullet> GetFactory_10mm<T>() where T : IBType_10mm;
}
