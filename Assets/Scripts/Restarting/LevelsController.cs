using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelsController : MonoBehaviour, IUsesGameStates
{

    public GameObject firstBlock;
    public List <GameObject> blocksPrefabs = null;
    
    List<BlockController> blocksControllers;
	BlockController nextBlock = null;
    
    GameStatesController gameStates;
	//public MacrophageManager macrosMngr;
    //public BackgroundScrolling scalesLocomotive;
    GlobalVars gameVars;
    GameSpeedManager speedManager;

    System.Random rSeed = new System.Random();

    public void RegisterToGameStateChangeEvents() {
        gameStates.ChangeState += OnGameStateChange;
    }
    public void OnGameStateChange(object sender, StateChangeEventArgs e) {
        Invoke(StateTools.ToString(e.NewState), 0.0f);
    }

    public delegate void stateDependantFunctionPointer();
    public stateDependantFunctionPointer doStateStuff;
    public stateDependantFunctionPointer pauseStateStore;

    void InstantiateAllBlockPrefabs()//reference will be kept in blocksControllers list
    {
        if (blocksControllers == null)
        {
            blocksControllers = new List<BlockController>();
            foreach (GameObject block in blocksPrefabs)
            {
                GameObject temp = GameObject.Instantiate(block);
                temp.GetComponent<BlockController>().myLvlCtrl = this;
                blocksControllers.Add(temp.GetComponent<BlockController>());
            }
        }
    }
    void InstantiateBlockPrefabs(int lvl = 0)//instantiate Blocks of a lvl - reference will be kept in blocksControllers list
    {
        if (blocksControllers == null)            
        {
            blocksControllers = new List<BlockController>();
            foreach (GameObject block in blocksPrefabs)
            {
                if (block.GetComponent<BlockController>().difficultyLevel == lvl)
                {
                    GameObject temp = GameObject.Instantiate(block);
                    temp.GetComponent<BlockController>().myLvlCtrl = this;
                    blocksControllers.Add(temp.GetComponent<BlockController>());
                }                
            }
        }
    }

    

    void Awake()
    {
        GameObject gameManager = GameObject.FindGameObjectWithTag(GlobalTags.GameManager);
        gameVars = gameManager.GetComponent<GlobalVars>();
        gameStates = gameManager.GetComponent<GameStatesController>();
        speedManager = gameManager.GetComponent<GameSpeedManager>();
		//macrosMngr = GameObject.FindObjectOfType<MacrophageManager>();
        RegisterToGameStateChangeEvents();
    }
    // Use this for initialization
    void Start () {
        if (firstBlock == null || !firstBlock.activeSelf) { Debug.Log("Warning First block not defined or inactive"); }

	}
	void PopulateBlocksControllerList()
    {
        if (blocksPrefabs != null)
        {
            InstantiateBlockPrefabs();
        }
        else Debug.Log("no blocks prefabs");
    }

    void InitBeginningBlock()
    {
        blocksControllers[0].CurrentBlockState = BlockController.blocksState.Approaching;
		nextBlock = blocksControllers [1];
		if (blocksControllers.Count > 1) {
			for (int i = 2; i < blocksControllers.Count; i++) {
				blocksControllers [i].gameObject.SetActive (false);
			}
		}
        if (firstBlock != null) { doStateStuff += TranslateFirstBlock; }
    }

    void TranslateFirstBlock()
    {
        if (firstBlock.transform.position.x > -80.0f )
        firstBlock.transform.position = new Vector3(Mathf.Lerp(firstBlock.transform.position.x, firstBlock.transform.position.x - gameVars.linearTranslationOffset, speedManager.GameSpeed * Time.deltaTime),
                                                        firstBlock.transform.position.y, firstBlock.transform.position.z);
        else
        {
            firstBlock.SetActive(false);
            doStateStuff -= TranslateFirstBlock;
        }
    }

    public void BlockStateChanged(BlockController bc)
    {
        
       switch (bc.CurrentBlockState)
        {
            case BlockController.blocksState.Ingame:                
                BlockController tempBlock = GetNextBlock(bc);
                if (tempBlock == null)
                {
                    Debug.Log("Warning No next block found !");                   
                }
                else
                {
                    //Debug.Log("next block found : "+tempBlock.gameObject.GetHashCode());
                    tempBlock.CurrentBlockState = BlockController.blocksState.Approaching;
                }
            break;
		case BlockController.blocksState.Waiting:
			//Debug.Log (bc.gameObject.GetHashCode () + " " + bc.CurrentBlockState);
			bc.gameObject.SetActive (false);
	        break;
            default:break;          
        }
        //Debug.Log(bc.gameObject.GetHashCode() + " " + bc.CurrentBlockState);
    }

    BlockController GetRandomWaitingBlock()
    {
        List<BlockController> waitingList = blocksControllers.FindAll(b => b.CurrentBlockState == BlockController.blocksState.Waiting);
        if (waitingList == null || waitingList.Count == 0) { Debug.Log("waiting list problem : " + waitingList); return null; }

        int randBlock = rSeed.Next(waitingList.Count);
        return waitingList[randBlock];
        
    }
    BlockController GetNextBlock (BlockController bc)
    {
        BlockController tempBlock = null;
		tempBlock = nextBlock;
		do {
			nextBlock = GetRandomWaitingBlock ();
		} while (tempBlock.gameObject.GetHashCode() == nextBlock.gameObject.GetHashCode());
		nextBlock.gameObject.SetActive (true);
        /*if (macrosMngr != null){
            macrosMngr.NextBlockDefined(nextBlock);//generate macrophages
        }
        else Debug.Log("macro manager not defined");

        if (scalesLocomotive != null) {
            BGScalesSpriteList nextSpriteList = new BGScalesSpriteList(gameVars, nextBlock.introSprites, nextBlock.middleSprites, nextBlock.endingSprites);
            scalesLocomotive.NextBlockDefined(nextSpriteList, nextBlock.GetHashCode());
        }else Debug.Log("BG Locomotive not defined");*/

        return tempBlock;
    }

    // Update is called once per frame
    void FixedUpdate () {
	    if (doStateStuff != null)
        {
            doStateStuff();
        }
	}

    public void Intro() { PopulateBlocksControllerList(); }
    public void Menu() { }
    public void Help() { }
    public void Beginning()
    {
        
        InitBeginningBlock();
    }
    public void Game()
    {
        
    }
    public void Pause() {
          
    }
    public void ExitPause() {
        
    }
    public void Reload() { }
    public void Restart() { }
    public void GameOver() { }
}
