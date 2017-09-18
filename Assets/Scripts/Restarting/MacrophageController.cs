using UnityEngine;
using System.Collections;

public class MacrophageController : MonoBehaviour {

	GlobalVars gameVars;
	GameSpeedManager speedManager;
	//MacrophageManager myManager;
	float mySpeed = 0.6f;

	Transform eotl;
	Vector3 baseScale;
	Vector3 startPosition;
	Animator myAnim;

	public float planeSpeed = 1.0f;
	public bool verticalTranslation = false;


	public enum macrophageState { Available, Active}
	private macrophageState currentState = macrophageState.Available;


	public macrophageState CurrentState{
		get { 
			return currentState;
		}
		private set{ 
			currentState = value;
			switch (value) {
			case macrophageState.Available:
				transform.position = new Vector3 (gameVars.deadZone, gameVars.deadZone, gameVars.deadZone);
				transform.localScale = baseScale;
				doStateStuff = null;
				//this.gameObject.SetActive (false);
				break;
			case macrophageState.Active:
				transform.position = new Vector3 (startPosition.x, startPosition.y, startPosition.z);
				doStateStuff += LinearTranslation;
				if (verticalTranslation)
					doStateStuff += VerticalTranslation;
				break;
			default :
				break;
			}
		}
	}

	public void Reactivate(float speed, float horizontalPosition, float verticalPosition, bool vertical){
		
		float xScaleOrientation = 1.0f;
		verticalTranslation = vertical;
		mySpeed = speed;
		myAnim.speed = speed;
		if (mySpeed > 1.0f) {
			xScaleOrientation = -1.0f;
		}
	
		CurrentState = macrophageState.Active;

		transform.position = new Vector3 (horizontalPosition, verticalPosition * transform.position.y, transform.position.z);
		transform.localScale = new Vector3 (transform.localScale.x * xScaleOrientation, transform.localScale.y * verticalPosition, transform.localScale.z);

	}

	void LinearTranslation()
	{
		if (transform.position.x > eotl.position.x)
		{
			transform.position = new Vector3(Mathf.Lerp(transform.position.x, transform.position.x - gameVars.linearTranslationOffset, speedManager.GameSpeed * mySpeed * planeSpeed * Time.deltaTime), transform.position.y, transform.position.z);            
		}
		else
		{
			CurrentState = macrophageState.Available;
		}        
	}

	void VerticalTranslation(){
		if ((transform.position.y > gameVars.roof) ||(transform.position.y < gameVars.floor) ) {
			mySpeed = -mySpeed;
		}
		transform.position = new Vector3(transform.position.x, Mathf.Lerp(transform.position.y, transform.position.y - gameVars.linearTranslationOffset, speedManager.GameSpeed * mySpeed * planeSpeed * Time.deltaTime), transform.position.z);            
	}

	void MonitorOrientation(){
		
	}

	public delegate void stateDependantFunctionPointer();
	public stateDependantFunctionPointer doStateStuff;  

	void Awake (){
		startPosition = new Vector3 (transform.position.x, transform.position.y, transform.position.z);
		myAnim = GetComponent<Animator> ();
	}
	// Use this for initialization
	void Start () {
		GameObject gameManager = GameObject.FindGameObjectWithTag(GlobalTags.GameManager);
		gameVars = gameManager.GetComponent<GlobalVars>();
		speedManager = gameManager.GetComponent<GameSpeedManager>();
		eotl = gameVars.endOfTheLine;
		baseScale = transform.localScale;
		CurrentState = macrophageState.Available;

		//myManager = GameObject.FindObjectOfType<MacrophageManager> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (doStateStuff != null) {
			doStateStuff ();
		}
	}
}
