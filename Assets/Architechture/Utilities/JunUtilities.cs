using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using UnityEngine;
using Unity.Mathematics;
using Matrix4x4 = UnityEngine.Matrix4x4;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;
using Vector4 = UnityEngine.Vector4;

namespace JunUtilities
{
    /// <summary>
    /// 拡張クラスたち
    /// </summary>
    public static class JunExpandUnityClass
    {
        /// <param name="GetComponentInChildren (Unity)"> 親含めコンポーネントを取得</param>
        /// <param name="GetChildrenComponent (Jun)">　親を含めずに取得</param>
        public static T[] GetChildrenComponent<T>(Transform transform) where T : Component
        {
            return
                transform.GetComponentsInChildren<T>(true)
                    .Where(item => item.transform != transform)
                    .ToArray();
        }
    }
    public static class JunGeometry
    {
        #region Vector関係
        public static Vector3 RotateAround(Vector3 vector, Vector3 rotateEulerAngles)
        {
            vector = RotateVector(vector, Vector3.right, -rotateEulerAngles.x);
            //さらにy軸に合わせて45度回転
            vector = RotateVector(vector, Vector3.up, -rotateEulerAngles.y);
            //さらにy軸に合わせて45度回転
            vector = RotateVector(vector, Vector3.forward, -rotateEulerAngles.z);

            return vector;
        }
        public static Vector3 RotateVector(Vector3 vector, Vector3 baseAxis, float angle)
        {
            //念のため正規化
            Vector3 axis = baseAxis.normalized; // 回転軸を正規化
            float cosTheta = Mathf.Cos(angle * Mathf.Deg2Rad);
            float sinTheta = Mathf.Sin(angle * Mathf.Deg2Rad);
            // 回転行列を計算
            // //ロドリゲスの回転公式
            // //回転軸方向の単位ベクトル(axis)をnとすると
            Matrix4x4 rotationMatrix = new Matrix4x4
            (
                new Vector4
                (
                    //nx^2 * (1 - cosθ) + cosθ
                    axis.x * axis.x * (1 - cosTheta) + cosTheta,
                    //nx * ny * (1 - cosθ) - xz * sinθ
                    axis.x * axis.y * (1 - cosTheta) - axis.z * sinTheta,
                    //nx * nz * (1 - cosθ) + ny * sinθ
                    axis.x * axis.z * (1 - cosTheta) + axis.y * sinTheta,
                    0
                ),
                new Vector4
                (
                    //nx * ny * (1 - cosθ) + nz * sinθ
                    axis.x * axis.y * (1 - cosTheta) + axis.z * sinTheta,
                    //ny^2 * (1 - cosθ) + cosθ
                    axis.y * axis.y * (1 - cosTheta) + cosTheta,
                    //ny * nz * (1 - cosθ) - nx * sinθ
                    axis.y * axis.z * (1 - cosTheta) - axis.x * sinTheta,
                    0
                ),
                new Vector4
                (
                    //nx * nz * (1 - cosθ) * ny * sinθ
                    axis.x * axis.z * (1 - cosTheta) - axis.y * sinTheta,
                    //ny * nz * (1 - cosθ) * nx * sinθ
                    axis.y * axis.z * (1 - cosTheta) + axis.x * sinTheta,
                    //nz^2 * (1 - cosθ) * cosθ
                    axis.z * axis.z * (1 - cosTheta) + cosTheta,
                    0
                ),

                //3*3行列には不要だが、4*4行列として整えるため追加
                new Vector4(0, 0, 0, 1)
            );

            return rotationMatrix.MultiplyPoint3x4(vector);
        }

