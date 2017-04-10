using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CharacterSelect : MonoBehaviour {

    private GameObject[] characterList;
    private string[] characterNames;
    private int index;
    public Text nameLabel;
	// Use this for initialization
	void Start () {
        characterList = new GameObject[transform.childCount];
        characterNames = new string[transform.childCount];
        for (int i=0; i<transform.childCount; i++)
        {
            characterList[i] = transform.GetChild(i).gameObject;
            characterNames[i] = transform.GetChild(i).name.Split('_')[0];
        }

        foreach (GameObject go in characterList)
            go.SetActive(false);

        if (characterList[0])
        {
            characterList[0].SetActive(true);
            index = 0;
            nameLabel.text = characterNames[index];
        }
		
	}

    public void switchPlayer(bool toLeft)
    {
        characterList[index].SetActive(false);
        if (toLeft)
        {
            index--;
            if (index < 0)
                index = characterList.Length - 1;
        }
        else
        {
            index++;
            if (index >= characterList.Length)
                index = 0;
        }
        characterList[index].SetActive(true);
        nameLabel.text = characterNames[index];
    }

    public void select()
    {
        int check = PlayerPrefs.GetInt("Player1", -1);
        Debug.Log("check: " + check);
        if (check == -1)
        {
            PlayerPrefs.SetInt("Player1", index);
            characterList[index].SetActive(false);
            index = 0;
            characterList[index].SetActive(true);
            nameLabel.text = characterNames[index];
            Debug.Log("check1");
        }
        else
        {
            PlayerPrefs.SetInt("Player2", index);
            Debug.Log("check2");
            SceneManager.LoadScene("Game");
        }
    }
	// Update is called once per frame
	void Update () {
		
	}
}
