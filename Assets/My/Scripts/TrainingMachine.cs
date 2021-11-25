using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainingMachine : MonoBehaviour
{

    public GameObject ThrowingObject;
    public GameObject TargetObject;
    [SerializeField, Range(0F, 90F), Tooltip("éÀèoÇ∑ÇÈäpìx")]
    private float ThrowingAngle;

    private void Start()
    {
        Collider collider = GetComponent<Collider>();
        if (collider != null)
        {
            collider.isTrigger = true;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown("f"))
        {
            ThrowingBall();
        }
    }

    private void ThrowingBall()
    {
        if (ThrowingObject != null && TargetObject != null)
        {
            GameObject ball = Instantiate(ThrowingObject, this.transform.position, Quaternion.identity);
            Vector3 targetPosition = TargetObject.transform.position;
            float angle = ThrowingAngle;
            Vector3 velocity = CalculateVelocity(this.transform.position, targetPosition, angle);
            Rigidbody rid = ball.GetComponent<Rigidbody>();
            rid.AddForce(velocity * rid.mass, ForceMode.Impulse);
        }
    }

    private Vector3 CalculateVelocity(Vector3 pointA, Vector3 pointB, float angle)
    {
        float rad = angle * Mathf.PI / 180;
        float x = Vector2.Distance(new Vector2(pointA.x, pointA.z), new Vector2(pointB.x, pointB.z));
        float y = pointA.y - pointB.y;
        float speed = Mathf.Sqrt(-Physics.gravity.y * Mathf.Pow(x, 2) / (2 * Mathf.Pow(Mathf.Cos(rad), 2) * (x * Mathf.Tan(rad) + y)));

        if (float.IsNaN(speed))
        {
            return Vector3.zero;
        }
        else
        {
            return (new Vector3(pointB.x - pointA.x, x * Mathf.Tan(rad), pointB.z - pointA.z).normalized * speed);
        }
    }

    IEnumerator Fade()
    {
        for (int i = 0;i < 5;i++)
        {
            ThrowingBall();
            yield return new WaitForSeconds(3f);
        }
    }
}
