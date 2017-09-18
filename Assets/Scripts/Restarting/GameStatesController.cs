using System;
using UnityEngine;
using System.Collections;

public class GameStatesController : MonoBehaviour {


    float temp_timescale = 1.0f;

    public delegate void GameStateChangedEventHandler(object sender, StateChangeEventArgs e);
    public event GameStateChangedEventHandler ChangeState;

	protected virtual void StateChanged(StateChangeEventArgs e)
    {
        if (ChangeState != null)
            ChangeState (null, e);
    }

	GameStates currentState = GameStates.Intro;
    public GameStates CurrentState
    {
        get
        {
            return currentState;
        }

        set
        {
            StateChangeEventArgs e = new StateChangeEventArgs(currentState,value);
            StateChanged (e);
            currentState = value;
            //Debug.Log("State change sent : " + currentState);
        }
    }

	void Start(){
		CurrentState = GameStates.Intro;
	}

    public void SetIntroState() {
        CurrentState = GameStates.Intro;
    }
    public void SetMenuState()
    {
        CurrentState = GameStates.Menu;
    }
    public void SetHelpState()
    {
        CurrentState = GameStates.Help;
    }
    public void SetBeginningState()
    {
        CurrentState = GameStates.Beginning;
        Invoke("SetGameState", 0.5f);
    }
    public void SetGameState()
    {
        CurrentState = GameStates.Game;
    }
    public void SetPauseState()
    {
        temp_timescale = Time.timeScale;
        Time.timeScale = 0.0f;
        CurrentState = GameStates.Pause;
    }
    public void SetExitPauseState()
    {
        Time.timeScale = temp_timescale;
        CurrentState = GameStates.Game;
    }
    public void SetReloadState()
    {
        CurrentState = GameStates.Reload;
    }
    public void SetRestartState()
    {
        CurrentState = GameStates.Restart;
    }
    public void SetGameOverState()
    {
        CurrentState = GameStates.GameOver;
    }
}
