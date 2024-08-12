using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGun
{
    void Reload();
    void Shot();
    void Jam();

    void SetUp();
    void OnUpdate();
}
