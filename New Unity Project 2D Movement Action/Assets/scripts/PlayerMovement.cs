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
    private int posPlayer = 0;
    private KeyCode[] keyset;
    private int playerNo;
    private int atkType = 0;
	// Use this for initialization

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
            keyset = new KeyCode[7] { KeyCode.UpArrow, KeyCode.DownArrow, KeyCode.LeftArrow, KeyCode.RightArrow, KeyCode.RightControl, KeyCode.RightShift, KeyCode.End};
        }
        else
        {
            curr_x = max_x-1; curr_y = max_y-1;
            keyset = new KeyCode[7] { KeyCode.W, KeyCode.S, KeyCode.A, KeyCode.D, KeyCode.Space, KeyCode.Q, KeyCode.E};
        }

        anim = GetComponent<Animator>();
        anim.SetFloat("x", -1);
    }
	
	// Update is called once per frame
	void Update () {
        float input_x = Input.GetAxisRaw("Horizontal");
        float input_y = Input.GetAxisRaw("Vertical");

        //bool isWalking = (Mathf.Abs(input_x) + Mathf.Abs(input_y)) > 0;
        bool isWalking = Input.GetKeyDown(keyset[0]) || Input.GetKeyDown(keyset[1]) || 
                         Input.GetKeyDown(keyset[2]) || Input.GetKeyDown(keyset[3]);
        anim.SetBool("isWalking", isWalking);

        bool isAttacking = Input.GetKeyDown(keyset[4]) || Input.GetKeyDown(keyset[5]) || Input.GetKeyDown(keyset[6]);
        anim.SetBool("isAttacking", isAttacking);
        /*if (isWalking)
        {
            anim.SetFloat("x", input_x);
            anim.SetFloat("y", input_y);

            transform.position += new Vector3(input_x, input_y, 0).normalized * 1;
        }*/

        if (Input.GetKeyDown(keyset[0]))
        {
            posPlayer = 1; // u
            input_y = 1;
            input_x = 0;
        }
        else if (Input.GetKeyDown(keyset[1]))
        {
            posPlayer = 2; //d
            input_y = -1;
            input_x = 0;

        }
        else if (Input.GetKeyDown(keyset[2]))
        {
            posPlayer = 3; //l
            input_x = -1;
            input_y = 0;
        } 
        else if (Input.GetKeyDown(keyset[3]))
        {
            posPlayer = 4; //r
            input_x = 1;
            input_y = 0;
        }
        // for type of attacks
        else if (Input.GetKeyDown(keyset[4]))
        {
            atkType = 0;
        }
        else if (Input.GetKeyDown(keyset[5]))
        {
            atkType = 1;
        }
        else if (Input.GetKeyDown(keyset[6]))
        {
            atkType = 2;
        }

        if (isWalking && isAttacking) //if sabay yung pindot, move muna bago kulay ng tile
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
            Debug.Log("Trigger attack " + (playerNo-1) + " pos " + posPlayer + " atkType " + atkType);
            if (playerNo == 1)
                tile_script.setTilesColor(posPlayer, (int)curr_x, (int)curr_y, 0, atkType);
            else
                tile_script.setTilesColor(posPlayer, (int)curr_x, (int)curr_y, 1, atkType);
        }
        else
        {
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
                Debug.Log("Trigger attack " + (playerNo - 1) + " pos " + posPlayer + " atkType " + atkType);
                if (playerNo == 1)
                    tile_script.setTilesColor(posPlayer, (int)curr_x, (int)curr_y, 0, atkType);
                else
                    tile_script.setTilesColor(posPlayer, (int)curr_x, (int)curr_y, 1, atkType);
            }
        }

        isAttacking = false;
        isWalking = false;
    }



}
