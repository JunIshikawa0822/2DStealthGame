using System;
using UnityEngine;
//using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Linq;

public class PlayerRayMarching1 : MonoBehaviour
{
    [SerializeField] private ComputeShader _rayMarchingComputeShader;
    [SerializeField] private int _rayCount = 1;
    // [SerializeField, Range(0, 360)] private float _rayAngle = 90f;
    [SerializeField] private float _thresHoldValue = 0.01f;
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
    
    // private int _debugBufferID;
    // private GraphicsBuffer _debugBuffer;
    // private Vector3[] _debugResultArray;
    //
    // private int _debugBuffer2ID;
    // private GraphicsBuffer _debugBuffer2;
    // private int[] _debugResultArray2;
    //
    // private int _debugBuffer3ID;
    // private GraphicsBuffer _debugBuffer3;
    // private float[] _debugResultArray3;
    //
    // private int _debugBuffer4ID;
    // private GraphicsBuffer _debugBuffer4;
    // private float[] _debugResultArray4;
    //
    // private int _debugBuffer5ID;
    // private GraphicsBuffer _debugBuffer5;
    // private float[] _debugResultArray5;
    //
    // private int _debugBuffer6ID;
    // private GraphicsBuffer _debugBuffer6;
    // private int[] _debugResultArray6;

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
        
        _objectDataSize = 32;

        int objectCountID = Shader.PropertyToID("_objectCount");
        int maxDistanceID = Shader.PropertyToID("_maxDistance");
        int thresHoldID = Shader.PropertyToID("_thresHold");
        int rayCountID = Shader.PropertyToID("_rayCount");

        _rayMarchingComputeShader.SetBuffer(0, _outputBufferID, _outputBuffer);
        
        _rayMarchingComputeShader.SetFloat(maxDistanceID, _maxDistance);
        _rayMarchingComputeShader.SetFloat(thresHoldID, _thresHoldValue);
        _rayMarchingComputeShader.SetInt(rayCountID, _rayCount);
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
    }
}