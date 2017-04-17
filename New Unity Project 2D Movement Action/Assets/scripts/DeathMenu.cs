using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeathMenu : MonoBehaviour {

    // Use this for initialization
    public Text winnerText;
	//public Button playAgainButton;
	//public Button mainMenuButton;
    void Start () {
		
		/*winnerText.enabled = false;
		playAgainButton.gameObject.SetActive(false);
		mainMenuButton.gameObject.SetActive(false);*/
        gameObject.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void showEndMenu(string msg)
    {
        /*winnerText.enabled = true;
		playAgainButton.gameObject.SetActive(true);
		mainMenuButton.gameObject.SetActive(true);*/
		gameObject.SetActive(true);
        winnerText.text = "Player "+msg+" won!";
		Debug.Log("FINISH NA DAPAT!");
    }
}
