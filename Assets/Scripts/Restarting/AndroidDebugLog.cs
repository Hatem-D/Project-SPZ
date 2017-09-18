using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AndroidDebugLog : MonoBehaviour {

    Text textComp;
    public GameObject player;
    public GameSpeedManager gsm;
    void Awake() {
        textComp = this.gameObject.GetComponent<Text>();
        
    }
	// Use this for initialization
	void Start () {
		if (player == null)
			player = GameObject.FindGameObjectWithTag ("Player");
	}
	
	// Update is called once per frame
	void Update () {
        textComp.text = "player position X : " + player.transform.position.x.ToString("0.00") + " - Game Speed : " + gsm.GameSpeed;
	}
}
