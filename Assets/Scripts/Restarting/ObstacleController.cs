using UnityEngine;
using System.Collections;

public class ObstacleController : MonoBehaviour,IUsesGameStates {

	public GameObject DeadMe;

	public int myHashCode;

	
	public bool linearTranslation = true;
    public bool zTranslationScale = false;
	
	public BlockController myBC;

	public Vector3 startPosition;
	Vector3 baseScale;

	public enum obstacleState { Waiting, Approaching, InGame}
	public obstacleState currentObstacleState = obstacleState.Waiting;

	public delegate void stateDependantFunctionPointer();
	public stateDependantFunctionPointer doObstacleStateStuff;    

    public GameSpeedManager gsm;
	GlobalVars gameVars;
    GameStatesController gameStates;

    public float mySpeed = 1.0f;
	public float pushForce = 5.0f;
	Transform eotl;
	float pushToPositionY;
	Animator myAnim;
	Rigidbody2D myRB2 = null;
    

    public void RegisterToGameStateChangeEvents()
    {
        gameStates.ChangeState += OnGameStateChange;
    }
    public void OnGameStateChange(object sender, StateChangeEventArgs e)
    {
        Invoke(StateTools.ToString(e.NewState), 0.0f);
    }


    public obstacleState CurrentObstacleState
	{
		get
		{
			return currentObstacleState;
		}

		set
		{
			currentObstacleState = value;
			switch (value) {

			case obstacleState.Approaching:
				SetApproaching ();
				break;

			case obstacleState.Waiting:
				SetWaiting ();
                //Debug.Log("Obstacle " + gameObject.GetHashCode() + " Waiting");
				if (myBC.CheckObstaclesState (obstacleState.Waiting)) {
					myBC.CurrentBlockState = BlockController.blocksState.Waiting;
                        //Debug.Log("blockcontroller waiting");
				}
				break;

			case obstacleState.InGame:
                SetIngame ();
                //Debug.Log("Obstacle " + gameObject.GetHashCode() + " Ingame");
                if (myBC.CheckNotObstaclesState (obstacleState.Approaching)) {                        
                    myBC.CurrentBlockState = BlockController.blocksState.Ingame;
                        //Debug.Log("blockcontroller ingame");
                    }
				break;

			default:break;            
			}
		}
	}

	// Use this for initialization
	void Start () {
		myHashCode = gameObject.GetHashCode();
		startPosition = transform.position;        
		GameObject gameManager = GameObject.FindGameObjectWithTag(GlobalTags.GameManager);
		gsm = gameManager.GetComponent<GameSpeedManager>();
		gameVars = gameManager.GetComponent<GlobalVars>();
		eotl = gameVars.endOfTheLine;
		myAnim = gameObject.GetComponent<Animator> ();
		myRB2 = gameObject.GetComponent<Rigidbody2D> ();
		baseScale = transform.localScale;
        gameStates = gameManager.GetComponent<GameStatesController>();        
        RegisterToGameStateChangeEvents();
    }

	// Update is called once per frame
	void Update () {
		if (doObstacleStateStuff != null)
		{
			doObstacleStateStuff();
		}
		//Debug.Log(myBC.gameObject.name + " " + gameObject.name + " " + CurrentObstacleState);
	}


	void SetWaiting()
	{
		doObstacleStateStuff = null;
		transform.position = new Vector3(startPosition.x, startPosition.y, startPosition.z);
		transform.localScale = baseScale;
		if (myRB2 != null) {
			myRB2.WakeUp ();
		}
	}
	void SetApproaching()
	{
		doObstacleStateStuff = null;
		doObstacleStateStuff += LinearTranslation;
		doObstacleStateStuff += MonitorInGame;
        if (zTranslationScale) doObstacleStateStuff += ZTranslation;
	}
	void SetIngame()
	{
		doObstacleStateStuff = null;
		if (linearTranslation) { doObstacleStateStuff += LinearTranslation; }
        if (zTranslationScale) { doObstacleStateStuff += ZTranslation; }
    }
	void LinearTranslation()
	{
		if (transform.position.x > eotl.position.x)
		{
			transform.position = new Vector3(Mathf.Lerp(transform.position.x, transform.position.x - gameVars.linearTranslationOffset, gsm.GameSpeed * mySpeed * Time.deltaTime), transform.position.y, transform.position.z);            
		}
		else
		{
			CurrentObstacleState = obstacleState.Waiting;
		}        
	}
	void VerticalTranslation(){
		//Debug.Log (gameObject.name + " Vertical T - push to : "+pushToPositionY + "diff : "+Mathf.Abs(transform.position.y - pushToPositionY));
		if (Mathf.Abs(transform.position.y - pushToPositionY) > 0.1f){
			transform.position = new Vector3(transform.position.x, Mathf.Lerp(transform.position.y, pushToPositionY, gsm.gameSpeed * pushForce * Time.deltaTime), transform.position.z);
		}
		else {doObstacleStateStuff -= VerticalTranslation;
		}
	}
    void ZTranslation()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, Mathf.Lerp(transform.position.z, 0.0f , 0.5f * Time.deltaTime));
    }
	public GameObject Die(GameObject killer){
		
		if (DeadMe != null) {
			GameObject dyingMe = GameObject.Instantiate (DeadMe);
			dyingMe.GetComponent<BacDeath> ().SetComponents (gameVars, gsm, transform.position, killer);
            transform.position = new Vector3(transform.position.x, transform.position.y, gameVars.deadZone);
            return dyingMe;
		} else Debug.Log ("no death prefab set " + gameObject.name);

		transform.position = new Vector3(transform.position.x, transform.position.y, gameVars.deadZone);
        return null;
	}

	public GameObject GetEaten (GameObject eater){
		//Debug.Log ("eat me");

		return(Die (eater));
	}


	public void Pushed(float pushToY){
		if (pushToY < transform.position.y) {
			
			transform.localScale = new Vector3 (transform.localScale.x, -1.0f, transform.localScale.z);

		} 
		myAnim.SetTrigger ("PushedUp");
		pushToPositionY = pushToY;
		doObstacleStateStuff += VerticalTranslation;
	}

	public void MonitorInGame() {
		if (transform.position.x < gameVars.rightWall) CurrentObstacleState = obstacleState.InGame;        
	}

    public void Intro() { }
    public void Menu() { }
    public void Help() { }
    public void Beginning() { }
    public void Game()
    {
        
    }
    public void Pause() {
        
    }
    public void ExitPause() {
        
        
    }
    public void Reload() { }
    public void Restart() { }
    public void GameOver() { }
}