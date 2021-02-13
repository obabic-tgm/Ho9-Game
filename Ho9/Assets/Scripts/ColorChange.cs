using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorChange : MonoBehaviour
{
    public Material[] material;
    private SpriteRenderer spriteRenderer;
    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.enabled = true;
        spriteRenderer.material = material[0];
    }

    // Update is called once per frame
    public void materialChange(int whatSprite)
    {
        //whatSprite
        // 0 = Default Sprite - ohne rot
        // 1 = Sprite for OnHit - mit rot
        spriteRenderer.material = material[whatSprite];
    }
}
