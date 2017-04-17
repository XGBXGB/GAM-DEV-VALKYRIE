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
	public Image beater;
	
	public Text[] pScoreLabel;
	
	public Text[] skill1CooldownLabel;
	public Text[] skill2CooldownLabel;
	
	public Text[] missLabel;
	
	
	private int healthIndex;
	private int playerIndex;
	private int characterId;
	private int attackType;
	
	Camera camera;
	private Color color1 = Color.black;
    private Color color2 = Color.white;
	private float beat;
	private float elapsedTime;
	
	
	/*private int[] skillcooldown;
	
	private int roundHp;
	
	
	public void Start(){
		skillcooldown = new int[2]{0,0};
		isStart = true;
		
		for(int i=0; i<2; i++){
			skill1CooldownUI[i].enabled = false;
			skill2CooldownUI[i].enabled = false;
		}
	}*/

	private int[,] skillcooldown;
    private int roundHp;
	
	
	public void animateMiss(int playerIndex, float x, float y){
		
		missLabel[playerIndex-1].GetComponent<RectTransform>().anchoredPosition = new Vector2(x,y);
		//Debug.Log("x:"+x+" y:"+y + " curr_x:"+ missLabel[playerIndex-1].transform.position.x + "  curr_y:"+missLabel[playerIndex-1].transform.position.y);
	}
	public void Start(){
		skillcooldown = new int[2, 2] { { 0, 0 }, { 0, 0 } };
		//camera = GetComponent<Camera>();
        //camera.clearFlags = CameraClearFlags.SolidColor;
		//camera.backgroundColor = color1;
    }

	public void setBeat(float beat){
		this.beat = beat;
		
		float scale = 1 + Mathf.Cos(Time.time * (110 / 60) * Mathf.PI * 2);
		beater.rectTransform.sizeDelta = new Vector2(scale*10, scale*10);
		//camera.backgroundColor = Color.Lerp(Color.white, Color.black, Mathf.PingPong(Time.time, beat));
		
		/*if(beater.Color == color1){
			beater.Color = color2;
			Debug.Log("original to white");
		}
			
		
		if(beater.Color == color2){
			beater.Color = color1;
			Debug.Log("white to original");
		}*/
			
		
		Debug.Log("change size:"+scale);
	}
	public void UpdateSkillCooldown(int playerIndex, int characterId, int attackType, int cooldown){
		
		
		this.playerIndex = playerIndex-1;
		this.characterId = characterId;

		this.attackType = attackType - 1;

		/*skillcooldown[this.attackType] = cooldown;
		
		if(this.attackType == 0){
			if(skillcooldown[this.attackType] == 0)
				skill1CooldownUI[this.playerIndex].enabled = false;
			else{
				skill1CooldownUI[this.playerIndex].enabled = true;
				skill1CooldownLabel[this.playerIndex].text = skillcooldown[this.attackType] +"";
			}
		}
		
	
		if(this.attackType == 1){
			if(skillcooldown[this.attackType] == 0)
				skill2CooldownUI[this.playerIndex].enabled = false;
			else{
				skill2CooldownUI[this.playerIndex].enabled = true;
				skill2CooldownLabel[this.playerIndex].text = skillcooldown[this.attackType] +"";
			}
		}*/

		Debug.Log("attackType:"+this.attackType);
        skillcooldown[this.playerIndex,this.attackType] = cooldown;


        //Debug.Log("skillcooldown:"+skillcooldown[this.attackType]);
		//Update();
		
		 for(int p=0; p< skillcooldown.GetLength(0); p++)
        {
            Debug.Log(skillcooldown[p, 0]+" "+p+" "+ skillcooldown[p, 1]);
            if (skill1CooldownUI[p].enabled == true)
            {
                skill1CooldownLabel[p].text = skillcooldown[p, 0] + "";
            }

            if (skill2CooldownUI[p].enabled == true)
            {
                skill2CooldownLabel[p].text = skillcooldown[p, 1] + "";
            }
        }
	}
	
	public void UpdateHealthUI(float hp,  int playerIndex, int characterId){
				
		roundHp = (int)Mathf.Round(hp);
		
		healthIndex = roundHp/10 + 1;
		if(roundHp%10 == 0)
			healthIndex = roundHp/10;

		if(roundHp < 0)
			roundHp = 0; 
		
		this.playerIndex = playerIndex-1;
		this.characterId = characterId;
		Update();
	}
	
	
	public void Update(){
		//Debug.Log("playerIndex:"+playerIndex+" healthIndex:"+healthIndex);

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
		
		

       
		
	}

    public void disableCooldown(int skillNo, int playerNo)
    {
        if(skillNo == 1)
        {
            skill1CooldownUI[playerNo - 1].enabled = false;
        }else
        {
            skill2CooldownUI[playerNo - 1].enabled = false;
        }
    }

    public void enableCooldown(int skillNo, int playerNo)
    {
        if (skillNo == 1)
        {
            skill1CooldownUI[playerNo - 1].enabled = true;
        }
        else
        {
            skill2CooldownUI[playerNo - 1].enabled = true;
        }
    }
}
