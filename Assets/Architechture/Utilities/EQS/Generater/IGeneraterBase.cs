using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public interface IGeneratorBase
{
    List<EnvQueryItem> GenerateItems(int numberOfStrategy, Transform centerOfItems);
}
