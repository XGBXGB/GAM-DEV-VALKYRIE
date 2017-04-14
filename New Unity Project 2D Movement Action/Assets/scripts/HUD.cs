using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour {
	public Sprite[] pHealthSprites;
	public Sprite[] pCharacterIconSprites;

	public Sprite[] skill1Sprites;
	public Sprite[] skill2Sprites;
	
	public Image[] pHealthUI;
	public Image[] pIconUI;
	
	public Image[] skill1CooldownUI;
	public Image[] skill2CooldownUI;
	public Image[] pSkill1SelectedSprites;
	public Image[] pSkill2SelectedSprites;
	
	
	public Text[] pScoreLabel;
	
	public Text[] skill1CooldownLabel;
	public Text[] skill2CooldownLabel;
	
	
	private int healthIndex;
	private int playerIndex;
	private int characterId;
	private int attackType;
	private int[] skillcooldown;
	
	private int roundHp;
	private bool isHealth;
	
	public void Start(){
		skillcooldown = new int[2]{0,0};
		
	}
	
	public void UpdateSkillCooldown(int playerIndex, int characterId, int attackType, int cooldown){
		
		
		
		isHealth = false;
		this.playerIndex = playerIndex-1;
		this.characterId = characterId;
		this.attackType = attackType - 1;
		Debug.Log("attackType:"+this.attackType);
		skillcooldown[this.attackType] = cooldown;
		
		
		Debug.Log("skillcooldown:"+skillcooldown[this.attackType]);
		Update();
	}
	
	public void UpdateHealthUI(float hp,  int playerIndex, int characterId){
		
		isHealth = true;
		
		roundHp = (int)Mathf.Round(hp);
		
		healthIndex = roundHp/10 + 1;
		if(roundHp%10 == 0)
			healthIndex = roundHp/10;
	
		this.playerIndex = playerIndex-1;
		this.characterId = characterId;
		Update();
	}
	
	
	public void Update(){
		Debug.Log("playerIndex:"+playerIndex+" healthIndex:"+healthIndex);
		pHealthUI[playerIndex].sprite = pHealthSprites[healthIndex];
		pIconUI[playerIndex].sprite = pCharacterIconSprites[characterId];
		pScoreLabel[playerIndex].text = roundHp + "/100";
		
		if(characterId == 0){
			pSkill1SelectedSprites[playerIndex].sprite = skill1Sprites[0];
			pSkill2SelectedSprites[playerIndex].sprite = skill2Sprites[0];
		}else{
			pSkill1SelectedSprites[playerIndex].sprite = skill1Sprites[1];
			pSkill2SelectedSprites[playerIndex].sprite = skill2Sprites[1];
		}
		
		if(skillcooldown[attackType] == 0)
			skill1CooldownUI[playerIndex].enabled = false;
		else if(skillcooldown[attackType] != 0){
			Debug.Log("PlyerIndex: "+playerIndex+" hindi zero skill 1");
			skill1CooldownUI[playerIndex].enabled = true;
			skill1CooldownLabel[playerIndex].text = skillcooldown[attackType] +"";
		}
	
		
		if(skillcooldown[attackType] == 0)
			skill2CooldownUI[playerIndex].enabled = false;
		else if(skillcooldown[attackType] != 0){
			Debug.Log("PlyerIndex: "+playerIndex+" hindi zero skill 2");
			skill2CooldownUI[playerIndex].enabled = true;
			skill2CooldownLabel[playerIndex].text = skillcooldown[attackType] +"";
		}
		
		
		
	}
}
