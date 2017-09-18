using UnityEngine;
using System.Collections;

public class GameSpeedManager : MonoBehaviour, IUsesGameStates {

    public float gameSpeed = 1.0f;
    public float speedModifier = 1.0f;
    public float distanceTravelled = 0.0f;

    public delegate void stateDependantFunctionPointer();
    public stateDependantFunctionPointer doStateStuff;

    GameStatesController gameStates;

    public float GameSpeed
    {
        get
        {
            return gameSpeed*speedModifier;
        }

        set
        {
            gameSpeed = value;
        }
    }

    public float SpeedModifier
    {
        get
        {
            return speedModifier;
        }

        set
        {
            speedModifier = value;
        }
    }

    void UpdateDistance()
    {
        distanceTravelled += Time.deltaTime * GameSpeed;        
    }
    void SwitchToMenu()
    {
        gameStates.SetMenuState();
    }
    void Awake()
    {
        gameStates = GameObject.FindGameObjectWithTag(GlobalTags.GameManager).GetComponent<GameStatesController>();
        RegisterToGameStateChangeEvents();    
    }

    void Update()
    {
        if (doStateStuff != null)
            doStateStuff ();
    }

    public void RegisterToGameStateChangeEvents() {
        gameStates.ChangeState += OnGameStateChange;
    }
    public void OnGameStateChange(object sender, StateChangeEventArgs e) {
        Invoke(StateTools.ToString(e.NewState), 0.0f);
    }

    public void Intro() {
        Invoke("SwitchToMenu", 2.0f);
    }
    public void Menu() { }
    public void Help() { }
    public void Beginning() { }
    public void Game() { doStateStuff = UpdateDistance; }
    public void Pause() { }
    public void ExitPause() { }
    public void Reload() { }
    public void Restart() { }
    public void GameOver() { }
}