        #endregion
        #region OctTree（モートン空間）
        public static int PosToMortonNumber(Vector3 objectPosition, Vector3 basePosition, int dimensionLevel, Vector3 cellSize)
        {
            //全てのマスの総数を計算
            int cellNum = (int)Mathf.Pow(8, dimensionLevel);

            #region 単位距離の計算 いらないね...
            //ルート空間における一辺あたりのマスの数を計算
            //一辺をb、マスの総数をaとすると a = b ^ 3なので両辺に1/3をかける
            //するとa ^ (1/3) = bとなる
            int baseNum = (int)Mathf.Pow(cellNum, 1f/3f);

            //ルート空間の一辺の長さをそれぞれ計算
            float width = cellSize.x * baseNum;
            float height = cellSize.y * baseNum;
            float depth = cellSize.z * baseNum;
            #endregion

            //cellNum個に分割された空間のx軸方向に何番目かを示しているよ。（indexなので0番始まりな点に注意）
            int numX = (int)((objectPosition.x - basePosition.x) / cellSize.x);

            //cellNum個に分割された空間のy軸方向に何番目かを示しているよ。（indexなので0番始まりな点に注意）
            int numY = (int)((objectPosition.y - basePosition.y) / cellSize.y);

            //cellNum個に分割された空間のz軸方向に何番目かを示しているよ。（indexなので0番始まりな点に注意）
            int numZ = (int)((objectPosition.z - basePosition.z) / cellSize.z);
            
            if(numX < 0 || numY < 0 || numZ < 0 || objectPosition.x < basePosition.x || objectPosition.y < basePosition.y || objectPosition.z < basePosition.z)
            {
                return -1;
            }

            return (int)GetMortonNumber((byte)numX, (byte)numY, (byte)numZ);
        }
        
        private static uint GetMortonNumber(byte x, byte y, byte z)
        {
            uint mortonNum = BitSeparateFor3D(x) | BitSeparateFor3D(y) << 1 | BitSeparateFor3D(z) << 2;
            return mortonNum;
        }
        
