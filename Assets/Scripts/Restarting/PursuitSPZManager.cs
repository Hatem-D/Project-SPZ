using UnityEngine;
using System.Collections;

public class PursuitSPZManager : MonoBehaviour, IUsesGameStates {

    public int minStepsAhead = -3, maxStepsAhead=3; // group will be in game screen between minSA and maxSA included
    delegate void stateDelegate ();
    stateDelegate doStateStuff;

    GameStatesController gameStates;
    GlobalVars gameVars;
    Vector3 initPosition;

    // Use this for initialization
    void Start () {

        GameObject gameManager = GameObject.FindGameObjectWithTag(GlobalTags.GameManager);
        gameVars = gameManager.GetComponent<GlobalVars>();
        gameStates = gameManager.GetComponent<GameStatesController>();
        initPosition = transform.position;
                
        RegisterToGameStateChangeEvents();
    }
	
	// Update is called once per frame
	void Update () {
	    if (doStateStuff != null)
        {
            doStateStuff();
        }
	}

    public void RegisterToGameStateChangeEvents() {
        gameStates.ChangeState += OnGameStateChange;
    }
    public void OnGameStateChange(object sender, StateChangeEventArgs e) {
        Invoke(StateTools.ToString(e.NewState), 0.0f);
    }

    public void MoveInGame()
    {
        transform.position = new Vector3(Mathf.Lerp(transform.position.x, 0.0f, Time.deltaTime), transform.position.y , transform.position.z);    
        if (Mathf.Abs(transform.position.x) < 0.1f)
        {
            doStateStuff -= MoveInGame;
        }
    }
    public void MoveOutOfGame()
    {
        transform.position = new Vector3(Mathf.Lerp(transform.position.x, initPosition.x, Time.deltaTime), transform.position.y, transform.position.z);
        if (Mathf.Abs(transform.position.x - initPosition.x) < 0.1f)
        {
            doStateStuff -= MoveOutOfGame;
        }
    }

    public void Indexer(int steps)
    {
       if (steps >= minStepsAhead && steps <= maxStepsAhead)
        {
            doStateStuff = MoveInGame;
            //Debug.Log("Steps : " + steps +" Moving IN : "+gameObject.name);
        }else
        {
            doStateStuff = MoveOutOfGame;
            //Debug.Log("Steps : " + steps + "Moving OUT : " + gameObject.name);
        }
    }


    public void Intro() { }
    public void Menu() { }
    public void Help() { }
    public void Beginning() {
        Indexer(gameVars.PlayerStepsAhead);
        gameVars.StepsChanged += Indexer;
    }
    public void Game() { }
    public void Pause() { }
    public void ExitPause() { }
    public void Reload() { }
    public void Restart() { }
    public void GameOver() { }
}
