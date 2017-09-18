using System;
using UnityEngine;
using System.Collections;

public class StateChangeEventArgs : EventArgs {

    private GameStates oldState;
	private GameStates newState;

	public GameStates OldState
    {
        get
        {
            return oldState;
        }

        set
        {
            oldState = value;
        }
    }

	public GameStates NewState
    {
        get
        {
            return newState;
        }

        set
        {
            newState = value;
        }
    }

	public StateChangeEventArgs(GameStates old, GameStates newS) { OldState = old;  NewState = newS; }
}
