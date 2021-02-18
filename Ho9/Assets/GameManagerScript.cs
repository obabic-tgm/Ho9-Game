using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerScript : MonoBehaviour
{
    public GameObject Player1;
    public GameObject Player2;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DisablePlayerScripts()
    {
        Player1.GetComponent<PlayerController>().enabled = false;
        Player2.GetComponent<PlayerController>().enabled = false;
    }
}
