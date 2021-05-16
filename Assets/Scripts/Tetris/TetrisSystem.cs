using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TetrisSystem : GameSystemBase
{
    const int FIELD_SIZE_X = 12;
    const int FIELD_SIZE_Y = 22;

    public const int MOVE_SIZE_X = 5;
    public const int MOVE_SIZE_Y = 5;

    const int DEFAULT_MOVE_X = 3;
    const int DEFAULT_MOVE_Y = 15;

    // ブロックの状態
    public enum eBlockState
    {
        eNone,
        eFrame,
        eSkyBlue,
        eYellow,
        ePurple,
        eBlue,
        eOrange,
        eGreen,
        eRed,

        eMax
    }

    // 水色
    static readonly int[,] BLOCKS_SKYBLUE = new int[MOVE_SIZE_X, MOVE_SIZE_Y]
    {
        { 0, 0, 0, 0, 0, },
        { 2, 2, 2, 2, 0, },
        { 0, 0, 0, 0, 0, },
        { 0, 0, 0, 0, 0, },
        { 0, 0, 0, 0, 0, },
    };

    // 黄色
    static readonly int[,] BLOCKS_YELLOW = new int[MOVE_SIZE_X, MOVE_SIZE_Y]
    {
        { 0, 0, 0, 0, 0, },
        { 0, 3, 3, 0, 0, },
        { 0, 3, 3, 0, 0, },
        { 0, 0, 0, 0, 0, },
        { 0, 0, 0, 0, 0, },
    };

    // 紫色
    static readonly int[,] BLOCKS_PURPLE = new int[MOVE_SIZE_X, MOVE_SIZE_Y]
    {
        { 0, 0, 0, 0, 0, },
        { 0, 0, 4, 0, 0, },
        { 0, 4, 4, 4, 0, },
        { 0, 0, 0, 0, 0, },
        { 0, 0, 0, 0, 0, },
    };

    // 青色
    static readonly int[,] BLOCKS_BLUE = new int[MOVE_SIZE_X, MOVE_SIZE_Y]
    {
        { 0, 0, 0, 0, 0, },
        { 0, 5, 0, 0, 0, },
        { 0, 5, 5, 5, 0, },
        { 0, 0, 0, 0, 0, },
        { 0, 0, 0, 0, 0, },
    };

    // オレンジ色
    static readonly int[,] BLOCKS_ORANGE = new int[MOVE_SIZE_X, MOVE_SIZE_Y]
    {
        { 0, 0, 0, 0, 0, },
        { 0, 0, 0, 6, 0, },
        { 0, 6, 6, 6, 0, },
        { 0, 0, 0, 0, 0, },
        { 0, 0, 0, 0, 0, },
    };

    // 緑色
    static readonly int[,] BLOCKS_GREEN = new int[MOVE_SIZE_X, MOVE_SIZE_Y]
    {
        { 0, 0, 0, 0, 0, },
        { 0, 0, 7, 7, 0, },
        { 0, 7, 7, 0, 0, },
        { 0, 0, 0, 0, 0, },
        { 0, 0, 0, 0, 0, },
    };

    // 赤色
    static readonly int[,] BLOCKS_RED = new int[MOVE_SIZE_X, MOVE_SIZE_Y]
    {
        { 0, 0, 0, 0, 0, },
        { 0, 8, 8, 0, 0, },
        { 0, 0, 8, 8, 0, },
        { 0, 0, 0, 0, 0, },
        { 0, 0, 0, 0, 0, },
    };

    static readonly int[][,] BLCOKS_LIST = new int[(int)eBlockState.eMax - (int)eBlockState.eSkyBlue][,]
    {
         BLOCKS_SKYBLUE,
         BLOCKS_YELLOW,
         BLOCKS_PURPLE,
         BLOCKS_BLUE,
         BLOCKS_ORANGE,
         BLOCKS_GREEN,
         BLOCKS_RED,
    };


    [SerializeField] GameObject _blockPrefab = null;

    // ブロックの実体
    private GameObject[,] _fieldBlocksObject = new GameObject[FIELD_SIZE_Y, FIELD_SIZE_X];
    private Block[,] _fieldBlocks = new Block[FIELD_SIZE_Y, FIELD_SIZE_X];

    // フィールド上にあるブロックの状態
    private eBlockState[,] _fieldBlocksState = new eBlockState[FIELD_SIZE_Y, FIELD_SIZE_X];

    // 最終的なブロックの状態
    private eBlockState[,] _fieldBlocksStateFinal = new eBlockState[FIELD_SIZE_Y, FIELD_SIZE_X];

    // 動作中ブロック
    private eBlockState[,] _moveBlocksState = new eBlockState[MOVE_SIZE_Y, MOVE_SIZE_X];
    private int _moveBlockX = DEFAULT_MOVE_X;
    private int _moveBlockY = DEFAULT_MOVE_Y;

    private eBlockState[,] _tempBlocksState = new eBlockState[MOVE_SIZE_Y, MOVE_SIZE_X];

    // 落下時間間隔
    private float _fallTime = 1.0f;
    private float _fallTimer = 1.0f;


    // ホールド用
    [SerializeField] private BlockUnit _blockUnitHold = null;   // ホールド用のBlockUnit
    bool _canHold = true;
    bool _isHold = false;

    // Nextブロック表示用
    [SerializeField] private BlockUnit[] _nextUnitHold = null;    // ホールド用のBlockUnit
    private int _nextUnitCount = 0;            // いくつ次のブロックまで表示させるか

    // 状態遷移
    enum GameState
    {
        Active,
        GameOver,

        Max
    }
    GameState _gameState = GameState.Active;


    // Start is called before the first frame update
    void Start()
    {
        // Test
#if UNITY_EDITOR
        Application.targetFrameRate = 60;
#endif

        // 初期状態の設定
        for (int i = 0; i < FIELD_SIZE_Y; i++)
        {
            for (int j = 0; j < FIELD_SIZE_X; j++)
            {
                // ブロックの実体
                GameObject newObject = GameObject.Instantiate<GameObject>(_blockPrefab);
                Block newBlock = newObject.GetComponent<Block>();
                newObject.transform.localPosition = new Vector3(j, i, 0.0f);
                _fieldBlocksObject[i, j] = newObject;
                _fieldBlocks[i, j] = newBlock;
                // ブロックの状態
                _fieldBlocksState[i, j] = (0 < i && i < FIELD_SIZE_Y - 1 && 0 < j && j < FIELD_SIZE_X - 1) ? eBlockState.eNone : eBlockState.eFrame;
                _fieldBlocksStateFinal[i, j] = _fieldBlocksState[i, j];
            }
        }

        // 最初の抽選
        _nextUnitCount = _nextUnitHold.Length;
        for (int i = 0; i < _nextUnitCount; i++)
        {
            LotNextBlock();
        }

        // ブロックを開始
        StartMove();
    }

    // Update is called once per frame
    void Update()
    {
        // アクティブでない場合は行わない
        if (_gameState != GameState.Active)
        {
            return;
        }

        // ブロックをホールド
        if (GetKeyEx(KeyCode.I))
        {
            HoldMoveBlocks();
        }
        // ブロックを左右移動させる
        if (GetKeyEx(KeyCode.S))
        {
            // ブロックの当たり判定（左）
            bool isCollision = CheckCollision(-1, 0);
            if (!isCollision)
            {
                // ブロックを移動
                _moveBlockX--;
            }
        }
        if (GetKeyEx(KeyCode.F))
        {
            // ブロックの当たり判定（右）
            bool isCollision = CheckCollision(1, 0);
            if (!isCollision)
            {
                // ブロックを移動
                _moveBlockX++;
            }
        }
        // ブロックの右回転
        if (GetKeyEx(KeyCode.L))
        {
            // ブロックの当たり判定（右回転）
            bool isCollision = CheckCollisionRotateRight();
            if (!isCollision)
            {
                // ブロックを回転
                RotateBlockRight();
            }
        }
        // ブロックの左回転
        if (GetKeyEx(KeyCode.J))
        {
            // ブロックの当たり判定（左回転）
            bool isCollision = CheckCollisionRotateLeft();
            if (!isCollision)
            {
                // ブロックを回転
                RotateBlockLeft();
            }
        }
        // ブロックを落下させる
        _fallTimer -= Time.deltaTime;
        if (_fallTimer <= 0.0f || GetKeyEx(KeyCode.D))
        {
            // ブロックの当たり判定（下）
            bool isCollision = CheckCollision(0, -1);
            if (isCollision)
            {
                // ブロックの落下を反映
                MergeBlock();
                // ブロック揃いチェック
                CheckLine();
                CheckLine();
                CheckLine();
                CheckLine();
                // タイマーをリセット
                _fallTimer = _fallTime;
                // ブロックを開始
                StartMove();
            }
            else
            {
                // ブロックを落下
                _moveBlockY--;
                // タイマーをリセット
                _fallTimer = _fallTime;
            }
        }

        // ブロックの状態を更新
        UpdateBlockState();
    }

    // 退避
    void CacheTempState()
    {
        for (int i = 0; i < MOVE_SIZE_Y; i++)
        {
            for (int j = 0; j < MOVE_SIZE_X; j++)
            {
                _tempBlocksState[i, j] = _moveBlocksState[i, j];
            }
        }
    }

    // 退避から戻す
    void RestoreTempState()
    {
        for (int i = 0; i < MOVE_SIZE_Y; i++)
        {
            for (int j = 0; j < MOVE_SIZE_X; j++)
            {
                _moveBlocksState[i, j] = _tempBlocksState[i, j];
            }
        }
    }

    // ブロックの回転処理
    void RotateBlockLeft()
    {
        // 退避
        CacheTempState();

        // 回転
        for (int i = 0; i < MOVE_SIZE_Y; i++)
        {
            for (int j = 0; j < MOVE_SIZE_X; j++)
            {
                _moveBlocksState[i, j] = _tempBlocksState[MOVE_SIZE_X - 1 - j, i];
            }
        }
    }

    // ブロックの回転処理
    void RotateBlockRight()
    {
        // 退避
        CacheTempState();

        // 回転
        for (int i = 0; i < MOVE_SIZE_Y; i++)
        {
            for (int j = 0; j < MOVE_SIZE_X; j++)
            {
                _moveBlocksState[i, j] = _tempBlocksState[j, MOVE_SIZE_Y - 1 - i];
            }
        }
    }

    // ブロックの当たり判定
    bool CheckCollision(int offsetX, int offsetY)
    {
        // ブロックの状態反映（動作中）
        for (int i = 0; i < MOVE_SIZE_Y; i++)
        {
            for (int j = 0; j < MOVE_SIZE_X; j++)
            {
                // ブロックの状態
                if (0 <= _moveBlockY + i + offsetY && _moveBlockY + i + offsetY < FIELD_SIZE_Y && 0 <= _moveBlockX + j + offsetX && _moveBlockX + j + offsetX < FIELD_SIZE_X)
                {
                    if (_fieldBlocksState[_moveBlockY + i + offsetY, _moveBlockX + j + offsetX] != eBlockState.eNone && _moveBlocksState[i, j] != eBlockState.eNone)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    // ブロックの当たり判定（回転）
    bool CheckCollisionRotate()
    {
        // ブロックの状態反映（動作中）
        for (int i = 0; i < MOVE_SIZE_Y; i++)
        {
            for (int j = 0; j < MOVE_SIZE_X; j++)
            {
                // ブロックの状態
                if (0 <= _moveBlockY + i && _moveBlockY + i < FIELD_SIZE_Y && 0 <= _moveBlockX + j && _moveBlockX + j < FIELD_SIZE_X)
                {
                    if (_fieldBlocksState[_moveBlockY + i, _moveBlockX + j] != eBlockState.eNone && _tempBlocksState[i, j] != eBlockState.eNone)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    bool CheckCollisionRotateLeft()
    {
        // 回転
        for (int i = 0; i < MOVE_SIZE_Y; i++)
        {
            for (int j = 0; j < MOVE_SIZE_X; j++)
            {
                _tempBlocksState[i, j] = _moveBlocksState[MOVE_SIZE_X - 1 - j, i];
            }
        }

        return CheckCollisionRotate();
    }

    bool CheckCollisionRotateRight()
    {
        // 回転
        for (int i = 0; i < MOVE_SIZE_Y; i++)
        {
            for (int j = 0; j < MOVE_SIZE_X; j++)
            {
                _tempBlocksState[i, j] = _moveBlocksState[j, MOVE_SIZE_Y - 1 - i];
            }
        }

        return CheckCollisionRotate();
    }

    // ブロックの状態を更新
    void UpdateBlockState()
    {
        // ブロックの状態反映（フィールド上）
        for (int i = 0; i < FIELD_SIZE_Y; i++)
        {
            for (int j = 0; j < FIELD_SIZE_X; j++)
            {
                // ブロックの状態
                _fieldBlocksStateFinal[i, j] = _fieldBlocksState[i, j];
            }
        }

        // ブロックの状態反映（動作中）
        for (int i = 0; i < MOVE_SIZE_Y; i++)
        {
            for (int j = 0; j < MOVE_SIZE_X; j++)
            {
                // ブロックの状態
                if (0 <= _moveBlockY + i && _moveBlockY + i < FIELD_SIZE_Y && 0 <= _moveBlockX + j && _moveBlockX + j < FIELD_SIZE_X)
                {
                    if (_fieldBlocksStateFinal[_moveBlockY + i, _moveBlockX + j] == eBlockState.eNone)
                    {
                        _fieldBlocksStateFinal[_moveBlockY + i, _moveBlockX + j] = _moveBlocksState[i, j];
                    }
                }
            }
        }

        // ブロックの状態反映
        for (int i = 0; i < FIELD_SIZE_Y; i++)
        {
            for (int j = 0; j < FIELD_SIZE_X; j++)
            {
                // ブロックの状態
                _fieldBlocks[i, j].SetState(_fieldBlocksStateFinal[i, j]);
            }
        }
    }

    void MergeBlock()
    {
        // ブロックの状態反映（動作中）
        for (int i = 0; i < MOVE_SIZE_Y; i++)
        {
            for (int j = 0; j < MOVE_SIZE_X; j++)
            {
                if (0 <= _moveBlockY + i && _moveBlockY + i < FIELD_SIZE_Y && 0 <= _moveBlockX + j && _moveBlockX + j < FIELD_SIZE_X)
                {
                    // ブロックの状態
                    if (_fieldBlocksState[_moveBlockY + i, _moveBlockX + j] == eBlockState.eNone)
                    {
                        _fieldBlocksState[_moveBlockY + i, _moveBlockX + j] = _moveBlocksState[i, j];
                    }
                }
            }
        }
    }

    /// <summary>
    /// ゲームオーバーかどうかを判定
    /// </summary>
    /// <returns></returns>
    bool CheckGameOver()
    {
        // ブロックの状態反映（動作中）
        for (int i = 0; i < MOVE_SIZE_Y; i++)
        {
            for (int j = 0; j < MOVE_SIZE_X; j++)
            {
                if (0 <= _moveBlockY + i && _moveBlockY + i < FIELD_SIZE_Y && 0 <= _moveBlockX + j && _moveBlockX + j < FIELD_SIZE_X)
                {
                    // ブロックの状態
                    if (_fieldBlocksState[_moveBlockY + i, _moveBlockX + j] != eBlockState.eNone && _moveBlocksState[i, j] != eBlockState.eNone)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    void CheckLine()
    {
        for (int i = 1; i < FIELD_SIZE_Y - 1; i++)
        {
            bool isBlank = false;
            for (int j = 1; j < FIELD_SIZE_X - 1; j++)
            {
                // ブロックの状態
                if (_fieldBlocksState[i, j] == eBlockState.eNone)
                {
                    isBlank = true;
                }
            }
            if (!isBlank)
            {
                DeleteLine(i);
            }
        }
    }

    void DeleteLine(int y)
    {
        for (int i = y; i < FIELD_SIZE_Y - 1; i++)
        {
            for (int j = 1; j < FIELD_SIZE_X - 1; j++)
            {
                if (_fieldBlocksState[i, j] >= eBlockState.eSkyBlue)
                {
                    _fieldBlocksState[i, j] = _fieldBlocksState[i + 1, j];
                }
            }
        }
    }

    /// <summary>
    /// 動かしてるブロックをホールドする
    /// </summary>
    /// <returns>ホールドできたか</returns>
    bool HoldMoveBlocks()
    {
        // ホールドできないときは行わない
        if (!_canHold)
        {
            return false;
        }

        // ホールド処理
        _blockUnitHold.ReadBlock(ref _tempBlocksState);
        _blockUnitHold.WriteBlock(ref _moveBlocksState);
        RestoreTempState();

        // ホールドしてなければ次のブロックを落とす
        if (!_isHold)
        {
            StartMove();
        }

        // ホールドした
        _isHold = true;

        // ホールドできなくする
        _canHold = false;

        return true;
    }

    /// <summary>
    /// ブロックの抽選
    /// </summary>
    /// <returns>ゲームオーバーかどうか</returns>
    bool LotNextBlock()
    {
        // 次に出すブロックを入れる
        _nextUnitHold[0].ReadBlock(ref _moveBlocksState);

        // 順に上に移す
        for (int i = 0; i < _nextUnitCount - 1; i++)
        {
            var fieldBlockState = _nextUnitHold[i + 1].fieldBlocksState;
            _nextUnitHold[i].WriteBlock(ref fieldBlockState);
        }

        // ランダム
        int pattern = Random.Range(0, eBlockState.eMax - eBlockState.eSkyBlue);
        //pattern = (int)(eBlockState.eSkyBlue - eBlockState.eSkyBlue);
        int[,] blocks = BLCOKS_LIST[pattern];

        // 状態を設定
        for (int i = 0; i < MOVE_SIZE_Y; i++)
        {
            for (int j = 0; j < MOVE_SIZE_X; j++)
            {
                _tempBlocksState[i, j] = (eBlockState)blocks[i, j];
            }
        }

        // ユニットに書き込み
        int index = _nextUnitCount - 1;
        _nextUnitHold[index].WriteBlock(ref _tempBlocksState);

        return CheckGameOver();
    }

    // ブロックを開始
    void StartMove()
    {
        // 初期位置を設定
        _moveBlockX = DEFAULT_MOVE_X;
        _moveBlockY = DEFAULT_MOVE_Y;

        // 抽選
        bool isGameOver = LotNextBlock();

        // ホールドを可能に
        _canHold = true;

        // ゲームオーバーの場合はゲームオーバー処理
        if (isGameOver)
        {
            StartCoroutine(GameOverProc());
        }
    }

    /// <summary>
    /// ゲームオーバー処理
    /// </summary>
    /// <returns></returns>
    private IEnumerator GameOverProc()
    {
        _gameState = GameState.GameOver;

        yield return null;

        for (int i = 0; i < FIELD_SIZE_Y; i++)
        {
            for (int j = 0; j < FIELD_SIZE_X; j++)
            {
                if (_fieldBlocksStateFinal[i, j] != eBlockState.eNone)
                {
                    _fieldBlocks[i, j].SetState(eBlockState.eFrame);
                }
            }

            yield return new WaitForSeconds(0.1f);
        }

        yield return new WaitForSeconds(1.0f);

        yield break;
    }
}
