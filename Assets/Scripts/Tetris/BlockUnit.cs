using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockUnit : MonoBehaviour
{
    [SerializeField] GameObject _blockPrefab = null;

    // ブロックの実体
    private GameObject[,] _fieldBlocksObject = new GameObject[TetrisSystem.MOVE_SIZE_Y, TetrisSystem.MOVE_SIZE_X];
    private Block[,] _fieldBlocks = new Block[TetrisSystem.MOVE_SIZE_Y, TetrisSystem.MOVE_SIZE_X];

    // フィールド上にあるブロックの状態
    private TetrisSystem.eBlockState[,] _fieldBlocksState = new TetrisSystem.eBlockState[TetrisSystem.MOVE_SIZE_Y, TetrisSystem.MOVE_SIZE_X];
    public TetrisSystem.eBlockState[,] fieldBlocksState { get { return _fieldBlocksState; } }

    /// <summary>
    /// ブロックの内容を書き込み
    /// </summary>
    /// <param name="srcBlocksState"></param>
    public void WriteBlock(ref TetrisSystem.eBlockState[,] srcBlocksState)
    {
        int nx = TetrisSystem.MOVE_SIZE_X;
        int ny = TetrisSystem.MOVE_SIZE_Y;

        // 初期状態の設定
        for (int i = 0; i < ny; i++)
        {
            for (int j = 0; j < nx; j++)
            {
                _fieldBlocksState[i, j] = srcBlocksState[i, j];
                _fieldBlocks[i, j].SetState(_fieldBlocksState[i, j]);
            }
        }
    }

    /// <summary>
    /// ブロックの内容を読み込み
    /// </summary>
    /// <param name="dstBlocksState"></param>
    public void ReadBlock(ref TetrisSystem.eBlockState[,] dstBlocksState)
    {
        int nx = TetrisSystem.MOVE_SIZE_X;
        int ny = TetrisSystem.MOVE_SIZE_Y;

        // 初期状態の設定
        for (int i = 0; i < ny; i++)
        {
            for (int j = 0; j < nx; j++)
            {
                dstBlocksState[i, j] = _fieldBlocksState[i, j];
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        int nx = TetrisSystem.MOVE_SIZE_X;
        int ny = TetrisSystem.MOVE_SIZE_Y;

        // 初期状態の設定
        for (int i = 0; i < ny; i++)
        {
            for (int j = 0; j < nx; j++)
            {
                // ブロックの実体
                GameObject newObject = GameObject.Instantiate<GameObject>(_blockPrefab);
                newObject.transform.SetParent(transform);
                Block newBlock = newObject.GetComponent<Block>();
                newObject.transform.localPosition = new Vector3(j - (nx - 1) * 0.5f, i - (ny - 1) * 0.5f, 0.0f);
                newObject.transform.localScale = Vector3.one;
                _fieldBlocksObject[i, j] = newObject;
                _fieldBlocks[i, j] = newBlock;
                // ブロックの状態
                _fieldBlocksState[i, j] = TetrisSystem.eBlockState.eNone;
                // 反映
                _fieldBlocks[i, j].SetState(_fieldBlocksState[i, j]);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
