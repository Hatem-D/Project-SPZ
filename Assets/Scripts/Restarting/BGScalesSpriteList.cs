using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class BGScalesSpriteList {

    List<Sprite>  introSprites, middleSprites, endingSprites, currentList;
    int indexer;
    int scalesPerScreen = 4;
    bool inSecondPass = false;
    
    public delegate Sprite nextSpriteFuntionPointer();
    public nextSpriteFuntionPointer GetNextSprite;

    GlobalVars gameVars;

    public int Indexer
    {
        get
        {
            if (indexer < currentList.Count-1) {
                if ( ( (indexer+1) % scalesPerScreen == 0) )
                {
                    if (!inSecondPass) { 
                        indexer -= scalesPerScreen;
                        inSecondPass = true;
                    }else { inSecondPass = false; }
                }
                indexer++;
                //Debug.Log("less than count ("+currentList.Count+") Returning indexer : " + (indexer)+" In second pass : "+inSecondPass);                
            }
            else
            {
                if (SetNextList()){
                    indexer = 0;
                }
                else {
                    indexer = indexer - scalesPerScreen + 1;
                }
                Debug.Log("Reseting Returning indexer : " + (indexer));
            }
            return (indexer);
        }        
    }

  
	
	public BGScalesSpriteList(GlobalVars gV)
    {
        indexer = 0;
        SetDefaultMode(gV);
        GetNextSprite = ReturnNextSprite;
    }

    void SetDefaultMode(GlobalVars gV) {
        if (gameVars == null)
        {        
            gameVars = gV;          
        }
        currentList = gameVars.defaultSpriteList;                    
    }

    enum listPointer { intro, mid, end}
    listPointer point = listPointer.intro;

    bool SetNextList()
    {
        switch (point)
        {
            case (listPointer.intro):
                if (middleSprites == null || middleSprites.Count < 1) { return false; }
                currentList = middleSprites;
                return true;

            case (listPointer.mid):
                if (endingSprites == null || endingSprites.Count < 1) { return false; }
                currentList = endingSprites;
                return true;

            case (listPointer.end):
                return false;
            default:
                return true;
                
        }
    }

    public BGScalesSpriteList(GlobalVars gV,List<Sprite> intro = null, List<Sprite> middle = null, List<Sprite> ending = null)
    {
        bool defaultMode = true;
        indexer = 0;
        
        if (ending != null && ending.Count > 0) {
            endingSprites = new List<Sprite>(ending);
            defaultMode = false;
            currentList = endingSprites;
            point = listPointer.end;
        }

        if (middle != null && middle.Count > 0) {
            middleSprites = new List<Sprite>(middle);
            defaultMode = false;
            currentList = middleSprites;
            point = listPointer.mid;
        }

        if (intro != null && intro.Count > 0) {
            introSprites = new List<Sprite>(intro);
            defaultMode = false;
            currentList = introSprites;
            point = listPointer.intro;
        }

        if (defaultMode) {
            SetDefaultMode(gV);           
        }        
        GetNextSprite = ReturnNextSprite;
    }
    

    Sprite ReturnNextSprite() {
        if (currentList == null) Debug.Log("current list noulle");
        return (currentList[Indexer]);
    }
    
}
