using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    static readonly Color DEFAULT_COLOR = new Color(1.0f, 1.0f, 1.0f);
    static readonly Color FRAME_COLOR = new Color(0.1f, 0.1f, 0.1f);
    static readonly Color SKY_BLUE_COLOR = new Color(0.5f, 0.5f, 1.0f);
    static readonly Color YELLOW_COLOR = new Color(1.0f, 1.0f, 0.0f);
    static readonly Color PURPLE_COLOR = new Color(1.0f, 0.0f, 1.0f);
    static readonly Color ORANGE_COLOR = new Color(1.0f, 0.5f, 0.0f);

    [SerializeField] Material _material = null;

    Material _myMaterial = null;

    [SerializeField] MeshRenderer _cubeA = null;
    [SerializeField] MeshRenderer _cubeB = null;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetColor(Color color)
    {
        if (_myMaterial == null)
        {
            _myMaterial = GameObject.Instantiate<Material>(_material);
            _cubeA.material = _myMaterial;
            _cubeB.material = _myMaterial;
        }
        _myMaterial.color = color;
    }

    public void SetState(TetrisSystem.eBlockState state)
    {
        bool isActive = (state != TetrisSystem.eBlockState.eNone);
        {
            _cubeA.gameObject.SetActive(isActive);
            _cubeB.gameObject.SetActive(isActive);
        }
        switch (state)
        {
            case TetrisSystem.eBlockState.eFrame:
                SetColor(Color.gray);
                break;
            case TetrisSystem.eBlockState.eSkyBlue:
                SetColor(SKY_BLUE_COLOR);
                break;
            case TetrisSystem.eBlockState.eYellow:
                SetColor(Color.yellow);
                break;
            case TetrisSystem.eBlockState.ePurple:
                SetColor(PURPLE_COLOR);
                break;
            case TetrisSystem.eBlockState.eBlue:
                SetColor(Color.blue);
                break;
            case TetrisSystem.eBlockState.eOrange:
                SetColor(ORANGE_COLOR);
                break;
            case TetrisSystem.eBlockState.eGreen:
                SetColor(Color.green);
                break;
            case TetrisSystem.eBlockState.eRed:
                SetColor(Color.red);
                break;
        }
    }
}
