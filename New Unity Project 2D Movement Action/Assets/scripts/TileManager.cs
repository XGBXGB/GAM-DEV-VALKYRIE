using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Attack type: 0=hazard,

public class TileManager : MonoBehaviour
{

    public DeathMenu deathMenu;
    private bool game_over = false;
    public GameObject[] tilePrefabs;
    public GameObject[] charPrefabs; //character gameobjects
    public GameObject[,] curr_tiles;
    private Transform playerTransform;
    public int max_y = 15;
    public int max_x = 15;
    private float tile_width, tile_height;

    private int[] index = new int[2];

    //for coloring
    private float[,,][] atkTiles;
    private int[] atkType = { 0, 0 };

    /*
    private int x = 0;
    private int y = 0;
    private float t = 0;
    private string posPlayer = "x";
    private string atkType = 1;

    private int x2 = 0;
    private int y2 = 0;
    private float t2 = 0;
    private string posPlayer2 = "x";
    private string atkType2 = 1;
    */

    Point[] hazardPoints;
    int[,] damageTiles;
    GameObject player1;
    GameObject player2;

    private float duration = 0.2f; //duration of tile lightup
    // Use this for initialization
    void Start()
    {

        index[0] = PlayerPrefs.GetInt("Player1");
        player1 = Instantiate(charPrefabs[index[0]]) as GameObject;
        PlayerMovement p1 = player1.AddComponent(typeof(PlayerMovement)) as PlayerMovement;
        p1.setPlayer(1);
        p1.setCharacter(index[0]);

        index[1] = PlayerPrefs.GetInt("Player2");
        player2 = Instantiate(charPrefabs[index[1]]) as GameObject;
        PlayerMovement p2 = player2.AddComponent(typeof(PlayerMovement)) as PlayerMovement;
        p2.setPlayer(2);
        p2.setCharacter(index[1]);

        PlayerPrefs.DeleteAll();

        tile_width = 2;
        tile_height = 2;
        curr_tiles = new GameObject[15, 15];

        float coor_x, coor_y;
        for (int y = 0; y < max_y; y++)
        {
            for (int x = 0; x < max_x; x++)
            {
                coor_x = x - (max_x / 2);
                coor_y = y - (max_y / 2);
                //                Debug.Log(coor_x + "," + coor_y);
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
                                                first_tile.transform.position.y + 1.8f, //1.8f para yung paa nasa center nung tile
                                                first_tile.transform.position.z);

        GameObject last_tile = curr_tiles[max_x - 1, max_y - 1];
        player2.transform.position = new Vector3(last_tile.transform.position.x,
                                                last_tile.transform.position.y + 1.8f, //1.8f para yung paa nasa center nung tile
                                                last_tile.transform.position.z);

        //
        // Initializing attack Tiles
        //[Player#][AtkType][X Y T pos][coordinate]
        atkTiles = new float[2, 3, 4][];
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
        //


        damageTiles = new int[15, 15];
        InvokeRepeating("GenerateHazard", 5, 5);
    }

    void GenerateHazard()
    {
        int nHazards = 20;
        if (hazardPoints != null)
            for (int i = 0; i < nHazards; i++)
            {
                curr_tiles[hazardPoints[i].x, hazardPoints[i].y].GetComponent<Renderer>().material.color = Color.green;
                damageTiles[hazardPoints[i].x, hazardPoints[i].y] = 0;
            }
        hazardPoints = new Point[nHazards];
        for (int i = 0; i < nHazards; i++)
        {
            hazardPoints[i] = new Point(Random.Range(0, 15), Random.Range(0, 15));
            curr_tiles[hazardPoints[i].x, hazardPoints[i].y].GetComponent<Renderer>().material.color = Color.red;
            damageTiles[hazardPoints[i].x, hazardPoints[i].y] = 5;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (game_over)
            return;
        if (player1.GetComponent<PlayerMovement>().hp <= 0)
        {
            deathMenu.showEndMenu("2");
        }
        if (player2.GetComponent<PlayerMovement>().hp <= 0)
        {
            deathMenu.showEndMenu("1");
        }


        if (damageTiles[(int)player1.GetComponent<PlayerMovement>().curr_x, (int)player1.GetComponent<PlayerMovement>().curr_y] > 0)
        {
            player1.GetComponent<PlayerMovement>().InflictDamage(damageTiles[(int)player1.GetComponent<PlayerMovement>().curr_x, (int)player1.GetComponent<PlayerMovement>().curr_y]);
        }
        if (damageTiles[(int)player2.GetComponent<PlayerMovement>().curr_x, (int)player2.GetComponent<PlayerMovement>().curr_y] > 0)
        {
            player2.GetComponent<PlayerMovement>().InflictDamage(damageTiles[(int)player2.GetComponent<PlayerMovement>().curr_x, (int)player2.GetComponent<PlayerMovement>().curr_y]);
        }


        for (int a = 0; a < 2; a++)
        {
            //coloring
            if (atkTiles[a, atkType[a], 3][0] != 0) // != "x"
            {
                for (int b = 0; b < atkTiles[a, atkType[a], 0].Length; b++)
                {
                    if ((int)atkTiles[a, atkType[a], 0][b] < max_x && (int)atkTiles[a, atkType[a], 1][b] < max_y
                        && (int)atkTiles[a, atkType[a], 0][b] >= 0 && (int)atkTiles[a, atkType[a], 1][b] >=0)
                    {
                        curr_tiles[(int)atkTiles[a, atkType[a], 0][b], (int)atkTiles[a, atkType[a], 1][b]].GetComponent<Renderer>().material.color = Color.Lerp(Color.red, Color.green, atkTiles[a, atkType[a], 2][b]);
                    }
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
        bool hurtOpponent = false;
        int opponent_x, opponent_y;
        if (player == 0)
        {
            Debug.Log("Opponent is player 2");
            opponent_x = (int)player2.GetComponent<PlayerMovement>().curr_x;
            opponent_y = (int)player2.GetComponent<PlayerMovement>().curr_y;
        }
        else
        {
            Debug.Log("Opponent is player 1");
            opponent_x = (int)player1.GetComponent<PlayerMovement>().curr_x;
            opponent_y = (int)player1.GetComponent<PlayerMovement>().curr_y;
        }

        for (int a = 0; a < atkTiles[player, atkType, 0].Length; a++)
        {
            atkTiles[player, atkType, 0][a] = input_x;
            atkTiles[player, atkType, 1][a] = input_y;
            atkTiles[player, atkType, 3][0] = position;
        }



        //////

        if (atkType == 0) //basic attack
        {
            //setting blocks to color
            if ((int)atkTiles[player, atkType, 3][0] == 1 && atkTiles[player, atkType, 2][0] == 0) // pos is "u" and T is 0
            {
                atkTiles[player, atkType, 1][0] = atkTiles[player, atkType, 1][0] + 1; // y[0] = y[0] + 1
                atkTiles[player, atkType, 1][1] = atkTiles[player, atkType, 1][0] + 1;
                atkTiles[player, atkType, 1][2] = atkTiles[player, atkType, 1][0] + 2;
            }
            else if ((int)atkTiles[player, atkType, 3][0] == 2 && atkTiles[player, atkType, 2][0] == 0) // pos is "d" and T is 0
            {
                atkTiles[player, atkType, 1][0] = atkTiles[player, atkType, 1][0] - 1; // y[0] = y[0] - 1
                atkTiles[player, atkType, 1][1] = atkTiles[player, atkType, 1][0] - 1;
                atkTiles[player, atkType, 1][2] = atkTiles[player, atkType, 1][0] - 2;
            }
            else if ((int)atkTiles[player, atkType, 3][0] == 3 && atkTiles[player, atkType, 2][0] == 0) // pos is "l" and T is 0
            {
                atkTiles[player, atkType, 0][0] = atkTiles[player, atkType, 0][0] - 1; // x[0] = x[0] - 1
                atkTiles[player, atkType, 0][1] = atkTiles[player, atkType, 0][0] - 1;
                atkTiles[player, atkType, 0][2] = atkTiles[player, atkType, 0][0] - 2;
                atkTiles[player, atkType, 1][1] = atkTiles[player, atkType, 1][0];
                atkTiles[player, atkType, 1][2] = atkTiles[player, atkType, 1][0];
            }
            else if ((int)atkTiles[player, atkType, 3][0] == 4 && atkTiles[player, atkType, 2][0] == 0) // pos is "r" and T is 0
            {
                atkTiles[player, atkType, 0][0] = atkTiles[player, atkType, 0][0] + 1; // x[0] = x[0] + 1
                atkTiles[player, atkType, 0][1] = atkTiles[player, atkType, 0][0] + 1;
                atkTiles[player, atkType, 0][2] = atkTiles[player, atkType, 0][0] + 2;
                atkTiles[player, atkType, 1][1] = atkTiles[player, atkType, 1][0];
                atkTiles[player, atkType, 1][2] = atkTiles[player, atkType, 1][0];
            }

            //coloring
        }
        else if (atkType == 1) //SS1
        {
            if ((int)atkTiles[player, atkType, 3][0] == 1 && atkTiles[player, atkType, 2][0] == 0) // pos is "u" and T is 0
            {
                atkTiles[player, atkType, 1][0] = atkTiles[player, atkType, 1][0] + 1; // y[0] = y[0] + 1
                atkTiles[player, atkType, 1][1] = atkTiles[player, atkType, 1][0] + 1;
                atkTiles[player, atkType, 1][2] = atkTiles[player, atkType, 1][0] + 2;
                atkTiles[player, atkType, 1][3] = atkTiles[player, atkType, 1][0] + 3;
                atkTiles[player, atkType, 1][4] = atkTiles[player, atkType, 1][0] + 4;
            }
            else if ((int)atkTiles[player, atkType, 3][0] == 2 && atkTiles[player, atkType, 2][0] == 0) // pos is "d" and T is 0
            {
                atkTiles[player, atkType, 1][0] = atkTiles[player, atkType, 1][0] - 1; // y[0] = y[0] - 1
                atkTiles[player, atkType, 1][1] = atkTiles[player, atkType, 1][0] - 1;
                atkTiles[player, atkType, 1][2] = atkTiles[player, atkType, 1][0] - 2;
                atkTiles[player, atkType, 1][3] = atkTiles[player, atkType, 1][0] - 3;
                atkTiles[player, atkType, 1][4] = atkTiles[player, atkType, 1][0] - 4;
            }
            else if ((int)atkTiles[player, atkType, 3][0] == 3 && atkTiles[player, atkType, 2][0] == 0) // pos is "l" and T is 0
            {
                atkTiles[player, atkType, 0][0] = atkTiles[player, atkType, 0][0] - 1; // x[0] = x[0] - 1
                atkTiles[player, atkType, 0][1] = atkTiles[player, atkType, 0][0] - 1;
                atkTiles[player, atkType, 0][2] = atkTiles[player, atkType, 0][0] - 2;
                atkTiles[player, atkType, 0][3] = atkTiles[player, atkType, 0][0] - 3;
                atkTiles[player, atkType, 0][4] = atkTiles[player, atkType, 0][0] - 4;
                atkTiles[player, atkType, 1][1] = atkTiles[player, atkType, 1][0];
                atkTiles[player, atkType, 1][2] = atkTiles[player, atkType, 1][0];
                atkTiles[player, atkType, 1][3] = atkTiles[player, atkType, 1][0];
                atkTiles[player, atkType, 1][4] = atkTiles[player, atkType, 1][0];
            }
            else if ((int)atkTiles[player, atkType, 3][0] == 4 && atkTiles[player, atkType, 2][0] == 0) // pos is "r" and T is 0
            {
                atkTiles[player, atkType, 0][0] = atkTiles[player, atkType, 0][0] + 1; // x[0] = x[0] + 1
                atkTiles[player, atkType, 0][1] = atkTiles[player, atkType, 0][0] + 1;
                atkTiles[player, atkType, 0][2] = atkTiles[player, atkType, 0][0] + 2;
                atkTiles[player, atkType, 0][3] = atkTiles[player, atkType, 0][0] + 3;
                atkTiles[player, atkType, 0][4] = atkTiles[player, atkType, 0][0] + 4;
                atkTiles[player, atkType, 1][1] = atkTiles[player, atkType, 1][0];
                atkTiles[player, atkType, 1][2] = atkTiles[player, atkType, 1][0];
                atkTiles[player, atkType, 1][3] = atkTiles[player, atkType, 1][0];
                atkTiles[player, atkType, 1][4] = atkTiles[player, atkType, 1][0];
            }
        }
        else if (atkType == 2)
        {
            if (index[player] == 0) //sustainy
            {
                //setting blocks to color
                if ((int)atkTiles[player, atkType, 3][0] == 1 && atkTiles[player, atkType, 2][0] == 0) // pos is "u" and T is 0
                {
                    atkTiles[player, atkType, 1][0] = atkTiles[player, atkType, 1][0] + 1; // y[0] = y[0] + 1
                    atkTiles[player, atkType, 1][1] = atkTiles[player, atkType, 1][0] + 1;
                    atkTiles[player, atkType, 1][2] = atkTiles[player, atkType, 1][0] + 2;

                    atkTiles[player, atkType, 1][3] = atkTiles[player, atkType, 1][0] + 1;
                    atkTiles[player, atkType, 0][3] = atkTiles[player, atkType, 0][0] - 1;

                    atkTiles[player, atkType, 1][4] = atkTiles[player, atkType, 1][0] + 1;
                    atkTiles[player, atkType, 0][4] = atkTiles[player, atkType, 0][0] + 1;

                    atkTiles[player, atkType, 1][5] = atkTiles[player, atkType, 1][0] + 2;
                    atkTiles[player, atkType, 0][5] = atkTiles[player, atkType, 0][0] + 1;

                    atkTiles[player, atkType, 1][6] = atkTiles[player, atkType, 1][0] + 2;
                    atkTiles[player, atkType, 0][6] = atkTiles[player, atkType, 0][0] - 1;

                    atkTiles[player, atkType, 1][7] = atkTiles[player, atkType, 1][0] + 2;
                    atkTiles[player, atkType, 0][7] = atkTiles[player, atkType, 0][0] + 2;

                    atkTiles[player, atkType, 1][8] = atkTiles[player, atkType, 1][0] + 2;
                    atkTiles[player, atkType, 0][8] = atkTiles[player, atkType, 0][0] - 2;

                }
                else if ((int)atkTiles[player, atkType, 3][0] == 2 && atkTiles[player, atkType, 2][0] == 0) // pos is "d" and T is 0
                {
                    atkTiles[player, atkType, 1][0] = atkTiles[player, atkType, 1][0] - 1; // y[0] = y[0] - 1
                    atkTiles[player, atkType, 1][1] = atkTiles[player, atkType, 1][0] - 1;
                    atkTiles[player, atkType, 1][2] = atkTiles[player, atkType, 1][0] - 2;

                    atkTiles[player, atkType, 1][3] = atkTiles[player, atkType, 1][0] - 1;
                    atkTiles[player, atkType, 0][3] = atkTiles[player, atkType, 0][0] + 1;

                    atkTiles[player, atkType, 1][4] = atkTiles[player, atkType, 1][0] - 1;
                    atkTiles[player, atkType, 0][4] = atkTiles[player, atkType, 0][0] - 1;

                    atkTiles[player, atkType, 1][5] = atkTiles[player, atkType, 1][0] - 2;
                    atkTiles[player, atkType, 0][5] = atkTiles[player, atkType, 0][0] - 1;

                    atkTiles[player, atkType, 1][6] = atkTiles[player, atkType, 1][0] - 2;
                    atkTiles[player, atkType, 0][6] = atkTiles[player, atkType, 0][0] + 1;

                    atkTiles[player, atkType, 1][7] = atkTiles[player, atkType, 1][0] - 2;
                    atkTiles[player, atkType, 0][7] = atkTiles[player, atkType, 0][0] - 2;

                    atkTiles[player, atkType, 1][8] = atkTiles[player, atkType, 1][0] - 2;
                    atkTiles[player, atkType, 0][8] = atkTiles[player, atkType, 0][0] + 2;
                }
                else if ((int)atkTiles[player, atkType, 3][0] == 3 && atkTiles[player, atkType, 2][0] == 0) // pos is "l" and T is 0
                {
                    atkTiles[player, atkType, 0][0] = atkTiles[player, atkType, 0][0] - 1; // x[0] = x[0] - 1
                    atkTiles[player, atkType, 0][1] = atkTiles[player, atkType, 0][0] - 1;
                    atkTiles[player, atkType, 1][1] = atkTiles[player, atkType, 1][0];
                    atkTiles[player, atkType, 0][2] = atkTiles[player, atkType, 0][0] - 2;
                    atkTiles[player, atkType, 1][2] = atkTiles[player, atkType, 1][0];

                    atkTiles[player, atkType, 0][3] = atkTiles[player, atkType, 0][0] - 1;
                    atkTiles[player, atkType, 1][3] = atkTiles[player, atkType, 1][0] + 1;

                    atkTiles[player, atkType, 0][4] = atkTiles[player, atkType, 0][0] - 1;
                    atkTiles[player, atkType, 1][4] = atkTiles[player, atkType, 1][0] - 1;

                    atkTiles[player, atkType, 0][5] = atkTiles[player, atkType, 0][0] - 2;
                    atkTiles[player, atkType, 1][5] = atkTiles[player, atkType, 1][0] - 1;

                    atkTiles[player, atkType, 0][6] = atkTiles[player, atkType, 0][0] - 2;
                    atkTiles[player, atkType, 1][6] = atkTiles[player, atkType, 1][0] + 1;

                    atkTiles[player, atkType, 0][7] = atkTiles[player, atkType, 0][0] - 2;
                    atkTiles[player, atkType, 1][7] = atkTiles[player, atkType, 1][0] - 2;

                    atkTiles[player, atkType, 0][8] = atkTiles[player, atkType, 0][0] - 2;
                    atkTiles[player, atkType, 1][8] = atkTiles[player, atkType, 1][0] + 2;


                }
                else if ((int)atkTiles[player, atkType, 3][0] == 4 && atkTiles[player, atkType, 2][0] == 0) // pos is "r" and T is 0
                {
                    atkTiles[player, atkType, 0][0] = atkTiles[player, atkType, 0][0] + 1; // x[0] = x[0] + 1
                    atkTiles[player, atkType, 0][1] = atkTiles[player, atkType, 0][0] + 1;
                    atkTiles[player, atkType, 0][2] = atkTiles[player, atkType, 0][0] + 2;
                    atkTiles[player, atkType, 1][1] = atkTiles[player, atkType, 1][0];
                    atkTiles[player, atkType, 1][2] = atkTiles[player, atkType, 1][0];

                    atkTiles[player, atkType, 0][3] = atkTiles[player, atkType, 0][0] + 1;
                    atkTiles[player, atkType, 1][3] = atkTiles[player, atkType, 1][0] + 1;

                    atkTiles[player, atkType, 0][4] = atkTiles[player, atkType, 0][0] + 1;
                    atkTiles[player, atkType, 1][4] = atkTiles[player, atkType, 1][0] - 1;

                    atkTiles[player, atkType, 0][5] = atkTiles[player, atkType, 0][0] + 2;
                    atkTiles[player, atkType, 1][5] = atkTiles[player, atkType, 1][0] - 1;

                    atkTiles[player, atkType, 0][6] = atkTiles[player, atkType, 0][0] + 2;
                    atkTiles[player, atkType, 1][6] = atkTiles[player, atkType, 1][0] + 1;

                    atkTiles[player, atkType, 0][7] = atkTiles[player, atkType, 0][0] + 2;
                    atkTiles[player, atkType, 1][7] = atkTiles[player, atkType, 1][0] - 2;

                    atkTiles[player, atkType, 0][8] = atkTiles[player, atkType, 0][0] + 2;
                    atkTiles[player, atkType, 1][8] = atkTiles[player, atkType, 1][0] + 2;
                }
            }
            else // trebleine
            {
                if ((int)atkTiles[player, atkType, 3][0] != 0 && atkTiles[player, atkType, 2][0] == 0) // any pos
                {
                    atkTiles[player, atkType, 1][0] = atkTiles[player, atkType, 1][0] + 1; // y[0] = y[0] + 1
                    atkTiles[player, atkType, 1][1] = atkTiles[player, atkType, 1][0] + 1;

                    atkTiles[player, atkType, 1][2] = atkTiles[player, atkType, 1][0]; // y[0] = y[0] + 1
                    atkTiles[player, atkType, 0][2] = atkTiles[player, atkType, 0][0] + 1;
                    atkTiles[player, atkType, 1][3] = atkTiles[player, atkType, 1][0] + 1; // y[0] = y[0] + 1
                    atkTiles[player, atkType, 0][3] = atkTiles[player, atkType, 0][1] + 1;

                    atkTiles[player, atkType, 1][4] = atkTiles[player, atkType, 1][0]; // y[0] = y[0] + 1
                    atkTiles[player, atkType, 0][4] = atkTiles[player, atkType, 0][0] - 1;
                    atkTiles[player, atkType, 1][5] = atkTiles[player, atkType, 1][0] + 1; // y[0] = y[0] + 1
                    atkTiles[player, atkType, 0][5] = atkTiles[player, atkType, 0][1] - 1;

                    atkTiles[player, atkType, 1][6] = atkTiles[player, atkType, 1][0]; // y[0] = y[0] + 1
                    atkTiles[player, atkType, 0][6] = atkTiles[player, atkType, 0][0] + 2;
                    atkTiles[player, atkType, 1][7] = atkTiles[player, atkType, 1][0] + 1; // y[0] = y[0] + 1
                    atkTiles[player, atkType, 0][7] = atkTiles[player, atkType, 0][1] + 2;

                    atkTiles[player, atkType, 1][8] = atkTiles[player, atkType, 1][0]; // y[0] = y[0] + 1
                    atkTiles[player, atkType, 0][8] = atkTiles[player, atkType, 0][0] - 2;
                    atkTiles[player, atkType, 1][9] = atkTiles[player, atkType, 1][0] + 1; // y[0] = y[0] + 1
                    atkTiles[player, atkType, 0][9] = atkTiles[player, atkType, 0][1] - 2;

                    atkTiles[player, atkType, 1][10] = atkTiles[player, atkType, 1][0] - 1; // y[0] = y[0] + 1
                    atkTiles[player, atkType, 0][10] = atkTiles[player, atkType, 0][0] - 1;
                    atkTiles[player, atkType, 1][11] = atkTiles[player, atkType, 1][0] - 1; // y[0] = y[0] + 1
                    atkTiles[player, atkType, 0][11] = atkTiles[player, atkType, 0][1] + 1;

                    atkTiles[player, atkType, 1][12] = atkTiles[player, atkType, 1][0] - 1; // y[0] = y[0] + 1
                    atkTiles[player, atkType, 0][12] = atkTiles[player, atkType, 0][0] - 2;
                    atkTiles[player, atkType, 1][13] = atkTiles[player, atkType, 1][0] - 1; // y[0] = y[0] + 1
                    atkTiles[player, atkType, 0][13] = atkTiles[player, atkType, 0][1] + 2;

                    atkTiles[player, atkType, 1][14] = atkTiles[player, atkType, 1][0] - 2; // y[0] = y[0] + 1
                    atkTiles[player, atkType, 1][15] = atkTiles[player, atkType, 1][0] - 3;

                    atkTiles[player, atkType, 1][16] = atkTiles[player, atkType, 1][0] - 2; // y[0] = y[0] + 1
                    atkTiles[player, atkType, 0][16] = atkTiles[player, atkType, 0][0] + 1;
                    atkTiles[player, atkType, 1][17] = atkTiles[player, atkType, 1][0] - 3; // y[0] = y[0] + 1
                    atkTiles[player, atkType, 0][17] = atkTiles[player, atkType, 0][1] + 1;

                    atkTiles[player, atkType, 1][18] = atkTiles[player, atkType, 1][0] - 2; // y[0] = y[0] + 1
                    atkTiles[player, atkType, 0][18] = atkTiles[player, atkType, 0][0] - 1;
                    atkTiles[player, atkType, 1][19] = atkTiles[player, atkType, 1][0] - 3; // y[0] = y[0] + 1
                    atkTiles[player, atkType, 0][19] = atkTiles[player, atkType, 0][1] - 1;

                    atkTiles[player, atkType, 1][20] = atkTiles[player, atkType, 1][0] - 2; // y[0] = y[0] + 1
                    atkTiles[player, atkType, 0][20] = atkTiles[player, atkType, 0][0] + 2;
                    atkTiles[player, atkType, 1][21] = atkTiles[player, atkType, 1][0] - 3; // y[0] = y[0] + 1
                    atkTiles[player, atkType, 0][21] = atkTiles[player, atkType, 0][1] + 2;

                    atkTiles[player, atkType, 1][22] = atkTiles[player, atkType, 1][0] - 2; // y[0] = y[0] + 1
                    atkTiles[player, atkType, 0][22] = atkTiles[player, atkType, 0][0] - 2;
                    atkTiles[player, atkType, 1][23] = atkTiles[player, atkType, 1][0] - 3; // y[0] = y[0] + 1
                    atkTiles[player, atkType, 0][23] = atkTiles[player, atkType, 0][1] - 2;


                }

            }

        }

        for (int b = 0; b < atkTiles[player, atkType, 0].Length; b++)
        {
            if (((int)atkTiles[player, atkType, 0][b]) == opponent_x && ((int)atkTiles[player, atkType, 1][b]) == opponent_y)
            {
                hurtOpponent = true;
            }
        }

        ///////




        this.atkType[player] = atkType;

        if (hurtOpponent)
        {
            if (player == 0)
            {
                player2.GetComponent<PlayerMovement>().harm(index[player], atkType);
            }
            else
            {
                player1.GetComponent<PlayerMovement>().harm(index[player], atkType);
            }
        }
    }

    public void EndGame(int playerNo)
    {
        game_over = true;
        deathMenu.showEndMenu("Player "+playerNo+" won!");
    }

    private class Point
    {
        public int x;
        public int y;

        public Point(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }
}
