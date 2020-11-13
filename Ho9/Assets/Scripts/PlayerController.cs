using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    protected enum CharState { Idle, Run, Punch, Block };

    Rigidbody2D playerRB;
    Vector3 Move;
    private int moveSpeed;
    private Animator animator;
    private CharState currentState;
    private CharState lastState;

    //Health
    public float curHealth = 100f;
    public float maxHealth = 100f;

    public Image healthBarImage;

    bool left;
    bool right;

    bool punch;
    bool block;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        playerRB = GetComponent<Rigidbody2D>();
        lastState = currentState = (CharState)animator.GetInteger("state");
        moveSpeed = 8;
    }

    // Update is called once per frame
    void Update()
    {
        HandleAnims();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            DamagePlayer(10);
        }
        if (gameObject.tag == "Player1")
        {
            //Physics2D.IgnoreCollision(gameObject.GetComponent<BoxCollider2D>(), GameObject.FindGameObjectWithTag("Player2").GetComponent<BoxCollider2D>());

            left = Input.GetKey(KeyCode.A);
            right = Input.GetKey(KeyCode.D);

            punch = Input.GetKeyDown(KeyCode.V);
            block = Input.GetKey(KeyCode.B);

            if (right && !block)
            {
                currentState = CharState.Run;
                playerRB.transform.position += new Vector3(1 * Time.deltaTime * moveSpeed, 0, 0);
            }
            if (left && !block)
            {
                currentState = CharState.Run;
                playerRB.transform.position += new Vector3(-1 * Time.deltaTime * moveSpeed, 0, 0);
            }

            if (punch)
            {
                StartCoroutine(PlayerPunch());
                Debug.Log("Punch");
            }

            if (block && !left && !right)
            {
                currentState = CharState.Block;
                Debug.Log("Block");
            }

            if (!right && !left && !punch && !block)
            {
                currentState = CharState.Idle;
            }
        }
        if(gameObject.tag == "Player2")
        {
            //Physics2D.IgnoreCollision(gameObject.GetComponent<BoxCollider2D>(), GameObject.FindGameObjectWithTag("Player1").GetComponent<BoxCollider2D>());

            left = Input.GetKey(KeyCode.LeftArrow);
            right = Input.GetKey(KeyCode.RightArrow);

            punch = Input.GetKeyDown(KeyCode.Keypad0);
            block = Input.GetKey(KeyCode.KeypadEnter);

            if (right && !block)
            {
                currentState = CharState.Run;
                playerRB.transform.position += new Vector3(1 * Time.deltaTime * moveSpeed, 0, 0);
            }
            if (left && !block)
            {
                currentState = CharState.Run;
                playerRB.transform.position += new Vector3(-1 * Time.deltaTime * moveSpeed, 0, 0);
            }

            if (punch)
            {
                StartCoroutine(PlayerPunch());
                Debug.Log("Punch");
            }

            if (block && !left && !right)
            {
                currentState = CharState.Block;
                Debug.Log("Block");
            }

            if(!right && !left && !punch && !block)
            {
                currentState = CharState.Idle;
            }
        }
    }
    public IEnumerator PlayerPunch()
    {
        //do punch
        currentState = CharState.Punch;
        gameObject.transform.GetChild(0).gameObject.GetComponent<BoxCollider2D>().enabled = true;
        yield return new WaitForSeconds(0.2f);
        //currentState = CharState.Idle;
        gameObject.transform.GetChild(0).gameObject.GetComponent<BoxCollider2D>().enabled = false;
    }

    private void DamagePlayer(float damage)
    {
        curHealth -= damage;
        healthBarImage.fillAmount = (curHealth / 100);
        gameObject.transform.GetChild(2).GetComponent<ParticleSystem>().Play();
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        Debug.Log("Player1 trying punching Player2");
        if (gameObject.tag == "Player1")
        {
            if(col.tag == "Player2Hit" && block == false)
            {
                DamagePlayer(10);
                Debug.Log("Take Damage");
            }
        }

        if (gameObject.tag == "Player2")
        {
            //Debug.Log("Player1 trying punching Player2");
            if (col.tag == "Player1Hit" && block == false)
            {
                DamagePlayer(10);
                Debug.Log("Take Damage");
            }
        }
    }

    private void HandleAnims()
    {
        if (lastState != currentState)
        {
            // set the animation state shere
            animator.SetInteger("state", (int)currentState);
            lastState = currentState;
        }

    }
}
