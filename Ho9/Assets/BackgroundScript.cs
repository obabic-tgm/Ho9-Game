using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundScript : MonoBehaviour
{
    private string currentAnimaton;
    private Animator animator;

    private float nextActionTime = 0.0f;
    public float period = 0.1f;

    const string IDLE = "bg_idle";
    const string MIT = "bg_mit";
    const string OHNE = "bg_ohne";

    int result = 0;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        result = (int) Random.Range(0, 3);
    }

    // Update is called once per frame
    void Update()
    {
        StartCoroutine(AnimBackground());
    }

    void ChangeAnimationState(string newAnimation)
    {
        if (currentAnimaton == newAnimation) return;

        animator.Play(newAnimation);
        currentAnimaton = newAnimation;
    }

    private IEnumerator AnimBackground()
    {
        yield return new WaitForSeconds(5f);
        if(result == 0)
        {
            ChangeAnimationState(IDLE);
        }
        if(result == 1)
        {
            ChangeAnimationState(OHNE);
        }
        if (result == 2)
        {
            ChangeAnimationState(MIT);
        }
    }
}
