using System.Collections;
using System.Collections.Generic;
using JunUtilities;
using Unity.Mathematics;
using UnityEngine;

public class PlayerRayMarching : MonoBehaviour
{
    //外部から設定したい値たち
    [SerializeField] private ComputeShader _rayMarchingComputeShader;
    [SerializeField] public int _rayCount = 1;
    [SerializeField, Range(0, 360)] public float _viewAngle = 90f;
    [SerializeField] public float _threshold = 0.01f;
    [SerializeField] public float _maxDistance = 30;
    
    private int _kernelIndex;//computeShaderをDispatchするのに必要
    
    private int _outputBufferID;
    private GraphicsBuffer _outputBuffer;
    private int _objectBufferID;
    private GraphicsBuffer _objectBuffer;
    private int _positionBufferID;//デバッグ用
    private GraphicsBuffer _positionBuffer;//デバッグ用
    
    //可変の変数のID
    private int _originPosID;
    private int _forwardVecID;
    private int _objectCountID;
    
    //当たった場所を入れる
    private Vector3[] _positionResultArray;//デバッグ用
    
    private AABB3D _cameraAABB3D;
    private Bounds _cameraBounds;
    
    //デバッグ用
    private int _debugBuffer1ID;
    private GraphicsBuffer _debugBuffer1;
    
    private int _debugBuffer2ID;
    private GraphicsBuffer _debugBuffer2;
    
    struct Object
    {
        public int type; //オブジェクトのタイプ 4
        public Vector3 center;//オブジェクトの中心 12
        public Vector3 axisX;//オブジェクトのX方向の大きさと向き 12
        public Vector3 axisY;//オブジェクトのY方向の大きさと向き 12
        public Vector3 axisZ;//オブジェクトのZ方向の大きさと向き 12
        public Vector3 padding;//16バイト境界の調整 12
    };

    //Objectのデータサイズ
    private int _objectDataSize = 64;