        private static uint BitSeparateFor3D(byte n)
        {
            //Debug.Log("n : " + n);
            uint s = n; // 1バイトを32ビットに拡張
            // Debug.Log("s : " + Convert.ToString(s, 2))
            // Debug.Log("(s << 8) : " + Convert.ToString(s << 8, 2));
            // Debug.Log("s | (s << 8) : " + Convert.ToString(s | (s << 8), 2));
            // Debug.Log("0x0000F00F : " + Convert.ToString(0x0000F00F, 2));
            // Debug.Log("(s | (s << 8)) & 0x0000F00F : " + Convert.ToString((s | (s << 8)) & 0x0000F00F , 2));
            s = (s | (s << 8)) & 0x0000F00F; // ビットを8ビット間隔で配置
            
            // Debug.Log("(s << 4) : " + Convert.ToString(s << 4, 2));
            // Debug.Log("s | (s << 4) : " + Convert.ToString(s | (s << 4), 2));
            // Debug.Log("0x000C30C3 : " + Convert.ToString(0x000C30C3, 2));
            // Debug.Log("(s | (s << 4)) & 0x000C30C3 : " + Convert.ToString((s | (s << 4)) & 0x000C30C3, 2));
            s = (s | (s << 4)) & 0x000C30C3; // ビットを4ビット間隔で配置

            // Debug.Log("(s << 2) : " + Convert.ToString(s << 2, 2));
            // Debug.Log("(s | (s << 2) : " + Convert.ToString(s | (s << 2), 2));
            // Debug.Log("0x00249249 : " + Convert.ToString(0x00249249, 2));
            // Debug.Log("(s | (s << 2)) & 0x00249249 : " + Convert.ToString((s | (s << 2)) & 0x00249249, 2));
            s = (s | (s << 2)) & 0x00249249; // ビットを2ビット間隔で配置

            //Debug.Log("結果 : " + Convert.ToString(s, 2));
            //Debug.Log("最後 : " + s);
            return s;
        }
        #endregion
        #region 頂点を完全にカバーするBoundsを作成
        public static Bounds GetBoundsFromVertices(List<Vector3> points)
        {
            float minX = points[0].x;
            float minY = points[0].y;
            float minZ = points[0].z;
        
            float maxX = points[0].x;
            float maxY = points[0].y;
            float maxZ = points[0].z;

            for (int i = 0; i < points.Count; i++)
            {
                if(points[i].x < minX) minX = points[i].x;
                if(points[i].y < minY) minY = points[i].y;
                if(points[i].z < minZ) minZ = points[i].z;
            
                if(points[i].x > maxX) maxX = points[i].x;
                if(points[i].y > maxY) maxY = points[i].y;
                if(points[i].z > maxZ) maxZ = points[i].z;
            }
        
            Bounds bounds = new Bounds();
            bounds.SetMinMax(new Vector3(minX, minY, minZ), new Vector3(maxX, maxY, maxZ));
            return bounds;
        }
        //頂点を完全にカバーするAABB3Dを作成
        public static AABB3D GetAABB3DFromVertices(List<Vector3> points)
        {
            float minX = points[0].x;
            float minY = points[0].y;
            float minZ = points[0].z;
        
            float maxX = points[0].x;
            float maxY = points[0].y;
            float maxZ = points[0].z;

            for (int i = 0; i < points.Count; i++)
            {
                if(points[i].x < minX) minX = points[i].x;
                if(points[i].y < minY) minY = points[i].y;
                if(points[i].z < minZ) minZ = points[i].z;
            
                if(points[i].x > maxX) maxX = points[i].x;
                if(points[i].y > maxY) maxY = points[i].y;
                if(points[i].z > maxZ) maxZ = points[i].z;
            }
        
            AABB3D aabb = new AABB3D(new Vector3(minX, minY, minZ), new Vector3(maxX, maxY, maxZ));
            return aabb;
        }
        #endregion
    }
    public static class JunCamera
    {
        /// <param name="camera"> 基準となるカメラ</param>
        /// <param name="clipPlane">　カメラからの距離　nearなのかfarかで変えましょう</param>
        public static Vector3[] CalculateFrustumCorners(Camera camera, float clipPlane)
        {
            bool cameraType = camera.orthographic;
            Vector3[] corners = new Vector3[4];

            if (cameraType)
            {
                //高さと幅の取得
                float halfHeight = camera.orthographicSize;
                float halfWidth = halfHeight * camera.aspect;

                //かどたち
                corners[0] = camera.transform.position + camera.transform.rotation * new Vector3(-halfWidth, -halfHeight, clipPlane);
                corners[1] = camera.transform.position + camera.transform.rotation * new Vector3(halfWidth, -halfHeight, clipPlane);
                corners[2] = camera.transform.position + camera.transform.rotation * new Vector3(halfWidth, halfHeight, clipPlane);
                corners[3] = camera.transform.position + camera.transform.rotation * new Vector3(-halfWidth, halfHeight, clipPlane);
            }
            else
            {
                camera.CalculateFrustumCorners(new Rect(0,0,1,1), clipPlane, Camera.MonoOrStereoscopicEye.Mono, corners);
            }
            
            return corners;
        }
    }
    public class DynamicTree3D
    {
        private TreeNode3D _root; // ルートノード
        private int _nodeCount; // ノードの数
        public int NodeCount
        {
            get => _nodeCount;
        }
        
        public DynamicTree3D()
        {
            _root = null;
            _nodeCount = 0;
        }
        
