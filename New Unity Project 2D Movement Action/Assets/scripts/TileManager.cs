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

    //for coloring
    private int x1 = 0;
    private int y1 = 0;
    private float t1 = 0;
    private string posPlayer1 = "x";

    private int x = 0;
    private int y = 0;
    private float t = 0;
    private string posPlayer = "x";


    private float duration = 0.2f; //duration of tile lightup
    // Use this for initialization
    void Start () {

        GameObject player1;
        int index1 = PlayerPrefs.GetInt("Player1");
        player1 = Instantiate(charPrefabs[index1]) as GameObject;
        PlayerMovement p1 = player1.AddComponent(typeof(PlayerMovement)) as PlayerMovement;
        p1.setPlayer(1);

        GameObject player2;
        int index2 = PlayerPrefs.GetInt("Player2");
        player2 = Instantiate(charPrefabs[index2]) as GameObject;
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
    }
	
	// Update is called once per frame
	void Update () {

        if (posPlayer1 == "u" && t1 == 0)
        {
            //y = y + 1;
        }
        else if (posPlayer1 == "d" && t1 == 0)
        {
            y1 = y1 - 2;
        }
        else if (posPlayer1 == "l" && t1 == 0)
        {
            x1 = x1 - 1;
            y1 = y1 - 1;
        }
        else if (posPlayer1 == "r" && t1 == 0)
        {
            x1 = x1 + 1;
            y1 = y1 - 1;
        }

        if (posPlayer1 != "x")
        {
            Debug.Log("UPDATED TILE " + posPlayer1);
            curr_tiles[x1, y1 + 1].GetComponent<Renderer>().material.color = Color.Lerp(Color.red, Color.green, t1);
            t1 += Time.deltaTime / duration;
            Debug.Log("U t is " + t1 + "and color is " + curr_tiles[x1, y1 + 1].GetComponent<Renderer>().material.color.ToString());
            Debug.Log("UPDATED TILE END " + posPlayer1);
        }

        if (t1 != 0)
        {
            Debug.Log("IN T != 0");
            if (t1 < 1)
            {
                t1 += Time.deltaTime / duration;
                Debug.Log("if t is " + t1 + "and color is " + curr_tiles[x1, y1 + 1].GetComponent<Renderer>().material.color.ToString());
            }
            else
            {
                t1 = 0;
                Debug.Log("else t is " + t1+ "and color is " + curr_tiles[x1, y1 + 1].GetComponent<Renderer>().material.color.ToString());
                posPlayer1 = "x";
            }
            Debug.Log("OUT T != 0");
        }
        else
        {
            Debug.Log("IN T == 0");
        }


        /*********************** P2 **********************/

        if (posPlayer == "u" && t == 0)
        {
            //y = y + 1;
        }
        else if (posPlayer == "d" && t == 0)
        {
            y = y - 2;
        }
        else if (posPlayer == "l" && t == 0)
        {
            x = x - 1;
            y = y - 1;
        }
        else if (posPlayer == "r" && t == 0)
        {
            x = x + 1;
            y = y - 1;
        }

        if (posPlayer != "x")
        {
            Debug.Log("UPDATED TILE " + posPlayer);
            curr_tiles[x, y + 1].GetComponent<Renderer>().material.color = Color.Lerp(Color.red, Color.green, t);
            t += Time.deltaTime / duration;
            Debug.Log("U t is " + t + "and color is " + curr_tiles[x, y + 1].GetComponent<Renderer>().material.color.ToString());
            Debug.Log("UPDATED TILE END " + posPlayer);
        }
        
        if (t != 0)
        {
            Debug.Log("IN T != 0");
            if (t < 1)
            {
                t += Time.deltaTime / duration;
                Debug.Log("if t is " + t + "and color is " + curr_tiles[x, y + 1].GetComponent<Renderer>().material.color.ToString());
            }
            else
            {
                t = 0;
                Debug.Log("else t is " + t + "and color is " + curr_tiles[x, y + 1].GetComponent<Renderer>().material.color.ToString());
                posPlayer = "x";
            }
            Debug.Log("OUT T != 0");
        }
        else {
            Debug.Log("IN T == 0");
        }
    }

    public void setTilesColor(string position, int input_x, int input_y, string player)
    {
        if (player == "p1")
        {
            x = input_x;
            y = input_y;
            posPlayer = position;
        }
        else
        {
            x1 = input_x;
            y1 = input_y;
            posPlayer1 = position;
        }

        /*float t = 0;
        if (position == "u")
        {
            //curr_tiles[input_x, input_y + 1].GetComponent<Renderer>().material.color = Color.green;
            curr_tiles[input_x, input_y + 1].GetComponent<Renderer>().material.color = Color.Lerp(Color.yellow,Color.green, t);
            while (t < 1)
            {
                t += Time.deltaTime / duration;
            }
           // curr_tiles[input_x, input_y + 1].GetComponent<Renderer>().material.color = Color.white;

        }
        else if (position == "d")
        {
                curr_tiles[input_x, input_y - 1].GetComponent<Renderer>().material.color = Color.green;
                curr_tiles[input_x, input_y - 1].GetComponent<Renderer>().material.color = Color.white;
        }
        else if (position == "l")
        {
            curr_tiles[input_x-1, input_y].GetComponent<Renderer>().material.color = Color.green;
            curr_tiles[input_x-1, input_y].GetComponent<Renderer>().material.color = Color.white;
        }
        else if (position == "r")
        {
            //curr_tiles[input_x+1, input_y - 1].GetComponent<Renderer>().material.color = Color.green;
            //curr_tiles[input_x+1, input_y - 1].GetComponent<Renderer>().material.color = Color.white;
        }*/
    }
}
