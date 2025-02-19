using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

namespace JunUtilitiesTest
{
    public static class EigenSystemTests
{
    private const double EPSILON = 1e-8; // 数値計算の誤差許容値

    public static bool ValidateEigenSystem(double[,] originalMatrix, double[] eigenValues, double[][] eigenVectors)
    {
        bool isValid = true;
        int n = eigenValues.Length;

        Console.WriteLine("固有システムの検証を開始します...");

        // テスト1: Av = λv の検証
        Console.WriteLine("\nテスト1: Av = λv の検証");
        for (int i = 0; i < n; i++)
        {
            double[] Av = MultiplyMatrixVector(originalMatrix, eigenVectors[i]);
            double[] lambdaV = ScaleVector(eigenVectors[i], eigenValues[i]);
            
            double error = CalculateVectorError(Av, lambdaV);
            bool test1Pass = error < EPSILON;
            isValid &= test1Pass;
            
            Debug.Log($"固有値 {eigenValues[i]:F6} のテスト：{(test1Pass ? "合格" : "不合格")}");
            Debug.Log($"誤差: {error:E}");
        }

        // テスト2: 固有ベクトルの正規化チェック
        Debug.Log("\nテスト2: 固有ベクトルの正規化チェック");
        for (int i = 0; i < n; i++)
        {
            double norm = CalculateNorm(eigenVectors[i]);
            bool test2Pass = Math.Abs(norm - 1.0) < EPSILON;
            isValid &= test2Pass;
            
            Debug.Log($"ベクトル{i + 1}の長さ: {norm:F6} ({(test2Pass ? "合格" : "不合格")})");
        }

        // テスト3: 固有ベクトルの直交性チェック
        Debug.Log("\nテスト3: 固有ベクトルの直交性チェック");
        for (int i = 0; i < n; i++)
        {
            for (int j = i + 1; j < n; j++)
            {
                if (Math.Abs(eigenValues[i] - eigenValues[j]) > EPSILON)
                {
                    double dotProduct = DotProduct(eigenVectors[i], eigenVectors[j]);
                    bool test3Pass = Math.Abs(dotProduct) < EPSILON;
                    isValid &= test3Pass;
                    
                    Debug.Log($"ベクトル{i + 1}とベクトル{j + 1}の内積: {dotProduct:E} ({(test3Pass ? "合格" : "不合格")})");
                }
            }
        }

        // テスト4: 固有値の実数性チェック
        Debug.Log("\nテスト4: 固有値の実数性チェック");
        bool allReal = eigenValues.All(λ => !double.IsNaN(λ) && !double.IsInfinity(λ));
        isValid &= allReal;
        Debug.Log($"すべての固有値が実数: {(allReal ? "合格" : "不合格")}");

        return isValid;
    }

    // 使用例を示すサンプルテスト
    public static void RunSampleTest()
    {
        // サンプルの対称行列（3x3）
        double[,] testMatrix = {
            {2, 1, 0},
            {1, 2, 1},
            {0, 1, 2}
        };

        // 期待される固有値と固有ベクトル
        double[] expectedEigenValues = {3.4142, 2.0, 0.5858}; // 近似値
        double[][] expectedEigenVectors = {
            new double[] {0.5774, 0.5774, 0.5774},
            new double[] {-0.7071, 0.0, 0.7071},
            new double[] {0.4082, -0.8165, 0.4082}
        };

        bool isValid = ValidateEigenSystem(testMatrix, expectedEigenValues, expectedEigenVectors);
        Debug.Log($"\n総合判定: {(isValid ? "すべてのテストに合格" : "一部のテストに不合格")}");
    }

    // ヘルパーメソッド
    private static double[] MultiplyMatrixVector(double[,] matrix, double[] vector)
    {
        int n = vector.Length;
        double[] result = new double[n];
        for (int i = 0; i < n; i++)
        {
            result[i] = 0;
            for (int j = 0; j < n; j++)
            {
                result[i] += matrix[i, j] * vector[j];
            }
        }
        return result;
    }

    private static double[] ScaleVector(double[] vector, double scalar)
    {
        return vector.Select(v => v * scalar).ToArray();
    }

    private static double CalculateVectorError(double[] v1, double[] v2)
    {
        return Math.Sqrt(v1.Zip(v2, (a, b) => Math.Pow(a - b, 2)).Sum());
    }

    private static double CalculateNorm(double[] vector)
    {
        return Math.Sqrt(vector.Sum(x => x * x));
    }

    private static double DotProduct(double[] v1, double[] v2)
    {
        return v1.Zip(v2, (a, b) => a * b).Sum();
    }
}
}
