using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {
    private TileManager tile_script;
	private HUD hud_script;
	
    private GameObject tile_script_go;
	private GameObject hud_go;
	
    Animator anim;
	
    private float move_range = 2.0f;
    private float max_x, max_y;
    public float curr_x, curr_y;
    //private string posPlayer = "x";
    private int posPlayer = 0;
    private int atkType = 0;
    private KeyCode[] keyset;
    private int playerNo;
	private int characterId;


	double bpm = 60;
	double nextTick = 0.0F; // The next tick in dspTime
	bool ticked = false;
	float ctr = 0.0f;
	float interval = 0.0f;
	bool canMove;
	bool stunned = false;
	float stunDuration, stunCtr = 0;

    bool poisoned = false;
    float poisonedDuration, poisonedTrack;

    bool bleeding = false;
    float bleedDuration;

    bool disabled = false;
    float disabledDuration;

    public int[] skillsCooldown;
    float skill1CooldownTracker=0, skill2CooldownTracker=0;

    public float hp = 100f;

    public void setCharacter(int characterId)
    {	
		this.characterId = characterId;
        if(characterId == 0)
        {
            skillsCooldown = new int[2] { 7, 4 };
        }
        else
        {
            skillsCooldown = new int[2] { 4, 10 };
        }
    }

    public void setPlayer(int number)
    {
        playerNo = number;
    }

    public int getPlayer()
    {
        return playerNo;
    }

    void Start () {
        tile_script_go = GameObject.FindGameObjectWithTag("TileManager");
        tile_script = tile_script_go.GetComponent("TileManager") as TileManager;
		
		hud_script = GameObject.FindGameObjectWithTag("HUDManager").GetComponent("HUD") as HUD;
        hud_go = GameObject.FindGameObjectWithTag("HUD");
		
		//hud_script.UpdateHealthUI(hp, playerNo);
		max_x = tile_script.max_x;
        max_y = tile_script.max_y;

		
        if (playerNo == 1)
        {
            curr_x = 0; curr_y = 0;
            keyset = new KeyCode[7] { KeyCode.UpArrow, KeyCode.DownArrow, KeyCode.LeftArrow, KeyCode.RightArrow, KeyCode.RightControl, KeyCode.RightShift, KeyCode.End};
        }
        else
        {
            curr_x = max_x-1; curr_y = max_y-1;
            keyset = new KeyCode[7] { KeyCode.W, KeyCode.S, KeyCode.A, KeyCode.D, KeyCode.Space, KeyCode.Q, KeyCode.E };
        }

        anim = GetComponent<Animator>();
        anim.SetFloat("x", -1);

		double startTick = AudioSettings.dspTime;
		nextTick = startTick + (60.0 / bpm);
    }
	
	// Update is called once per frame
	void Update () {
		ctr+=Time.deltaTime;
		double timePerTick = 60.0f / bpm;
		double dspTime = AudioSettings.dspTime;
		while ( dspTime >= nextTick ) {
			ticked = false;
			nextTick += timePerTick;
		}
        //Debug.Log(nextTick+" BEAT1 " +(interval));
		canMove = (ctr >= interval * 0.0);

        if(skill1CooldownTracker > 0)
        {
			hud_script.UpdateSkillCooldown(playerNo, characterId, 1, (int) skill1CooldownTracker);
            skill1CooldownTracker -= Time.deltaTime;
        }

        if (skill2CooldownTracker > 0)
        {
			hud_script.UpdateSkillCooldown(playerNo, characterId, 2, (int)skill2CooldownTracker);
            skill2CooldownTracker -= Time.deltaTime;
        }

        if (stunned) {
			stunDuration = interval * 3;
			if (stunCtr < stunDuration) {
				stunCtr += Time.deltaTime;
			} else {
				stunned = false;
				stunCtr = 0;
			}
		}

        if (bleeding)
        {
            if (bleedDuration > 0)
            {
                bleedDuration -= Time.deltaTime;
            }
            else
            {
                bleeding = false;
                Debug.Log("BLEEDING STOPPED");
            }
        }

        if (disabled)
        {
            if (disabledDuration > 0)
            {
                disabledDuration -= Time.deltaTime;
            }
            else
            {
                disabled = false;
                Debug.Log("DISABLED STOPPED");
            }
        }

        if (poisoned)
        {
            if (poisonedDuration > 0) 
            {
                poisonedTrack += Time.deltaTime;
                if(poisonedTrack >= interval)
                {
                    Debug.Log("HP DEDUCTED FROM POISON!");
                    hp -= 5;
                    Debug.Log("Received 5 damage! Remaining HP: " + hp);
                    poisonedTrack = 0;
                }
                poisonedDuration -= Time.deltaTime;
                //if(poisonedCtr)
            }
            else
            {
                poisoned = false;
                Debug.Log("disabled STOPPED");
            }
        }

        float input_x = Input.GetAxisRaw("Horizontal");
        float input_y = Input.GetAxisRaw("Vertical");
        bool isWalking = false, isAttacking = false;

        //bool isWalking = (Mathf.Abs(input_x) + Mathf.Abs(input_y)) > 0;
        if (!disabled)
        {
            isWalking = Input.GetKeyDown(keyset[0]) || Input.GetKeyDown(keyset[1]) ||
                             Input.GetKeyDown(keyset[2]) || Input.GetKeyDown(keyset[3]);
            anim.SetBool("isWalking", isWalking);

            isAttacking = Input.GetKeyDown(keyset[4]);
            if(Input.GetKeyDown(keyset[5]))
                if (skill1CooldownTracker <= 0)
                    isAttacking = true;
            if (Input.GetKeyDown(keyset[6]))
                if (skill2CooldownTracker <= 0)
                    isAttacking = true;

            anim.SetBool("isAttacking", isAttacking);
        }
		if (isWalking || isAttacking) {
			if (canMove) {
				if (Input.GetKeyDown(keyset[0]))
				{
                    //posPlayer = "u";	
                    posPlayer = 1;
                    input_y = 1;
					input_x = 0;
				}
				else if (Input.GetKeyDown(keyset[1]))
				{
                    //posPlayer = "d";
                    posPlayer = 2;
                    input_y = -1;
					input_x = 0;

				}
				else if (Input.GetKeyDown(keyset[2]))
				{
                    //posPlayer = "l";
                    posPlayer = 3;
                    input_x = -1;
					input_y = 0;
				} 
				else if (Input.GetKeyDown(keyset[3]))
				{
                    //posPlayer = "r";
                    posPlayer = 4;
                    input_x = 1;
					input_y = 0;
				}
                else if (Input.GetKeyDown(keyset[4]))
                {
                    atkType = 0;
                }
                else if (Input.GetKeyDown(keyset[5]))
                {
                    atkType = 1;
                    skill1CooldownTracker = interval * skillsCooldown[0];

                }
                else if (Input.GetKeyDown(keyset[6]))
                {
                    atkType = 2;
                    skill2CooldownTracker = interval * skillsCooldown[1];
                }


                if (isWalking)
				{
                    if (bleeding)
                    {
                        Debug.Log("Player"+playerNo+" IS BLEEDING");
                        hp -= 10;
                        Debug.Log("Received 10 damage! Remaining HP: " + hp);
                    }
                    if ((curr_x + input_x >= 0 && curr_x + input_x < max_x) && (curr_y + input_y >= 0 && curr_y + input_y < max_y))
					{
//						Debug.Log("curr_x:" + curr_x + " curr_y:" + curr_y);
						curr_x += input_x;
						curr_y += input_y;

						anim.SetFloat("x", input_x);
						anim.SetFloat("y", input_y);
						transform.position = new Vector2(transform.position.x + (input_x * move_range), transform.position.y + (input_y * move_range));
					}
				}
				if (isAttacking)
				{
					Debug.Log("Trigger attack");
					if (playerNo == 1)
                        tile_script.setTilesColor(posPlayer, (int)curr_x, (int)curr_y, 0, atkType);
                    //tile_script.setTilesColor(posPlayer, (int)curr_x, (int)curr_y, "p1");
                    else
                        tile_script.setTilesColor(posPlayer, (int)curr_x, (int)curr_y, 1, atkType);
                    //tile_script.setTilesColor(posPlayer, (int)curr_x, (int)curr_y, "p2"); 
                }
			} else {
				Debug.Log ("Miss! " + ctr);
			}
		}
		
		hud_script.UpdateHealthUI(hp, playerNo, characterId);
	
		//isAttacking = false;
		//isWalking = false;
    }


	void LateUpdate() {
		if ( !ticked && nextTick >= AudioSettings.dspTime ) {
			if (interval == 0.0f)
				interval = ctr;
			else
				interval = (interval + ctr) / 2;
			ctr = 0;
			ticked = true;
		}
	}
	 
	public void InflictDamage(float dmg){
		if (!stunned) {
			hp -= dmg;
			Debug.Log ("Received " + dmg + " damage! Remaining HP: "+hp);
			stunned = true;
		}
	}

    public void MakeBleed()
    {
        if (!bleeding)
        {
            bleeding = true;
            Debug.Log("Player" + playerNo + " STARTED BLEEDING");
        }
    }

    public void harm(int characterId, int atkType)
    {
        if (characterId == 0)
        {
            if (atkType == 0)
            {
                Debug.Log("Player" + playerNo + " was basic attacked by sustainy!");
                hp -= 20;
                Debug.Log("Received 20 damage! Remaining HP: " + hp);
            }
            else if (atkType == 1)
            {
                Debug.Log("Player" + playerNo + " was SS1ed by sustainy!");
                poisoned = true;
                poisonedDuration = interval * 3;
                poisonedTrack = 0;
            }
            else if (atkType == 2)
            {
                disabled = true;
                disabledDuration = interval * 3;
                Debug.Log("Player" + playerNo + " was SS2ed by sustainy!");
            }
        }
        else if(characterId == 1)
        {
            if (atkType == 0)
            {
                Debug.Log("Player" + playerNo + " was basic attacked by trebleine!");
                hp -= 6;
                Debug.Log("Received 6 damage! Remaining HP: " + hp);
            }
            else if (atkType == 1)
            {
                bleeding = true;
                bleedDuration = interval * 3;
                Debug.Log("Player" + playerNo + " was SS1ed by trebleine!");
                Debug.Log("Player" + playerNo + " STARTED BLEEDING");
            }
            else if (atkType == 2)
            {
                Debug.Log("Player" + playerNo + " was SS2ed by trebleine!");
                hp -= 30;
                Debug.Log("Received 30 damage! Remaining HP: " + hp);
            }
        }
		
		
    }
}
