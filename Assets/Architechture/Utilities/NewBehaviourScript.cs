using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OBBTest : MonoBehaviour
{
    private float _max = 0.0001f;

    private Vector3[] _edges = new Vector3[0];
    private Vector3 _origin;

    private void OnDrawGizmos()
    {
        if (_edges.Length == 0)
        {
            return;
        }

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(_origin, 0.01f);

        Gizmos.color = Color.blue;

        Vector3 from, to;

        Vector3 ftr = _origin + (_edges[0] * 0.5f) + (_edges[1] * 0.5f) + (_edges[2] * 0.5f);
        Vector3 fbr = ftr - _edges[2];
        Vector3 ftl = ftr - _edges[0];
        Vector3 fbl = ftl - _edges[2];

        Vector3 btr = ftr - _edges[1];
        Vector3 bbr = btr - _edges[2];
        Vector3 btl = btr - _edges[0];
        Vector3 bbl = btl - _edges[2];

        Gizmos.DrawLine(ftr, fbr);
        Gizmos.DrawLine(ftr, ftl);
        Gizmos.DrawLine(ftl, fbl);
        Gizmos.DrawLine(fbl, fbr);

        Gizmos.DrawLine(btr, bbr);
        Gizmos.DrawLine(btr, btl);
        Gizmos.DrawLine(btl, bbl);
        Gizmos.DrawLine(bbl, bbr);

        Gizmos.DrawLine(ftr, btr);
        Gizmos.DrawLine(ftl, btl);
        Gizmos.DrawLine(fbr, bbr);
        Gizmos.DrawLine(fbl, bbl);
    }

    private void Awake()
    {
        CalcEdges();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            CalcEdges();
        }
    }

    private void CalcEdges()
    {
        MeshFilter filter = GetComponent<MeshFilter>();

        Vector3[] vertices = filter.mesh.vertices;

        float[,] eigenVectors = GetEigenVectors(CollectMatrix(vertices));

        int rank = eigenVectors.Rank;

        Vector3 vec1, vec2, vec3;
        {
            float x = eigenVectors[0, 0];
            float y = eigenVectors[1, 0];
            float z = eigenVectors[2, 0];

            vec1 = new Vector3(x, y, z);
        }
        {
            float x = eigenVectors[0, 1];
            float y = eigenVectors[1, 1];
            float z = eigenVectors[2, 1];

            vec2 = new Vector3(x, y, z);
        }
        {
            float x = eigenVectors[0, 2];
            float y = eigenVectors[1, 2];
            float z = eigenVectors[2, 2];

            vec3 = new Vector3(x, y, z);
        }

        // 全頂点に対して内積を取り、最小値・最大値を計算する
        float min1 = float.MaxValue;
        float min2 = float.MaxValue;
        float min3 = float.MaxValue;
        float max1 = float.MinValue;
        float max2 = float.MinValue;
        float max3 = float.MinValue;

        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 pos = vertices[i];
            float dot1 = Vector3.Dot(vec1, pos);
            if (dot1 > max1)
            {
                max1 = dot1;
            }
            if (dot1 < min1)
            {
                min1 = dot1;
            }

            float dot2 = Vector3.Dot(vec2, pos);
            if (dot2 > max2)
            {
                max2 = dot2;
            }
            if (dot2 < min2)
            {
                min2 = dot2;
            }

            float dot3 = Vector3.Dot(vec3, pos);
            if (dot3 > max3)
            {
                max3 = dot3;
            }
            if (dot3 < min3)
            {
                min3 = dot3;
            }
        }

        float len1 = max1 - min1;
        float len2 = max2 - min2;
        float len3 = max3 - min3;

        Vector3 edge1 = transform.localToWorldMatrix.MultiplyVector(vec1 * len1);
        Vector3 edge2 = transform.localToWorldMatrix.MultiplyVector(vec2 * len2);
        Vector3 edge3 = transform.localToWorldMatrix.MultiplyVector(vec3 * len3);

        _edges = new[]
        {
            edge1, edge2, edge3,
        };

        Vector3 center1 = (vec1 * (max1 + min1)) * 0.5f;
        Vector3 center2 = (vec2 * (max2 + min2)) * 0.5f;
        Vector3 center3 = (vec3 * (max3 + min3)) * 0.5f;

        _origin = transform.localToWorldMatrix.MultiplyPoint(center1 + center2 + center3);
    }

    private float[,] CollectMatrix(Vector3[] vertices)
    {
        // 各成分の平均を計算
        Vector3 m = Vector3.zero;

        for (int i = 0; i < vertices.Length; i++)
        {
            m += vertices[i];
        }

        m /= vertices.Length;

        float c11 = 0; float c22 = 0; float c33 = 0;
        float c12 = 0; float c13 = 0; float c23 = 0;

        for (int i = 0; i < vertices.Length; i++)
        {
            c11 += (vertices[i].x - m.x) * (vertices[i].x - m.x);
            c22 += (vertices[i].y - m.y) * (vertices[i].y - m.y);
            c33 += (vertices[i].z - m.z) * (vertices[i].z - m.z);

            c12 += (vertices[i].x - m.x) * (vertices[i].y - m.y);
            c13 += (vertices[i].x - m.x) * (vertices[i].z - m.z);
            c23 += (vertices[i].y - m.y) * (vertices[i].z - m.z);
        }

        c11 /= vertices.Length;
        c22 /= vertices.Length;
        c33 /= vertices.Length;
        c12 /= vertices.Length;
        c13 /= vertices.Length;
        c23 /= vertices.Length;

        float[,] matrix = new float[,]
        {
            { c11, c12, c13 },
            { c12, c22, c23 },
            { c13, c23, c33 },
        };

        return matrix;
    }

    /// 

    /// 行列の中の絶対値の最大値とその位置を返す
    /// 

    /// 評価する行列
    /// 最大値の行位置
    /// 最大値の列位置
    /// 最大値
    private float GetMaxValue(float[,] matrix, out int p, out int q)
    {
        p = 0;
        q = 0;

        int rank = matrix.Rank;

        float max = float.MinValue;

        for (int i = 0; i < rank; i++)
        {
            int len = matrix.GetLength(i);

            for (int j = 0; j < len; j++)
            {
                // 対角成分は評価しない
                if (i == j)
                {
                    continue;
                }

                float absmax = Mathf.Abs(matrix[i, j]);
                if (max <= absmax)
                {
                    max = absmax;
                    p = i;
                    q = j;
                }
            }
        }

        if (p > q)
        {
            int temp = p;
            p = q;
            q = temp;
        }

        return max;
    }

    /// 

    /// 固有ベクトルを得る
    /// 

    /// 評価する行列
    private float[,] GetEigenVectors(float[,] matrix)
    {
        // 固有ベクトルのための行列を正規化
        float[,] eigenVectors = new float[3,3];

        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if (i == j)
                {
                    eigenVectors[i,j] = 1f;
                }
                else
                {
                    eigenVectors[i,j] = 0;
                }
            }
        }

        int limit = 100;
        int count = 0;
        int p, q;
        while (true)
        {
            count++;

            if (count >= limit)
            {
                Debug.Log("somothing was wrong.");
                break;
            }

            float max = GetMaxValue(matrix, out p, out q);
            if (max <= _max)
            {
                break;
            }

            float app = matrix[p, p];
            float apq = matrix[p, q];
            float aqq = matrix[q, q];

            float alpha = (app - aqq) / 2f;
            float beta = -apq;
            float gamma = Mathf.Abs(alpha) / Mathf.Sqrt(alpha * alpha + beta * beta);

            float sin = Mathf.Sqrt((1f - gamma) / 2f);
            float cos = Mathf.Sqrt((1f + gamma) / 2f);

            if (alpha * beta < 0)
            {
                sin = -sin;
            }

            for (int i = 0; i < 3; i++)
            {
                float temp = cos * matrix[p, i] - sin * matrix[q, i];
                matrix[q, i] = sin * matrix[p, i] + cos * matrix[q, i];
                matrix[p, i] = temp;
            }

            for (int i = 0; i < 3; i++)
            {
                matrix[i, p] = matrix[p, i];
                matrix[i, q] = matrix[q, i];
            }

            matrix[p, p] = cos * cos * app + sin * sin * aqq - 2 * sin * cos * apq;
            matrix[p, q] = sin * cos * (app - aqq) + (cos * cos - sin * sin) * apq;
            matrix[q, p] = sin * cos * (app - aqq) + (cos * cos - sin * sin) * apq;
            matrix[q, q] = sin * sin * app + cos * cos * aqq + 2 * sin * cos * apq;

            for (int i = 0; i < 3; i++)
            {
                float temp = cos * eigenVectors[i, p] - sin * eigenVectors[i, q];
                eigenVectors[i, + q] = sin * eigenVectors[i, p] + cos * eigenVectors[i, q];
                eigenVectors[i, + p] = temp;
            }
        }

        return eigenVectors;
    }
}
