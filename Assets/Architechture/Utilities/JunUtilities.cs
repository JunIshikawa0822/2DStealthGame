using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Matrix4x4 = UnityEngine.Matrix4x4;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;
using Vector4 = UnityEngine.Vector4;

namespace JunUtilities
{
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

        /// <summary>
        /// 任意の配列を被りなしにして返す
        /// </summary>
        public static T[] ConvertToUniqueArray<T>(T[] array)
        {
            HashSet<T> uniqueHashSet = new HashSet<T>(array);
            T[] uniqueArray = new T[uniqueHashSet.Count];
            uniqueHashSet.CopyTo(uniqueArray);
            return uniqueArray;
        }
    }
    public static class JunMath
    {
        /// <summary>
        /// ベクトルの内積
        /// </summary>
        public static float VectorDot(Vector3 a, Vector3 b)
        {
            return a.x * b.x + a.y * b.y + a.z * b.z;
        }
        public static double[] CubicSolver(float a, float b, float c, float d)
        {
            //3次方程式の解を求めるよ
            // 計算の精度向上のため、doubleに変換
            double A = a, B = b, C = c, D = d;
            // 抑制化した3次方程式: t^3 + p*t + q = 0, x = t - B/(3A)
            double p = (3 * A * C - B * B) / (3 * A * A);
            double q = (2 * B * B * B - 9 * A * B * C + 27 * A * A * D) / (27 * A * A * A);
    
            // 判別式 Δ = (q/2)^2 + (p/3)^3
            double Delta = Math.Pow(q / 2.0, 2) + Math.Pow(p / 3.0, 3);
            double offset = B / (3 * A); // x = t - offset

            if (Delta > 1e-10)
            {
                // Δ > 0 の場合: 実数解は1つ
                double sqrtDelta = Math.Sqrt(Delta);
                double u = Math.Cbrt(-q / 2.0 + sqrtDelta);
                double v = Math.Cbrt(-q / 2.0 - sqrtDelta);
                double t = u + v;
                double realSolution = t - offset;
                // 実数解1つと、複素解はdoubleでは扱えないため、NaNで返す
                return new double[] { realSolution, double.NaN, double.NaN };
            }
            else if (Math.Abs(Delta) < 1e-10)
            {
                // Δ = 0 の場合: 重解を含む実数解（2重根または3重根）
                double u = Math.Cbrt(-q / 2.0);
                double t1 = 2 * u;   // 単根
                double t2 = -u;      // 重根（2重解の場合）
                double x1 = t1 - offset;
                double x2 = t2 - offset;
                return new double[] { x1, x2, x2 }; // 重根は同じ値を返す
            }
            else // Delta < 0 の場合
            {
                // 3つの異なる実数解が得られる
                double r = 2 * Math.Sqrt(-p / 3.0);
                // cosθ = (-q/2) / sqrt[(-p/3)^3]
                double cosTheta = -q / (2 * Math.Sqrt(Math.Pow(-p / 3.0, 3)));
                // 数値誤差対策として、[-1, 1]にクランプ
                cosTheta = Math.Max(-1, Math.Min(1, cosTheta));
                double theta = Math.Acos(cosTheta);
                double t1 = r * Math.Cos(theta / 3.0);
                double t2 = r * Math.Cos((theta + 2 * Math.PI) / 3.0);
                double t3 = r * Math.Cos((theta + 4 * Math.PI) / 3.0);
                double x1 = t1 - offset;
                double x2 = t2 - offset;
                double x3 = t3 - offset;
                return new double[] { x1, x2, x3 };
            }
        }
    }
    public static class JunGeometry
    {
        #region ・Vector関係

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
        #region ・OctTree（モートン空間）

        public static int PosToMortonNumber(Vector3 objectPosition, Vector3 basePosition, int dimensionLevel, Vector3 cellSize)
        {
            //全てのマスの総数を計算
            int cellNum = (int)Mathf.Pow(8, dimensionLevel);

            #region 単位距離の計算 いらないね...

            //ルート空間における一辺あたりのマスの数を計算
            //一辺をb、マスの総数をaとすると a = b ^ 3なので両辺に1/3をかける
            //するとa ^ (1/3) = bとなる
            int baseNum = (int)Mathf.Pow(cellNum, 1f / 3f);

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

            if (numX < 0 || numY < 0 || numZ < 0 || objectPosition.x < basePosition.x ||
                objectPosition.y < basePosition.y || objectPosition.z < basePosition.z)
            {
                return -1;
            }

            return (int)GetMortonNumber((byte)numX, (byte)numY, (byte)numZ);
        }
        //綺麗にしたバージョン　対応する数もふやしてある
        public static int PositionToMortonNumber(Vector3 objectPosition, Vector3 basePosition, int dimensionLevel, Vector3 cellSize)
        {
            // セル空間内の位置を計算
            int numX = Mathf.FloorToInt((objectPosition.x - basePosition.x) / cellSize.x);
            int numY = Mathf.FloorToInt((objectPosition.y - basePosition.y) / cellSize.y);
            int numZ = Mathf.FloorToInt((objectPosition.z - basePosition.z) / cellSize.z);

            // 範囲チェック
            int cellsPerDimension = 1 << dimensionLevel; // 2^dimensionLevel
            if (numX < 0 || numY < 0 || numZ < 0 || 
                numX >= cellsPerDimension || numY >= cellsPerDimension || numZ >= cellsPerDimension)
            {
                return -1;
            }

            return (int)Get3DMortonNumber((uint)numX, (uint)numY, (uint)numZ);
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
        
        //綺麗にしたバージョン byteだと256までしか対応しないのでuint
        private static uint Get3DMortonNumber(uint x, uint y, uint z)
        {
            // 各座標を10ビットまでサポート（最大値1023）
            x &= 0x3FF;
            y &= 0x3FF;
            z &= 0x3FF;
    
            //ビットシフトで計算むず
            x = (x | (x << 16)) & 0x30000FF;
            x = (x | (x << 8)) & 0x300F00F;
            x = (x | (x << 4)) & 0x30C30C3;
            x = (x | (x << 2)) & 0x9249249;

            y = (y | (y << 16)) & 0x30000FF;
            y = (y | (y << 8)) & 0x300F00F;
            y = (y | (y << 4)) & 0x30C30C3;
            y = (y | (y << 2)) & 0x9249249;

            z = (z | (z << 16)) & 0x30000FF;
            z = (z | (z << 8)) & 0x300F00F;
            z = (z | (z << 4)) & 0x30C30C3;
            z = (z | (z << 2)) & 0x9249249;

            return x | (y << 1) | (z << 2);
        }

        /// <summary>
        /// AABB3Dが交差しているモートン空間を調べる
        /// </summary>
        public static int[] GetMortonCodesFromAABB(AABB3D bounds, Vector3 mortonBasePos, int dimensionLevel, Vector3 cellSize)
        {
            int separateX = Mathf.FloorToInt((bounds.Max.x - bounds.Min.x) / cellSize.x);
            int separateY = Mathf.FloorToInt((bounds.Max.y - bounds.Min.y) / cellSize.y);
            int separateZ = Mathf.FloorToInt((bounds.Max.z - bounds.Min.z) / cellSize.z);
        
            //Debug.Log($"{separateX}, {separateY}, {separateZ}");
            int[] intersectMortonSpaces = new int[separateX * separateY * separateZ];
            
            //Debug.Log(intersectMortonSpaces);
        
            for (int i = 0; i < separateX; i++)
            {
                for (int j = 0; j < separateY; j++)
                {
                    for (int k = 0; k < separateZ; k++)
                    {
                        int num = i * (separateY * separateZ) + j * separateZ + k;
                        // Debug.Log(num);
                        Vector3 pos = bounds.Min + new Vector3(i * cellSize.x, j * cellSize.y, k * cellSize.z);
                        int mortonNum = JunGeometry.PosToMortonNumber(pos, mortonBasePos, dimensionLevel, cellSize);
                        intersectMortonSpaces[num] = mortonNum;
                    }
                }
            }

            return JunExpandUnityClass.ConvertToUniqueArray(intersectMortonSpaces)
                .Where(n => n < Mathf.Pow(Mathf.Pow(2, dimensionLevel), 3)).ToArray();
        }
        //綺麗にしたバージョン
        public static int[] GetMortonNumbersFromAABB(AABB3D bounds, Vector3 mortonBasePos, int dimensionLevel, Vector3 cellSize)
        {
            // AABB3Dが占めるセル数を計算（切り上げ）
            // 例えばAABB3Dがきっちり2マス分だとまずかったので修正
            int minX = Mathf.FloorToInt((bounds.Min.x - mortonBasePos.x) / cellSize.x);
            int minY = Mathf.FloorToInt((bounds.Min.y - mortonBasePos.y) / cellSize.y);
            int minZ = Mathf.FloorToInt((bounds.Min.z - mortonBasePos.z) / cellSize.z);
    
            int maxX = Mathf.CeilToInt((bounds.Max.x - mortonBasePos.x) / cellSize.x);
            int maxY = Mathf.CeilToInt((bounds.Max.y - mortonBasePos.y) / cellSize.y);
            int maxZ = Mathf.CeilToInt((bounds.Max.z - mortonBasePos.z) / cellSize.z);
    
            // 範囲の制限
            int cellsPerDimension = 1 << dimensionLevel; // 2^dimensionLevel
            minX = Mathf.Max(0, minX);
            minY = Mathf.Max(0, minY);
            minZ = Mathf.Max(0, minZ);
            maxX = Mathf.Min(cellsPerDimension - 1, maxX);
            maxY = Mathf.Min(cellsPerDimension - 1, maxY);
            maxZ = Mathf.Min(cellsPerDimension - 1, maxZ);
    
            int separateX = maxX - minX + 1;
            int separateY = maxY - minY + 1;
            int separateZ = maxZ - minZ + 1;
    
            List<int> mortonCodes = new List<int>(separateX * separateY * separateZ);
    
            for (int i = 0; i < separateX; i++)
            {
                for (int j = 0; j < separateY; j++)
                {
                    for (int k = 0; k < separateZ; k++)
                    {
                        Vector3 pos = mortonBasePos + new Vector3(
                            (minX + i) * cellSize.x,
                            (minY + j) * cellSize.y,
                            (minZ + k) * cellSize.z);
                
                        int mortonNum = PosToMortonNumber(pos, mortonBasePos, dimensionLevel, cellSize);
                        if (mortonNum >= 0)
                        {
                            mortonCodes.Add(mortonNum);
                        }
                    }
                }
            }
    
            // 重複を削除して返す
            return mortonCodes.Distinct().ToArray();
        }

        #endregion
        #region ・頂点を完全にカバーするBoundsを作成
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
                if (points[i].x < minX) minX = points[i].x;
                if (points[i].y < minY) minY = points[i].y;
                if (points[i].z < minZ) minZ = points[i].z;

                if (points[i].x > maxX) maxX = points[i].x;
                if (points[i].y > maxY) maxY = points[i].y;
                if (points[i].z > maxZ) maxZ = points[i].z;
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
                if (points[i].x < minX) minX = points[i].x;
                if (points[i].y < minY) minY = points[i].y;
                if (points[i].z < minZ) minZ = points[i].z;

                if (points[i].x > maxX) maxX = points[i].x;
                if (points[i].y > maxY) maxY = points[i].y;
                if (points[i].z > maxZ) maxZ = points[i].z;
            }

            AABB3D aabb = new AABB3D(new Vector3(minX, minY, minZ), new Vector3(maxX, maxY, maxZ));
            return aabb;
        }

        #endregion
        #region　・共分散（分散）を求める

        /// <summary>
        /// 共分散（分散）を求める <br/>
        /// 同じ要素を二つ入れると分散になる
        /// </summary>
        public static float CoVariance(float[] values1, float[] values2)
        {
            if (values1.Length != values2.Length) return 0;
            float avarage1 = values1.Average();
            float avarage2 = values2.Average();
            float covariance = 0f;

            for (int i = 0; i < values1.Length; i++)
            {
                covariance += ((values1[i] - avarage1) * (values2[i] - avarage2));
            }

            //今回は普通に母集団共分散（分散）、不偏共分散にしたい場合はvalues1.Length-1で割る方がいい
            return covariance / values1.Length;
        }

        #endregion
        #region ・重心を求める

        public static Vector3 Centroid(Vector3[] points)
        {
            Vector3 sum = Vector3.zero;
            foreach (Vector3 point in points)
                sum += point;

            return sum / (float)points.Length;
        }

        #endregion
        #region　・主成分分析

        /// <summary>
        /// 固有ベクトルを導出する
        /// （いまは第3主成分までしかもとまらないのでもうすこし汎用的にすることも視野に入れよう）
        /// </summary>
        /// <param name="points">固有ベクトルを求めたい点群</param>
        /// <returns></returns>
        public static (Vector3[] eigenVectors, float[] eigenValues) CalculateEigens(Vector3[] points)
        {
            // Debug.Log(string.Join(",", points));
            float[] xArray = new float[points.Length];
            float[] yArray = new float[points.Length];
            float[] zArray = new float[points.Length];

            for (int i = 0; i < points.Length; i++)
            {
                xArray[i] = points[i].x;
                yArray[i] = points[i].y;
                zArray[i] = points[i].z;
            }

            Vector3 centroid = new Vector3(xArray.Average(), yArray.Average(), zArray.Average());

            // まず標準偏差を計算
            float stdDevX = Mathf.Sqrt(CoVariance(xArray, xArray));
            float stdDevY = Mathf.Sqrt(CoVariance(yArray, yArray));
            float stdDevZ = Mathf.Sqrt(CoVariance(zArray, zArray));

            // その後、すべての要素を標準化
            //OBBではそもそもいらないかも
            for (int i = 0; i < points.Length; i++)
            {
                xArray[i] = (xArray[i] - centroid.x) / stdDevX;
                yArray[i] = (yArray[i] - centroid.y) / stdDevY;
                zArray[i] = (zArray[i] - centroid.z) / stdDevZ;
            }
            
            float a1 = CoVariance(xArray, xArray);
            float a2 = CoVariance(yArray, yArray);
            float a3 = CoVariance(zArray, zArray);
            float n1 = CoVariance(xArray, yArray);
            float n2 = CoVariance(xArray, zArray);
            float n3 = CoVariance(yArray, zArray);
            
            float a = -1;
            float b = a1 + a2 + a3;
            float c = -(a1 * a2 + a2 * a3 + a3 * a1 - n2 * n2 - n3 * n3 - n1 * n1);
            float d = a1 * a2 * a3 + 2 * (n1 * n2 * n3) - a1 * n3 * n3 - a2 * n2 * n2 - a3 * n1 * n1;

            double[,] matrix = new double[,] { { a1, n1, n2 }, { n1, a2, n3 }, { n2, n3, a3 } };
            //covarianceMatrix - λE　= 0となるλを探すため、行列の行列式を解く
            //分散共分散行列の全ての値は負にならないため、3つの異なる実数か、重解をもつ。複素数にはならない。
            double[] ans = JunMath.CubicSolver(a, b, c, d);

            //固有値を大きい順にソート
            double[] sortedAns = ans.OrderByDescending(value => value).ToArray();
            Debug.Log($"sortedAns {string.Join(",", sortedAns)}");
            
            const double EIGENVALUE_THRESHOLD = 1e-6;
            bool isDoubleEigenvalue = false;
            bool isTripleEigenvalue = false;

            if (Math.Abs(sortedAns[0] - sortedAns[1]) < EIGENVALUE_THRESHOLD ||
                Math.Abs(sortedAns[1] - sortedAns[2]) < EIGENVALUE_THRESHOLD)
            {
                isDoubleEigenvalue = true;
            }

            if (Math.Abs(sortedAns[0] - sortedAns[1]) < EIGENVALUE_THRESHOLD &&
                Math.Abs(sortedAns[1] - sortedAns[2]) < EIGENVALUE_THRESHOLD)
            {
                isTripleEigenvalue = true;
            }
            
            if (isTripleEigenvalue)
            {
                return (new Vector3[] { Vector3.right, Vector3.up, Vector3.forward },
                    new float[] { (float)sortedAns[0], (float)sortedAns[1], (float)sortedAns[2] });
            }
            
            if(isDoubleEigenvalue)
            {
                //3つの固有値のうち重解でない単独のものを探す
                //大きい順に並んでいるから0か2のどちらかが単独
                double eigenValue = Math.Abs(sortedAns[0] - sortedAns[1]) < EIGENVALUE_THRESHOLD ? sortedAns[2] : sortedAns[0];
                double[] axis1 = CalculateEigenVector(matrix, eigenValue);
                Vector3 v1 = new Vector3((float)axis1[0], (float)axis1[1], (float)axis1[2]);
                Vector3 v2 = Vector3.zero;
                
                if (Mathf.Abs(v1.x) > Mathf.Abs(v1.z))
                {
                    v2 = new Vector3(-v1.y, v1.x, 0).normalized;
                }
                else
                {
                    v2 = new Vector3(0, -v1.z, v1.y).normalized;
                }
                Vector3 v3 = Vector3.Cross(v1, v2).normalized;
                
                return (new Vector3[] { v1, v2, v3 },
                    new float[] { (float)sortedAns[0], (float)sortedAns[1], (float)sortedAns[2] });
            }
            
            //固有値が重解でない
            Vector3[] eigenVectors = new Vector3[3];
            float[] eigenValues = new float[3];
            for (int i = 0; i < sortedAns.Length; i++)
            {
                double[] vec = CalculateEigenVector(matrix, sortedAns[i]);
                
                eigenVectors[i] = new Vector3((float)vec[0], (float)vec[1], (float)vec[2]);
                eigenValues[i] = (float)sortedAns[i];
            }

            return (eigenVectors, eigenValues);
        }

        static void PrintMatrix(double[,] matrix)
        {
            int rowCount = matrix.GetLength(0);
            int colCount = matrix.GetLength(1);

            Debug.Log($"{matrix[0, 0]}, {matrix[0, 1]}, {matrix[0, 2]} \n " +
                      $"{matrix[1, 0]}, {matrix[1, 1]}, {matrix[1, 2]} \n" +
                      $"{matrix[2, 0]}, {matrix[2, 1]}, {matrix[2, 2]}");
        }
        /// <summary>
        /// 固有値と行列から固有ベクトルを計算する
        /// </summary>
        public static double[] CalculateEigenVector(double[,] matrix, double lambda)
        {
            //[ x1 - λ, y1, z1 ]
            //[ x2, y2 - λ, z2 ]
            //[ x3, y3, z3 - λ ]　こういうmatrix
            int matrixSize = matrix.GetLength(0);

            double[,] baseMatrix = (double[,])matrix.Clone();

            Debug.Log("基本行列計算前");
            PrintMatrix(baseMatrix);

            // A - λIの計算
            for (int i = 0; i < matrixSize; i++)
            {
                baseMatrix[i, i] -= lambda; // 対角成分からλを引く
            }

            for (int i = 0; i < matrixSize; i++)
            {
                double max = Math.Abs(baseMatrix[i, i]);
                int maxRow = i;

                //x, y,　zについて最大を探す
                for (int k = i + 1; k < matrixSize; k++)
                {
                    if (Math.Abs(baseMatrix[k, i]) > max)
                    {
                        max = Math.Abs(baseMatrix[k, i]);
                        maxRow = k;
                    }
                }

                //現在確認中の行（xなら1行、yなら2、zなら3）をそれぞれ入れ替える
                for (int j = 0; j < matrixSize; j++)
                {
                    (baseMatrix[i, j], baseMatrix[maxRow, j]) = (baseMatrix[maxRow, j], baseMatrix[i, j]);
                }

                //0にかぎりなく近いのであればcontinue いるかな〜?
                if (Math.Abs(baseMatrix[i, i]) < 1e-10) continue;

                double pivotValue = baseMatrix[i, i];
                for (int j = 0; j < matrixSize; j++)
                {
                    baseMatrix[i, j] /= pivotValue; // ピボットを1にする
                }

                // 下の行を0にする
                for (int k = i + 1; k < matrixSize; k++)
                {
                    double factor = baseMatrix[k, i];
                    for (int j = 0; j < matrixSize; j++)
                    {
                        baseMatrix[k, j] -= factor * baseMatrix[i, j];
                    }
                }
            }

            Debug.Log("基本行列計算後");
            PrintMatrix(baseMatrix);

            double[] eigenVector = new double[matrixSize];
            int freeVariableIndex = 2; // z を自由変数とする
            // 自由変数として、z の値を仮に1に設定
            eigenVector[freeVariableIndex] = 1;

            bool hasFreeVariable = false;

            // 後退代入でz、y、xの順に値を求める
            //i = 2, 1, 0
            for (int i = matrixSize - 1; i >= 0; i--)
            {
                if (i == freeVariableIndex)
                {
                    continue;
                }

                double sum = 0;
                // 現在の行より右側（下にある変数）の値を使ってsumを計算する
                for (int j = i + 1; j < matrixSize; j++)
                {
                    sum += baseMatrix[i, j] * eigenVector[j];
                }

                // 対角成分が0に近ければ、自由変数と見なしてスルー
                if (Math.Abs(baseMatrix[i, i]) < 1e-10)
                {
                    hasFreeVariable = true;
                    // すでに自由変数として値を設定している場合、そのまま使用する
                    if (Math.Abs(eigenVector[i]) < 1e-10)
                    {
                        eigenVector[i] = 1;
                    }

                    continue; // この行は後退代入で更新せず次へ
                }
                else
                {
                    eigenVector[i] = -sum / baseMatrix[i, i];
                }
            }

            // 自由変数があれば、ベクトルを調整
            if (hasFreeVariable)
            {
                // ここでは単純に1つの自由変数を用いてベクトルを正規化
                double norm = Math.Sqrt(eigenVector.Sum(v => v * v));
                if (norm > 1e-10)
                {
                    for (int i = 0; i < matrixSize; i++)
                    {
                        eigenVector[i] /= norm;
                    }
                }
            }
            else
            {
                // 正規化処理
                double norm = Math.Sqrt(eigenVector.Sum(v => v * v));
                if (norm > 1e-10)
                {
                    for (int i = 0; i < matrixSize; i++)
                    {
                        eigenVector[i] /= norm;
                    }
                }
            }

            return eigenVector;
        }
        #endregion
        #region　グラムシュミットの正規直交法
        public static Vector3[] GramSchmidt(Vector3[] vectors)
        {
            Vector3[] orthonormal = new Vector3[vectors.Length];

            for (int n = 0; n < vectors.Length; n++)
            {
                Vector3 unPrime = vectors[n];

                for (int j = 0; j < n; j++)
                {
                    unPrime -= JunMath.VectorDot(orthonormal[j], vectors[n]) * orthonormal[j];
                }

                //引数に同じ固有ベクトルが2個あった場合の対応
                //もしゼロベクトルに近ければ、新しい補助ベクトルを生成
                if (unPrime.magnitude < 1e-6f)
                {
                    unPrime = GenerateNewBasis(orthonormal, n);
                }

                orthonormal[n] = unPrime.normalized;
            }

            return orthonormal;
            
            Vector3 GenerateNewBasis(Vector3[] basis, int count)
            {
                Vector3 newVec = Vector3.zero;

                // 単位ベクトルセットから選択
                Vector3[] candidates = { Vector3.right, Vector3.up, Vector3.forward };

                foreach (var candidate in candidates)
                {
                    bool isValid = true;

                    // 既存の直交ベクトルと直交しているかチェック
                    for (int i = 0; i < count; i++)
                    {
                        if (Mathf.Abs(JunMath.VectorDot(basis[i], candidate)) > 0.9f)
                        {
                            isValid = false;
                            break;
                        }
                    }

                    if (isValid)
                    {
                        newVec = candidate;
                        break;
                    }
                }

                return newVec;
            }
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
    
    ///<summary> 汎用性を上げたAABB3DTree</summary>
    /// <typeparam name="T">保存したい情報のタプル</typeparam>
    public class AABB3DTree<T>
    {
        public TreeNode3D<T> rootNode;

        public AABB3DTree<T> BuildTree(List<(AABB3D bounds, T Info)> objects)
        {
            rootNode = BuildNode(objects);
            return this;
        }

        private TreeNode3D<T> BuildNode(List<(AABB3D bounds, T Info)> objects)
        {
            if (objects == null) return null;
            if (objects.Count == 0) return null;
            // 単一要素ならリーフノードを作成
            if (objects.Count == 1)
            {
                return new TreeNode3D<T>
                {
                    Bounds = objects[0].bounds, 
                    
                    InformationTuple = objects[0].Info
                };
            }
            //Debug.Log("ノード作成");
            //1.全てくっつける
            AABB3D combineAllBounds = new AABB3D(objects[0].bounds);
            //全部結合する
            foreach ((AABB3D bounds, T Info) obj in objects)
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
            List<(AABB3D bounds, T Info)> leftObjects = objects.GetRange(0, midIndex);
            List<(AABB3D bounds, T Info)> rightObjects = objects.GetRange(midIndex, objects.Count - midIndex);
            
            //5. ノードを作る
            TreeNode3D<T> node = new TreeNode3D<T> { Bounds = combineAllBounds};
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
        public List<(AABB3D bounds, T Info)> GetIntersectAABB3D(AABB3D frustumBounds)
        {
            List<(AABB3D bounds, T Info)> result = new List<(AABB3D bounds, T Info)>();
            IntersectAABB3DSearch(rootNode, frustumBounds, result);
            return result;
        }

        public List<TreeNode3D<T>> GetIntersectNode(AABB3D frustumBounds)
        {
            return IntersectNodeSearch(rootNode, frustumBounds);
        }

        private List<TreeNode3D<T>> IntersectNodeSearch(TreeNode3D<T> node, AABB3D frustumBounds)
        {
            List<TreeNode3D<T>> intersectedNodes = new List<TreeNode3D<T>>();

            if (node == null) return intersectedNodes;

            // AABB が視錐台の境界と交差しているか？
            if (node.Bounds.Intersects(frustumBounds))
            {
                // リーフノードならこのノードをリストに追加
                if (node.Left == null && node.Right == null)
                {
                    intersectedNodes.Add(node); // Nodeそのものを追加
                }
                else
                {
                    // 左右の子ノードを再帰的に探索
                    intersectedNodes.AddRange(IntersectNodeSearch(node.Left, frustumBounds));
                    intersectedNodes.AddRange(IntersectNodeSearch(node.Right, frustumBounds));
                }
            }

            return intersectedNodes; // 交差したノードのリストを返す
        }

        ///<summary> ///frustramBoundsと交差する全てのBoundsをさがしてくる ///</summary>
        private void IntersectAABB3DSearch(TreeNode3D<T> node, AABB3D frustumBounds, List<(AABB3D bounds, T Info)> result)
        {
            if (node == null) return;

            // AABB が視錐台の境界と交差しているか？
            if (node.Bounds.Intersects(frustumBounds))
            {
                // リーフノードならリストに追加
                if (node.Left == null && node.Right == null)
                {
                    result.Add((node.Bounds, node.InformationTuple));
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
    
    /// <summary>
    /// Tにはタプル
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TreeNode3D<T>
    {
        public AABB3D Bounds; //ノードのバウンディングボックス
        public TreeNode3D<T> Left; // 左の子ノード
        public TreeNode3D<T> Right; // 右の子ノード
        public TreeNode3D<T> Parent; //親のノード
        public T InformationTuple;
        
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
    /// <summary>
    /// 静的オブジェクトの管理に適したTree 動的な追加は考慮されない
    /// </summary>
    public class AABB3DTree
    {
        public TreeNode3D rootNode;

        public AABB3DTree BuildTree(List<(AABB3D bounds, OBB orientedBounds, Transform transform)> objects)
        {
            rootNode = BuildNode(objects);
            return this;
        }

        private TreeNode3D BuildNode(List<(AABB3D bounds, OBB orientedBounds, Transform transform)> objects)
        {
            if (objects == null) return null;
            if (objects.Count == 0) return null;
            // 単一要素ならリーフノードを作成
            if (objects.Count == 1)
            {
                return new TreeNode3D
                {
                    Bounds = objects[0].bounds, 
                    OrientedBounds = objects[0].orientedBounds, 
                    Transform = objects[0].transform
                };
            }
            //Debug.Log("ノード作成");
            //1.全てくっつける
            AABB3D combineAllBounds = new AABB3D(objects[0].bounds);
            //全部結合する
            foreach ((AABB3D bounds, OBB orientedBounds, Transform transform) obj in objects)
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
            List<(AABB3D bounds, OBB orientedBounds, Transform transform)> leftObjects = objects.GetRange(0, midIndex);
            List<(AABB3D bounds, OBB orientedBounds, Transform transform)> rightObjects = objects.GetRange(midIndex, objects.Count - midIndex);
            
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
        public List<(AABB3D bounds, OBB orientedBounds, Transform transform)> GetIntersectAABB3D(AABB3D frustumBounds)
        {
            List<(AABB3D bounds, OBB orientedBounds, Transform transform)> result = new List<(AABB3D bounds, OBB orientedBounds, Transform transform)>();
            IntersectAABB3DSearch(rootNode, frustumBounds, result);
            return result;
        }

        public List<TreeNode3D> GetIntersectNode(AABB3D frustumBounds)
        {
            return IntersectNodeSearch(rootNode, frustumBounds);
        }

        private List<TreeNode3D> IntersectNodeSearch(TreeNode3D node, AABB3D frustumBounds)
        {
            List<TreeNode3D> intersectedNodes = new List<TreeNode3D>();

            if (node == null) return intersectedNodes;

            // AABB が視錐台の境界と交差しているか？
            if (node.Bounds.Intersects(frustumBounds))
            {
                // リーフノードならこのノードをリストに追加
                if (node.Left == null && node.Right == null)
                {
                    intersectedNodes.Add(node); // Nodeそのものを追加
                }
                else
                {
                    // 左右の子ノードを再帰的に探索
                    intersectedNodes.AddRange(IntersectNodeSearch(node.Left, frustumBounds));
                    intersectedNodes.AddRange(IntersectNodeSearch(node.Right, frustumBounds));
                }
            }

            return intersectedNodes; // 交差したノードのリストを返す
        }

        ///<summary> ///frustramBoundsと交差する全てのBoundsをさがしてくる ///</summary>
        private void IntersectAABB3DSearch(TreeNode3D node, AABB3D frustumBounds, List<(AABB3D bounds, OBB orientedBounds, Transform transform)> result)
        {
            if (node == null) return;

            // AABB が視錐台の境界と交差しているか？
            if (node.Bounds.Intersects(frustumBounds))
            {
                // リーフノードならリストに追加
                if (node.Left == null && node.Right == null)
                {
                    result.Add((node.Bounds, node.OrientedBounds, node.Transform));
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
        public AABB3D Bounds; //ノードのバウンディングボックス
        public OBB OrientedBounds; //OrientedBoundingBoxだよん
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
        public Vector3 Size => (Max - Min);

        public AABB3D(Bounds bounds)
        {
            Min = bounds.min;
            Max = bounds.max;
        }

        public AABB3D(Vector3 a, Vector3 b)
        {
            float minX = Mathf.Min(a.x, b.x);
            float minY = Mathf.Min(a.y, b.y);
            float minZ = Mathf.Min(a.z, b.z);
            
            float maxX = Mathf.Max(a.x, b.x);
            float maxY = Mathf.Max(a.y, b.y);
            float maxZ = Mathf.Max(a.z, b.z);

            Min = new Vector3(minX, minY, minZ);
            Max = new Vector3(maxX, maxY, maxZ);
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

        public float Volume()
        {
            return (Max.x - Min.x) * (Max.y - Min.y) * (Max.z - Min.z);
        }
    }
    
    //オブジェクトの回転に合わせたバウンディングボックス
    //下にあるAllignedOBBの方が僕向きに改造してあるのであんま使わなそう
    //わざわざ3次方程式の解を調べたり固有ベクトル計算したのに...
    public class OBB
    {
        public Vector3 Center{get;}
        public Vector3[] Axis {get;}
        public Vector3[] Size {get;}
        public Vector3[] Vertices {get;}

        public OBB(Transform transform, Vector3[] points)
        {
            (Vector3[] eigenVectors, float[] eigenValues) = JunGeometry.CalculateEigens(points);
            Debug.Log($"{transform.right}, {transform.up}, {transform.forward}");
            Debug.Log($"{eigenVectors[0]}, {eigenVectors[1]}, {eigenVectors[2]}");
            
            //Vector3[] axis = JunGeometry.GramSchmidt(eigenVectors);
            (Vector3 min, Vector3 max) = CalculateSize(eigenVectors, points); 
            
            Vector3 origin = Vector3.zero;
            Vector3[] axis = new Vector3[3];
            for (int i = 0; i < 3; i++)
            {
                origin += (eigenVectors[i] * (min[i] + max[i]) * 0.5f);
                axis[i] = transform.localToWorldMatrix.MultiplyVector(eigenVectors[i] * (max[i] - min[i]));
            }

            Axis = axis;
            Center = transform.localToWorldMatrix.MultiplyPoint(origin);
            Vertices = CalculateVertices(Center, Axis);
            Size = new Vector3[]{axis[0] * 0.5f, axis[1] * 0.5f, axis[2] * 0.5f};
            // Debug.Log($"このオブジェクトの重心 : {Center}");
        }

        private (Vector3 min, Vector3 max) CalculateSize(Vector3[] axis, Vector3[] points)
        {
            Vector3 center = JunGeometry.Centroid(points);
            Vector3 minPos = Vector3.zero;
            Vector3 maxPos = Vector3.zero;

            for (int i = 0; i < axis.Length; i++)
            {
                float min = float.MaxValue;
                float max = float.MinValue;

                foreach (Vector3 point in points)
                {
                    //固有ベクトル　＝　軸に各点を投影し、大きさを比べる
                    //主成分分析でも同じ考え方だったね
                    float projection = JunMath.VectorDot(point - center, axis[i]);
                    min = Mathf.Min(min, projection);
                    max = Mathf.Max(max, projection);
                }

                //size[i] = max - min; // 各軸のサイズを格納
                minPos[i] = min;
                maxPos[i] = max;
            }

            return (minPos, maxPos);
        }
        
        private Vector3[] CalculateVertices(Vector3 center, Vector3[] axis)
        {
            Vector3 halfSizeX = axis[0] * 0.5f; // X軸方向の半分のサイズ
            Vector3 halfSizeY = axis[1] * 0.5f; // Y軸方向の半分のサイズ
            Vector3 halfSizeZ = axis[2] * 0.5f; // Z軸方向の半分のサイズ

            Vector3[] vertices = new Vector3[8];

            // 各頂点を計算
            vertices[0] = center - halfSizeX - halfSizeY - halfSizeZ; // (-X, -Y, -Z)
            vertices[1] = center + halfSizeX - halfSizeY - halfSizeZ; // (+X, -Y, -Z)
            vertices[2] = center - halfSizeX + halfSizeY - halfSizeZ; // (-X, +Y, -Z)
            vertices[3] = center + halfSizeX + halfSizeY - halfSizeZ; // (+X, +Y, -Z)
            vertices[4] = center - halfSizeX - halfSizeY + halfSizeZ; // (-X, -Y, +Z)
            vertices[5] = center + halfSizeX - halfSizeY + halfSizeZ; // (+X, -Y, +Z)
            vertices[6] = center - halfSizeX + halfSizeY + halfSizeZ; // (-X, +Y, +Z)
            vertices[7] = center + halfSizeX + halfSizeY + halfSizeZ; // (+X, +Y, +Z)

            return vertices;
        }
    }

    /// <summary>
    /// オブジェクトのtransformを使用したOOB　いい感じ
    /// </summary>
    /// <param name="Center">重心</param>
    /// <param name="Axis">OBBの軸と大きさを兼ねたベクトル</param>
    /// <param name="Vertices">頂点</param>
    /// <param name="Min, Max">最小、最大</param>
    /// <returns></returns>
    public class AlignedOBB
    {
        //OBBの中心（ワールド座標）
        public Vector3 Center { get; }

        //OBBの軸（ワールド座標）
        public Vector3[] Axis { get; }

        //OBBのバウンドにおける一番小さい座標（ワールド座標）
        public Vector3 Min { get; }

        //OBBのバウンドにおける一番大きい座標（ワールド座標）
        public Vector3 Max { get; }

        //OBBのバウンドの頂点（ワールド座標）
        public Vector3[] Vertices { get; }

        //OBBの各辺のサイズ（ワールド座標）
        public Vector3 Size { get; }

        public AlignedOBB(Transform transform, Vector3[] points)
        {
            Vector3[] eigenVectorsInWorld = new[] { transform.right, transform.up, transform.forward };
            Vector3[] eigenVectors = new Vector3[eigenVectorsInWorld.Length];

            for (int i = 0; i < eigenVectors.Length; i++)
            {
                eigenVectors[i] = transform.worldToLocalMatrix.MultiplyVector(eigenVectorsInWorld[i]);
            }

            //ローカル座標軸で最小と最大を見つける
            (Vector3 min, Vector3 max) = findMinAndMax(points);

            //ワールド座標軸で計算
            Min = transform.localToWorldMatrix.MultiplyPoint(min);
            Max = transform.localToWorldMatrix.MultiplyPoint(max);
            //Vector3 origin = Vector3.zero;
            Vector3[] axis = new Vector3[3];

            for (int i = 0; i < 3; i++)
            {
                //origin += (eigenVectors[i] * (min[i] + max[i]) * 0.5f);
                //localToWorldMatrix.MultiplyVectorはスケールの影響を受けない（常に長さが1になる）ので
                //ワールド空間における各辺の大きさを計算して掛け合わせる
                float distance = Vector3.Dot(Max - Min, eigenVectorsInWorld[i]);
                axis[i] = transform.localToWorldMatrix.MultiplyVector(eigenVectors[i]) * distance;
                //axis[i] = transform.localToWorldMatrix.MultiplyVector(eigenVectors[i] * (max[i] - min[i]));
            }
            //Debug.Log($"{axis[0].magnitude}, {axis[1].magnitude}, {axis[2].magnitude}");

            Axis = axis;
            Size = new Vector3(axis[0].magnitude, axis[1].magnitude, axis[2].magnitude);
            // foreach (Vector3 point in points)
            // {
            //     origin += point;
            // }
            //Center = transform.localToWorldMatrix.MultiplyPoint(origin / points.Length);
            //重心はメッシュの偏りに対して無力なので、普通に最大と最小の間を中心としよう...
            Center = (Min + Max) * 0.5f;
            Vertices = CalculateVertices(Center, Axis);
        }

        public AlignedOBB(Vector3 center, Vector3[] axis, Vector3 size)
        {
            Center = center;
            Axis = new[] {axis[0] * size.x, axis[1] * size.y, axis[2] * size.z};
            Size = size;
            Vertices = CalculateVertices(Center, Axis);
        }

        //ローカル空間で最小と最大を探す
        private (Vector3 min, Vector3 max) findMinAndMax(Vector3[] points)
        {
            Vector3 minPos = Vector3.zero;
            Vector3 maxPos = Vector3.zero;
            float minX = float.MaxValue, maxX = float.MinValue;
            float minY = float.MaxValue, maxY = float.MinValue;
            float minZ = float.MaxValue, maxZ = float.MinValue;

            for (int i = 0; i < points.Length; i++)
            {
                minX = Mathf.Min(minX, points[i].x);
                minY = Mathf.Min(minY, points[i].y);
                minZ = Mathf.Min(minZ, points[i].z);
                maxX = Mathf.Max(maxX, points[i].x);
                maxY = Mathf.Max(maxY, points[i].y);
                maxZ = Mathf.Max(maxZ, points[i].z);
            }

            minPos = new Vector3(minX, minY, minZ);
            maxPos = new Vector3(maxX, maxY, maxZ);

            return (minPos, maxPos);
        }

        private (Vector3 min, Vector3 max) CalculateSize(Vector3[] axis, Vector3[] points)
        {
            Vector3 center = JunGeometry.Centroid(points);
            Vector3 minPos = Vector3.zero;
            Vector3 maxPos = Vector3.zero;

            for (int i = 0; i < axis.Length; i++)
            {
                float min = float.MaxValue;
                float max = float.MinValue;

                foreach (Vector3 point in points)
                {
                    //固有ベクトル　＝　軸に各点を投影し、大きさを比べる
                    //主成分分析でも同じ考え方だったね
                    float projection = JunMath.VectorDot(point - center, axis[i]);
                    min = Mathf.Min(min, projection);
                    max = Mathf.Max(max, projection);
                }

                //size[i] = max - min; // 各軸のサイズを格納
                minPos[i] = min;
                maxPos[i] = max;
            }

            return (minPos, maxPos);
        }

        private Vector3[] CalculateVertices(Vector3 center, Vector3[] axis)
        {
            Vector3 halfSizeX = axis[0] * 0.5f; // X軸方向の半分のサイズ
            Vector3 halfSizeY = axis[1] * 0.5f; // Y軸方向の半分のサイズ
            Vector3 halfSizeZ = axis[2] * 0.5f; // Z軸方向の半分のサイズ

            Vector3[] vertices = new Vector3[8];

            // 各頂点を計算
            vertices[0] = center - halfSizeX - halfSizeY - halfSizeZ; // (-X, -Y, -Z)
            vertices[1] = center + halfSizeX - halfSizeY - halfSizeZ; // (+X, -Y, -Z)
            vertices[2] = center - halfSizeX + halfSizeY - halfSizeZ; // (-X, +Y, -Z)
            vertices[3] = center + halfSizeX + halfSizeY - halfSizeZ; // (+X, +Y, -Z)
            vertices[4] = center - halfSizeX - halfSizeY + halfSizeZ; // (-X, -Y, +Z)
            vertices[5] = center + halfSizeX - halfSizeY + halfSizeZ; // (+X, -Y, +Z)
            vertices[6] = center - halfSizeX + halfSizeY + halfSizeZ; // (-X, +Y, +Z)
            vertices[7] = center + halfSizeX + halfSizeY + halfSizeZ; // (+X, +Y, +Z)

            return vertices;
        }

        public bool IsOBBIntersection(AlignedOBB otherObb)
        {
            // OBB間のベクトル
            Vector3 distance = Center - otherObb.Center;
            Vector3[] testAxes = new Vector3[15];

            Vector3[] axes = new Vector3[] { Axis[0].normalized, Axis[1].normalized, Axis[3].normalized };
            Vector3[] otherAxes = new Vector3[] { otherObb.Axis[0].normalized, otherObb.Axis[1].normalized, otherObb.Axis[2].normalized };
            //どうせSizeもあって正規化するならAxisは大きさ持たなくてもいい気がするけど...
            testAxes[0] = axes[0];
            testAxes[1] = axes[1];
            testAxes[2] = axes[2];
            testAxes[3] = otherAxes[0];
            testAxes[4] = otherAxes[1];
            testAxes[5] = otherAxes[2];
            
            //各軸同士の外積（9本）
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    Vector3 cross = Vector3.Cross(axes[i], otherAxes[j]);
                    testAxes[6 + 3 * i + j] = cross;
                }
            }

            foreach(Vector3 axis in axes)
            {
                if(axis.sqrMagnitude < 1e-6f) continue;
                
                Vector3 normalizedAxis = axis.normalized;
                
                float radiusA = 0f;
                float radiusB = 0f;
        
                for (int i = 0; i < 3; i++)
                {
                    radiusA += Mathf.Abs(Vector3.Dot(axes[i] * Size[i] * 0.5f, normalizedAxis));
                    radiusB += Mathf.Abs(Vector3.Dot(otherAxes[i] * otherObb.Size[i] * 0.5f, normalizedAxis));
                }
        
                // 中心間距離の投影
                float distance_proj = Mathf.Abs(Vector3.Dot(distance, normalizedAxis));
        
                // 分離軸が見つかった場合、衝突していない
                if (distance_proj > radiusA + radiusB)
                {
                    return false;
                }
            }
            
            return true;
        }
        
        public bool IsThickLineIntersection(Vector3 lineStart, Vector3 lineEnd, float width, float height)
        {
            Vector3 lineDir = lineEnd - lineStart;
            float lineLength = lineDir.magnitude;
            
            // 線分の方向が0に近い場合は計算できないため早期リターン
            if (lineLength < 1e-6f)
                return false;
                
            lineDir = lineDir.normalized;

            // 線分の端とOBBの中心との相対位置ベクトル
            Vector3 relativeStart = lineStart - Center;
            Vector3 relativeEnd = lineEnd - Center;

            // 線分の上方向と右方向を決定
            // 線分の方向に垂直な2つのベクトルを作成
            Vector3 upDir;
            if (Mathf.Abs(Vector3.Dot(lineDir, Vector3.up)) > 0.99f)
            {
                // 線分がほぼ上方向を向いている場合は右方向を基準にする
                upDir = Vector3.Cross(lineDir, Vector3.right).normalized;
            }
            else
            {
                // それ以外の場合は上方向を基準にする
                upDir = Vector3.Cross(Vector3.up, lineDir).normalized;
            }
            Vector3 rightDir = Vector3.Cross(upDir, lineDir).normalized;

            // 「ライン vs OBB」判定に必要な分離軸を生成
            // 幅と高さを持つ線分は実質的には直方体として扱うので、軸は最大15本必要
            Vector3[] testAxes = new Vector3[15];

            //OBBの3つの軸
            Vector3 axisX = Axis[0].normalized;
            Vector3 axisY = Axis[1].normalized;
            Vector3 axisZ = Axis[2].normalized;

            testAxes[0] = axisX;
            testAxes[1] = axisY;
            testAxes[2] = axisZ;

            //線分の3つの軸（方向、幅、高さ）
            testAxes[3] = lineDir;
            testAxes[4] = rightDir;
            testAxes[5] = upDir;

            //OBBの軸と線分の3軸との外積（9本）
            testAxes[6] = Vector3.Cross(axisX, lineDir);
            testAxes[7] = Vector3.Cross(axisX, rightDir);
            testAxes[8] = Vector3.Cross(axisX, upDir);
            testAxes[9] = Vector3.Cross(axisY, lineDir);
            testAxes[10] = Vector3.Cross(axisY, rightDir);
            testAxes[11] = Vector3.Cross(axisY, upDir);
            testAxes[12] = Vector3.Cross(axisZ, lineDir);
            testAxes[13] = Vector3.Cross(axisZ, rightDir);
            testAxes[14] = Vector3.Cross(axisZ, upDir);
            
            // 各軸に対して射影するよ
            foreach (Vector3 axisRaw in testAxes)
            {
                if (axisRaw.sqrMagnitude < 1e-6f) continue;
                Vector3 axis = axisRaw.normalized;

                //OBBの投影範囲
                float halfWidth =
                    Mathf.Abs(Vector3.Dot(axisX, axis) * Size.x * 0.5f) +
                    Mathf.Abs(Vector3.Dot(axisY, axis) * Size.y * 0.5f) +
                    Mathf.Abs(Vector3.Dot(axisZ, axis) * Size.z * 0.5f);

                //太い線分の投影範囲を計算
                //線分自体の始点と終点
                float lineProjStart = Vector3.Dot(relativeStart, axis);
                float lineProjEnd = Vector3.Dot(relativeEnd, axis);
                
                //幅方向と高さ方向の寄与を計算
                float widthContribution = Mathf.Abs(Vector3.Dot(rightDir * width * 0.5f, axis));
                float heightContribution = Mathf.Abs(Vector3.Dot(upDir * height * 0.5f, axis));
                
                //線分の太さを考慮した投影範囲
                float lineProjMin = Mathf.Min(lineProjStart, lineProjEnd) - widthContribution - heightContribution;
                float lineProjMax = Mathf.Max(lineProjStart, lineProjEnd) + widthContribution + heightContribution;

                //中心点の投影は0（relativeを使っているため）
                float centerMinProj = -halfWidth;
                float centerMaxProj = +halfWidth;

                // 分離軸が存在するかチェック（1次元の範囲が離れていれば衝突なし）
                if (lineProjMax < centerMinProj || lineProjMin > centerMaxProj)
                {
                    // この軸で完全に分離しているので衝突していない
                    return false;
                }
            }

            // すべての軸で分離されていない = 衝突している
            return true;
        }
        
        public bool IsLineIntersection(Vector3 lineStart, Vector3 lineEnd)
        {
            Vector3 lineDir = lineEnd - lineStart;
            float lineLength = lineDir.magnitude;
            lineDir = lineDir.normalized;

            // 線分の端とOBBの中心との相対位置ベクトル
            Vector3 relativeStart = lineStart - Center;
            Vector3 relativeEnd   = lineEnd   - Center;

            // 「ライン vs OBB」判定に必要な分離軸だけ生成
            Vector3[] testAxes = new Vector3[7];

            //どうせSizeもあって正規化するならAxisは大きさ持たなくてもいい気がするけど...
            Vector3 axisX = Axis[0].normalized;
            Vector3 axisY = Axis[1].normalized;
            Vector3 axisZ = Axis[2].normalized;

            //OBB vs OBBは15本の軸がいるらしいけど、OBB vs 線分　なら7本でいいらしい
            //OBBの3つの軸
            testAxes[0] = axisX;
            testAxes[1] = axisY;
            testAxes[2] = axisZ;

            //線分の方向ベクトル
            testAxes[3] = lineDir;

            //OBBの軸と線分方向ベクトルの外積
            testAxes[4] = Vector3.Cross(axisX, lineDir);
            testAxes[5] = Vector3.Cross(axisY, lineDir);
            testAxes[6] = Vector3.Cross(axisZ, lineDir);
            
            // 各軸に対して射影するよ
            foreach (Vector3 axisRaw in testAxes)
            {
                if (axisRaw.sqrMagnitude < 1e-6f) continue;
                Vector3 axis = axisRaw.normalized;

                //OBBの投影範囲
                float halfWidth =
                    Mathf.Abs(Vector3.Dot(axisX, axis) * Size.x * 0.5f) +
                    Mathf.Abs(Vector3.Dot(axisY, axis) * Size.y * 0.5f) +
                    Mathf.Abs(Vector3.Dot(axisZ, axis) * Size.z * 0.5f);

                //線分の投影範囲
                float lineProjStart = Vector3.Dot(relativeStart, axis);
                float lineProjEnd   = Vector3.Dot(relativeEnd,   axis);
                float lineProjMin   = Mathf.Min(lineProjStart, lineProjEnd);
                float lineProjMax   = Mathf.Max(lineProjStart, lineProjEnd);

                //中心点の投影は0（relativeを使っているため）
                float centerMinProj = -halfWidth;
                float centerMaxProj = +halfWidth;

                // 分離軸が存在するかチェック（1次元の範囲が離れていれば衝突なし）
                if (lineProjMax < centerMinProj || lineProjMin > centerMaxProj)
                {
                    // この軸で完全に分離しているので衝突していない
                    return false;
                }
            }

            // すべての軸で分離されていない = 衝突している
            return true;
        }
    }

    public class RRTStar
    {
        private List<RRTSNode> _rrtsNodes;
        //一定方向にどれだけ進むんだい
        private float _stepDistance;
        //効率的なノードにするために、近くのノードができた場合に対処するのが通常のRRTとの違い（*）
        //近くにあるノードを調べるために一定の範囲を設定しておく 
        //_stepLengthよりはちょい大きめがいいかも？
        private float _neighborRadius;
        //どこまで近づいたらごーるとするか
        private float _thresholdDistance;
        // 最大試行回数
        private int _maxIterations;
        // ゴール方向を基準として設定する確率
        private float _goalBias;
        // サンプリング範囲
        private float _minRange;
        private float _maxRange;

        //ステージの中心座標
        private Vector3 _stageCenter;

        //線分との当たり判定　中身は外付けでいけるのでいいね
        private Func<Vector3, Vector3, bool> _collideFunc;
        
        public RRTStar(float stepDistance, float neighborRadius, float thresholdDistance, 
            Func<Vector3, Vector3, bool> collideFunc, int maxIterations = 5000, 
            float goalBias = 0.1f, float minRange = -100f, float maxRange = 100f, Vector3 stageCenter = default)
        {
            _stepDistance = stepDistance;
            _neighborRadius = neighborRadius;
            _thresholdDistance = thresholdDistance;
            _collideFunc = collideFunc;
            _maxIterations = maxIterations;
            _goalBias = goalBias;
            _minRange = minRange;
            _maxRange = maxRange;

            _stageCenter = stageCenter;
        
            _rrtsNodes = new List<RRTSNode>();
        }

        public List<Vector3> FindPath(Vector3 startPos, Vector3 goalPos)
        {
            _rrtsNodes.Clear(); // 初期化
            _rrtsNodes.Add(new RRTSNode(startPos, null, 0f));

            RRTSNode goalNode = null;
            float bestGoalDistance = float.MaxValue;

            for(int i = 0; i < _maxIterations; i++)
            {
                //適当な点を取ってくる
                Vector3 randomPoint = GetRandomPoint(goalPos);

                //これまで辿ってきたNodeのなかから適当な点と一番近い点のノードを選ぶ
                RRTSNode nearestNode = GetNearestNode(randomPoint);

                //ノードの点から適当な点の方向にある程度(stepDistanceぶん)進める
                Vector3 newPoint = VectorStep(nearestNode.Position, randomPoint);
                
                // 障害物チェック
                if (!IsCollide(nearestNode.Position, newPoint))
                {
                    // コスト計算
                    float newCost = nearestNode.Cost + Vector3.Distance(nearestNode.Position, newPoint);
                    
                    // Debug.Log($"RandomPoint : {randomPoint}");
                    // Debug.Log($"NewPoint : {newPoint}");
                    //
                    RRTSNode newNode = new RRTSNode(newPoint, nearestNode, newCost);
                    _rrtsNodes.Add(newNode);

                    // 近傍ノードを探して最適化 ここがRRT*アルゴリズムの肝だよ
                    OptimizeWithNeighbors(newNode);

                    // ゴールとの距離をチェック
                    float distToGoal = Vector3.Distance(newPoint, goalPos);
                    if (distToGoal < _thresholdDistance)
                    {
                        // ゴールに到達したノードを記録
                        if (distToGoal < bestGoalDistance)
                        {
                            bestGoalDistance = distToGoal;
                            goalNode = newNode;
                        }
                    }

                    // デバッグ情報
                    if (i % 100 == 0)
                    {
                        // Debug.Log($"反復: {i}, 最良のゴール距離: {bestGoalDistance}");
                    }
                }
            }

            //ゴールが見つかればそこまでの経路を返すよ
            if (goalNode != null)
            {
                Debug.Log("経路を発見しました！");
                return ConstructPath(goalNode);
            }

            //ゴールが見つからないなら一番ゴールまで近い場所を返す
            if (_rrtsNodes.Count > 1)
            {
                RRTSNode closestToGoal = null;
                float minDist = float.MaxValue;
                
                foreach (RRTSNode node in _rrtsNodes)
                {
                    float dist = Vector3.Distance(node.Position, goalPos);
                    if (dist < minDist)
                    {
                        minDist = dist;
                        closestToGoal = node;
                    }
                }
                
                Debug.Log($"完全な経路は見つかりませんでした。最も近いノードまでの距離: {minDist}");
                return ConstructPath(closestToGoal);
            }

            //無駄足
            return null;
        }
        
        private bool IsCollide(Vector3 start, Vector3 end)
        {
            return _collideFunc(start, end);
        }

        private void OptimizeWithNeighbors(RRTSNode newNode)
        {
            // 近傍ノードを取得
            List<RRTSNode> neighbors = GetNeighborNodes(newNode);
            
            // 近傍ノードへの再ワイヤリング（RRT* の核心部分）
            foreach (RRTSNode neighbor in neighbors)
            {
                // 自分自身と親は除外
                if (neighbor == newNode || neighbor == newNode.Parent)
                    continue;
                    
                // この近傍ノードへの直接パスに障害物がないか確認
                if (!IsCollide(newNode.Position, neighbor.Position))
                {
                    // 新しいコストを計算
                    float potentialCost = neighbor.Cost + Vector3.Distance(neighbor.Position, newNode.Position);
                    
                    // より良いパスが見つかれば更新
                    if (potentialCost < newNode.Cost)
                    {
                        newNode.Parent = neighbor;
                        newNode.Cost = potentialCost;
                    }
                }
            }
            
            // 近傍ノードのコスト最適化（これもRRT* の重要部分）
            foreach (RRTSNode neighbor in neighbors)
            {
                // 自分自身は除外
                if (neighbor == newNode)
                    continue;
                    
                // 新しいノードを経由したパスに障害物がないか確認
                if (!IsCollide(newNode.Position, neighbor.Position))
                {
                    // 新しいコストを計算
                    float potentialCost = newNode.Cost + Vector3.Distance(newNode.Position, neighbor.Position);
                    
                    // より良いパスが見つかれば更新
                    if (potentialCost < neighbor.Cost)
                    {
                        neighbor.Parent = newNode;
                        neighbor.Cost = potentialCost;
                    }
                }
            }
        }
        
        private RRTSNode GetNearestNode(Vector3 point)
        {
            RRTSNode nearestNode = null;
            float minDistance = float.MaxValue;

            foreach (RRTSNode node in _rrtsNodes)
            {
                //Nodeに入っている各点データとの距離
                float distance = Vector3.Distance(node.Position, point);
                
                //近いものをえらぶ
                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearestNode = node; // 最も近いノードを更新
                }
            }

            return nearestNode; // 最も近いノードを返す
        }

        private Vector3 GetRandomPoint(Vector3 goal)
        {
            // ゴールにバイアスをかける
            if (UnityEngine.Random.value < _goalBias)
            {
                return goal;
            }
            else
            {
                float randomX = UnityEngine.Random.Range(_minRange, _maxRange);
                float randomY = UnityEngine.Random.Range(_minRange, _maxRange);
                float randomZ = UnityEngine.Random.Range(_minRange, _maxRange);
                return new Vector3(randomX + _stageCenter.x, _stageCenter.y, randomZ + _stageCenter.z);
            }
        }
        
        //適当な点に向かって一定の長さだけ歩みを進める処理
        private Vector3 VectorStep(Vector3 from, Vector3 to)
        {
            Vector3 direction = (to - from).normalized; // 方向を計算
            return from + direction * _stepDistance; // 新しいポイントを計算
        }

        private List<RRTSNode> GetNeighborNodes(RRTSNode node)
        {
            List<RRTSNode> neighbors = new List<RRTSNode>();

            foreach (RRTSNode rrtsNode in _rrtsNodes)
            {
                //近いか判定
                if (Vector3.Distance(rrtsNode.Position, node.Position) < _neighborRadius)
                {
                    neighbors.Add(rrtsNode); // 近傍ノードとして追加
                }
            }

            return neighbors; // 近傍ノードのリストを返す
        }
        
        private List<Vector3> ConstructPath(RRTSNode endNode)
        {
            List<Vector3> path = new List<Vector3>();
            RRTSNode currentNode = endNode;

            //さかのぼっていく（最初のNodeはかならず親がnullのはず）
            while (currentNode != null)
            {
                path.Add(currentNode.Position); // 経路を構築
                currentNode = currentNode.Parent; // 親ノードを辿る
            }
            
            path.Reverse(); // 逆順にして開始点からの経路にする
            return path; // 経路を返す
        }
        
        private class RRTSNode
        {
            public Vector3 Position;
            public RRTSNode Parent;
            public float Cost; // 開始点からのコスト

            public RRTSNode(Vector3 position, RRTSNode parent, float cost)
            {
                Position = position;
                Parent = parent;
                Cost = cost;
            }
        }
    }
    
    public class RRTS
    {
        private List<RRTSNode> _rrtsNodes;
        private float _stepDistance;
        private float _neighborRadius;
        private float _thresholdDistance;
        private int _maxIterations;
        private float _goalBias;
        private float _minRange;
        private float _maxRange;
        private Vector3 _stageCenter;
        private Func<Vector3, Vector3, bool> _collideFunc;

        // Regeneration and pruning parameters
        private int _stagnationThreshold = 500;
        private int _maintenanceInterval = 100;
        private int _pruneInterval = 1000;
        private float _regenerationRadiusMultiplier = 2f;
        
        // Parameters for improved sampling strategy
        private List<Vector3> _collisionPoints = new List<Vector3>();
        private float _collisionBias = 0.15f;
        private float _collisionSamplingRadius = 5f;

        public RRTS(Vector3 stageCenter, float stepDistance, float neighborRadius, float thresholdDistance, 
            Func<Vector3, Vector3, bool> collideFunc, int maxIterations = 5000, 
            float goalBias = 0.1f, float minRange = -100f, float maxRange = 100f)
        {
            _stepDistance = stepDistance;
            _neighborRadius = neighborRadius;
            _thresholdDistance = thresholdDistance;
            _collideFunc = collideFunc;
            _maxIterations = maxIterations;
            _goalBias = goalBias;
            _minRange = minRange;
            _maxRange = maxRange;
            _stageCenter = stageCenter;
            _rrtsNodes = new List<RRTSNode>();
        }

        public List<Vector3> FindPath(Vector3 startPos, Vector3 goalPos)
        {
            _rrtsNodes.Clear();
            _collisionPoints.Clear();
            // Create initial node (with iteration 0)
            _rrtsNodes.Add(new RRTSNode(startPos, null, 0f, 0));

            RRTSNode goalNode = null;
            float bestGoalDistance = float.MaxValue;

            for (int i = 0; i < _maxIterations; i++)
            {
                Vector3 randomPoint = GetRandomPoint(goalPos);
                RRTSNode nearestNode = GetNearestNode(randomPoint);
                Vector3 newPoint = VectorStep(nearestNode.Position, randomPoint);

                // Check for collision and record collision points
                if (!IsCollide(nearestNode.Position, newPoint))
                {
                    float newCost = nearestNode.Cost + Vector3.Distance(nearestNode.Position, newPoint);
                    RRTSNode newNode = new RRTSNode(newPoint, nearestNode, newCost, i);
                    _rrtsNodes.Add(newNode);
                    nearestNode.Children.Add(newNode); // Track child nodes for proper pruning
                    nearestNode.LastExtensionIteration = i;

                    OptimizeWithNeighbors(newNode);

                    float distToGoal = Vector3.Distance(newPoint, goalPos);
                    if (distToGoal < _thresholdDistance)
                    {
                        if (distToGoal < bestGoalDistance)
                        {
                            bestGoalDistance = distToGoal;
                            goalNode = newNode;
                        }
                    }

                    if (i % 100 == 0)
                    {
                        Debug.Log($"Iteration: {i}, Best goal distance: {bestGoalDistance}");
                    }
                }
                else
                {
                    // Record collision point for improved sampling
                    _collisionPoints.Add(newPoint);
                    // Limit size of collision points list to prevent memory issues
                    if (_collisionPoints.Count > 1000)
                    {
                        _collisionPoints.RemoveAt(0);
                    }
                }

                // Maintenance phase: Node regeneration
                if (i % _maintenanceInterval == 0)
                {
                    foreach (var node in _rrtsNodes.ToList()) // Create a copy to avoid collection modification issues
                    {
                        if (i - node.LastExtensionIteration > _stagnationThreshold)
                        {
                            // Sample around collision points if any, otherwise sample around node
                            Vector3 newSample;
                            if (_collisionPoints.Count > 0 && UnityEngine.Random.value < _collisionBias)
                            {
                                // Sample near a random collision point
                                Vector3 collisionPoint = _collisionPoints[UnityEngine.Random.Range(0, _collisionPoints.Count)];
                                newSample = SampleAround3D(collisionPoint, _collisionSamplingRadius);
                            }
                            else
                            {
                                newSample = SampleAround3D(node.Position, _stepDistance * _regenerationRadiusMultiplier);
                            }

                            if (!IsCollide(node.Position, newSample))
                            {
                                float regenCost = node.Cost + Vector3.Distance(node.Position, newSample);
                                RRTSNode regeneratedNode = new RRTSNode(newSample, node, regenCost, i);
                                _rrtsNodes.Add(regeneratedNode);
                                node.Children.Add(regeneratedNode); // Track child nodes
                                node.LastExtensionIteration = i;
                            }
                        }
                    }
                }

                // Maintenance phase: Node pruning
                if (i % _pruneInterval == 0)
                {
                    List<RRTSNode> nodesToRemove = new List<RRTSNode>();
                    foreach (var node in _rrtsNodes)
                    {
                        if (node.Parent != null && (i - node.LastExtensionIteration > _stagnationThreshold * 2))
                        {
                            // If node has no children and hasn't been extended recently, mark for removal
                            if (node.Children.Count == 0)
                            {
                                nodesToRemove.Add(node);
                            }
                        }
                    }

                    // Safe removal of nodes, handling parent-child references
                    foreach (var node in nodesToRemove)
                    {
                        if (node.Parent != null)
                        {
                            node.Parent.Children.Remove(node); // Remove from parent's children list
                        }
                        _rrtsNodes.Remove(node);
                    }
                }
            }

            if (goalNode != null)
            {
                Debug.Log("Path found!");
                return ConstructPath(goalNode);
            }
            if (_rrtsNodes.Count > 1)
            {
                RRTSNode closestToGoal = null;
                float minDist = float.MaxValue;
                foreach (RRTSNode node in _rrtsNodes)
                {
                    float dist = Vector3.Distance(node.Position, goalPos);
                    if (dist < minDist)
                    {
                        minDist = dist;
                        closestToGoal = node;
                    }
                }
                Debug.Log($"No complete path found. Closest node distance: {minDist}");
                return ConstructPath(closestToGoal);
            }
            return null;
        }

        private bool IsCollide(Vector3 start, Vector3 end)
        {
            return _collideFunc(start, end);
        }

        private void OptimizeWithNeighbors(RRTSNode newNode)
        {
            List<RRTSNode> neighbors = GetNeighborNodes(newNode);
            foreach (RRTSNode neighbor in neighbors)
            {
                if (neighbor == newNode || neighbor == newNode.Parent)
                    continue;
                if (!IsCollide(newNode.Position, neighbor.Position))
                {
                    float potentialCost = neighbor.Cost + Vector3.Distance(neighbor.Position, newNode.Position);
                    if (potentialCost < newNode.Cost)
                    {
                        // Update parent reference
                        if (newNode.Parent != null)
                        {
                            newNode.Parent.Children.Remove(newNode);
                        }
                        newNode.Parent = neighbor;
                        neighbor.Children.Add(newNode);
                        newNode.Cost = potentialCost;
                    }
                }
            }
            foreach (RRTSNode neighbor in neighbors)
            {
                if (neighbor == newNode)
                    continue;
                if (!IsCollide(newNode.Position, neighbor.Position))
                {
                    float potentialCost = newNode.Cost + Vector3.Distance(newNode.Position, neighbor.Position);
                    if (potentialCost < neighbor.Cost)
                    {
                        // Update parent reference
                        if (neighbor.Parent != null)
                        {
                            neighbor.Parent.Children.Remove(neighbor);
                        }
                        neighbor.Parent = newNode;
                        newNode.Children.Add(neighbor);
                        neighbor.Cost = potentialCost;
                    }
                }
            }
        }

        private RRTSNode GetNearestNode(Vector3 point)
        {
            RRTSNode nearestNode = null;
            float minDistance = float.MaxValue;
            foreach (RRTSNode node in _rrtsNodes)
            {
                float distance = Vector3.Distance(node.Position, point);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearestNode = node;
                }
            }
            return nearestNode;
        }

        private Vector3 GetRandomPoint(Vector3 goal)
        {
            // Biased sampling strategy
            float rand = UnityEngine.Random.value;
            
            // Goal biased sampling
            if (rand < _goalBias)
            {
                return goal;
            }
            // Collision biased sampling - sample near collision points
            else if (rand < _goalBias + _collisionBias && _collisionPoints.Count > 0)
            {
                Vector3 collisionPoint = _collisionPoints[UnityEngine.Random.Range(0, _collisionPoints.Count)];
                return SampleAround3D(collisionPoint, _collisionSamplingRadius);
            }
            // Regular random sampling
            else
            {
                float randomX = UnityEngine.Random.Range(_minRange, _maxRange);
                float randomY = UnityEngine.Random.Range(_minRange, _maxRange);
                float randomZ = UnityEngine.Random.Range(_minRange, _maxRange);
                return new Vector3(randomX + _stageCenter.x, randomY + _stageCenter.y, randomZ + _stageCenter.z);
            }
        }

        private Vector3 VectorStep(Vector3 from, Vector3 to)
        {
            Vector3 direction = (to - from).normalized;
            return from + direction * _stepDistance;
        }

        private List<RRTSNode> GetNeighborNodes(RRTSNode node)
        {
            List<RRTSNode> neighbors = new List<RRTSNode>();
            foreach (RRTSNode rrtsNode in _rrtsNodes)
            {
                if (Vector3.Distance(rrtsNode.Position, node.Position) < _neighborRadius)
                {
                    neighbors.Add(rrtsNode);
                }
            }
            return neighbors;
        }

        private List<Vector3> ConstructPath(RRTSNode endNode)
        {
            List<Vector3> path = new List<Vector3>();
            RRTSNode currentNode = endNode;
            while (currentNode != null)
            {
                path.Add(currentNode.Position);
                currentNode = currentNode.Parent;
            }
            path.Reverse();
            return path;
        }

        // Improved 3D sampling around a center point
        private Vector3 SampleAround3D(Vector3 center, float radius)
        {
            // Random direction in 3D space
            Vector3 randomDir = UnityEngine.Random.onUnitSphere;
            // Random distance within radius
            float distance = UnityEngine.Random.Range(0, radius);
            return center + randomDir * distance;
        }

        private class RRTSNode
        {
            public Vector3 Position;
            public RRTSNode Parent;
            public List<RRTSNode> Children; // Track children for proper reference management
            public float Cost; // Start-to-node cost
            public int LastExtensionIteration; // Iteration when last extended

            public RRTSNode(Vector3 position, RRTSNode parent, float cost, int iteration)
            {
                Position = position;
                Parent = parent;
                Cost = cost;
                LastExtensionIteration = iteration;
                Children = new List<RRTSNode>();
            }
        }
    }
}
