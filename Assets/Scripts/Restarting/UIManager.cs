using UnityEngine;
using System.Collections;

public class UIManager : MonoBehaviour, IUsesGameStates {

	public delegate void ShowGUIFunctionPointer();
	public ShowGUIFunctionPointer doStateStuff;

    GameStatesController gameStates;

    public GameObject introCanvas;
    public GameObject menuCanvas;
    public GameObject beginningCanvas;
    public GameObject inGameCanvas;
    public GameObject pauseGameCanvas;
    // Use this for initialization
    void Awake()
    {
        GameObject gameManager = GameObject.FindGameObjectWithTag(GlobalTags.GameManager);        
        gameStates = gameManager.GetComponent<GameStatesController>();
        RegisterToGameStateChangeEvents();
    }

    // Update is called once per frame
    /*void Update () {
        if (doStateStuff != null)
            doStateStuff();
    }

	void OnGUI(){
		
	}*/


    public void RegisterToGameStateChangeEvents() {
        gameStates.ChangeState += OnGameStateChange;
    }
    public void OnGameStateChange(object sender, StateChangeEventArgs e) {
        Invoke(StateTools.ToString(e.NewState), 0.0f);
    }

    void DisableAll()
    {
        introCanvas.SetActive(false);
        menuCanvas.SetActive(false);
        beginningCanvas.SetActive(false);
        inGameCanvas.SetActive(false);
        pauseGameCanvas.SetActive(false);
    }

    public void Intro() {
        DisableAll();
        introCanvas.SetActive(true);
    }
    public void Menu() {
        menuCanvas.SetActive(true);
    }
    public void Help() { }
    public void Beginning() {
        DisableAll();
        beginningCanvas.SetActive(true);
    }
    public void Game() {
        DisableAll();
        inGameCanvas.SetActive(true);
    }
    public void Pause() {
        DisableAll();
        pauseGameCanvas.SetActive(true);
    }
    public void ExitPause() { }
    public void Reload() { }
    public void Restart() { }
    public void GameOver() { }
}
