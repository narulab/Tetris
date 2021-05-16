using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSystemBase : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // キー入力
    protected Dictionary<KeyCode, int> _keyImputTimer = new Dictionary<KeyCode, int>();
    protected bool GetKeyEx(KeyCode keyCode)
    {
        if (!_keyImputTimer.ContainsKey(keyCode))
        {
            _keyImputTimer.Add(keyCode, -1);
        }

        if (Input.GetKey(keyCode))
        {
            _keyImputTimer[keyCode]++;
        }
        else
        {
            _keyImputTimer[keyCode] = -1;
        }

        return (_keyImputTimer[keyCode] == 0 || _keyImputTimer[keyCode] >= 10);
    }
}
