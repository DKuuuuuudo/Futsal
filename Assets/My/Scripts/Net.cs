using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Net : MonoBehaviour
{
    
    [SerializeField]
    private Text scoretxt;
    [SerializeField]
    private Text goaltxt;
    private int score;

    public int getScore
    {
        get { return score; }
    }

    private void Start()
    {
        score = 0;
        scoretxt.text = score.ToString();
        goaltxt.text = "";
    }

    private void OnCollisionEnter(Collision coll)
    {
        if (coll.gameObject.tag == "Ball")
        {
            score++;
            scoretxt.text = score.ToString();
            goaltxt.text = "Goal!!";
            Instantiate(coll.gameObject, new Vector3(0f, 10f, 0f), Quaternion.Euler(0f, 0f, 0f));
            Destroy(coll.gameObject);
            StartCoroutine(DellGoalTxt());
        }
    }

    IEnumerator DellGoalTxt()
    {
        yield return new WaitForSeconds(1.5f);
        goaltxt.text = "";
    }
}
