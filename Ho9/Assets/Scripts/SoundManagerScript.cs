using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManagerScript : MonoBehaviour
{
    public static AudioClip playerHit, blockHit;
    static AudioSource audioSrc;
    // Start is called before the first frame update
    void Start()
    {
        playerHit = Resources.Load<AudioClip>("playerHit");
        blockHit = Resources.Load<AudioClip>("blockHit");

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
            case "blockHit":
                audioSrc.PlayOneShot(blockHit);
                break;
        }
    }
}
