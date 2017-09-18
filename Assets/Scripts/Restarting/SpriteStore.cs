using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpriteStore : MonoBehaviour {

    public Sprite[] hitSprites;
    GlobalVars gameVars;
    int hitSpritesLength;    
    SpriteRenderer myRenderer;

    void Start()
    {
        hitSpritesLength = hitSprites.Length;
        if (hitSprites == null || hitSpritesLength < 3)
        {
            Debug.Log("warning no hitsprites set");
        }
        myRenderer = gameObject.GetComponent<SpriteRenderer>();
        if (myRenderer == null)
        {
            Debug.Log("Warning no renderer set for " + gameObject.name);
        }
        GameObject gameManager = GameObject.FindGameObjectWithTag(GlobalTags.GameManager);
        gameVars = gameManager.GetComponent<GlobalVars>();
        gameVars.StepsChanged += SelectSprite;     
    }
    public void SelectSprite(int stepsAhead)
    {        
        if (stepsAhead >= -1)
        {
            myRenderer.sprite = hitSprites[0];    
        }else if (stepsAhead == -2)
        {
            myRenderer.sprite = hitSprites[1];
        }
        else if (stepsAhead < -2)
        {
            myRenderer.sprite = hitSprites[2];
        }
    }
   
}
