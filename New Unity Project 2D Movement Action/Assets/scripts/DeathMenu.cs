using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeathMenu : MonoBehaviour {

    // Use this for initialization
    public Text winnerText;
    void Start () {
        gameObject.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void showEndMenu(string msg)
    {
        gameObject.SetActive(true);
        winnerText.text = "Player "+msg+" won!";
    }
}
