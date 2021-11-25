using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    [SerializeField]
    private GameObject shadowObject;
    private GameObject shadow;

    void Start()
    {
        shadow = Instantiate(shadowObject, new Vector3(this.gameObject.transform.position.x, 0.1f, this.gameObject.transform.position.z), Quaternion.Euler(90f, 0f, 0f));    
    }

    void Update()
    {
        shadowMove();
    }

    private void shadowMove()
    {
        Vector3 pos = shadow.transform.position;
        pos.x = this.gameObject.transform.position.x;
        pos.y = 0.1f;
        pos.z = this.gameObject.transform.position.z;
        shadow.transform.position = pos;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Court")
        {
            //Destroy(this.gameObject,3.0f);
            //Destroy(shadow,3.0f);
        }
    }
}
