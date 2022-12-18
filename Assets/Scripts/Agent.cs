using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent : MonoBehaviour
{
    // Start is called before the first frame update
    public float speed = 1f;
    public float health = 1f;
    public float defense = 1f;

    public bool goalReached = false;

    Animator animator;
    GameObject body;
    GameObject armL;
    GameObject armR;
    GameObject legL;
    GameObject legR;

    void Start()
    {
        animator = GetComponent<Animator>();
        Transform root = transform.Find("Armature").Find("Root_M");
        body = root.Find("Spine1_M").gameObject;
        legL = root.Find("Hip_L").gameObject;
        legR = root.Find("Hip_R").gameObject;
        armL = body.transform.Find("Spine2_M").Find("Chest_M").Find("Scapula_L").gameObject;
        armR = body.transform.Find("Spine2_M").Find("Chest_M").Find("Scapula_R").gameObject;


        body.transform.localScale *= health;
        legL.transform.localScale *= (speed);
        legR.transform.localScale *= (speed);
        //armL.transform.localScale *= (defense / health);
        //armR.transform.localScale *= (defense / health);

        //Debug.Log("STATS:");
        //Debug.Log(health);
        //Debug.Log(speed);
    }

    // Update is called once per frame
    void Update()
    {
        if (goalReached)
        {
            animator.Play("MeleeAttack_OneHanded");
        }
    }

    // 0 = arm, 1 = body, 2 = legs
    public void TakeDamage(int part) {
        animator.Play("GetHit");
        switch(part)
        {
            case 0:
                Debug.Log("arms");
                health -= (1f / defense);
                break;
            case 1:
                health -= (2f / defense);
                break;
            case 2:
                health -= 5f;
                break;
            default:
                break;
        }
        if (health <= 0f)
        {
            speed = 0f;
            animator.Play("StunnedLoop");
        }
    }

    public void UpdateAgent()
    {
        goalReached = false;
        animator.Play("Sprint");

        body.transform.localScale = Vector3.one * health;
        legL.transform.localScale = Vector3.one * (speed / 2f);
        legR.transform.localScale = Vector3.one * (speed / 2f);
        //armL.transform.localScale = Vector3.one * (defense / health);
        //armR.transform.localScale = Vector3.one *(defense / health);

        //Debug.Log("STATS:");
        //Debug.Log(health);
        //Debug.Log(speed);
    }

}
