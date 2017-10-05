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
        SetNewControllerMode(currentControllerMode);
    }
    
    enum playerStates { Resting, MonitorPull, Grabbed, BoostOnEndDrag, InBoost, BoostStarted, BoostEnding}
    playerStates currentPlayerState = playerStates.Resting;
    enum playerCollisionStates { Invulnerable, Vulnerable}
    playerCollisionStates currentCollisionState;
    public enum controllerModes { FreeMove, Lines, Jetpack }
    controllerModes currentControllerMode = controllerModes.Jetpack;

    public void SetNextCtrlMode()
    {
        if ((int)currentControllerMode == 2) currentControllerMode = controllerModes.FreeMove;
        else currentControllerMode++;
        SetNewControllerMode(currentControllerMode);
    }
    public void SetPreviousCtrlMode()
    {
        if ((int)currentControllerMode == 0) currentControllerMode = controllerModes.Jetpack;
        else currentControllerMode--;
        SetNewControllerMode(currentControllerMode);
    }
    public controllerModes GetCtrlMode()
    {
        return currentControllerMode;
    }

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
                setRestingParams = SetRestingParamsLines;
                setMonitorPullParams = SetMonitorPullParamsLines;
                setGrabbedParams = SetGrabbedParamsLines;
                setBoostOnEndDragParams = SetBoostOnEndDragParamsLines;
                setInBoostParams = SetInBoostParamsLines;
                setBoostStartedParams = SetBoostStartedParamsLines;

                linesMgr = gameVars.linesHolder;
                break;
            case controllerModes.Jetpack:
                currentControllerMode = controllerModes.Jetpack;
                setRestingParams = SetRestingParamsJetpack;
                setMonitorPullParams = SetMonitorPullParamsJetpack;
                setGrabbedParams = SetGrabbedParamsJetpack;
                setBoostOnEndDragParams = SetBoostOnEndDragParamsJetpack;
                setInBoostParams = SetInBoostParamsJetpack;
                setBoostStartedParams = SetBoostStartedParamsJetpack;
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
	//private Vector3 offset;
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
        Debug.Log("state switching to " + currentPlayerState);
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

    #region SetParamsJetpack
    void SetRestingParamsJetpack()
    {
        gameVars.playerDefaultX = gameVars.PlayerRestingPosition;           
        doPlayerStateStuff = HandleHRestedSliding;        
        doPlayerStateStuff += Swim;
        doPlayerStateStuff += FallAcceleration;
        doPlayerStateStuff += HandleRotation;
        doPlayerStateStuff += MonitorJetpackClick;       
    }
    void SetMonitorPullParamsJetpack()
    {
        doPlayerStateStuff = HandleVSliding;
        doPlayerStateStuff += HandleHRestedSliding;
        doPlayerStateStuff += MonitorXBoostStart;
    }
    void SetGrabbedParamsJetpack() { }
    void SetBoostOnEndDragParamsJetpack()
    {
        avatar.SetBool("Grabbed", true);
        doPlayerStateStuff = HandleHRestedSliding;
        SetCurrentCollisionState(playerCollisionStates.Invulnerable);

        Invoke("OnEndDrag", 0.5f);
    }
    void SetInBoostParamsJetpack()
    {
        StartBoost();        
        doPlayerStateStuff += HandleVSliding;
        doPlayerStateStuff += HandleHBoostedSliding;
        doubleClickTimer = 0.0f;
        doPlayerStateStuff += TemporizeDoubleClick;
    }
    void SetBoostStartedParamsJetpack()
    {
        doPlayerStateStuff = null;        
        doPlayerStateStuff = HandleVSliding;
        doPlayerStateStuff += HandleHBoostedSliding;
        doubleClickTimer = 0.0f;
        doPlayerStateStuff += MonitorDoubleClick;
    }
    #endregion

    #region SetParamsLines
    void SetRestingParamsLines()
    {
        gameVars.playerDefaultX = gameVars.PlayerRestingPosition;
        doPlayerStateStuff = HandleHRestedSliding;        
        doPlayerStateStuff += HandleVSliding;
        doPlayerStateStuff += MonitorVerticalSwipe;
    }
    void SetMonitorPullParamsLines()
    {
        doPlayerStateStuff = HandleVSliding;
        doPlayerStateStuff += HandleHRestedSliding;
        doPlayerStateStuff += MonitorXBoostStart;
    }
    void SetGrabbedParamsLines() { }
    void SetBoostOnEndDragParamsLines()
    {
        avatar.SetBool("Grabbed", true);
        doPlayerStateStuff = HandleHRestedSliding;
        SetCurrentCollisionState(playerCollisionStates.Invulnerable);

        Invoke("OnEndDrag", 0.5f);
    }
    void SetInBoostParamsLines()
    {
        StartBoost();        
        doPlayerStateStuff = HandleVSliding;
        doPlayerStateStuff += HandleHBoostedSliding;
        doubleClickTimer = 0.0f;
        doPlayerStateStuff += TemporizeDoubleClick;
    }
    void SetBoostStartedParamsLines()
    {
        doPlayerStateStuff = null;        
        doPlayerStateStuff = HandleVSliding;
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
            SetPlayerStateParams();
        }
    }
    void GoToPlayerBoostPosition()
    {
        gameVars.playerDefaultX = gameVars.playerBoostPosition;
    }
    public void ClickSetTargetVector(){//sets target vector after single click anywhere on the screen (free mode)
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
    #region Swim parameters
    public float swimUpAngle;
    public float swimUpOffset;
    public float swimDownOffset;
    public float swimUpSpeed;    
    public float swimDownSpeed;
    public float fallAccelerationRate = 0.1f;


    bool swimmingUp;
    float fallAcceleration = 0.0f;
    #endregion

    #region Lines parameters
    public float swipeMinMagnitude = 0.5f;
    public float LaneChangeSpeed = 1.0f;
    public float swipeMagnitude = 0;
    public float laneThickness = 0.05f;

    float swipeStart = 0;    
    bool swiping = false;
    LinesManager linesMgr;
    #endregion



    void MonitorVerticalSwipe()//for lines mode
    {
        if (!swiping)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Debug.Log("Swipe start");
                Vector3 mouseDown = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0.0f));
                if (gameVars.IsInGameScreen(mouseDown))
                {
                    swipeStart = mouseDown.y;
                    swiping = true;
                }
            }
        }
        else if (swiping)
        {
            if (Input.GetMouseButtonUp(0))
            {
                Debug.Log("Swipe end");
                Vector3 mouseDown = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0.0f));
                swiping = false;
                swipeMagnitude = mouseDown.y - swipeStart;
                if (swipeMagnitude > swipeMinMagnitude) LaneUp();
                else if (swipeMagnitude < -swipeMinMagnitude) LaneDown();
            }
        }

    }

    void LaneUp()
    {
        Debug.Log("Lane Up : " + linesMgr.GetUpperLane(transform.position.y, laneThickness));
        targetY = TrimTargetVectorY(linesMgr.GetUpperLane(transform.position.y, laneThickness));
        allowVSlide = true;
    }

    void LaneDown()
    {
        Debug.Log("Lane Down : " + linesMgr.GetLowerLane(transform.position.y, laneThickness));
        targetY = TrimTargetVectorY(linesMgr.GetLowerLane(transform.position.y, laneThickness));
        allowVSlide = true;
    }

    void Swim()
    {
        if (swimmingUp)
        {
            if ((Mathf.Abs(transform.position.y - targetY) > 0.05f))
            {
                transform.position = new Vector2(transform.position.x, Mathf.LerpUnclamped(transform.position.y, targetY, swimUpSpeed * Time.deltaTime));
            } else {swimmingUp = false;}
        }
        else if (Mathf.Abs(transform.position.x - gameVars.PlayerDefaultX) < 0.75f)
        {
            transform.position = new Vector2(transform.position.x, 
                Mathf.LerpUnclamped(transform.position.y, TrimTargetVectorY(transform.position.y - swimDownOffset), (swimDownSpeed + fallAcceleration )* Time.deltaTime));
        }
        
    }

    void FallAcceleration()
    {
        if (Mathf.Abs(transform.position.x - gameVars.PlayerDefaultX) < 0.75f)
            fallAcceleration += fallAccelerationRate;
    }

    void MonitorJetpackClick()
    {
        if (Input.GetMouseButtonUp(0))
        {
            targetY = TrimTargetVectorY(transform.position.y + swimUpOffset);
            swimmingUp = true;
            fallAcceleration = 0.0f;
        }
    }

    void HandleRotation()
    {
        
    }

    void RotateUp()
    {
        transform.RotateAround(avatar.transform.position, Vector3.forward, swimUpAngle);
    }
    void ResetRotation()
    {
        transform.rotation.Set(0.0f, 0.0f, 0.0f,transform.rotation.w);
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

    void VerticalSlide (){//slide vertically if spz has not reached target vector destination
        if ((Mathf.Abs(transform.position.y - targetY) > 0.05f))
        {
           transform.position = new Vector2(transform.position.x, Mathf.LerpUnclamped(transform.position.y, targetY, verticalSpeed * Time.deltaTime));           
        }
        else allowVSlide = false;
	}

    IEnumerator HorizontalRestedSlide (){//adjust horizontal position to default (e.g. rested position) at default speed
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
    IEnumerator HorizontalBoostedSlide() {//adjust horizontal position to boost position at boost speed
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
		//offset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, transform.position.z));
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

	/*void MoveGrab(){        //used to move the spz under the player's finger or mouse click position
        Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, transform.position.z);
		Vector3 curPosition = Camera.main.ScreenToWorldPoint (curScreenPoint) + offset;        
		curPosition.Set(TrimGrabbedX(curPosition.x), TrimTargetVectorY(transform.position.y), transform.position.z);
        transform.position = curPosition;
    }*/

	void MonitorXBoostStart(){
        float pullDistance = grabbedScreenPosition.x - Input.mousePosition.x;       
        if ((pullDistance > 0) && (Mathf.Abs(pullDistance) > pullThreshold) ){
            
            currentPlayerState = playerStates.BoostOnEndDrag;
            SetPlayerStateParams();            
        }       
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