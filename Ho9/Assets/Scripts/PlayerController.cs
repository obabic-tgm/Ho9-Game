using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Security;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public enum CharState { Idle, Run, Punch, Block, Jump, Fall, Hit, Death };

    //=====================================================
    // Escaping Animator Hell
    //=====================================================

    public int groundMask;

    //Animation States
    const string PLAYER_IDLE = "Player_idle";
    const string PLAYER_RUN = "Player_run";
    const string PLAYER_ATTACK = "Player_attack";
    const string PLAYER_BLOCK = "Player_block";
    //const string PLAYER_AIR_ATTACK = "Player_air_attack";
    const string PLAYER_JUMP = "Player_jump";
    const string PLAYER_FALL = "Player_fall";
    const string PLAYER_HIT = "Player_hit";
    const string PLAYER_DEATH = "Player_death";

    private string currentAnimaton;


    [SerializeField]
    private float attackDelay = 0.3f;

    private bool isAttackPressed;
    private bool isAttacking;
    private bool isLeftPressed;
    private bool isRightPressed;
    private bool isBlockPressed;
    private bool isJumpPressed;

    private bool isTurned;

    //=====================================================
    // Escaping Animator Hell
    //=====================================================

    Rigidbody2D playerRB;
    Vector3 Move;
    public int moveSpeed;
    private Animator animator;
    public CharState currentState;
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

    public GameObject gameManager;
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
        groundMask = LayerMask.GetMask("Ground");
        animator = GetComponent<Animator>();
        playerRB = GetComponent<Rigidbody2D>();
        lastState = currentState = (CharState)animator.GetInteger("state");
        punchCooldown = false;
        isGrounded = false;
        flipped = false;
        isFalling = false;
        alive = true;
        isTurned = false;
    }
    /*
    // Update is called once per frame
    void Update()
    {
        Debug.Log(tag + ": " + currentState);
        HandleAnims();
        HandleMovement();
        RestartGame();
    }*/

    
    void Update()
    {
        HandleInputs();
        RestartGame();
    }

    public void PlayerPunch()
    {
        if (currentState != CharState.Punch)
        {
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
        }
    }

    private void DamagePlayer(float damage)
    {
        if (isBlockPressed == false)
        {
            curHealth -= damage;

            //Plays hitSound!
            SoundManagerScript.PlaySound("playerHit");

            healthBarImage.fillAmount = (curHealth / 100);
            gameObject.transform.GetChild(2).GetComponent<ParticleSystem>().Play();
            ChangeAnimationState(PLAYER_HIT);
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
        if (curHealth <= 0)
        {
            ChangeAnimationState(PLAYER_DEATH);
            restartButton.SetActive(true);
            alive = false;
            gameManager.GetComponent<GameManagerScript>().DisablePlayerScripts();
        }
    }

    void ChangeAnimationState(string newAnimation)
    {
        if (currentAnimaton == newAnimation) return;

        animator.Play(newAnimation);
        currentAnimaton = newAnimation;
    }

    void FixedUpdate()
    {
        //Debug.Log(LayerMask.NameToLayer("Ground"));
        //check if player is on the ground
        Debug.DrawRay(transform.position, Vector2.down * 3.1f, Color.red);
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 3.1f, groundMask);
        if (hit.collider != null)
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }

        //------------------------------------------

        //Check update movement based on input

        //=====================================================
        // MEINS
        //=====================================================



        if (isGrounded)
        {
            isFalling = false;
            if (!isAttacking)
            {
                if(isLeftPressed)
                {
                    ChangeAnimationState(PLAYER_RUN);
                    playerRB.transform.position += new Vector3(-1 * Time.deltaTime * moveSpeed, 0, 0);
                    if (gameObject.tag == "Player1" && !isTurned)
                    {
                        isTurned = true;
                        Vector3 newScale = transform.localScale;
                        newScale.x *= -1;
                        transform.localScale = newScale;
                        punchVectorPosition.x = -punchVectorPosition.x;
                    }
                    if (gameObject.tag == "Player2" && isTurned)
                    {
                        isTurned = false;
                        Vector3 newScale = transform.localScale;
                        newScale.x *= -1;
                        transform.localScale = newScale;
                        punchVectorPosition.x = -punchVectorPosition.x;
                    }
                }
                if (isRightPressed)
                {
                    ChangeAnimationState(PLAYER_RUN);
                    playerRB.transform.position += new Vector3(1 * Time.deltaTime * moveSpeed, 0, 0);
                    if (gameObject.tag == "Player1" && isTurned)
                    {
                        isTurned = false;
                        Vector3 newScale = transform.localScale;
                        newScale.x *= -1;
                        transform.localScale = newScale;
                        punchVectorPosition.x = -punchVectorPosition.x;
                    }
                    if (gameObject.tag == "Player2" && !isTurned)
                    {
                        isTurned = true;
                        Vector3 newScale = transform.localScale;
                        newScale.x *= -1;
                        transform.localScale = newScale;
                        punchVectorPosition.x = -punchVectorPosition.x;
                    }
                }
            }
            if (isAttackPressed)
            {
                isAttackPressed = false;
                if (!isAttacking)
                {
                    isAttacking = true;
                    ChangeAnimationState(PLAYER_ATTACK);
                    PlayerPunch();
                }
                attackDelay = animator.GetCurrentAnimatorStateInfo(0).length;
                Invoke("AttackComplete", attackDelay);
            }

            if (isBlockPressed)
            {
                ChangeAnimationState(PLAYER_BLOCK);
            }

                // Jump Pressed
            if (isJumpPressed)
            {
                if(playerRB.velocity.y >= 0){
                    playerRB.gravityScale = 1;
                }
                isJumping = true;
                playerRB.AddForce(Vector2.up * jumpVel, ForceMode2D.Impulse);
                isJumpPressed = false;
                ChangeAnimationState(PLAYER_JUMP);
            }
            if(!isJumpPressed && !isLeftPressed && !isRightPressed && !isBlockPressed && !isFalling && !isAttacking)
            {
                ChangeAnimationState(PLAYER_IDLE);
            }
        }
        else
        {
            if (playerRB.velocity.y < 0)
            {
                playerRB.gravityScale = fallVel;
                isFalling = true;
                isJumping = false;
                ChangeAnimationState(PLAYER_FALL);
            }
        }

        //=====================================================
        // MEINS
        //=====================================================
    }
    void AttackComplete()
    {
        isAttacking = false;
    }

    public void HandleInputs()
    {
        if (gameObject.tag == "Player1")
        {
            //space Atatck key pressed?
            if (Input.GetKey(KeyCode.A))
            {
                isLeftPressed = true;
            }
            else isLeftPressed = false;

            if (Input.GetKey(KeyCode.D))
            {
                isRightPressed = true;
            }
            else isRightPressed = false;

            if (Input.GetKeyDown(KeyCode.V))
            {
                isAttackPressed = true;
            }
            else isAttackPressed = false;

            if (Input.GetKey(KeyCode.B))
            {
                isBlockPressed = true;
            }
            else isBlockPressed = false;

            if (Input.GetKey(KeyCode.Space))
            {
                isJumpPressed = true;
            }
            else isJumpPressed = false;
        }
        if (gameObject.tag == "Player2")
        {
            //space Atatck key pressed?
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                isLeftPressed = true;
            }
            else isLeftPressed = false;

            if (Input.GetKey(KeyCode.RightArrow))
            {
                isRightPressed = true;
            }
            else isRightPressed = false;

            if (Input.GetKeyDown(KeyCode.Keypad0))
            {
                isAttackPressed = true;
            }
            else isAttackPressed = false;

            if (Input.GetKey(KeyCode.KeypadEnter))
            {
                isBlockPressed = true;
            }
            else isBlockPressed = false;

            if (Input.GetKey(KeyCode.DownArrow))
            {
                isJumpPressed = true;
            }
            else isJumpPressed = false;
        }
    }
}