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

    const string PLAYER_DEATH = "Player_death";

    //Animation States Boxer
    const string BOXER_IDLE = "boxer_idle";
    const string BOXER_RUN = "boxer_run";
    const string BOXER_ATTACK_QP = "boxer_attack_quick_punch";
    const string BOXER_BLOCK = "boxer_block";
    const string BOXER_JUMP = "boxer_jump";
    const string BOXER_FALL = "boxer_fall";
    const string BOXER_ATTACK_HP = "boxer_attack_heavy_punch";
    const string BOXER_HIT = "boxer_hit";

    //Animation States Boxer2
    const string ALI_IDLE = "ali_idle";
    const string ALI_RUN = "ali_run";
    const string ALI_ATTACK_QP = "ali_attack_quick_punch";
    const string ALI_BLOCK = "ali_block";
    const string ALI_JUMP = "ali_jump";
    const string ALI_FALL = "ali_fall";
    const string ALI_ATTACK_JUMP = "ali_attack_jump";
    const string ALI_HIT = "ali_hit";

    //OFFEN
    const string BOXER_DEATH = "Player_death";

    private string currentAnimaton;


    [SerializeField]
    private float attackDelay = 0.6666667f;

    private bool isAttackPressed;
    private bool isAttacking;
    private bool isHeavyAttackPressed;
    private bool isHeavyAttacking;

    private bool isLeftPressed;
    private bool isRightPressed;
    private bool isBlockPressed;
    private bool isJumpPressed;

    private bool isDamaged;

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
    public float GroundedRay;
    public float jumpVel;
    public float fallVel;

    public bool flipped;
    public bool isFalling;
    public bool jumpKey;
    public bool isJumping;
    public bool isGrounded;
    public bool isJumpAttackPressed;
    public bool isJumpAttacking;

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
        isDamaged = false;
        isJumpAttacking = false;
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
                    if (isAttacking)
                        GameObject.FindGameObjectWithTag("Player2").GetComponent<PlayerController>().DamagePlayer(5);
                    if (isHeavyAttacking)
                        GameObject.FindGameObjectWithTag("Player2").GetComponent<PlayerController>().DamagePlayer(15);
                }
            }

            if (tag == "Player2")
            {
                if (boxCast[i].rigidbody.gameObject.tag == "Player1")
                {
                    if (isAttacking)
                        GameObject.FindGameObjectWithTag("Player1").GetComponent<PlayerController>().DamagePlayer(7);
                    if (isJumpAttacking)
                        GameObject.FindGameObjectWithTag("Player1").GetComponent<PlayerController>().DamagePlayer(12);
                }
            }
        }
    }

    private void DamagePlayer(float damage)
    {
        if (isBlockPressed == false)
        {
            isDamaged = true;
            curHealth -= damage;

            //Plays hitSound!
            SoundManagerScript.PlaySound("playerHit");

            healthBarImage.fillAmount = (curHealth / 100);
            gameObject.transform.GetChild(2).GetComponent<ParticleSystem>().Play();
            if(tag == "Player1")
                ChangeAnimationState(BOXER_HIT);
            if (tag == "Player2")
                ChangeAnimationState(ALI_HIT);
            StartCoroutine(DamageEffect());
        }
        else
        {
            SoundManagerScript.PlaySound("blockHit");
        }
    }

    public IEnumerator DamageEffect()
    {
        for (int i = 0; i < 10; i++)
        {
            GetComponent<ColorChange>().materialChange(1);
            yield return new WaitForSeconds(0.05f);
            GetComponent<ColorChange>().materialChange(0);
            yield return new WaitForSeconds(0.05f);
        }
        isDamaged = false;
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
            //ChangeAnimationState(PLAYER_DEATH);
            Destroy(gameObject);
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
        Debug.DrawRay(transform.position, Vector2.down * GroundedRay, Color.red);
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, GroundedRay, groundMask);
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
            if (!isAttacking && !isHeavyAttacking)
            {
                if (isLeftPressed)
                {
                    if (tag == "Player1")
                        ChangeAnimationState(BOXER_RUN);
                    if (tag == "Player2")
                        ChangeAnimationState(ALI_RUN);
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
                    if (tag == "Player1")
                        ChangeAnimationState(BOXER_RUN);
                    if (tag == "Player2")
                        ChangeAnimationState(ALI_RUN);
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
                    if (tag == "Player1")
                        ChangeAnimationState(BOXER_ATTACK_QP);
                    if (tag == "Player2")
                        ChangeAnimationState(ALI_ATTACK_QP);
                    PlayerPunch();
                }
                Invoke("AttackComplete", attackDelay);
            }

            if (isHeavyAttackPressed)
            {
                isHeavyAttackPressed = false;
                if (!isHeavyAttacking)
                {
                    isHeavyAttacking = true;
                    ChangeAnimationState(BOXER_ATTACK_HP);
                    PlayerPunch();
                }
                Invoke("HeavyAttackComplete", attackDelay*2-0.4f);
            }

            if (isBlockPressed)
            {
                if(tag == "Player1")
                    ChangeAnimationState(BOXER_BLOCK);
                if (tag == "Player2")
                    ChangeAnimationState(ALI_BLOCK);
            }

            // Jump Pressed
            if (isJumpPressed)
            {
                if (playerRB.velocity.y >= 0)
                {
                    playerRB.gravityScale = 1;
                }
                isJumping = true;
                playerRB.AddForce(Vector2.up * jumpVel, ForceMode2D.Impulse);
                isJumpPressed = false;
                if (tag == "Player1")
                    ChangeAnimationState(BOXER_JUMP);
                if (tag == "Player2"){
                    ChangeAnimationState(ALI_JUMP);
                }
            }

            if (isJumpAttackPressed && isJumping && tag == "Player2")
            {
                isJumpAttackPressed = false;
                isHeavyAttackPressed = false;
                if (!isJumpAttacking)
                {
                    isJumpAttacking = true;
                    ChangeAnimationState(ALI_ATTACK_JUMP);
                    PlayerPunch();
                }
                Invoke("AttackJumpComplete", attackDelay * 2 - 0.4f);
            }

            if (!isJumpPressed && !isLeftPressed && !isRightPressed && !isBlockPressed && !isFalling && !isAttacking && !isHeavyAttacking && !isDamaged && !isJumpAttacking)
            {
                if (tag == "Player1")
                    ChangeAnimationState(BOXER_IDLE);
                if (tag == "Player2")
                    ChangeAnimationState(ALI_IDLE);
            }
        }
        else
        {
            if (playerRB.velocity.y < 0)
            {
                playerRB.gravityScale = fallVel;
                isFalling = true;
                isJumping = false;
                if (tag == "Player1")
                    ChangeAnimationState(BOXER_FALL);
                if (tag == "Player2")
                    ChangeAnimationState(ALI_FALL);
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

    void HeavyAttackComplete()
    {
        isHeavyAttacking = false;
    }

    void AttackJumpComplete()
    {
        isJumpAttacking = false;
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

            if (Input.GetKeyDown(KeyCode.C))
            {
                isHeavyAttackPressed = true;
            }
            else isHeavyAttackPressed = false;

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

            if (Input.GetKey(KeyCode.KeypadPlus))
            {
                isJumpAttackPressed = true;
            }
            else isJumpAttackPressed = false;
        }
    }
}