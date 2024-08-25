using UnityEngine;
public class InputSystem : ASystem, IOnPreUpdate
{
    public override void OnSetUp()
    {
        
    }

    public void OnPreUpdate()
    {
        if(Input.GetMouseButtonDown(0))
        {
            gameStat.onClick = true;
        }
        else
        {
            gameStat.onClick = false;
        }
    }
}
