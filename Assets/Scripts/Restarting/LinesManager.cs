using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinesManager : MonoBehaviour {

    float defaultHighLane = 3.8f;
    float defaultMidLane = 0.0f;
    float defaultLowLane = -3.8f;

    public List<float> LinesY = null;
    public int startLane;

    // Use this for initialization
    void Start () {
	    if (LinesY == null || LinesY.Count == 0)
        {
            LinesY = new List<float> { defaultHighLane, defaultMidLane, defaultLowLane };
            startLane = 1;
        }
        sortLanes();
        /*int i = 0;
        foreach (float Y in LinesY)
        {
            Debug.Log("element " + i + " : " + Y);
            i++;
        }*/
    }

    public void sortLanes()
    {
        LinesY.Sort();
    }

	public float GetStartLane()
    {
        return (startLane);
    }

    float TrimPositionToLane(float position, float threshold)
    {
        foreach (float lane in LinesY)
        {
            if (Mathf.Abs(lane - position) < threshold)
                position = lane;
        }
        return position;
    }

    public float GetLowerLane(float currentPosition, float lineThickness)
    {
        currentPosition = TrimPositionToLane(currentPosition, lineThickness);
        Debug.Log("trimmed position : " + currentPosition);
        if (currentPosition <= LinesY[0]) return LinesY[0];
        return LinesY.FindLast(x => x < currentPosition);        
    }

    public float GetUpperLane(float currentPosition, float lineThickness)
    {
        currentPosition = TrimPositionToLane(currentPosition, lineThickness);
        Debug.Log("trimmed position : " + currentPosition);
        if (currentPosition >= LinesY[LinesY.Count - 1]) return (LinesY[LinesY.Count - 1]);        
        return LinesY.Find(x => x > currentPosition);
    }
}
