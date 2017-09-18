using System;
using UnityEngine;
using System.Collections;

public static class StateTools{

	public static string ToString(GameStates state){
		string result="";
		switch (state) {
		    case GameStates.Intro :
			    result = "Intro";
			    break;
            case GameStates.Menu:
                result = "Menu";
                break;
            case GameStates.Help:
                result = "Help";
                break;
            case GameStates.Beginning:
                result = "Beginning";
                break;
            case GameStates.Game:
                result = "Game";
                break;
            case GameStates.Pause:
                result = "Pause";
                break;
            case GameStates.ExitPause:
                result = "ExitPause";
                break;
            case GameStates.Reload:
                result = "Reload";
                break;
            case GameStates.Restart:
                result = "Restart";
                break;
            case GameStates.GameOver:
                result = "GameOver";
                break;

            default :
			result = "Intro";
			break;
		}
		return result;
	}
}