        // 最適な親ノードを見つけるメソッド
        private TreeNode3D FindBestFitNode(TreeNode3D node, AABB3D aabb)
        {
            // AABBが交差するノードを見つける
            if (node.Left == null && node.Right == null) return node;

            float leftCost = GetExpansionCost(node.Left.Bounds, aabb);
            float rightCost = GetExpansionCost(node.Right.Bounds, aabb);

            TreeNode3D lowerCostNode = (leftCost < rightCost) ? node.Left : node.Right;
            TreeNode3D higherCostNode = (lowerCostNode == node.Left) ? node.Right : node.Left;

            //コストが小さい方が交差せず、コストが大きい方と交差している可能性はあるか？
            //chatGPTは「ある」と言っているが信じがたい
            //一応両方試すか
            //再帰的に探そう
            if (lowerCostNode.Bounds.Intersects(aabb))
            {
                return FindBestFitNode(lowerCostNode, aabb);
            }
            else if(higherCostNode.Bounds.Intersects(aabb))
            {
                return FindBestFitNode(higherCostNode, aabb);
            }
            //両方ともと交差しないとき、
            return node; // 交差しない場合、親ノードを返す
            
            float GetExpansionCost(AABB3D baseAABB, AABB3D insertAABB)
            {
                AABB3D expand = new AABB3D(baseAABB);
                expand.Merge(insertAABB);
                return expand.Volume() - baseAABB.Volume();
            }
        }

        public void Insert(AABB3D aabb, int objectId)
        {
            TreeNode3D newNode = new TreeNode3D { Bounds = aabb};
            _nodeCount++;
            
            if (_root == null)
            {
                _root = newNode;
                return;
            }
            else
            {
                //aabbを入れるべきノード
                TreeNode3D fitNode = FindBestFitNode(_root, aabb);
                //toNodeを一段階下げて、parentNodeを作成
                //  a             a　となるために b + new の部分を作って入れる
                //b   c      b+new   c
                //         b    new    
                TreeNode3D newParentNode = new TreeNode3D { Bounds = new AABB3D(fitNode.Bounds)};
                TreeNode3D oldParent = fitNode.Parent;
                //ステージ空間を分割するなら座標の方がいいか
                if (ShouldPlaceLeft(fitNode.Bounds, newNode.Bounds))
                {
                    newParentNode.Right = fitNode;
                    newParentNode.Left = newNode;
                }
                else
                {
                    newParentNode.Right = newNode;
                    newParentNode.Left = fitNode;
                }
                
                newParentNode.Bounds.Merge(newParentNode.Right.Bounds);
                newParentNode.Bounds.Merge(newParentNode.Left.Bounds);
                
                newParentNode.Parent = oldParent;
                fitNode.Parent = newParentNode;
                newNode.Parent = newParentNode;
                
                if (oldParent == null)
                {
                    _root = newParentNode;
                }
                else
                {
                    if (fitNode == oldParent.Left)
                    {
                        oldParent.Left = newParentNode;
                    }
                    else
                    {
                        oldParent.Right = newParentNode; 
                    }
                    
                    RecalculateBounds(oldParent);
                }
            }
            
            bool ShouldPlaceLeft(AABB3D fitBounds, AABB3D newBounds)
            {
                return fitBounds.Center.x + fitBounds.Center.y + fitBounds.Center.z > 
                       newBounds.Center.x + newBounds.Center.y + newBounds.Center.z;
            }
            
            void RecalculateBounds(TreeNode3D node)
            {
                //親を遡ってAABBを更新する
                //再帰でもいいかな？と思うけど、大量のオブジェクトが配置される場合、反復の方が安全か
                while (node != null)
                {
                    // 左右の子が両方存在する場合のみ更新
                    if (node.Left != null && node.Right != null)
                    {
                        node.Bounds = new AABB3D(node.Left.Bounds);
                        node.Bounds.Merge(node.Right.Bounds);
                    }

                    // 親ノードに移動
                    node = node.Parent;
                }
            }
        }
    }
    
    /// <summary>
    /// 静的オブジェクトの管理に適したTree 動的な追加は考慮されない
    /// </summary>
    public class AABB3DTree
    {
        public TreeNode3D rootNode;

        public void BuildTree(List<(AABB3D bounds, Transform transform)> objects)
        {
            rootNode = BuildNode(objects);
        }

