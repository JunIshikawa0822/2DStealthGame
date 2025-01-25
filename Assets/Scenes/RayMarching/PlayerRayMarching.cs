using System;
using UnityEngine;
//using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Linq;

public class PlayerRayMarching : MonoBehaviour
{
    [SerializeField] private ComputeShader _rayMarchingComputeShader;
    [SerializeField] private int _rayCount = 10;
    [SerializeField, Range(0, 360)] private float _rayAngle = 90f;
    [SerializeField] private float _thresHoldValue = 0.01f;
    [SerializeField] private float _radius = 30;
    
    private int _kernelIndex;
    private int _outputBufferID;
    private int _objectBufferID;
    
    private int _originPosID;
    private int _forwardVecID;
    private int _objectCountID;

    private GraphicsBuffer _outputBuffer;
    private GraphicsBuffer _objectBuffer;
    
    private int _objectDataSize;
    public GameObject[] objectArray;
    //public List<Transform> oldTargets = new List<Transform>();
    //public List<Transform> newTargets = new List<Transform>();
    
    private int _debugBufferID;
    private GraphicsBuffer _debugBuffer;
    private Vector3[] _debugResultArray;

    private int _debugBuffer2ID;
    private GraphicsBuffer _debugBuffer2;
    private int[] _debugResultArray2;
    
    private int _debugBuffer3ID;
    private GraphicsBuffer _debugBuffer3;
    private float[] _debugResultArray3;
    
    private int _debugBuffer4ID;
    private GraphicsBuffer _debugBuffer4;
    private float[] _debugResultArray4;
    
    private int _debugBuffer5ID;
    private GraphicsBuffer _debugBuffer5;
    private float[] _debugResultArray5;
    
    private int _debugBuffer6ID;
    private GraphicsBuffer _debugBuffer6;
    private int[] _debugResultArray6;

    struct GameObjectData
    {
        public Vector3 position; //12バイト
        public float size; //4バイト
        public int type; //4バイト
        public Vector3 padding; //12バイト 16バイトの境界にあわせて整理
    }

    void Start()
    {
        _kernelIndex = _rayMarchingComputeShader.FindKernel("CSMain");
        Debug.Log($"_kernelIndex : {_kernelIndex}");
        
        if (_kernelIndex < 0)
        {
            Debug.LogError("Kernel not found! Check the kernel name in the compute shader.");
            return;
        }
        
        _outputBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, _rayCount, sizeof(int));
        // _objectBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured,);
        _outputBufferID = Shader.PropertyToID("_outputBuffer");
        _objectBufferID = Shader.PropertyToID("_objectsBuffer");
        
        _forwardVecID = Shader.PropertyToID("_forwardVec");
        _originPosID = Shader.PropertyToID("_originPos");
        _objectCountID = Shader.PropertyToID("_objectCount");
        
        _objectDataSize = 32;

        int objectCountID = Shader.PropertyToID("_objectCount");
        int maxDistanceID = Shader.PropertyToID("_maxDistance");
        int thresHoldID = Shader.PropertyToID("_threshold");
        int rayCountID = Shader.PropertyToID("_rayCount");
        int viewAngleID = Shader.PropertyToID("_viewAngle");

        _rayMarchingComputeShader.SetBuffer(0, _outputBufferID, _outputBuffer);
        
        _rayMarchingComputeShader.SetFloat(maxDistanceID, _radius);
        _rayMarchingComputeShader.SetFloat(thresHoldID, _thresHoldValue);
        _rayMarchingComputeShader.SetInt(rayCountID, _rayCount);
        _rayMarchingComputeShader.SetFloat(viewAngleID, _rayAngle);
        
