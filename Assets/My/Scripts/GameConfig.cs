using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameConfig : MonoBehaviour
{
    private bool flg = false;

    void Start()
    {
        Cursor.visible = flg;
        Cursor.lockState = CursorLockMode.Confined;
    }

    void Update()
    {
        ChangeCursorVisible();
    }

    void ChangeCursorVisible()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            flg = !flg;
            Cursor.visible = flg;
        }
    }
}
