using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CSecBackgroundScrolling : MonoBehaviour, IUsesGameStates {

	public float scrollSpeed;
	public Transform resetPosition;
	public bool locomotion = false;
	private Vector3 startPosition;

	
	float linearTranslationOffset = 5.0f;

	GameStatesController gameStates;
	//GlobalVars gameVars;
    GameSpeedManager speedManager;
    
	public List<CSecBackgroundScrolling> meFriends;

	public delegate void stateDependantFunctionPointer();
	public stateDependantFunctionPointer doStateStuff;

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
        //gameVars = gameManager.GetComponent<GlobalVars>();
        startPosition = transform.position;        
        RegisterToGameStateChangeEvents ();
        //handler("test");
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

	public void RestartPosition (){
		transform.position = startPosition;
	}

	public void SendResetMsg(){
		if (meFriends == null ) {
			Debug.Log ("Pas d'amis");
			RestartPosition();
			return;
		}
		if (transform.position.x < resetPosition.position.x) {
			foreach (CSecBackgroundScrolling bs in meFriends) {
                bs.RestartPosition();
			}
            RestartPosition();
		}
	}

	public void Intro(){
        		
    }

	public void Menu(){}
	public void Help(){}
	public void Beginning(){
        
    }
	public void Game(){
        doStateStuff = ScrollBackground;
        if (locomotion)
            doStateStuff += SendResetMsg;
    }
	public void Pause(){}
	public void ExitPause(){}
	public void Reload(){}
	public void Restart(){}
	public void GameOver(){}
}