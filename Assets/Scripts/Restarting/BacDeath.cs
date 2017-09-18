using UnityEngine;
using System.Collections;

public class BacDeath : MonoBehaviour {

	GlobalVars gameVars;
	GameSpeedManager gsm;
	float yDirection = -1.0f;
	public bool isFood = false;
	GameObject target;
	public float mySpeed = 0.8f;
    public float tailDistance = 0.4f;

    public delegate void foodDependantFunctionPointer();
	public foodDependantFunctionPointer doDeathStateStuff;
	// Use this for initialization

	void Start () {
		if (isFood)
			doDeathStateStuff = Magnet;
		else doDeathStateStuff = VerticalMove;
	}

	public void SetComponents(GlobalVars gv, GameSpeedManager gm, Vector3 startPosition, float direction){
		gameVars = gv;
		gsm = gm;
		transform.position = new Vector3 (startPosition.x, startPosition.y, startPosition.z);
		yDirection = direction;
	}

	public void SetComponents(GlobalVars gv, GameSpeedManager gm, Vector3 startPosition, GameObject magnet){
		gameVars = gv;
		gsm = gm;
		transform.position = new Vector3 (startPosition.x, startPosition.y, startPosition.z);
		target = magnet;
	}
	
	// Update is called once per frame
	void Update () {
		if (doDeathStateStuff != null) {
			doDeathStateStuff ();
		}
	}

	void Magnet (){
		if (Vector3.Distance (gameObject.transform.position, target.transform.position + Vector3.left) > 0.4f) {
			transform.position = Vector3.Lerp (transform.position, new Vector3(target.transform.position.x - tailDistance, target.transform.position.y, target.transform.position.z), mySpeed * Time.deltaTime);
			//transform.localScale = Vector3.Lerp (transform.localScale, Vector3.zero, 0.07f * Time.deltaTime);
		}
	}

	void VerticalMove(){
		if (Mathf.Abs(transform.position.y) < gameVars.roof + 1.0f )
		{
			transform.position = new Vector3(Mathf.Lerp(transform.position.x, transform.position.x - gameVars.linearTranslationOffset, gsm.GameSpeed * mySpeed * Time.deltaTime)
				, Mathf.Lerp(transform.position.y, transform.position.y + yDirection + (gameVars.linearTranslationOffset * Mathf.Sign(yDirection)), gsm.gameSpeed * Time.deltaTime), 
				transform.position.z);
		}
		else
		{
            Consumed();
		}
	}

    public void Consumed()
    {
        GameObject.Destroy(this.gameObject);
    }
}
