using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BlockController : MonoBehaviour {

    public int myHashCode;
    public int difficultyLevel = 0;
    public LevelsController myLvlCtrl;

    /*public int minMacrophages = 1;
	public int maxMacrophages = 10;
	public float leftXLimit = 10.25f;
	public float rightXLimit = 29.0f;*/

    public enum blocksState { Waiting, Approaching, Ingame }
    public blocksState currentBlockState = blocksState.Waiting;

    List<ObstacleController> myObstaclesList;

	/*public List<Sprite> introSprites;
    public List<Sprite> middleSprites;
    public List<Sprite> endingSprites;*/

    public blocksState CurrentBlockState
    {
        get
        {
            return currentBlockState;
        }

        set
        {
            currentBlockState = value;            
            switch (value)
            {
                case blocksState.Approaching:
                    SetObstaclesApproaching();
                    break;
                case blocksState.Waiting:
                    myLvlCtrl.BlockStateChanged(this);
                    break;
                case blocksState.Ingame:
                    myLvlCtrl.BlockStateChanged(this);
                    break;

                default:break;
            }            
        }

    }

    void SetObstaclesApproaching()
    {
        if (myObstaclesList != null)
        {
            foreach(ObstacleController oc in myObstaclesList)
            {
                oc.CurrentObstacleState = ObstacleController.obstacleState.Approaching;
            }
        }
    }

    void InitMyObstaclesList()//Get list of children obstacle controllers et set their obstacle controller ref to this
    {
        myObstaclesList = new List<ObstacleController>();
        ObstacleController[] tempObstacleList = gameObject.GetComponentsInChildren<ObstacleController>();
        
        foreach (ObstacleController oc in tempObstacleList)
        {
            oc.myBC = this;
            myObstaclesList.Add(oc);
        }
        if (myObstaclesList.Count == 0)
        {
            Debug.Log("Liste VIDE ! " + gameObject.name);
        }
    }
    
    public bool CheckObstaclesState(ObstacleController.obstacleState os)//returns true if all the obstacles are in os parameter state
    {
        foreach(ObstacleController oc in myObstaclesList)
        {
            if (oc.CurrentObstacleState != os) { return false; }
        }
        return true;
    }

    public bool CheckNotObstaclesState(ObstacleController.obstacleState os)//returns false if any of the obstacles are in os parameter state
    {
        foreach (ObstacleController oc in myObstaclesList)
        {
            if (oc.CurrentObstacleState == os) { return false; }
        }
        return true;
    }


    // Use this for initialization
    void Start () {
        myHashCode = gameObject.GetHashCode();
        if (myLvlCtrl == null) Debug.Log("Erreur ! level ctrl null ! " + gameObject.name);
        InitMyObstaclesList();
	}
	

}
