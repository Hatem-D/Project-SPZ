using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GlobalVars : MonoBehaviour {

	int maxStepsAhead = 3, minStepsAhead = -3;
	public int playerStepsAhead = 0;
	public int PlayerStepsAhead{
		get{ 
			return playerStepsAhead;
		}
		set{ 
			if (value >= minStepsAhead && value <= maxStepsAhead) { 
				playerStepsAhead = value;
                //Debug.Log("New steps ahead : " + value);                
            }
            if (StepsChanged != null)
                StepsChanged(playerStepsAhead);
        }
	}
    public float defaultPursuitSpzTimeBetweenShots = 5.0f;
    public float defaultPursuitSpzPreboostTime = 3.0f;

    public delegate void OnStepsChangedFunctionPointer(int steps);
    public OnStepsChangedFunctionPointer StepsChanged;

	public Transform endOfTheLine;

    public List<Sprite> defaultSpriteList;

	public Vector2 screenSize = new Vector2 (0, 0);	
	
	public float roof = 3.45f;
	public float floor = -3.45f;
    public float leftWall;
    public float rightWall;
    GlobalVarsStore save = new GlobalVarsStore();

	public float playerDefaultX = -5.5f;
    public float playerIntroPosition = -11.32f;
    public float playerRestingPosition = -5.5f;
    public float playerBoostPosition = 0.0f;
	public float playerPreBoostPosition = -7.24f;

	public float macrophageFrontRoof = 3.9f;
    public float macrophageMidRoof = 4.1f;
    public float macrophageBackRoof = 4.3f;

	public float deadZone = -200.0f;

    public float linearTranslationOffset = 5.0f;

    public LinesManager linesHolder;

	public float PlayerDefaultX
    {
		get
		{
			return playerDefaultX;
		}
		set
		{
			//Debug.Log ("Someone set playerposition at : " + value);
			playerDefaultX = value;
		}
	}

    public float PlayerIntroPosition
    {
        get
        {
            return playerIntroPosition;
        }
    }

    public float PlayerRestingPosition
    {
        get
        {
            return playerRestingPosition;
        }
    }

    public float PlayerBoostPosition
    {
        get
        {
            return playerBoostPosition;
        }
    }

    void Awake(){
		screenSize.x = Screen.width;
		screenSize.y = Screen.height;
        //Debug.Log("screensize "+screenSize);
        //Debug.Log("Screensize transformé " + Camera.main.ScreenToWorldPoint(screenSize));

        /*roof = 0.67f * Camera.main.ScreenToWorldPoint(screenSize).y;
        floor = -roof;*/
        rightWall = Camera.main.ScreenToWorldPoint(screenSize).x;
        leftWall = -rightWall;

		playerDefaultX = PlayerRestingPosition;

        //QualitySettings.vSyncCount = 0;
		Application.targetFrameRate = 30;
        if (linesHolder == null)
        {
            linesHolder = transform.GetComponentInChildren<LinesManager>();
        }
    }

    public bool IsInGameScreen(Vector3 point)
    {
        if (point.y < floor) return false;
        if (point.y > roof) return false;
        if (point.x > rightWall) return false;
        if (point.x < leftWall) return false;
        return true;
    }

	void SaveDefaultParams(){
        save.roof = roof;
		save.floor = floor;
	}

	void LoadDefaultParams(){
        roof = save.roof;
		floor = save.floor;
	}

}
