using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManagerScript : MonoBehaviour
{
    public static AudioClip playerHit, playerJump;
    static AudioSource audioSrc;
    // Start is called before the first frame update
    void Start()
    {
        playerHit = Resources.Load<AudioClip>("playerHit");
        playerJump = Resources.Load<AudioClip>("playerJump");

        audioSrc = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static void PlaySound(string clip)
    {
        switch (clip)
        {
            case "playerHit":
                audioSrc.PlayOneShot(playerHit);
                break;
            case "playerJump":
                audioSrc.PlayOneShot(playerJump);
                break;
        }
    }
}