        private TreeNode3D BuildNode(List<(AABB3D bounds, Transform transform)> objects)
        {
            if (objects.Count == 0) return null;
            // 単一要素ならリーフノードを作成
            if (objects.Count == 1)
            {
                return new TreeNode3D { Bounds = objects[0].bounds, Transform = objects[0].transform };
            }
            //Debug.Log("ノード作成");
            //1.全てくっつける
            AABB3D combineAllBounds = new AABB3D(objects[0].bounds);
            //全部結合する
            foreach ((AABB3D bounds, Transform transform) obj in objects)
            {
                combineAllBounds = combineAllBounds.Merge(obj.bounds);
            }
            
            //2. 一番でかいBoundingBoxにおける一番長い辺を探す
            Vector3 size = combineAllBounds.Max - combineAllBounds.Min;
            int longestAxis = 0;
            if (size.x >= size.y && size.x >= size.z) longestAxis = 0;
            if (size.y >= size.x && size.y >= size.z) longestAxis = 1; // y軸が最長
            else longestAxis = 2; // z軸が最長
            
            //Debug.Log(longestAxis);
            //Debug.Log($"ソート前 : {string.Join(",", objects.Select(item => item.transform.name))}");
            //3. 長い辺（軸）にそってソート
            objects.Sort((a, b) => a.bounds.Center[longestAxis].CompareTo(b.bounds.Center[longestAxis]));
            
            //Debug.Log($"ソート後 : {string.Join(",", objects.Select(item => item.transform.name))}");

            //4. 分割してリスト再作成
            int midIndex = objects.Count / 2;
            List<(AABB3D bounds, Transform transform)> leftObjects = objects.GetRange(0, midIndex);
            List<(AABB3D bounds, Transform transform)> rightObjects = objects.GetRange(midIndex, objects.Count - midIndex);
            
            //5. ノードを作る
            TreeNode3D node = new TreeNode3D { Bounds = combineAllBounds};
            //再帰
            //Debug.Log("左ノード");
            node.Left = BuildNode(leftObjects);
            //Debug.Log("右ノード");
            node.Right = BuildNode(rightObjects);
            
            if (node.Left != null) node.Left.Parent = node;
            if (node.Right != null) node.Right.Parent = node;

            return node;
        }
        
        ///<summary> ///frustramBoundsと交差する全てのBoundsとそのTransformをリストにして返す ///</summary>
        public List<(AABB3D bounds, Transform transform)> GetIntersectAABB3D(AABB3D frustumBounds)
        {
            List<(AABB3D bounds, Transform transform)> result = new List<(AABB3D bounds, Transform transform)>();
            IntersectAABB3DSearch(rootNode, frustumBounds, result);
            return result;
        }

        ///<summary> ///frustramBoundsと交差する全てのBoundsをさがしてくる ///</summary>
        private void IntersectAABB3DSearch(TreeNode3D node, AABB3D frustumBounds, List<(AABB3D bounds, Transform transform)> result)
        {
            if (node == null) return;

            // AABB が視錐台の境界と交差しているか？
            if (node.Bounds.Intersects(frustumBounds))
            {
                // リーフノードならリストに追加
                if (node.Left == null && node.Right == null)
                {
                    result.Add((node.Bounds, node.Transform));
                }
                else
                {
                    // 再帰的に探索
                    IntersectAABB3DSearch(node.Left, frustumBounds, result);
                    IntersectAABB3DSearch(node.Right, frustumBounds, result);
                }
            }
        }
    }
    
    public class TreeNode3D
    {
        public AABB3D Bounds; // ノードのバウンディングボックス
        public TreeNode3D Left; // 左の子ノード
        public TreeNode3D Right; // 右の子ノード
        public TreeNode3D Parent; //親のノード
        public Transform Transform;
        
        public TreeNode3D()
        {
            Left = null;
            Right = null;
            Parent = null;
        }