        _debugBufferID = Shader.PropertyToID("_debugBuffer");
        _debugBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, 200, sizeof(float) * 3);
        _rayMarchingComputeShader.SetBuffer(0, _debugBufferID, _debugBuffer);
        _debugResultArray = new Vector3[200];
        
        _debugBuffer2ID = Shader.PropertyToID("_debugBuffer2");
        _debugBuffer2 = new GraphicsBuffer(GraphicsBuffer.Target.Structured, 10, sizeof(int));
        _rayMarchingComputeShader.SetBuffer(0, _debugBuffer2ID, _debugBuffer2);
        _debugResultArray2 = new int[10];
        
        _debugBuffer3ID = Shader.PropertyToID("_debugBuffer3");
        _debugBuffer3 = new GraphicsBuffer(GraphicsBuffer.Target.Structured, 200, sizeof(float));
        _rayMarchingComputeShader.SetBuffer(0, _debugBuffer3ID, _debugBuffer3);
        _debugResultArray3 = new float[200];
        
        _debugBuffer4ID = Shader.PropertyToID("_debugBuffer4");
        _debugBuffer4 = new GraphicsBuffer(GraphicsBuffer.Target.Structured, 200, sizeof(float));
        _rayMarchingComputeShader.SetBuffer(0, _debugBuffer4ID, _debugBuffer4);
        _debugResultArray4 = new float[200];
        
        _debugBuffer5ID = Shader.PropertyToID("_debugBuffer5");
        _debugBuffer5 = new GraphicsBuffer(GraphicsBuffer.Target.Structured, 2000, sizeof(float));
        _rayMarchingComputeShader.SetBuffer(0, _debugBuffer5ID, _debugBuffer5);
        _debugResultArray5 = new float[2000];
        
        _debugBuffer6ID = Shader.PropertyToID("_debugBuffer6");
        _debugBuffer6 = new GraphicsBuffer(GraphicsBuffer.Target.Structured, 2000, sizeof(int));
        _rayMarchingComputeShader.SetBuffer(0, _debugBuffer6ID, _debugBuffer6);
        _debugResultArray6 = new int[2000];
    }

    void Update()
    {
        Debug.Log("くぎり始め");
        // gameObjectBufferがすでに存在する場合は解放
        if (_objectBuffer != null)
        {
            _objectBuffer.Dispose();
        }

        //ゲームオブジェクト情報を渡すための変換
        int objectLength = objectArray.Length;

        GameObjectData[] objectDataArray = new GameObjectData[objectLength];
        for (int i = 0; i < objectLength; i++)
        {
            objectDataArray[i] = new GameObjectData
            {
                position = objectArray[i].transform.position,
                size = 0.5f,
                type = 0, // タイプを 0, 1, 2 のどれかに設定
                padding = Vector3.zero //16バイト境界の調整
            };
        }

        //オブジェクトの情報を受け渡すためのBufferをつくる
        _objectBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, objectLength, _objectDataSize);
        //データをBufferに入れる
        _objectBuffer.SetData(objectDataArray);
        //Bufferをセット
        _rayMarchingComputeShader.SetBuffer(_kernelIndex, _objectBufferID, _objectBuffer);
        _rayMarchingComputeShader.SetInt(_objectCountID, objectLength);
        
        //可変データをセット
        _rayMarchingComputeShader.SetVector(_originPosID, this.transform.position);
        _rayMarchingComputeShader.SetVector(_forwardVecID, this.transform.forward);
        _rayMarchingComputeShader.SetInt(_objectCountID, objectLength);
        //実行
        _rayMarchingComputeShader.Dispatch(_kernelIndex, 1, 1, 1);
        
        int[] resultArray = new int[_rayCount];
        _outputBuffer.GetData((resultArray));
        //被りなしを作る
        HashSet<int> uniqueHashSet = new HashSet<int>(resultArray);
        // 結果を配列に変換
        int[] uniqueArray = new int[uniqueHashSet.Count];
        uniqueHashSet.CopyTo(uniqueArray);

        for (int i = 0; i < resultArray.Length; i++)
        {
            Debug.Log(resultArray[i]);
        }
        // for (int i = 0; i < uniqueArray.Length; i++)
        // {
        //     Debug.Log(objectArray[uniqueArray[i]].gameObject.name);
        // }
        
        _debugBuffer.GetData(_debugResultArray);
        _debugBuffer2.GetData(_debugResultArray2);
        _debugBuffer3.GetData(_debugResultArray3);
        _debugBuffer4.GetData(_debugResultArray4);
        _debugBuffer5.GetData(_debugResultArray5);
        _debugBuffer6.GetData(_debugResultArray6);
        for (int i = 0; i < _rayCount; i++)
        {
            Debug.Log($"{i}番目のレイについて: 試行回数は {_debugResultArray2[i]}回");
            for (int j = 0; j < 20; j++)
            {
                Debug.Log($"{i}番目のレイについて: ステップ{j}は {_debugResultArray[10 * i + j]}にいる");
                Debug.Log($"{i}番目のレイについて: ステップ{j}では、最短距離が {_debugResultArray3[10 * i + j]}である");
                Debug.Log($"{i}番目のレイについて: ステップ{j}では、 {_debugResultArray4[10 * i + j]}まで進んでいる");

                for (int k = 0; k < 10; k++)
                {
                    //Debug.Log($"{i}番目のレイについて: ステップ{j}で {_debugResultArray6[i * (10 + 20) + 10 * j + k]}と比較");
                    Debug.Log($"{i}番目のレイについて: ステップ{j}でインデックス{_debugResultArray6[i * (10 + 20) + 10 * j + k]}のオブジェクトとは {_debugResultArray5[i * (10 + 20) + 10 * j + k]}だけ離れている");
                }
            }
        }
        
        //_debugBuffer3.GetData(_debugResultArray3);
        
        // for (int i = 0; i < _debugResultArray3.Length; i++)
        // {
        //     Debug.Log($"オブジェクトの位置情報 : {_debugResultArray3[i].position}");
        //     Debug.Log($"オブジェクトのサイズ : {_debugResultArray3[i].size}");
        //     Debug.Log($"オブジェクトのタイプ : {_debugResultArray3[i].type}");
        //     Debug.Log($"オブジェクトのパディング : {_debugResultArray3[i].padding}");
        // }
        //_outputBuffer.Release();
        Debug.Log("くぎり終わり");
    }

    void OnDrawGizmos()
    {
        if (!Application.isPlaying)
        {
            return;
        }
        
        // XZ平面
        // for (int i = 0; i < _rayCount; i++)
        // {
        //     Vector3 from = this.transform.position;
        //     // Debug.Log(_debugResultArray[i]);
        //     Vector3 to = _debugResultArray[i];
        //     Gizmos.DrawLine(from, to);
        // }
    }
    private void OnDestroy()
    {
        _outputBuffer.Dispose();
        _objectBuffer.Dispose();
        _debugBuffer.Dispose();
        _debugBuffer2.Dispose();
        _debugBuffer3.Dispose();
    }
}