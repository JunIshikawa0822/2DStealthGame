using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRayMarching : MonoBehaviour
{
    public Transform player;
    public Transform[] gameObjects;
    public float CubeSize = 1.0f; // Cubeのサイズ
    public float maxDistance = 10.0f; // 最大レイの距離
    public float epsilon = 0.01f; // 許容誤差

    private int stepNum;

    //あくまで本来レイマーチングは表面の描画を行うためのもの。立方体の表面に一番近い位置を返すことで、表面に一番近い値を返している。
    public float DistanceToSphere(Vector3 rayPoint, Vector3 objectPos)
    {
        Vector3 d = new Vector3(
            Mathf.Abs(rayPoint.x - objectPos.x) - CubeSize / 2,
            Mathf.Abs(rayPoint.y - objectPos.y) - CubeSize / 2,
            Mathf.Abs(rayPoint.z - objectPos.y) - CubeSize / 2
        );

        return Mathf.Max(d.x, Mathf.Max(d.y, d.z));
    }

    void Update()
    {
        Vector3 rayOrigin = player.transform.position; // Cubeの位置
        Vector3 rayDirection = transform.forward; // レイの進行方向
        float distanceTraveled = 0.0f;

        while (distanceTraveled < maxDistance)
        {
            float min = 1000;
            Transform minObject = null;

            Debug.Log($"stepNum: {stepNum}");

            foreach(Transform gameObject in gameObjects)
            {
                float distanceToSphere = DistanceToSphere(rayOrigin + rayDirection * distanceTraveled, gameObject.position);

                if(distanceToSphere < min)
                {
                    minObject = gameObject;
                    min = distanceToSphere;
                }
            }
            
            Debug.Log($"このステップでのSphereとの一番近い距離:{min}, オブジェクト:{minObject}"); 

            if (min < epsilon) // 衝突判定
            {
                Debug.Log("Hit the Cube at distance: " + distanceTraveled);
                break; // 衝突した場合、ループを終了
            }

            distanceTraveled += min; // 次のステップへ進む
            stepNum++;
        }
    }
}