        public TreeNode3D(AABB3D aabb)
        {
            Bounds = new AABB3D(aabb.Min, aabb.Max);
            Left = null;
            Right = null;
            Parent = null;
        }

        public TreeNode3D(Bounds bounds)
        {
            Bounds = new AABB3D(bounds.min, bounds.max);
            Left = null;
            Right = null;
            Parent = null;
            //ObjectId = -1;
        }
    }
    public struct AABB3D
    {
        public Vector3 Min; // AABBの最小点
        public Vector3 Max; // AABBの最大点
        public Vector3 Center => (Min + Max) / 2.0f;

        public AABB3D(Bounds bounds)
        {
            Min = bounds.min;
            Max = bounds.max;
        }

        public AABB3D(Vector3 min, Vector3 max)
        {
            Min = min;
            Max = max;
        }

        public AABB3D(AABB3D aabb)
        {
            Min = aabb.Min;
            Max = aabb.Max;
        }

        // AABBが他のAABBと交差するかをチェック
        public bool Intersects(AABB3D other)
        {
            return (Min.x <= other.Max.x && Max.x >= other.Min.x) &&
                   (Min.y <= other.Max.y && Max.y >= other.Min.y) &&
                   (Min.z <= other.Max.z && Max.z >= other.Min.z);
        }

        // AABBを拡張する（比較してAABBを合体
        public AABB3D Merge(AABB3D other)
        {
            Vector3 newMin = new Vector3(
                Math.Min(Min.x, other.Min.x), 
                Math.Min(Min.y, other.Min.y), 
                Math.Min(Min.z, other.Min.z)
                );
            
            Vector3 newMax = new Vector3(
                Math.Max(Max.x, other.Max.x), 
                Math.Max(Max.y, other.Max.y), 
                Math.Max(Max.z, other.Max.z)
                );
            
            return new AABB3D(newMin, newMax);
        }
        
        public AABB3D Merge(Bounds other)
        {
            Vector3 newMin = new Vector3(
                Math.Min(Min.x, other.min.x), 
                Math.Min(Min.y, other.min.y), 
                Math.Min(Min.z, other.min.z)
            );
            
            Vector3 newMax = new Vector3(
                Math.Max(Max.x, other.max.x), 
                Math.Max(Max.y, other.max.y), 
                Math.Max(Max.z, other.max.z)
            );
            
            return new AABB3D(newMin, newMax);
        }
        
        // AABBのサイズを取得する
        public Vector3 Size()
        {
            return Max - Min;
        }

        public float Volume()
        {
            return (Max.x - Min.x) * (Max.y - Min.y) * (Max.z - Min.z);
        }
    }
    public class TreeNode2D
    {
        public AABB2D Bounds; // ノードのバウンディングボックス
        public int ObjectId; // 物体のID（リーフノードの場合）
        public TreeNode3D Left; // 左の子ノード
        public TreeNode3D Right; // 右の子ノード

        public TreeNode2D()
        {
            Left = null;
            Right = null;
            ObjectId = -1; // -1はリーフノードでないことを示す
        }
    }
    public struct AABB2D
    {
        public Vector2 Min; // AABBの最小点
        public Vector2 Max; // AABBの最大点

        public AABB2D(Vector2 min, Vector2 max)
        {
            Min = min;
            Max = max;
        }

        // AABBが他のAABBと交差するかをチェック
        public bool Intersects(AABB other)
        {
            return (Min.x <= other.Max.x && Max.x >= other.Min.x) &&
                   (Min.y <= other.Max.y && Max.y >= other.Min.y);
        }

        // AABBを拡張する
        public void Extend(AABB other)
        {
            Min = new Vector2(Math.Min(Min.x, other.Min.x), Math.Min(Min.x, other.Min.x));
            Max = new Vector2(Math.Max(Max.x, other.Max.x), Math.Max(Max.x, other.Max.x));
        }

        public Vector2 Size()
        {
            return Max - Min;
        }
    }
}
