using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {
    private TileManager tile_script;
    private GameObject tile_script_go;
    Animator anim;
    private float move_range = 2.0f;
    private float max_x, max_y;
    private float curr_x, curr_y;
    private string posPlayer = "x";
    private KeyCode[] keyset;
    private int playerNo;

	double bpm = 100;
	double nextTick = 0.0F; // The next tick in dspTime
	bool ticked = false;
	float ctr = 0.0f;
	float interval = 0.0f;
	bool canMove;

    public void setPlayer(int number)
    {
        playerNo = number;
    }

	void Start () {
        tile_script_go = GameObject.FindGameObjectWithTag("TileManager");
        tile_script = tile_script_go.GetComponent("TileManager") as TileManager;
        max_x = tile_script.max_x;
        max_y = tile_script.max_y;


        if (playerNo == 1)
        {
            curr_x = 0; curr_y = 0;
            keyset = new KeyCode[5] { KeyCode.UpArrow, KeyCode.DownArrow, KeyCode.LeftArrow, KeyCode.RightArrow, KeyCode.RightControl };
        }
        else
        {
            curr_x = max_x-1; curr_y = max_y-1;
            keyset = new KeyCode[5] { KeyCode.W, KeyCode.S, KeyCode.A, KeyCode.D, KeyCode.Space };
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
		canMove = (ctr >= interval * 0.5);

        float input_x = Input.GetAxisRaw("Horizontal");
        float input_y = Input.GetAxisRaw("Vertical");

        //bool isWalking = (Mathf.Abs(input_x) + Mathf.Abs(input_y)) > 0;
        bool isWalking = Input.GetKeyDown(keyset[0]) || Input.GetKeyDown(keyset[1]) || 
                         Input.GetKeyDown(keyset[2]) || Input.GetKeyDown(keyset[3]);
        anim.SetBool("isWalking", isWalking);

        bool isAttacking = Input.GetKeyDown(keyset[4]);
        anim.SetBool("isAttacking", isAttacking);
        /*if (isWalking)
        {
            anim.SetFloat("x", input_x);
            anim.SetFloat("y", input_y);

            transform.position += new Vector3(input_x, input_y, 0).normalized * 1;
        }*/
		if (isWalking || isAttacking) {
			if (canMove) {
				if (Input.GetKeyDown(keyset[0]))
				{
					posPlayer = "u";	
					input_y = 1;
					input_x = 0;
				}
				else if (Input.GetKeyDown(keyset[1]))
				{
					posPlayer = "d";
					input_y = -1;
					input_x = 0;

				}
				else if (Input.GetKeyDown(keyset[2]))
				{
					posPlayer = "l";
					input_x = -1;
					input_y = 0;
				} 
				else if (Input.GetKeyDown(keyset[3]))
				{
					posPlayer = "r";
					input_x = 1;
					input_y = 0;
				}


				if (isWalking)
				{
					if ((curr_x + input_x >= 0 && curr_x + input_x < max_x) && (curr_y + input_y >= 0 && curr_y + input_y < max_y))
					{
						Debug.Log("curr_x:" + curr_x + " curr_y:" + curr_y);
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
						tile_script.setTilesColor(posPlayer, (int)curr_x, (int)curr_y, "p1");
					else
						tile_script.setTilesColor(posPlayer, (int)curr_x, (int)curr_y, "p2"); 
				}
			} else {
				Debug.Log ("Miss! " + ctr);
			}
		}
		isAttacking = false;
		isWalking = false;
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
}
