using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MacrophageManager : MonoBehaviour, IUsesGameStates {

	GameStatesController gameStates;
	//GlobalVars gameVars;
	//GameSpeedManager speedManager;

	public GameObject frontMacrophagePrefab;
	public float frontProportion;
	public GameObject midMacrophagePrefab;
	public float midProportion;
	public GameObject backMacrophagePrefab;
	public float backProportion;


	public int nbFrontMacrophageInstances = 36; //base number of instantiated Front Macrophages
	public int nbMidMacrophageInstances = 36;
	public int nbBackMacrophageInstances = 36;

	List <MacrophageController> instantiatedFrontMacrophagesList = new List<MacrophageController> ();
	List <MacrophageController> instantiatedMidMacrophagesList = new List<MacrophageController> ();
	List <MacrophageController> instantiatedBackMacrophagesList = new List<MacrophageController> ();

	//System.Random rSeed = new System.Random();


	public void RegisterToGameStateChangeEvents(){ 
		gameStates.ChangeState += OnGameStateChange;
	}

	public void OnGameStateChange(object sender, StateChangeEventArgs e){ 
		Invoke(StateTools.ToString(e.NewState),0.0f);
	}

	// Use this for initialization
	void Awake () {
		GameObject gameManager = GameObject.FindGameObjectWithTag(GlobalTags.GameManager);
		//gameVars = gameManager.GetComponent<GlobalVars>();
		gameStates = gameManager.GetComponent<GameStatesController>();
		//speedManager = gameManager.GetComponent<GameSpeedManager>();
		RegisterToGameStateChangeEvents();

	}

	void AddMacrophageList(List<MacrophageController> macroList, GameObject macroPrefab, int nbInstances){
		for (int i = 0; i < nbInstances; i++) {
			GameObject temp = GameObject.Instantiate (macroPrefab);
			macroList.Add(temp.GetComponent<MacrophageController>());
		}
	}

	List <MacrophageController> GetMacrophageStateList(List<MacrophageController> macList, GameObject macPrefab, MacrophageController.macrophageState macState, int instances){// gets macrophages in macState from macList and instantiates if necessary
		List<MacrophageController> stateList = macList.FindAll (m => m.CurrentState == macState);
		while (stateList.Count < instances + 1){
			AddMacrophageList (macList, macPrefab, instances - stateList.Count);
			stateList = macList.FindAll (m => m.CurrentState == macState);
		}
		return stateList;
	}
	// Update is called once per frame
	//void Update () {
		
	//}

	public void NextBlockDefined (BlockController incomingBlock){

		/*if (incomingBlock == null) {
            Debug.Log("incoming block not defined");
            return;
        }else Debug.Log("incoming block defined");

        int nbMacros = rSeed.Next(incomingBlock.minMacrophages, incomingBlock.maxMacrophages);

		float randPositionX = 10.0f;
		int nbMacrosToActivate = nbMacros * (int) frontProportion;
		List<MacrophageController> waitingList = GetMacrophageStateList (instantiatedFrontMacrophagesList, frontMacrophagePrefab, MacrophageController.macrophageState.Available, nbMacrosToActivate);
		int counter = 1;
		foreach (MacrophageController mc in waitingList) {
			randPositionX = Random.Range (incomingBlock.leftXLimit, incomingBlock.rightXLimit);
			if (counter < 3) {
				mc.Reactivate (1.0f + Random.Range (-0.1f, 0.1f), randPositionX, 1.0f, false);
			} else if (counter < 5) {
				mc.Reactivate (1.0f + Random.Range (-0.1f, 0.1f), randPositionX, -1.0f, false);
			} else if (counter < 7) {
				float randPositionY = Random.Range (gameVars.floor, gameVars.roof);
				mc.Reactivate (1.0f + Random.Range (-0.1f, 0.1f), randPositionX, randPositionY, true);
			}
			counter++;
			if (counter > 6)
				counter = 1;
		}



		Debug.Log ("waiting list : "+waitingList.Count);*/



        /*float floor = -1.0f; 

		for (int i = 0; i < nbMacros - 1; i++) {

			randPosition = Random.Range (incomingBlock.leftXLimit, incomingBlock.rightXLimit);

			if (randPosition > MidFloats (incomingBlock.leftXLimit, incomingBlock.rightXLimit)) {
				floor = -floor;
			}
			//waitingList [i].gameObject.SetActive (true);
			waitingList [i].Reactivate (1.0f + Random.Range(-0.1f,0.1f),randPosition, floor);
		}*/
    }

    public float MidFloats(float A, float B){

		return ((B + A) / 2);
	
	}
	public void Intro(){}
	public void Menu(){}
	public void Help(){}
	public void Beginning(){
		//int counter = 0;
		AddMacrophageList (instantiatedFrontMacrophagesList, frontMacrophagePrefab, nbFrontMacrophageInstances);
		AddMacrophageList (instantiatedMidMacrophagesList, midMacrophagePrefab, nbMidMacrophageInstances);
		AddMacrophageList (instantiatedBackMacrophagesList, backMacrophagePrefab, nbBackMacrophageInstances);
	}
	public void Game(){}
	public void Pause(){}
	public void ExitPause(){}
	public void Reload(){}
	public void Restart(){}
	public void GameOver(){}
}
