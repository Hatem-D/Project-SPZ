using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinesManager : MonoBehaviour {

    float defaultHighLane = 3.8f;
    float defaultMidLane = 0.0f;
    float defaultLowLane = -3.8f;

    public List<float> LinesY = null;

    // Use this for initialization
    void Start () {
	    if (LinesY == null || LinesY.Count == 0)
        {
            LinesY = new List<float> { defaultHighLane, defaultMidLane, defaultLowLane };
        }
        int i = 0;
        foreach(float lane in LinesY)
        {
            Debug.Log("LinesY[" + i + "] = " + lane);
            i++;
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
