using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AndroidSpeedControls : MonoBehaviour {


    public Text textComp;
    public GameSpeedManager gsm;
    // Use this for initialization
    void Start () {
	    
    }
	
	// Update is called once per frame
	void Update () {
        textComp.text = gsm.GameSpeed.ToString("0.00");
    }

    public void UpSpeedPoint5() {
        gsm.gameSpeed += 0.5f;
    }
    public void DownSpeedPoint5() {
        gsm.gameSpeed -= 0.5f;
    }
}
