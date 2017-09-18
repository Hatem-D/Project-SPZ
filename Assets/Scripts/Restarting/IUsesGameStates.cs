using System;


public interface IUsesGameStates  {

    
    void RegisterToGameStateChangeEvents(); //In this method register the OnGameStateMethod
	void OnGameStateChange(object sender, StateChangeEventArgs e);//OnGameStateMethod to be registered
    void Intro();//init delegate for intro state
	void Menu(); //init delegate for Menu state
    void Help(); //....
	void Beginning();
	void Game();
	void Pause();
	void ExitPause();
	void Reload();
	void Restart();
	void GameOver();//....

}
/*
    
    public void RegisterToGameStateChangeEvents(){ }
	public void OnGameStateChange(object sender, StateChangeEventArgs e){ }
    public void Intro(){Debug.Log ("It's Alive Sigmund");}
	public void Menu(){}
	public void Help(){}
	public void Beginning(){}
	public void Game(){}
	public void Pause(){}
	public void ExitPause(){}
	public void Reload(){}
	public void Restart(){}
	public void GameOver(){}
*/
