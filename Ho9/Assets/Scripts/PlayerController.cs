using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    protected enum CharState { Idle, Run, Punch, Block, Jump, Fall, Hit, Death };

    Rigidbody2D playerRB;
    Vector3 Move;
    public int moveSpeed;
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

    bool alive;

    public GameObject restartButton;

    public GameObject ground;
    public float bonusGravity;

    //BoxCastAll Punchvektor
    [SerializeField]
    public Vector2 punchVector;
    public Vector2 punchVectorPosition;
    public float jumpVel;
    public float fallVel;

    public bool flipped;
    public bool isFalling;
    public bool jumpKey;
    public bool isJumping;
    public bool isGrounded;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        playerRB = GetComponent<Rigidbody2D>();
        lastState = currentState = (CharState)animator.GetInteger("state");
        punchCooldown = false;
        isGrounded = false;
        flipped = false;
        isFalling = false;
        alive = true;
    }

    // Update is called once per frame
    void Update()
    {
        RestartGame();
        HandleAnims();

        /*
        if(currentState == CharState.Hit)
        {
            GetComponent<ColorChange>().materialChange(1);
        }
        else
        {
            GetComponent<ColorChange>().materialChange(0);
        }*/

        if (gameObject.tag == "Player1")
        {
            //Physics2D.IgnoreCollision(gameObject.GetComponent<BoxCollider2D>(), GameObject.FindGameObjectWithTag("Player2").GetComponent<BoxCollider2D>());

            left = Input.GetKey(KeyCode.A);
            right = Input.GetKey(KeyCode.D);

            punch = Input.GetKeyDown(KeyCode.V);
            block = Input.GetKey(KeyCode.B);

            jumpKey = Input.GetKeyDown(KeyCode.Space);

            //Right+Left
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

            //Fall
            if (playerRB.velocity.y < 0)
            {
                playerRB.gravityScale = fallVel;
                isFalling = true;
                isJumping = false;
                currentState = CharState.Fall;
                if (isGrounded == true)
                {
                    Debug.Log("SWITCH!");
                    currentState = CharState.Idle;
                }
            }

            //Jump
            if (playerRB.velocity.y >= 0)
            {
                playerRB.gravityScale = 1;
                isFalling = false;
            }

            if (jumpKey && isGrounded)
            {
                isJumping = true;
                Debug.Log("Jump");
                currentState = CharState.Jump;
                playerRB.AddForce(Vector2.up * jumpVel, ForceMode2D.Impulse);
                isGrounded = false;
            }

            //Punch+Block
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

            //Idle
            if (!right && !left && !punch && !block && !isJumping && !isFalling)
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

            jumpKey = Input.GetKeyDown(KeyCode.DownArrow);


            //Fall
            if (playerRB.velocity.y < 0)
            {
                playerRB.gravityScale = fallVel;
                isFalling = true;
                isJumping = false;
                currentState = CharState.Fall;
                if (isGrounded == true)
                {
                    Debug.Log("SWITCH!");
                    currentState = CharState.Idle;
                }
            }

            //Jump
            if (playerRB.velocity.y >= 0)
            {
                playerRB.gravityScale = 1;
                isFalling = false;
            }

            if (jumpKey && isGrounded)
            {
                isJumping = true;
                Debug.Log("Jump");
                currentState = CharState.Jump;
                playerRB.AddForce(Vector2.up * jumpVel, ForceMode2D.Impulse);
                isGrounded = false;
            }

            //Right+Left
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

            //Punch+Block
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

            //Idle
            if (!right && !left && !punch && !block && !isJumping && !isFalling)
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
            boxCast = Physics2D.BoxCastAll(new Vector2(transform.position.x + punchVectorPosition.x, transform.position.y + punchVectorPosition.y), punchVector, 0, transform.forward, punchVector.x);
        }
        if (tag == "Player2")
        {
            boxCast = Physics2D.BoxCastAll(new Vector2(transform.position.x - punchVectorPosition.x, transform.position.y + punchVectorPosition.y), punchVector, 0, transform.forward, punchVector.x);
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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject == ground) isGrounded = true;
    }

    private void DamagePlayer(float damage)
    {
        if (block == false)
        {
            curHealth -= damage;

            //Plays hitSound!
            SoundManagerScript.PlaySound("playerHit");

            healthBarImage.fillAmount = (curHealth / 100);
            gameObject.transform.GetChild(2).GetComponent<ParticleSystem>().Play();
            currentState = CharState.Hit;
            StartCoroutine(DamageEffect());
        }
        else
        {
            SoundManagerScript.PlaySound("blockHit");
        }
    }

    public IEnumerator DamageEffect()
    {
        for(int i = 0; i < 10; i++)
        {
            GetComponent<ColorChange>().materialChange(1);
            yield return new WaitForSeconds(0.05f);
            GetComponent<ColorChange>().materialChange(0);
            yield return new WaitForSeconds(0.05f);
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
        if (tag == "Player1") Gizmos.DrawWireCube(new Vector2(transform.position.x + punchVectorPosition.x, transform.position.y + punchVectorPosition.y), punchVector);
        if (tag == "Player2") Gizmos.DrawWireCube(new Vector2(transform.position.x - punchVectorPosition.x, transform.position.y + punchVectorPosition.y), punchVector);
        Gizmos.matrix = oldMatrix;
    }

    private void RestartGame()
    {
        if(curHealth <= 0)
        {
            restartButton.SetActive(true);
            alive = false;
            currentState = CharState.Death;
        }
    }

    public void Jump()
    {
        if(playerRB.velocity.y < 0)
        { 
            playerRB.gravityScale = fallVel;
        } else
        {
            playerRB.gravityScale = 1;
        }

        if (isGrounded)
        {
            if (gameObject.tag == "Player1")
            {
                jumpKey = Input.GetKeyDown(KeyCode.Space);
                Debug.Log("Jump");

                if (jumpKey)
                {
                    isGrounded = false;
                    playerRB.AddForce(Vector2.up * jumpVel, ForceMode2D.Impulse);
                }
            }
            if (gameObject.tag == "Player2")
            {
                jumpKey = Input.GetKeyDown(KeyCode.DownArrow);

                if (jumpKey)
                {
                    isGrounded = false;
                    playerRB.AddForce(Vector2.up * jumpVel, ForceMode2D.Impulse);
                }
            }
        }
    }
}