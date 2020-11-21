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
    private RaycastHit2D[] boxCast;
    private CharState lastState;

    //Health
    public float curHealth = 100f;
    public float maxHealth = 100f;

    public Image healthBarImage;

    bool left;
    bool right;

    bool punch;
    bool block;

    bool punchCooldown;

    //BoxCastAll Punchvektor
    Vector2 punchVector;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        playerRB = GetComponent<Rigidbody2D>();
        lastState = currentState = (CharState)animator.GetInteger("state");
        moveSpeed = 8;
        punchVector = new Vector2(1, 0.2f);
        punchCooldown = false;
    }

    // Update is called once per frame
    void Update()
    {
        HandleAnims();

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

            if (punch && punchCooldown == false)
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
        if (gameObject.tag == "Player2")
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

            if (punch && punchCooldown == false)
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
    }
    public IEnumerator PlayerPunch()
    {
        punchCooldown = true;
        //do punch
        currentState = CharState.Punch;
        if (tag == "Player1")
        {
            boxCast = Physics2D.BoxCastAll(new Vector2(transform.position.x + 0.5f, transform.position.y), punchVector, 0, transform.forward, punchVector.x);
        }
        if (tag == "Player2")
        {
            boxCast = Physics2D.BoxCastAll(new Vector2(transform.position.x - 0.5f, transform.position.y), punchVector, 0, transform.forward, punchVector.x);
        }
        for (int i = 0; i < boxCast.Length; i++)
        {
            if (tag == "Player1")
            {
                if (boxCast[i].rigidbody.gameObject.tag == "Player2")
                {
                    GameObject.FindGameObjectWithTag("Player2").GetComponent<PlayerController>().DamagePlayer(5);
                }
            }

            if (tag == "Player2")
            {
                if (boxCast[i].rigidbody.gameObject.tag == "Player1")
                {
                    GameObject.FindGameObjectWithTag("Player1").GetComponent<PlayerController>().DamagePlayer(5);
                }
            }
        }
        //gameObject.transform.GetChild(0).gameObject.GetComponent<BoxCollider2D>().enabled = true;
        yield return new WaitForSeconds(0.2f);
        currentState = CharState.Idle;
        punchCooldown = false;
        //gameObject.transform.GetChild(0).gameObject.GetComponent<BoxCollider2D>().enabled = false;
    }

    private void DamagePlayer(float damage)
    {
        if (block == false)
        {
            curHealth -= damage;
            healthBarImage.fillAmount = (curHealth / 100);
            gameObject.transform.GetChild(2).GetComponent<ParticleSystem>().Play();
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
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Matrix4x4 oldMatrix = Gizmos.matrix;
        if (tag == "Player1") Gizmos.DrawWireCube(new Vector2(transform.position.x + 0.5f, transform.position.y), punchVector);
        if (tag == "Player2") Gizmos.DrawWireCube(new Vector2(transform.position.x - 0.5f, transform.position.y), punchVector);
        Gizmos.matrix = oldMatrix;
    }
}
