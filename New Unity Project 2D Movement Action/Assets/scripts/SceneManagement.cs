using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagement : MonoBehaviour {

	public void ToCharacterSelect(){
		SceneManager.LoadScene("Character_Select");
	}
	
	public void ToControlGuide(){
		SceneManager.LoadScene("Control_Guide");
	}
	
	public void ToMainMenu(){
		SceneManager.LoadScene("Main_Menu");
	}
}
