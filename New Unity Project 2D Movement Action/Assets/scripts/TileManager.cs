using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour {

    
    public GameObject[] tilePrefabs;
    public GameObject[] charPrefabs; //character gameobjects
    public GameObject[,] curr_tiles;
    private Transform playerTransform;
    public int max_y = 15;
    public int max_x = 15;
    private float tile_width, tile_height;

    // for type of player
    private int[] index = new int[2];

    //for coloring

    //for multi attack

    //[Player#][AtkType][X Y or T][coordinate]
    private float[,,][] atkTiles;
    private int[] atkType = {0,0};

    /*
    //P1
    private int[] x = new int[3];
    private int[] y = new int[3];
    private float[] t = new float[3];
    private string posPlayer = "x";
    private string atkType = 1;

    //P2
    private int[] x2 = new int[3];
    private int[] y2 = new int[3];
    private float[] t2 = new float[3];
    private string posPlayer2 = "x";
    private string atkType2 = 1;
    */

    private float duration = 0.2f; //duration of tile lightup
    // Use this for initialization
    void Start () {

        GameObject player1;
        index[0] = PlayerPrefs.GetInt("Player1");
        player1 = Instantiate(charPrefabs[index[0]]) as GameObject;
        PlayerMovement p1 = player1.AddComponent(typeof(PlayerMovement)) as PlayerMovement;
        p1.setPlayer(1);

        GameObject player2;
        index[1] = PlayerPrefs.GetInt("Player2");
        player2 = Instantiate(charPrefabs[index[1]]) as GameObject;
        PlayerMovement p2 = player2.AddComponent(typeof(PlayerMovement)) as PlayerMovement;
        p2.setPlayer(2);

        PlayerPrefs.DeleteAll();

        tile_width = 2;
        tile_height = 2;
        curr_tiles = new GameObject[20,20];

        float coor_x, coor_y;
        for(int y=0; y<max_y; y++)
        {
            for(int x=0; x<max_x; x++)
            {
                coor_x = x - (max_x / 2);
                coor_y = y - (max_y / 2);
                Debug.Log(coor_x + "," + coor_y);
                GameObject go;
                go = Instantiate(tilePrefabs[0]) as GameObject;
                go.transform.SetParent(transform);
                go.transform.position = new Vector3(tile_width * coor_x, tile_height * coor_y, 0);
                curr_tiles[x, y] = go;
                curr_tiles[x, y].GetComponent<Renderer>().material.color = Color.green;

            }
        }
        GameObject first_tile = curr_tiles[0, 0];
        player1.transform.position = new Vector3(first_tile.transform.position.x,
                                                first_tile.transform.position.y+1.8f, //1.8f para yung paa nasa center nung tile
                                                first_tile.transform.position.z);

        GameObject last_tile = curr_tiles[max_x-1, max_y-1];
        player2.transform.position = new Vector3(last_tile.transform.position.x,
                                                last_tile.transform.position.y + 1.8f, //1.8f para yung paa nasa center nung tile
                                                last_tile.transform.position.z);

        // Initializing attack Tiles
        //[Player#][AtkType][X Y T pos][coordinate]
        atkTiles = new float[2,3,4][];
        atkTiles[0, 0, 0] = new float[3]; //p1,basic,X
        atkTiles[0, 0, 1] = new float[3]; //p1,basic,Y
        atkTiles[0, 0, 2] = new float[3]; //p1,basic,T
        atkTiles[0, 0, 3] = new float[1]; //p1,basic,pos

        atkTiles[1, 0, 0] = new float[3]; //p2,basic,X
        atkTiles[1, 0, 1] = new float[3]; //p2,basic,Y
        atkTiles[1, 0, 2] = new float[3]; //p2,basic,T
        atkTiles[1, 0, 3] = new float[1]; //p2,basic,pos

        atkTiles[0, 1, 0] = new float[5]; //p1,SS1,X
        atkTiles[0, 1, 1] = new float[5]; //p1,SS1,Y
        atkTiles[0, 1, 2] = new float[5]; //p1,SS1,T
        atkTiles[0, 1, 3] = new float[1]; //p1,SS1,pos

        atkTiles[1, 1, 0] = new float[5]; //p2,SS1,X
        atkTiles[1, 1, 1] = new float[5]; //p2,SS1,Y
        atkTiles[1, 1, 2] = new float[5]; //p2,SS1,T
        atkTiles[1, 1, 3] = new float[1]; //p2,SS1,pos

        atkTiles[0, 2, 0] = new float[25]; //p1,SS2,X
        atkTiles[0, 2, 1] = new float[25]; //p1,SS2,Y
        atkTiles[0, 2, 2] = new float[25]; //p1,SS2,T
        atkTiles[0, 2, 3] = new float[1]; //p1,SS2,pos

        atkTiles[1, 2, 0] = new float[24]; //p2,SS2,X
        atkTiles[1, 2, 1] = new float[24]; //p2,SS2,Y
        atkTiles[1, 2, 2] = new float[24]; //p2,SS2,T
        atkTiles[1, 2, 3] = new float[1]; //p2,SS2,pos

        //initialize all to 0
        for (int a = 0; a < 2; a++)
        {
            for (int b = 0; b < 3; b++)
            {
                for (int c = 0; c < 4; c++)
                {
                    for (int d = 0; d < atkTiles[a, b, c].Length; d++)
                    {
                        atkTiles[a, b, c][d] = 0.0f;
                    }
                }
            }
        }

    }
	
	// Update is called once per frame
	void Update () {

        //[Player#][AtkType][X Y T pos][coordinate]
        //a is for players 1 and 2
        for (int a = 0; a < 2; a++)
        {
            if (atkType[a] == 0) //basic attack
            {
                //setting blocks to color
                if ((int)atkTiles[a, atkType[a], 3][0] == 1 && atkTiles[a, atkType[a], 2][0] == 0) // pos is "u" and T is 0
                {
                    atkTiles[a, atkType[a], 1][0] = atkTiles[a, atkType[a], 1][0] + 1; // y[0] = y[0] + 1
                    atkTiles[a, atkType[a], 1][1] = atkTiles[a, atkType[a], 1][0] + 1;
                    atkTiles[a, atkType[a], 1][2] = atkTiles[a, atkType[a], 1][0] + 2;
                }
                else if ((int)atkTiles[a, atkType[a], 3][0] == 2 && atkTiles[a, atkType[a], 2][0] == 0) // pos is "d" and T is 0
                {
                    atkTiles[a, atkType[a], 1][0] = atkTiles[a, atkType[a], 1][0] - 1; // y[0] = y[0] - 1
                    atkTiles[a, atkType[a], 1][1] = atkTiles[a, atkType[a], 1][0] - 1;
                    atkTiles[a, atkType[a], 1][2] = atkTiles[a, atkType[a], 1][0] - 2;
                }
                else if ((int)atkTiles[a, atkType[a], 3][0] == 3 && atkTiles[a, atkType[a], 2][0] == 0) // pos is "l" and T is 0
                {
                    atkTiles[a, atkType[a], 0][0] = atkTiles[a, atkType[a], 0][0] - 1; // x[0] = x[0] - 1
                    atkTiles[a, atkType[a], 0][1] = atkTiles[a, atkType[a], 0][0] - 1;
                    atkTiles[a, atkType[a], 0][2] = atkTiles[a, atkType[a], 0][0] - 2;
                    atkTiles[a, atkType[a], 1][1] = atkTiles[a, atkType[a], 1][0];
                    atkTiles[a, atkType[a], 1][2] = atkTiles[a, atkType[a], 1][0];
                }
                else if ((int)atkTiles[a, atkType[a], 3][0] == 4 && atkTiles[a, atkType[a], 2][0] == 0) // pos is "r" and T is 0
                {
                    atkTiles[a, atkType[a], 0][0] = atkTiles[a, atkType[a], 0][0] + 1; // x[0] = x[0] + 1
                    atkTiles[a, atkType[a], 0][1] = atkTiles[a, atkType[a], 0][0] + 1;
                    atkTiles[a, atkType[a], 0][2] = atkTiles[a, atkType[a], 0][0] + 2;
                    atkTiles[a, atkType[a], 1][1] = atkTiles[a, atkType[a], 1][0];
                    atkTiles[a, atkType[a], 1][2] = atkTiles[a, atkType[a], 1][0];
                }

                //coloring
            }
            else if (atkType[a] == 1) //SS1
            {
                if ((int)atkTiles[a, atkType[a], 3][0] == 1 && atkTiles[a, atkType[a], 2][0] == 0) // pos is "u" and T is 0
                {
                    atkTiles[a, atkType[a], 1][0] = atkTiles[a, atkType[a], 1][0] + 1; // y[0] = y[0] + 1
                    atkTiles[a, atkType[a], 1][1] = atkTiles[a, atkType[a], 1][0] + 1;
                    atkTiles[a, atkType[a], 1][2] = atkTiles[a, atkType[a], 1][0] + 2;
                    atkTiles[a, atkType[a], 1][3] = atkTiles[a, atkType[a], 1][0] + 3;
                    atkTiles[a, atkType[a], 1][4] = atkTiles[a, atkType[a], 1][0] + 4;
                }
                else if ((int)atkTiles[a, atkType[a], 3][0] == 2 && atkTiles[a, atkType[a], 2][0] == 0) // pos is "d" and T is 0
                {
                    atkTiles[a, atkType[a], 1][0] = atkTiles[a, atkType[a], 1][0] - 1; // y[0] = y[0] - 1
                    atkTiles[a, atkType[a], 1][1] = atkTiles[a, atkType[a], 1][0] - 1;
                    atkTiles[a, atkType[a], 1][2] = atkTiles[a, atkType[a], 1][0] - 2;
                    atkTiles[a, atkType[a], 1][3] = atkTiles[a, atkType[a], 1][0] - 3;
                    atkTiles[a, atkType[a], 1][4] = atkTiles[a, atkType[a], 1][0] - 4;
                }
                else if ((int)atkTiles[a, atkType[a], 3][0] == 3 && atkTiles[a, atkType[a], 2][0] == 0) // pos is "l" and T is 0
                {
                    atkTiles[a, atkType[a], 0][0] = atkTiles[a, atkType[a], 0][0] - 1; // x[0] = x[0] - 1
                    atkTiles[a, atkType[a], 0][1] = atkTiles[a, atkType[a], 0][0] - 1;
                    atkTiles[a, atkType[a], 0][2] = atkTiles[a, atkType[a], 0][0] - 2;
                    atkTiles[a, atkType[a], 0][3] = atkTiles[a, atkType[a], 0][0] - 3;
                    atkTiles[a, atkType[a], 0][4] = atkTiles[a, atkType[a], 0][0] - 4;
                    atkTiles[a, atkType[a], 1][1] = atkTiles[a, atkType[a], 1][0];
                    atkTiles[a, atkType[a], 1][2] = atkTiles[a, atkType[a], 1][0];
                    atkTiles[a, atkType[a], 1][3] = atkTiles[a, atkType[a], 1][0];
                    atkTiles[a, atkType[a], 1][4] = atkTiles[a, atkType[a], 1][0];
                }
                else if ((int)atkTiles[a, atkType[a], 3][0] == 4 && atkTiles[a, atkType[a], 2][0] == 0) // pos is "r" and T is 0
                {
                    atkTiles[a, atkType[a], 0][0] = atkTiles[a, atkType[a], 0][0] + 1; // x[0] = x[0] + 1
                    atkTiles[a, atkType[a], 0][1] = atkTiles[a, atkType[a], 0][0] + 1;
                    atkTiles[a, atkType[a], 0][2] = atkTiles[a, atkType[a], 0][0] + 2;
                    atkTiles[a, atkType[a], 0][3] = atkTiles[a, atkType[a], 0][0] + 3;
                    atkTiles[a, atkType[a], 0][4] = atkTiles[a, atkType[a], 0][0] + 4;
                    atkTiles[a, atkType[a], 1][1] = atkTiles[a, atkType[a], 1][0];
                    atkTiles[a, atkType[a], 1][2] = atkTiles[a, atkType[a], 1][0];
                    atkTiles[a, atkType[a], 1][3] = atkTiles[a, atkType[a], 1][0];
                    atkTiles[a, atkType[a], 1][4] = atkTiles[a, atkType[a], 1][0];
                }
            }
            else if (atkType[a] == 2)
            {
                if (index[a] == 0) //sustainy
                {
                    //setting blocks to color
                    if ((int)atkTiles[a, atkType[a], 3][0] == 1 && atkTiles[a, atkType[a], 2][0] == 0) // pos is "u" and T is 0
                    {
                        atkTiles[a, atkType[a], 1][0] = atkTiles[a, atkType[a], 1][0] + 1; // y[0] = y[0] + 1
                        atkTiles[a, atkType[a], 1][1] = atkTiles[a, atkType[a], 1][0] + 1;
                        atkTiles[a, atkType[a], 1][2] = atkTiles[a, atkType[a], 1][0] + 2;

                        atkTiles[a, atkType[a], 1][3] = atkTiles[a, atkType[a], 1][0] + 1;
                        atkTiles[a, atkType[a], 0][3] = atkTiles[a, atkType[a], 0][0] - 1;

                        atkTiles[a, atkType[a], 1][4] = atkTiles[a, atkType[a], 1][0] + 1;
                        atkTiles[a, atkType[a], 0][4] = atkTiles[a, atkType[a], 0][0] + 1;

                        atkTiles[a, atkType[a], 1][5] = atkTiles[a, atkType[a], 1][0] + 2;
                        atkTiles[a, atkType[a], 0][5] = atkTiles[a, atkType[a], 0][0] + 1;

                        atkTiles[a, atkType[a], 1][6] = atkTiles[a, atkType[a], 1][0] + 2;
                        atkTiles[a, atkType[a], 0][6] = atkTiles[a, atkType[a], 0][0] - 1;

                        atkTiles[a, atkType[a], 1][7] = atkTiles[a, atkType[a], 1][0] + 2;
                        atkTiles[a, atkType[a], 0][7] = atkTiles[a, atkType[a], 0][0] + 2;

                        atkTiles[a, atkType[a], 1][8] = atkTiles[a, atkType[a], 1][0] + 2;
                        atkTiles[a, atkType[a], 0][8] = atkTiles[a, atkType[a], 0][0] - 2;

                    }
                    else if ((int)atkTiles[a, atkType[a], 3][0] == 2 && atkTiles[a, atkType[a], 2][0] == 0) // pos is "d" and T is 0
                    {
                        atkTiles[a, atkType[a], 1][0] = atkTiles[a, atkType[a], 1][0] - 1; // y[0] = y[0] - 1
                        atkTiles[a, atkType[a], 1][1] = atkTiles[a, atkType[a], 1][0] - 1;
                        atkTiles[a, atkType[a], 1][2] = atkTiles[a, atkType[a], 1][0] - 2;

                        atkTiles[a, atkType[a], 1][3] = atkTiles[a, atkType[a], 1][0] - 1;
                        atkTiles[a, atkType[a], 0][3] = atkTiles[a, atkType[a], 0][0] + 1;

                        atkTiles[a, atkType[a], 1][4] = atkTiles[a, atkType[a], 1][0] - 1;
                        atkTiles[a, atkType[a], 0][4] = atkTiles[a, atkType[a], 0][0] - 1;

                        atkTiles[a, atkType[a], 1][5] = atkTiles[a, atkType[a], 1][0] - 2;
                        atkTiles[a, atkType[a], 0][5] = atkTiles[a, atkType[a], 0][0] - 1;

                        atkTiles[a, atkType[a], 1][6] = atkTiles[a, atkType[a], 1][0] - 2;
                        atkTiles[a, atkType[a], 0][6] = atkTiles[a, atkType[a], 0][0] + 1;

                        atkTiles[a, atkType[a], 1][7] = atkTiles[a, atkType[a], 1][0] - 2;
                        atkTiles[a, atkType[a], 0][7] = atkTiles[a, atkType[a], 0][0] - 2;

                        atkTiles[a, atkType[a], 1][8] = atkTiles[a, atkType[a], 1][0] - 2;
                        atkTiles[a, atkType[a], 0][8] = atkTiles[a, atkType[a], 0][0] + 2;
                    }
                    else if ((int)atkTiles[a, atkType[a], 3][0] == 3 && atkTiles[a, atkType[a], 2][0] == 0) // pos is "l" and T is 0
                    {
                        atkTiles[a, atkType[a], 0][0] = atkTiles[a, atkType[a], 0][0] - 1; // x[0] = x[0] - 1
                        atkTiles[a, atkType[a], 0][1] = atkTiles[a, atkType[a], 0][0] - 1;
                        atkTiles[a, atkType[a], 1][1] = atkTiles[a, atkType[a], 1][0];
                        atkTiles[a, atkType[a], 0][2] = atkTiles[a, atkType[a], 0][0] - 2;
                        atkTiles[a, atkType[a], 1][2] = atkTiles[a, atkType[a], 1][0];

                        atkTiles[a, atkType[a], 0][3] = atkTiles[a, atkType[a], 0][0] - 1;
                        atkTiles[a, atkType[a], 1][3] = atkTiles[a, atkType[a], 1][0] + 1;

                        atkTiles[a, atkType[a], 0][4] = atkTiles[a, atkType[a], 0][0] - 1;
                        atkTiles[a, atkType[a], 1][4] = atkTiles[a, atkType[a], 1][0] - 1;

                        atkTiles[a, atkType[a], 0][5] = atkTiles[a, atkType[a], 0][0] - 2;
                        atkTiles[a, atkType[a], 1][5] = atkTiles[a, atkType[a], 1][0] - 1;

                        atkTiles[a, atkType[a], 0][6] = atkTiles[a, atkType[a], 0][0] - 2;
                        atkTiles[a, atkType[a], 1][6] = atkTiles[a, atkType[a], 1][0] + 1;

                        atkTiles[a, atkType[a], 0][7] = atkTiles[a, atkType[a], 0][0] - 2;
                        atkTiles[a, atkType[a], 1][7] = atkTiles[a, atkType[a], 1][0] - 2;

                        atkTiles[a, atkType[a], 0][8] = atkTiles[a, atkType[a], 0][0] - 2;
                        atkTiles[a, atkType[a], 1][8] = atkTiles[a, atkType[a], 1][0] + 2;


                    }
                    else if ((int)atkTiles[a, atkType[a], 3][0] == 4 && atkTiles[a, atkType[a], 2][0] == 0) // pos is "r" and T is 0
                    {
                        atkTiles[a, atkType[a], 0][0] = atkTiles[a, atkType[a], 0][0] + 1; // x[0] = x[0] + 1
                        atkTiles[a, atkType[a], 0][1] = atkTiles[a, atkType[a], 0][0] + 1;
                        atkTiles[a, atkType[a], 0][2] = atkTiles[a, atkType[a], 0][0] + 2;
                        atkTiles[a, atkType[a], 1][1] = atkTiles[a, atkType[a], 1][0];
                        atkTiles[a, atkType[a], 1][2] = atkTiles[a, atkType[a], 1][0];

                        atkTiles[a, atkType[a], 0][3] = atkTiles[a, atkType[a], 0][0] + 1;
                        atkTiles[a, atkType[a], 1][3] = atkTiles[a, atkType[a], 1][0] + 1;

                        atkTiles[a, atkType[a], 0][4] = atkTiles[a, atkType[a], 0][0] + 1;
                        atkTiles[a, atkType[a], 1][4] = atkTiles[a, atkType[a], 1][0] - 1;

                        atkTiles[a, atkType[a], 0][5] = atkTiles[a, atkType[a], 0][0] + 2;
                        atkTiles[a, atkType[a], 1][5] = atkTiles[a, atkType[a], 1][0] - 1;

                        atkTiles[a, atkType[a], 0][6] = atkTiles[a, atkType[a], 0][0] + 2;
                        atkTiles[a, atkType[a], 1][6] = atkTiles[a, atkType[a], 1][0] + 1;

                        atkTiles[a, atkType[a], 0][7] = atkTiles[a, atkType[a], 0][0] + 2;
                        atkTiles[a, atkType[a], 1][7] = atkTiles[a, atkType[a], 1][0] - 2;

                        atkTiles[a, atkType[a], 0][8] = atkTiles[a, atkType[a], 0][0] + 2;
                        atkTiles[a, atkType[a], 1][8] = atkTiles[a, atkType[a], 1][0] + 2;
                    }
                }
                else // trebleine
                {
                    if ((int)atkTiles[a, atkType[a], 3][0] != 0 && atkTiles[a, atkType[a], 2][0] == 0) // any pos
                    {
                        atkTiles[a, atkType[a], 1][0] = atkTiles[a, atkType[a], 1][0] + 1; // y[0] = y[0] + 1
                        atkTiles[a, atkType[a], 1][1] = atkTiles[a, atkType[a], 1][0] + 1;

                        atkTiles[a, atkType[a], 1][2] = atkTiles[a, atkType[a], 1][0]; // y[0] = y[0] + 1
                        atkTiles[a, atkType[a], 0][2] = atkTiles[a, atkType[a], 0][0] + 1;
                        atkTiles[a, atkType[a], 1][3] = atkTiles[a, atkType[a], 1][0] + 1; // y[0] = y[0] + 1
                        atkTiles[a, atkType[a], 0][3] = atkTiles[a, atkType[a], 0][1] + 1;

                        atkTiles[a, atkType[a], 1][4] = atkTiles[a, atkType[a], 1][0]; // y[0] = y[0] + 1
                        atkTiles[a, atkType[a], 0][4] = atkTiles[a, atkType[a], 0][0] - 1;
                        atkTiles[a, atkType[a], 1][5] = atkTiles[a, atkType[a], 1][0] + 1; // y[0] = y[0] + 1
                        atkTiles[a, atkType[a], 0][5] = atkTiles[a, atkType[a], 0][1] - 1;

                        atkTiles[a, atkType[a], 1][6] = atkTiles[a, atkType[a], 1][0]; // y[0] = y[0] + 1
                        atkTiles[a, atkType[a], 0][6] = atkTiles[a, atkType[a], 0][0] + 2;
                        atkTiles[a, atkType[a], 1][7] = atkTiles[a, atkType[a], 1][0] + 1; // y[0] = y[0] + 1
                        atkTiles[a, atkType[a], 0][7] = atkTiles[a, atkType[a], 0][1] + 2;

                        atkTiles[a, atkType[a], 1][8] = atkTiles[a, atkType[a], 1][0]; // y[0] = y[0] + 1
                        atkTiles[a, atkType[a], 0][8] = atkTiles[a, atkType[a], 0][0] - 2;
                        atkTiles[a, atkType[a], 1][9] = atkTiles[a, atkType[a], 1][0] + 1; // y[0] = y[0] + 1
                        atkTiles[a, atkType[a], 0][9] = atkTiles[a, atkType[a], 0][1] - 2;

                        atkTiles[a, atkType[a], 1][10] = atkTiles[a, atkType[a], 1][0] - 1; // y[0] = y[0] + 1
                        atkTiles[a, atkType[a], 0][10] = atkTiles[a, atkType[a], 0][0] - 1;
                        atkTiles[a, atkType[a], 1][11] = atkTiles[a, atkType[a], 1][0] - 1; // y[0] = y[0] + 1
                        atkTiles[a, atkType[a], 0][11] = atkTiles[a, atkType[a], 0][1] + 1;

                        atkTiles[a, atkType[a], 1][12] = atkTiles[a, atkType[a], 1][0] - 1; // y[0] = y[0] + 1
                        atkTiles[a, atkType[a], 0][12] = atkTiles[a, atkType[a], 0][0] - 2;
                        atkTiles[a, atkType[a], 1][13] = atkTiles[a, atkType[a], 1][0] - 1; // y[0] = y[0] + 1
                        atkTiles[a, atkType[a], 0][13] = atkTiles[a, atkType[a], 0][1] + 2;

                        atkTiles[a, atkType[a], 1][14] = atkTiles[a, atkType[a], 1][0] - 2; // y[0] = y[0] + 1
                        atkTiles[a, atkType[a], 1][15] = atkTiles[a, atkType[a], 1][0] - 3;

                        atkTiles[a, atkType[a], 1][16] = atkTiles[a, atkType[a], 1][0] - 2; // y[0] = y[0] + 1
                        atkTiles[a, atkType[a], 0][16] = atkTiles[a, atkType[a], 0][0] + 1;
                        atkTiles[a, atkType[a], 1][17] = atkTiles[a, atkType[a], 1][0] - 3; // y[0] = y[0] + 1
                        atkTiles[a, atkType[a], 0][17] = atkTiles[a, atkType[a], 0][1] + 1;

                        atkTiles[a, atkType[a], 1][18] = atkTiles[a, atkType[a], 1][0] - 2; // y[0] = y[0] + 1
                        atkTiles[a, atkType[a], 0][18] = atkTiles[a, atkType[a], 0][0] - 1;
                        atkTiles[a, atkType[a], 1][19] = atkTiles[a, atkType[a], 1][0] - 3; // y[0] = y[0] + 1
                        atkTiles[a, atkType[a], 0][19] = atkTiles[a, atkType[a], 0][1] - 1;

                        atkTiles[a, atkType[a], 1][20] = atkTiles[a, atkType[a], 1][0] - 2; // y[0] = y[0] + 1
                        atkTiles[a, atkType[a], 0][20] = atkTiles[a, atkType[a], 0][0] + 2;
                        atkTiles[a, atkType[a], 1][21] = atkTiles[a, atkType[a], 1][0] - 3; // y[0] = y[0] + 1
                        atkTiles[a, atkType[a], 0][21] = atkTiles[a, atkType[a], 0][1] + 2;

                        atkTiles[a, atkType[a], 1][22] = atkTiles[a, atkType[a], 1][0] - 2; // y[0] = y[0] + 1
                        atkTiles[a, atkType[a], 0][22] = atkTiles[a, atkType[a], 0][0] - 2;
                        atkTiles[a, atkType[a], 1][23] = atkTiles[a, atkType[a], 1][0] - 3; // y[0] = y[0] + 1
                        atkTiles[a, atkType[a], 0][23] = atkTiles[a, atkType[a], 0][1] - 2;


                    }

                }

            }

            //coloring
            if (atkTiles[a, atkType[a], 3][0] != 0) // != "x"
            {
                for (int b = 0; b < atkTiles[a, atkType[a], 0].Length; b++)
                {
                    curr_tiles[(int)atkTiles[a, atkType[a], 0][b], (int)atkTiles[a, atkType[a], 1][b]].GetComponent<Renderer>().material.color = Color.Lerp(Color.red, Color.green, atkTiles[a, atkType[a], 2][b]);
                    atkTiles[a, atkType[a], 2][b] += Time.deltaTime / duration;
                }
            }

            if (atkTiles[a, atkType[a], 2][2] != 0) // t[2] !=0
            {
                for (int b = 0; b < atkTiles[a, atkType[a], 0].Length; b++)
                {
                    if (atkTiles[a, atkType[a], 2][b] < 1)
                    {
                        atkTiles[a, atkType[a], 2][b] += Time.deltaTime / duration;
                    }
                    else
                    {
                        atkTiles[a, atkType[a], 2][b] = 0;
                        atkTiles[a, atkType[a], 3][0] = 0;
                    }
                }
            }
        }
        
    }

    public void setTilesColor(int position, int input_x, int input_y, int player, int atkType)
    {
        //[Player#][AtkType][X Y T pos][coordinate]
        for (int a = 0; a < atkTiles[player,atkType, 0].Length; a++)
        {
            atkTiles[player, atkType, 0][a] = input_x;
            atkTiles[player, atkType, 1][a] = input_y;
            atkTiles[player, atkType, 3][0] = position;
        }
        
         this.atkType[player] = atkType;

    }
}
