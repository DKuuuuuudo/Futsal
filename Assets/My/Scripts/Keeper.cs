using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Keeper : MonoBehaviour
{
    [SerializeField]
    private GameObject keeper;
    private bool flg;

    void Start()
    {
        flg = true;
    }

    void Update()
    {
        Move();
    }

    private void Move()
    {
        var pos = keeper.transform.position;
        if (pos.x > 4.0) flg = false;
        if (pos.x < -4.0) flg = true;

        if (flg)
        {
            pos.x += 0.05f;
        }
        else
        {
            pos.x -= 0.05f;
        }

        keeper.transform.position = pos;
    }
}
