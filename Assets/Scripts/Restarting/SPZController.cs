using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class SPZController : MonoBehaviour, IUsesGameStates {

    GameStatesController gameStates;
    GlobalVars gameVars;
    GameSpeedManager speedManager;

    public delegate void stateDependantFunctionPointer();
    public stateDependantFunctionPointer doStateStuff;    
    public stateDependantFunctionPointer doPlayerStateStuff;

	public delegate void BodyCollision();
	public BodyCollision bodyCollision;
	public GameObject collidedGameObject = null;

	public delegate void MouthCollision(GameObject go);
	public MouthCollision mouthCollision;
    List<GameObject> lunch = new List<GameObject>();

    public void RegisterToGameStateChangeEvents() {
        gameStates.ChangeState += OnGameStateChange;
    }
    public void OnGameStateChange(object sender, StateChangeEventArgs e) {
        Invoke(StateTools.ToString(e.NewState), 0.0f);
    }
    public void Intro() { }
    public void Menu() { }
    public void Help() { }
    public void Beginning() { }
    public void Game()
    {
        SetPlayerStateParams();        
    }
    public void Pause() { doStateStuff = null; }
    public void ExitPause() { }
    public void Reload() { }
    public void Restart() { }
    public void GameOver() { }

    
    void Awake()
    {
        GameObject gameManager = GameObject.FindGameObjectWithTag(GlobalTags.GameManager);
        gameVars = gameManager.GetComponent<GlobalVars>();
        gameStates = gameManager.GetComponent<GameStatesController>();
        speedManager = gameManager.GetComponent<GameSpeedManager>();        
        RegisterToGameStateChangeEvents();        
		mouthCollision = EatFood;
        SetCurrentCollisionState(playerCollisionStates.Vulnerable);
        lunch.Add(tail);
        particleSys.SetActive(false);
        SetNewControllerMode(controllerModes.FreeMove);
    }
    
    enum playerStates { Resting, MonitorPull, Grabbed, BoostOnEndDrag, InBoost, BoostStarted, BoostEnding}
    playerStates currentPlayerState = playerStates.Resting;
    enum playerCollisionStates { Invulnerable, Vulnerable}
    playerCollisionStates currentCollisionState;
    enum controllerModes { FreeMove, Lines, Jetpack }
    controllerModes currentControllerMode = controllerModes.FreeMove;

    void SetNewControllerMode(controllerModes newCtrl)
    {
        switch (newCtrl)
        {
            case controllerModes.FreeMove:
                currentControllerMode = controllerModes.FreeMove;
                setRestingParams = SetRestingParamsFreeMove;
                setMonitorPullParams = SetMonitorPullParamsFreeMove;
                setGrabbedParams = SetGrabbedParamsFreeMove;
                setBoostOnEndDragParams = SetBoostOnEndDragParamsFreeMove;
                setInBoostParams = SetInBoostParamsFreeMove;
                setBoostStartedParams = SetBoostStartedParamsFreeMove;
                break;
            case controllerModes.Lines:
                currentControllerMode = controllerModes.Lines;
                break;
            case controllerModes.Jetpack:
                currentControllerMode = controllerModes.Jetpack;
                break;
            default:
                break;
        }
    }

    void SetCurrentCollisionState(playerCollisionStates pcs) {
        switch (pcs)
        {
            case playerCollisionStates.Invulnerable:
                bodyCollision = PushNoHit;
                break;
            case playerCollisionStates.Vulnerable:
                if (currentPlayerState != playerStates.BoostOnEndDrag && currentPlayerState != playerStates.InBoost &&
                    currentPlayerState != playerStates.BoostStarted)
                {
                    bodyCollision = HitPush;
                }else { bodyCollision = PushNoHit; }
                break;
            default:
                break;
        }
    }


    public GameObject tail;
    public GameObject mouth;
    public Animator avatar;
	public GameObject hitAvatar;    
    SpriteStore hitAvatarSpriteStore;
    public GameObject myBody, particleSys;
    

      
    public float verticalSpeed = 30.0f; bool allowVSlide = false;
    public float restingHorizontalSpeed = 2.0f;
    float targetY = 0.0f;
	public float pushForce = 5.0f;

    public float invulTimeAfterHit = 0.6f;

    #region Boost Parameters
    public float startBoostPositionX = -6.0f;
	public float boostFactor = 1.3f;
    public float boostHorizontalSpeed = 4.0f;
    public float boostTimer = 2.0f;
    public float pullThreshold = 100.0f;
    public float doubleClickThreshold = 0.3f;
    public float doubleClickTimer = 0.0f;
    #endregion

    int maxFood = 4;

    private Vector3 grabbedScreenPosition;
	private Vector3 offset;
    private float myLeftLimit;

	// Use this for initialization
	void Start () {
        
        myLeftLimit = gameVars.leftWall + Mathf.Abs(tail.transform.position.x - transform.position.x);
        if (hitAvatar == null)
        {
            Debug.Log("Warning no hit avatar set");
        }
        else
        {
            hitAvatarSpriteStore = hitAvatar.GetComponent<SpriteStore>();
            if (hitAvatarSpriteStore == null)
            {
                Debug.Log("Warning no sprite Store for hit avatar set");
            }
        }
    }

	
	// Update is called once per frame
	void Update () {
        if (doStateStuff != null)
            doStateStuff();       
    }

    delegate void SetParamsFunctionPointer();
    SetParamsFunctionPointer setRestingParams;
    SetParamsFunctionPointer setMonitorPullParams;
    SetParamsFunctionPointer setGrabbedParams;
    SetParamsFunctionPointer setBoostOnEndDragParams;
    SetParamsFunctionPointer setInBoostParams;
    SetParamsFunctionPointer setBoostStartedParams;

    void SetPlayerStateParams() {
        switch (currentPlayerState) {
            case playerStates.Resting:
                setRestingParams();
                break;
            case playerStates.MonitorPull:
                setMonitorPullParams();
                break;
            case playerStates.Grabbed:
                setGrabbedParams();
                break;
            case playerStates.BoostOnEndDrag:
                setBoostOnEndDragParams();
                break;
            case playerStates.InBoost:
                setInBoostParams();
                break;
            case playerStates.BoostStarted:
                setBoostStartedParams();
                break;
            default:
                setRestingParams();
                break;
        }
		doStateStuff = doPlayerStateStuff;        
    }

    #region SetParamsFreeMove
    void SetRestingParamsFreeMove() {
		gameVars.playerDefaultX = gameVars.PlayerRestingPosition;
		doPlayerStateStuff = HandleHRestedSliding;
		doPlayerStateStuff += ClickSetTargetVector;
		doPlayerStateStuff += HandleVSliding;
    }
    void SetMonitorPullParamsFreeMove()
    {
		doPlayerStateStuff = ClickSetTargetVector;
		doPlayerStateStuff += HandleVSliding;
		doPlayerStateStuff += HandleHRestedSliding;
		doPlayerStateStuff += MonitorXBoostStart;
    }
    void SetGrabbedParamsFreeMove() {}
    void SetBoostOnEndDragParamsFreeMove()
    {
        avatar.SetBool("Grabbed", true);
		doPlayerStateStuff = HandleHRestedSliding;
        SetCurrentCollisionState(playerCollisionStates.Invulnerable);        

		Invoke("OnEndDrag",0.5f);
    }
	void SetInBoostParamsFreeMove()
    {
        StartBoost();
        doPlayerStateStuff = ClickSetTargetVector;
        doPlayerStateStuff += HandleVSliding;
        doPlayerStateStuff += HandleHBoostedSliding;
        doubleClickTimer = 0.0f;
        doPlayerStateStuff += TemporizeDoubleClick; 
    }
    void SetBoostStartedParamsFreeMove() {
        doPlayerStateStuff = null;        
        doPlayerStateStuff = ClickSetTargetVector;
        doPlayerStateStuff += HandleVSliding;
        doPlayerStateStuff += HandleHBoostedSliding;
        doubleClickTimer = 0.0f;
        doPlayerStateStuff += MonitorDoubleClick;
    }
    #endregion

    void StartBoost()
    {
        avatar.SetBool("Boost", true);
        avatar.speed = 2.0f;
        speedManager.SpeedModifier = boostFactor;
        tail.SetActive(false);
        particleSys.SetActive(true);
        gameVars.PlayerStepsAhead++;        
        Invoke("BoostEnding", boostTimer);
    }
	void BoostEnding(){
		if (currentPlayerState == playerStates.BoostStarted ) { 
			currentPlayerState = playerStates.BoostEnding;
            SetCurrentCollisionState(playerCollisionStates.Vulnerable);
			avatar.SetBool("Boost", false);
			avatar.speed = 1.0f;
			tail.SetActive(true);
			speedManager.SpeedModifier = 1.0f;
			Invoke("StopBoost", 0.8f);
		}
	}
    void StopBoost() {
		if (currentPlayerState == playerStates.BoostEnding) {            
            currentPlayerState = playerStates.Resting;
            //Debug.Log("resting called from stopboost");
            SetPlayerStateParams();
        }
    }
    void GoToPlayerBoostPosition()
    {
        gameVars.playerDefaultX = gameVars.playerBoostPosition;
    }
    public void ClickSetTargetVector(){
        if (Input.GetMouseButton(0))
        {
            Vector3 mouseDown = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0.0f));
            if (gameVars.IsInGameScreen(mouseDown)) { 
                Vector3 targetVector3 = mouseDown;
                targetY = TrimTargetVectorY(targetVector3.y);
                if (Mathf.Abs(transform.position.y - targetY) > 0.05f) allowVSlide = true;
            }
        }
	}

    void HandleVSliding()
    {
        if (allowVSlide) { VerticalSlide(); }        
    }
    void HandleHRestedSliding() {
        StartCoroutine("HorizontalRestedSlide");
    }
    void HandleHBoostedSliding()
    {
		StartCoroutine("HorizontalBoostedSlide");
    }

    void VerticalSlide (){
        if ((Mathf.Abs(transform.position.y - targetY) > 0.05f))
        {
           transform.position = new Vector2(transform.position.x, Mathf.LerpUnclamped(transform.position.y, targetY, verticalSpeed * Time.deltaTime));           
        }
        else allowVSlide = false;
	}

    IEnumerator HorizontalRestedSlide (){
        if ((Mathf.Abs(transform.position.x - gameVars.PlayerDefaultX) > 0.03f))
        {
			transform.position = new Vector2(Mathf.Lerp(transform.position.x, gameVars.PlayerDefaultX, restingHorizontalSpeed * Time.deltaTime), transform.position.y);
        }
        else
        {
			transform.position.Set(gameVars.PlayerDefaultX, transform.position.y, transform.position.z);
        }
        yield return null;
	}
    IEnumerator HorizontalBoostedSlide() {		
		if ((Mathf.Abs(transform.position.x - gameVars.PlayerDefaultX) > 0.03f))
        {
			transform.position = new Vector2(Mathf.Lerp(transform.position.x, gameVars.PlayerDefaultX, boostHorizontalSpeed * Time.deltaTime), transform.position.y);
        }
        else
        {
			transform.position.Set(gameVars.playerDefaultX, transform.position.y, transform.position.z);
        }
        yield return null;
    }
	/*void HBoostedSlide() {
		Debug.Log ("BoostSlide");
		if ((Mathf.Abs(transform.position.x - gameVars.PlayerDefaultX) > 0.03f))
		{
			transform.position = new Vector2(Mathf.Lerp(transform.position.x, gameVars.PlayerDefaultX, boostHorizontalSpeed * Time.deltaTime), transform.position.y);
		}
		else
		{
			transform.position.Set(gameVars.PlayerDefaultX, transform.position.y, transform.position.z);
		}
	}*/
    float TrimTargetVectorY(float y){//Stops spz from going through roof or floor; R & F are defined in gamevars
		if (y > gameVars.roof) return(gameVars.roof);
		if (y < gameVars.floor) return(gameVars.floor);
		return y;
	}
	float TrimToDefaultPosX(float x){
		if (x > gameVars.PlayerDefaultX)
			return gameVars.PlayerDefaultX;
        float v = gameVars.leftWall + Vector3.Distance(tail.transform.position, mouth.transform.position) / 2;
        
        if (x < v) return v;
        else
            return x;            
    }
    float TrimGrabbedX(float x)
    {
        if (x > transform.position.x) return transform.position.x;
        else if (x < myLeftLimit) return myLeftLimit;
        else return x;
    }

    public void OnBeginDrag (){        
		grabbedScreenPosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, transform.position.z);
		offset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, transform.position.z));
		if (currentPlayerState == playerStates.Resting || currentPlayerState == playerStates.BoostEnding )
        {
            currentPlayerState = playerStates.MonitorPull;
            SetPlayerStateParams();
        }
	}

	public void OnEndDrag(){
        avatar.SetBool("Grabbed", false);
        targetY = transform.position.y;
		ClickSetTargetVector();
        if (currentPlayerState == playerStates.BoostOnEndDrag && lunch.Count > 1)// && if hasEnoughBoost
        {
            currentPlayerState = playerStates.InBoost;
            gameVars.PlayerDefaultX = gameVars.PlayerRestingPosition;
            GameObject tempBacDeath = lunch[lunch.Count - 1];
            lunch.RemoveAt(lunch.Count - 1);
            tempBacDeath.GetComponent<BacDeath>().Consumed();
            
            Invoke("GoToPlayerBoostPosition", 0.15f);
        }
        else if (currentPlayerState != playerStates.InBoost && currentPlayerState != playerStates.BoostStarted)
        {
            currentPlayerState = playerStates.Resting;            
            //Debug.Log("resting called from OnEndDrag");
        }
        SetPlayerStateParams();
    }

	void MoveGrab(){        
        Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, transform.position.z);
		Vector3 curPosition = Camera.main.ScreenToWorldPoint (curScreenPoint) + offset;        
		curPosition.Set(TrimGrabbedX(curPosition.x), TrimTargetVectorY(transform.position.y), transform.position.z);
        transform.position = curPosition;
    }

	void MonitorXBoostStart(){
        float pullDistance = grabbedScreenPosition.x - Input.mousePosition.x;       
        if ((pullDistance > 0) && (Mathf.Abs(pullDistance) > pullThreshold) ){
            
            currentPlayerState = playerStates.BoostOnEndDrag;
            SetPlayerStateParams();            
        }       
	}

    void MonitorEscapeButton()
    {
        /*if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("esc pressed");
            if (gameStates.CurrentState == GameStates.Game) { gameStates.SetPauseState();  }
            else if (gameStates.CurrentState == GameStates.Pause) gameStates.SetExitPauseState();
        }*/
    }

    void TemporizeDoubleClick() {
        //Debug.Log("Before IF tempo dbclicktimer : " + doubleClickTimer + " " + doPlayerStateStuff.GetInvocationList().Length);
        
        if (doubleClickTimer < doubleClickThreshold)
        {
            doubleClickTimer += Time.deltaTime;
        }
        else {
            //Debug.Log("into the else");
            currentPlayerState = playerStates.BoostStarted;
            particleSys.SetActive(false);
            SetPlayerStateParams();
        }
        
    }

    void MonitorDoubleClick()
    {
        if (DoubleClick()) { 
			Debug.Log("Double click Stopping Boost"); 
			doubleClickTimer = 0.0f; BoostEnding(); 
		}
    }
    bool DoubleClick() {
        if (Input.GetMouseButtonUp(0)) { 
            if ( (doubleClickTimer > 0.0f) && (doubleClickTimer < doubleClickThreshold) )
            {
                doubleClickTimer = 0.0f;
                return true;
            }else
            {
                if (doubleClickTimer == 0.0f) { doubleClickTimer += Time.deltaTime; return false; }
                if (doubleClickTimer > doubleClickThreshold) { doubleClickTimer = 0.0f; return false; }
            }
            if (doubleClickTimer > 0.0f) doubleClickTimer += Time.deltaTime;
        }else if (doubleClickTimer > 0.0f) { 
                
                if (doubleClickTimer <= doubleClickThreshold) doubleClickTimer += Time.deltaTime;
        }
        if (doubleClickTimer >= doubleClickThreshold) doubleClickTimer = 0.0f;
        return false;
    }



    public void EatFood(GameObject food)
    {
        GameObject deadFood = food.GetComponent<ObstacleController>().GetEaten(lunch[lunch.Count - 1]);
        if (lunch.Count < maxFood) { lunch.Add(deadFood); }
        else
        {
            deadFood.GetComponent<BacDeath>().Consumed();
        }
    }
	public void HitPush(){
		//Debug.Log ("Owweee!!! ");
		if (collidedGameObject != null) {
			//Debug.Log (collidedGameObject.name);
			int pushDirection = 1;
			float cG = collidedGameObject.transform.position.y;
			if (transform.position.y > cG)
				pushDirection = -1;
			cG = cG + (pushDirection);
			collidedGameObject.GetComponent<ObstacleController> ().Pushed (cG);

			targetY = TrimTargetVectorY((-pushDirection) * pushForce + transform.position.y);
			allowVSlide = true;            
            StartHitSequence ();
		}
	}

	void StartHitSequence (){
        //Debug.Log("steps ahead : " + gameVars.PlayerStepsAhead);
        SetCurrentCollisionState(playerCollisionStates.Invulnerable);
        avatar.gameObject.transform.position = new Vector3 (avatar.gameObject.transform.position.x, avatar.gameObject.transform.position.y, gameVars.deadZone);
		hitAvatar.transform.position = new Vector3 (hitAvatar.gameObject.transform.position.x, hitAvatar.gameObject.transform.position.y, 0.0f);
		gameVars.PlayerDefaultX -= 0.5f;
        gameVars.PlayerStepsAhead--;
        Invoke("StopHitSequence", 1.0f);
	}
	
    void StopHitSequence(){        
        SetCurrentCollisionState(playerCollisionStates.Vulnerable);        
		gameVars.PlayerDefaultX += 0.5f;
		avatar.gameObject.transform.position = new Vector3 (avatar.gameObject.transform.position.x, avatar.gameObject.transform.position.y, 0.0f);
		hitAvatar.gameObject.transform.position = new Vector3 (hitAvatar.gameObject.transform.position.x, hitAvatar.gameObject.transform.position.y, gameVars.deadZone);		
	}

	public void PushNoHit(){
		//Debug.Log ("Owweee!!! ");
		if (collidedGameObject != null) {
			//Debug.Log (collidedGameObject.name);
			int pushDirection = 1;
			float cG = collidedGameObject.transform.position.y;
			if (transform.position.y > cG)
				pushDirection = -1;
			cG = cG + (pushDirection);
			collidedGameObject.GetComponent<ObstacleController> ().Pushed (cG);
		}
	}
}