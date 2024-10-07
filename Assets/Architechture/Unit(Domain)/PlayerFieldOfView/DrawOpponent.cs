using UnityEngine;
using System.Collections.Generic;
public class DrawOpponent
{
    private List<Transform> _newVisibleTargets;
    private List<Transform> _oldVisibleTargets;

    public DrawOpponent()
    {
        _newVisibleTargets = new List<Transform>();
        _oldVisibleTargets = new List<Transform>();
    }

    public void DrawTargets(List<Transform> transformsArray)
    {
        _newVisibleTargets = transformsArray;

        DisplayVisibleTargets();
        //新旧を比較し、描画するリストを更新
        UnDisplayInvisibleTargets();

        _oldVisibleTargets = _newVisibleTargets;
    }

    private void DisplayVisibleTargets()
    {
        foreach(Transform target in _newVisibleTargets)
        {
            AEntity entity = target.GetComponent<AEntity>();

            entity.OnEntityMeshAble();
        }
    }

    private void UnDisplayInvisibleTargets()
    {
        foreach (Transform oldTarget in _oldVisibleTargets)
        {
            bool isInclude = false;

            foreach (Transform newTarget in _newVisibleTargets)
            {
                //newにoldが含まれていればok
                if (oldTarget == newTarget)
                {
                    isInclude = true;
                    break;
                }
            }

            //含まれていないならオフ
            if (isInclude == false)
            {
                AEntity entity = oldTarget.GetComponent<AEntity>();

                entity.OnEntityMeshDisable();
            }
        }
    }
}
