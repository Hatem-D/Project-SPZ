using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BackgroundScrolling : MonoBehaviour, IUsesGameStates {

	public float scrollSpeed;
	public Transform resetPosition;
	public bool locomotion = false;
	private Vector3 startPosition;

	BGScalesSpriteList activeSpriteList;
	
    float linearTranslationOffset = 5.0f;

	GameStatesController gameStates;
	GlobalVars gameVars;
    GameSpeedManager speedManager;
    
	public List<BackgroundScrolling> meFriends;
    public List<SpriteRenderer> meChildren;
    SpriteRenderer myRenderer;


	public delegate void stateDependantFunctionPointer();
	public stateDependantFunctionPointer doStateStuff;

	delegate void ResetFunctionPointer(Sprite sp);
	ResetFunctionPointer ResetPosition;

	public void RegisterToGameStateChangeEvents(){
        gameStates.ChangeState += OnGameStateChange;
	}  

    public void OnGameStateChange(object sender, StateChangeEventArgs e){		
        Invoke(StateTools.ToString(e.NewState),0.0f);
	}

   void Awake ()
	{
        GameObject gameManager = GameObject.FindGameObjectWithTag(GlobalTags.GameManager);        
        gameStates = gameManager.GetComponent<GameStatesController> ();
        speedManager = gameManager.GetComponent<GameSpeedManager>();
        gameVars = gameManager.GetComponent<GlobalVars>();
        startPosition = transform.position;
        myRenderer = gameObject.GetComponent<SpriteRenderer>();
        RegisterToGameStateChangeEvents (); 
        
    }
	
	void Update ()
	{
        if (doStateStuff !=null)
            doStateStuff();        
    }

    void ScrollBackground()
    {
		transform.position = new Vector3(Mathf.Lerp(transform.position.x, transform.position.x - linearTranslationOffset, speedManager.GameSpeed * scrollSpeed * Time.deltaTime), transform.position.y, transform.position.z);
    }

	
    
    void ChangeChildrenSprite (Sprite newSprite)
    {
        if (meChildren == null || meChildren.Count == 0)
        {
            Debug.Log("Pas d'enfants");
            return;
        }
        else
        {
            foreach (SpriteRenderer sr in meChildren)
            {
                sr.sprite = newSprite;
            }
        }
    }
    void ChangeMySprite (Sprite newSprite)
    {
        myRenderer.sprite = newSprite;        
    }

    bool childrenDone = false;

    public void RestartPosition (Sprite s){
		transform.position = startPosition;

        if (s != null)
        {
            if (childrenDone)
            {
                ChangeMySprite(s);
                childrenDone = false;
            }else
            {
                ChangeChildrenSprite(s);
                childrenDone = true;
            }
        }
    }

	public void SendResetMsg(){

		if (transform.position.x < resetPosition.position.x) {
            ResetPosition(activeSpriteList.GetNextSprite());
            foreach (BackgroundScrolling bs in meFriends) {
                bs.ResetPosition(activeSpriteList.GetNextSprite());
			}			
		}
	}

    public void NextBlockDefined(BGScalesSpriteList spriteList, int blockHashCode)
    {
        activeSpriteList = spriteList;
        Debug.Log("Active list block : " + blockHashCode);
    }

	public void Intro(){

        if (activeSpriteList == null)
        {
            activeSpriteList = new BGScalesSpriteList(gameVars, null, null, null);
        }



        doStateStuff = ScrollBackground;
		if (locomotion)
			doStateStuff += SendResetMsg;

		ResetPosition = RestartPosition;
    }
	public void Menu(){}
	public void Help(){}
	public void Beginning(){}
	public void Game(){
     
    }
	public void Pause(){}
	public void ExitPause(){}
	public void Reload(){}
	public void Restart(){}
	public void GameOver(){}
}