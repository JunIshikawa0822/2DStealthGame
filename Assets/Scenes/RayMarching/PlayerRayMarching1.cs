using System;
using UnityEngine;
//using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;

public class PlayerRayMarching1 : MonoBehaviour
{
    [SerializeField] private ComputeShader _rayMarchingComputeShader;
    [SerializeField] private int _rayCount = 1;
    [SerializeField, Range(0, 360)] private float _rayAngle = 90f;
    [SerializeField] private float _thresholdValue = 0.01f;
    [SerializeField] private float _maxDistance = 30;
    
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
    
    private int _positionBufferID;
    private GraphicsBuffer _positionBuffer;
    private Vector3[] _positionResultArray;
    //
    // private int _debugBuffer2ID;
    // private GraphicsBuffer _debugBuffer2;
    // private int[] _debugResultArray2;
    
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
        
        _outputBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, _rayCount, sizeof(int));
        _outputBufferID = Shader.PropertyToID("_outputBuffer");
        _objectBufferID = Shader.PropertyToID("_objectsBuffer");
        
        _forwardVecID = Shader.PropertyToID("_forwardVec");
        _originPosID = Shader.PropertyToID("_originPos");
        _objectCountID = Shader.PropertyToID("_objectCount");
        
        _objectDataSize = sizeof(float) * 3 + sizeof(float) + sizeof(int) + sizeof(float) * 3;
        
        int maxDistanceID = Shader.PropertyToID("_maxDistance");
        int thresholdID = Shader.PropertyToID("_threshold");
        int rayCountID = Shader.PropertyToID("_rayCount");
        int viewAngleID = Shader.PropertyToID("_viewAngle");

        _rayMarchingComputeShader.SetBuffer(0, _outputBufferID, _outputBuffer);
        
        _rayMarchingComputeShader.SetFloat(maxDistanceID, _maxDistance);
        _rayMarchingComputeShader.SetFloat(thresholdID, _thresholdValue);
        _rayMarchingComputeShader.SetInt(rayCountID, _rayCount);
        _rayMarchingComputeShader.SetFloat(viewAngleID, _rayAngle);
        
        _positionBufferID = Shader.PropertyToID("_positionBuffer");
        _positionResultArray = new Vector3[_rayCount];
        _positionBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, _positionResultArray.Length, sizeof(float) * 3);
        _rayMarchingComputeShader.SetBuffer(0, _positionBufferID, _positionBuffer);
        
        // _debugBuffer2ID = Shader.PropertyToID("_debugBuffer2");
        // _debugResultArray2 = new int[_rayCount];
        // _debugBuffer2 = new GraphicsBuffer(GraphicsBuffer.Target.Structured, _debugResultArray2.Length, sizeof(int));
        // _rayMarchingComputeShader.SetBuffer(0, _debugBuffer2ID, _debugBuffer2);
    }

    void Update()
    {
        // Debug.Log("くぎり始め");
        // gameObjectBufferがすでに存在する場合は解放
        if (_objectBuffer != null)
        {
            _objectBuffer.Dispose();
        }

        //ゲームオブジェクト情報を渡すための変換
        int objectLength = objectArray.Length;
        // Debug.Log(objectLength);

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
        //可変データをセット
        _rayMarchingComputeShader.SetVector(_originPosID, this.transform.position);
        
        _rayMarchingComputeShader.SetVector(_forwardVecID, this.transform.forward);
        _rayMarchingComputeShader.SetInt(_objectCountID, objectLength);
        //実行
        _rayMarchingComputeShader.Dispatch(_kernelIndex, 16, 1, 1);
        
        int[] resultArray = new int[_rayCount];
        _outputBuffer.GetData((resultArray));
        //被りなしを作る
        HashSet<int> uniqueHashSet = new HashSet<int>(resultArray);
        // 結果を配列に変換
        int[] uniqueArray = new int[uniqueHashSet.Count];
        uniqueHashSet.CopyTo(uniqueArray);

        List<int> debugResult = new List<int>(uniqueArray);
        
        Debug.Log($"このフレームで、({string.Join(", ", debugResult)})とぶつかっている");
        
        _positionBuffer.GetData((_positionResultArray));
        // _debugBuffer2.GetData((_debugResultArray2));
        // _debugBuffer3.GetData((_debugResultArray3));
        // // // _debugBuffer4.GetData((_debugResultArray4));
        // // _debugBuffer5.GetData((_debugResultArray5));
        // _debugBuffer6.GetData((_debugResultArray6));
        // _debugBuffer7.GetData((_debugResultArray7));
        //_debugBuffer8.GetData((_debugResultArray8));
        // for (int i = 0; i < _rayCount; i++)
        // {
        //     // Debug.Log($"レイ {i}で angleは{_debugResultArray3[i]}");
        //     // Debug.Log($"レイ {i}で rayDirは{_debugResultArray6[i]}");
        //     Debug.Log($"レイ {i}で idは{_debugResultArray2[i]}");
        // }
        
        // for (int i = 0; i < objectLength; i++)
        // {
        //     GameObjectData obj = _debugResultArray4[i];
        //     Debug.Log($"データとして読まれているオブジェクトは{obj.position}, {obj.size}, {obj.type}, {obj.padding}");
        //     
        // }
        //
        // for (int i = 0; i < objectLength; i++)
        // {
        //     Debug.Log($"データとして読まれているオブジェクトの位置は{_debugResultArray3[i]}");
        // }
        //
        // Debug.Log(_debugResultArray7[0]);
        // //Debug.Log(_debugResultArray8[0]);
        //
        // Debug.Log($"originPos : {_debugResultArray[0]}, forwardVec : {_debugResultArray[1]}");
        // for (int i = 0; i < 20; i++)
        // {
        //     Debug.Log($"ステップ {i}で originPosは{_debugResultArray6[i]}");
        //     Debug.Log($"ステップ {i}で thresholdは{_debugResultArray2[i]}");
        //     Debug.Log($"ステップ {i}で、レイの位置は{_debugResultArray5[i]}");
        //     
        //     // for (int j = 0; j < 8; j++)
        //     // {
        //     //     Debug.Log($"ステップ {i}で、{_debugResultArray[20 * i + j]}にいるオブジェクトと比較");
        //     //     Debug.Log($"そのオブジェクトとの距離は{_debugResultArray2[20 * i + j]}");
        //     // }
        //     Debug.Log($"ステップ {i}で、もっとも短い距離は{_debugResultArray3[i]}");
        // }
        // Debug.Log("くぎり終わり");
    }

    // void OnDrawGizmos()
    // {
    //     //
    //     // // XZ平面
    //     for (int i = 0; i < _rayCount; i++)
    //     {
    //         Vector3 from = this.transform.position;
    //         //Debug.Log(_positionResultArray[i]);
    //         Vector3 to = _positionResultArray[i];
    //         Gizmos.DrawLine(from, to);
    //     }
    // }
    private void OnDestroy()
    {
        _outputBuffer.Dispose();
        _objectBuffer.Dispose();
        //_debugBuffer6.Dispose();
        _positionBuffer.Dispose();
    }
}