    public void OnSetUp()
    {
        _kernelIndex = _rayMarchingComputeShader.FindKernel("CSMain");
        _outputBufferID = Shader.PropertyToID("_outputBuffer");
        _objectBufferID = Shader.PropertyToID("_objectsBuffer");
        _positionBufferID = Shader.PropertyToID("_positionBuffer");
        
        //可変の変数たち
        _forwardVecID = Shader.PropertyToID("_forwardVec");
        _originPosID = Shader.PropertyToID("_originPos");
        _objectCountID = Shader.PropertyToID("_objectCount");
        
        //固定の変数に関しては、ID取得して初期化したら終わり
        _rayMarchingComputeShader.SetFloat(Shader.PropertyToID("_maxDistance"), _maxDistance);
        _rayMarchingComputeShader.SetFloat(Shader.PropertyToID("_threshold"), _threshold);
        _rayMarchingComputeShader.SetInt(Shader.PropertyToID("_rayCount"), _rayCount);
        _rayMarchingComputeShader.SetFloat(Shader.PropertyToID("_viewAngle"), _viewAngle);
        
        //バッファ作ってセット
        _outputBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, _rayCount, sizeof(int));
        _rayMarchingComputeShader.SetBuffer(_kernelIndex, _outputBufferID, _outputBuffer);
        _positionBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, _rayCount, sizeof(float) * 3);
        _rayMarchingComputeShader.SetBuffer(_kernelIndex, _positionBufferID, _positionBuffer);
        
        //デバッグ用
        _debugBuffer1ID = Shader.PropertyToID("_debugBuffer1");
        _debugBuffer1 = new GraphicsBuffer(GraphicsBuffer.Target.Structured, 1, sizeof(float) * 3);
        _rayMarchingComputeShader.SetBuffer(_kernelIndex, _debugBuffer1ID, _debugBuffer1);
        
        _debugBuffer2ID = Shader.PropertyToID("_debugBuffer2");
        _debugBuffer2 = new GraphicsBuffer(GraphicsBuffer.Target.Structured, _rayCount, sizeof(float) * 3);
        _rayMarchingComputeShader.SetBuffer(_kernelIndex, _debugBuffer2ID, _debugBuffer2);
    }
    
    /// <param name="objectDataArray">衝突比較を行うオブジェクトのデータ</param>
    public int[] OnRayMarchingUpdate((Transform transform, int objType, OBB obb)[] objectDataArray)
    {
        // gameObjectBufferがすでに存在する場合は解放
        if (_objectBuffer != null)
        {
            _objectBuffer.Dispose();
        }
        
        //引き渡すオブジェクトのデータを渡す
        //まずは長さ
        int objectLength = objectDataArray.Length;
        Object[] objectArray = new Object[objectLength];
        
        //当たるものが何もない場合はobjectArrayに何も入れない
        if (objectDataArray.Length > 0)
        {
            for (int i = 0; i < objectLength; i++)
            {
                objectArray[i] = ConstructObjectData(objectDataArray[i]);
            }
        }
        
        //可変データを引き渡し祭り
        _rayMarchingComputeShader.SetVector(_originPosID, this.transform.position);
        _rayMarchingComputeShader.SetVector(_forwardVecID, this.transform.forward);
        _rayMarchingComputeShader.SetInt(_objectCountID, objectLength);
        
        //オブジェクトのバッファを作ってデータを入れて引き渡し
        //GraphicsBufferに長さが0のものを渡せないので便宜上1
        if (objectLength > 0)
        {
            _objectBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, objectLength, _objectDataSize);
        }
        else
        {
            _objectBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, 1, _objectDataSize);
        }
        
        _objectBuffer.SetData(objectArray);
        _rayMarchingComputeShader.SetBuffer(_kernelIndex, _objectBufferID, _objectBuffer);
        //実行
        _rayMarchingComputeShader.Dispatch(_kernelIndex, 16, 1, 1);
        
        //結果を受け取る
        int[] resultArray = new int[_rayCount];
        Debug.Log(_outputBuffer);
        _outputBuffer.GetData(resultArray);
        int[] uniqueResultArray = JunExpandUnityClass.ConvertToUniqueArray(resultArray);
        //Debug.Log($"このフレームで、({string.Join(", ", uniqueResultArray)})とぶつかっている");
        _positionResultArray = new Vector3[_rayCount];
        _positionBuffer.GetData(_positionResultArray);
        
        Vector3[] debugArray1 = new Vector3[1];
        _debugBuffer1.GetData(debugArray1);
        Debug.Log(debugArray1[0]);
        
        Vector3[] debugArray2 = new Vector3[_rayCount];
        _debugBuffer2.GetData(debugArray2);
        Debug.Log(string.Join(", ", debugArray2));
        
        return uniqueResultArray;
    }
    
    private Object ConstructObjectData((Transform transform, int objType, OBB obb) objectData)
    {
        int type = objectData.objType;
        Vector3 center = Vector3.zero;
        Vector3 axisX = Vector3.zero;
        Vector3 axisY = Vector3.zero;
        Vector3 axisZ = Vector3.zero;
        Vector3 padding = Vector3.zero;

        if (objectData.objType == 0)/*球*/
        {
            center = objectData.transform.position; 
            axisX = objectData.transform.lossyScale;
        }
        else if (objectData.objType == 1)/*OBB*/
        {
            center = objectData.obb.Center;
            axisX = objectData.obb.Axis[0];
            axisY = objectData.obb.Axis[1];
            axisZ = objectData.obb.Axis[2];
        }
            
        return new Object { type = type, center = center, axisX = axisX, axisY = axisY, axisZ = axisZ, padding = padding };
    }

    public void SetAABB3D( AABB3D bounds )
    {
        _cameraAABB3D = bounds;
    }
    
    public void SetBounds( Bounds bounds )
    {
        _cameraBounds = bounds;
    }
    
    void OnDrawGizmos()
    {
        
        if(Application.isPlaying == false) return;
        
        for (int i = 0; i < _positionResultArray.Length; i++)
        {
            Vector3 from = this.transform.position;
            Vector3 to = _positionResultArray[i];
            Gizmos.DrawLine(from, to);
        }
        
        Debug.Log("うごく");
        Gizmos.color = Color.green; // 緑色で描画
        Gizmos.DrawWireCube(_cameraBounds.center, _cameraBounds.size);
        //Gizmos.DrawWireCube(_cameraAABB3D.Center, _cameraAABB3D.Size);
    }
    
    private void OnDestroy()
    {
        if(_outputBuffer != null)_outputBuffer.Dispose();
        if(_objectBuffer != null)_objectBuffer.Dispose();
        if(_positionBuffer != null)_positionBuffer.Dispose();
    }
}
