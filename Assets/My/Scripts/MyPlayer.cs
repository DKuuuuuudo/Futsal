using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MyPlayer : MonoBehaviourPunCallbacks, IPunObservable
{
    [SerializeField]
    private GameObject playerUiPrefab;
    private GameObject cursor;
    private Rigidbody rigidBody;
    private Rigidbody cursorRb;
    private CapsuleCollider myCollider;
    private Animator animator;
    private Vector3 targetPos;
    private Vector3 input;
    private Vector3 velocity;
    private LineRenderer lineRenderer;
    private bool isGrounded;
    private float moveSpeed;
    private float jumpPower;
    private float power;
    private float maxPower;
    private int vectorNum;

    public static GameObject LocalPlayerInstance;

    void Awake()
    {
        if (photonView.IsMine)
        {
            MyPlayer.LocalPlayerInstance = this.gameObject;
            targetPos = this.gameObject.transform.position;
            cursor = PhotonNetwork.Instantiate("Cursor", new Vector3(0f, 0.1f, 0f), Quaternion.Euler(90f, 0f, 0f));
            cursorRb = cursor.GetComponent<Rigidbody>();
        }

        DontDestroyOnLoad(this.gameObject);
    }

    void Start()
    {
#if UNITY_5_4_OR_NEWER
        // Unity 5.4 has a new scene management. register a method to call CalledOnLevelWasLoaded.
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += (scene, loadingMode) =>
        {
            this.CalledOnLevelWasLoaded(scene.buildIndex);
        };
#endif

        rigidBody = GetComponent<Rigidbody>();
        myCollider = GetComponent<CapsuleCollider>();
        animator = GetComponent<Animator>();
        moveSpeed = 6f;
        jumpPower = 7.5f;
        power = 5.0f;
        maxPower = 15.0f;
        vectorNum = 12;
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.2f;
        lineRenderer.positionCount = vectorNum;

        if (playerUiPrefab != null)
        {
            GameObject _uiGo = Instantiate(playerUiPrefab);
            _uiGo.SendMessage("SetTarget", this, SendMessageOptions.RequireReceiver);
        }
        else
        {
            Debug.LogWarning("<Color=Red><a>Missing</a></Color> PlayerUiPrefab reference on player Prefab.", this);
        }
    }


    void Update()
    {
        if (photonView.IsMine == false && PhotonNetwork.IsConnected == true)
        {
            return;
        }
        CheckGround();
        PlayerAction();
        CursorMove();
        PowerControll();
        DrowLine();
        followCamera();
    }

    private void FixedUpdate()
    {
        if (!Mathf.Approximately(input.x, 0f) || !Mathf.Approximately(input.z, 0f))
        {
            rigidBody.MoveRotation(Quaternion.LookRotation(input.normalized));
        }
    }

    private void PlayerAction()
    {
        //Over();
        Move();
        Under();
        Jump();
        Spike();
        //Block();
    }

    private void Move()
    {
        if (isGrounded)
        {
            if (velocity.magnitude > 0f)
            {
                animator.SetFloat("isRun", velocity.magnitude);
            }
            else
            {
                animator.SetFloat("isRun", 0f);
            }
            velocity = Vector3.zero;
        }

        var inputHorizontal = Input.GetAxis("Horizontal");
        var inputVertical = Input.GetAxis("Vertical");

        input = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
        velocity = new Vector3(input.normalized.x * moveSpeed, 0f, input.normalized.z * moveSpeed);


        Vector3 cameraForward = Vector3.Scale(Camera.main.transform.forward, new Vector3(1, 0, 1)).normalized;
        Vector3 moveForward = cameraForward * inputVertical + Camera.main.transform.right * inputHorizontal;
        rigidBody.velocity = moveForward * moveSpeed + new Vector3(0, rigidBody.velocity.y, 0);
        cursorRb.velocity = moveForward * moveSpeed + new Vector3(0, rigidBody.velocity.y, 0);

        if (moveForward != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(moveForward);
        }
    }

    private void Spike()
    {
        if (Input.GetMouseButton(0) && !isGrounded)
        {
            animator.SetTrigger("isSpike");
        }
    }

    private void Under()
    {
        if (Input.GetMouseButton(0) && isGrounded)
        {
            animator.SetTrigger("isUnder");
        }
    }

    private void Over()
    {
        if (Input.GetMouseButton(0) && isGrounded)
        {
            animator.SetTrigger("isOver");
        }
    }

    private void Jump()
    {
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            isGrounded = false;
            animator.SetTrigger("isJump");
            rigidBody.velocity = new Vector3(rigidBody.velocity.x, jumpPower, rigidBody.velocity.z);
        }
    }

    private void Block()
    {
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            isGrounded = false;
            animator.SetTrigger("isBlock");
            rigidBody.velocity = new Vector3(rigidBody.velocity.x, jumpPower, rigidBody.velocity.z);
        }
    }

    private void CheckGround()
    {
        if (isGrounded)
        {
            return;
        }

        if (Physics.CheckSphere(rigidBody.position, myCollider.radius - 0.1f, LayerMask.GetMask("Ground")))
        {
            isGrounded = true;
            velocity.y = 0f;
        }
        else
        {
            isGrounded = false;
        }
    }

    private void OnTriggerStay(Collider coll)
    {
        if (coll.CompareTag("Ball") && Input.GetMouseButton(0))
        {
            HittingBall(coll.gameObject);
        }
    }

    private void HittingBall(GameObject ball)
    {
        if (ball != null)
        {

            Vector3 targetPosition = cursor.transform.position;

            float angle = power * 6;

            Vector3 velocity = CalculateVelocity(this.transform.position, targetPosition, angle);

            Rigidbody rid = ball.GetComponent<Rigidbody>();
            rid.isKinematic = true;
            rid.isKinematic = false;
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

    void CursorMove()
    {
        if (!Input.GetMouseButton(1))
        {
            var h = Input.GetAxis("Mouse X");
            var v = Input.GetAxis("Mouse Y");

            var range = 10;
            var left = this.gameObject.transform.position.x + range;
            var right = this.gameObject.transform.position.x - range;
            var forward = this.gameObject.transform.position.z + range;
            var back = this.gameObject.transform.position.z - range;
            var newPos = cursor.transform.position + new Vector3(h, 0f, v);

            if (newPos.x < left && newPos.x > right && newPos.z < forward && newPos.z > back)
            {
                cursor.transform.position = newPos;
            }
        }
    }

    void PowerControll()
    {
        var scroll = Input.mouseScrollDelta.y;
        power += scroll;
        if (power < 1.0f) power = 1.0f;
        if (power > maxPower) power = maxPower;
    }

    private void DrowLine()
    {
        var playerTrans = this.gameObject.transform.position;
        var cursorTrans = cursor.transform.position;
        var halfTrans = (playerTrans + cursorTrans) / 2;
        var positions = new Vector3[vectorNum];

        var playerVector = new Vector3(playerTrans.x, playerTrans.y, playerTrans.z);
        var cursorVector = new Vector3(cursorTrans.x, cursorTrans.y, cursorTrans.z);
        var halfVector = new Vector3(halfTrans.x, power, halfTrans.z);

        positions[0] = playerVector;
        for (int i = 1; i < vectorNum - 1; i++)
        {
            var t = (float)i / (float)(vectorNum - 1);
            positions[i] = CurvePoint(playerVector, cursorVector, halfVector, t);
        }

        positions[11] = cursorVector;
        lineRenderer.SetPositions(positions);
    }

    private Vector3 CurvePoint(Vector3 start, Vector3 end, Vector3 control, float t)
    {

        Vector3 q0 = Vector3.Lerp(start, control, t);
        Vector3 q1 = Vector3.Lerp(control, end, t);
        Vector3 q2 = Vector3.Lerp(q0, q1, t);

        return q2;
    }

    private void followCamera()
    {
        Camera.main.transform.position += this.transform.position - targetPos;
        targetPos = this.transform.position;

        if (Input.GetMouseButton(1))
        {
            float mouseInputX = Input.GetAxis("Mouse X");
            Camera.main.transform.RotateAround(targetPos, Vector3.up, mouseInputX * Time.deltaTime * 400f);
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {

        }
        else
        {

        }
    }

#if !UNITY_5_4_OR_NEWER
/// <summary>See CalledOnLevelWasLoaded. Outdated in Unity 5.4.</summary>
void OnLevelWasLoaded(int level)
{
    this.CalledOnLevelWasLoaded(level);
}
#endif


    void CalledOnLevelWasLoaded(int level)
    {
        GameObject _uiGo = Instantiate(this.playerUiPrefab);
        _uiGo.SendMessage("SetTarget", this, SendMessageOptions.RequireReceiver);
    }
}
