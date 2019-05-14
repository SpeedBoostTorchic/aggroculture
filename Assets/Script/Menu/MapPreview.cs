using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapPreview : MonoBehaviour {

    public static MapPreview mp;

    public GameObject baseTile;  //Basic maptile - used for reference
    public int previewNum;      //Which map is being previewed?

    [Header("Map Info")]
    public int mapSize = 0;
    public int numSpring = 0;
    public int numRock = 0;
    public int numHole = 0;
    public int[,] tileMap;
    public PreviewTile[,] curMap;


    // Use this for initialization
    void Awake()
    {
        mp = this;
        tileMap = new int[mapSize, mapSize];
        curMap = new PreviewTile[mapSize, mapSize];
        setMap();
        drawMap();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    //Randomly generates map data
    public void setMap()
    {
        //Sets all map tiles to grass
        for (int i = 0; i < mapSize; i++)
        {
            for (int j = 0; j < mapSize; j++)
            {
                tileMap[i, j] = 1;
            }
        }

        //Sets the spring tiles
        springsAtCorners();
        //Randomly sets x tiles to hole
        ranTile(numHole, 0);
        //Randomly sets x tiles to Rock
        ranTile(numRock, 3);
    }    
    
    //Instantiates the maps initial tiles
    public void drawMap()
    {
        for (int i = 0; i < mapSize; i++)
        {
            //Draws each tile in sequence, then sets it to the corrosponding array position
            for (int j = 0; j < mapSize; j++)
            {
                curMap[i, j] = Instantiate(baseTile, new Vector3((3.2f * i) + (3.2f * j), (-1.6f * j) + (1.6f * i), (j * -0.2f) + (i * 2f)), Quaternion.identity).gameObject.GetComponent<PreviewTile>();
                curMap[i, j].tileNum = tileMap[i, j];
                curMap[i, j].xPos = i;
                curMap[i, j].yPos = j;
                curMap[i, j].transform.SetParent(gameObject.transform);
            }
        }
    }

    public void updateMap()
    {
        for(int i = 0; i < mapSize; i++)
        {
            for(int j = 0; j < mapSize; j++)
            {
                curMap[i, j].tileNum = tileMap[i, j];
            }
        }
    }


    //Used by setMap() to replace the requried number of given tiles
    public void ranTile(int n, int t)
    {

        //Randomly sets n tiles to t
        for (int i = 0; i < n; i++)
        {
            while (true)
            {
                int x = Random.Range(0, mapSize);
                int y = Random.Range(0, mapSize);

                //if tile is grass, set it to t. Else, find new tile
                if (tileMap[x, y] == 1)
                {
                    tileMap[x, y] = t;
                    break;
                }
            }
        }
    }

    public void springsAtCorners()
    {
        tileMap[1, 1] = 4;
        tileMap[1, mapSize - 2] = 4;
        tileMap[mapSize - 2, 1] = 4;
        tileMap[mapSize - 2, mapSize - 2] = 4;
    }

    public PreviewTile[] getNeighbors(int x, int y)
    {
        PreviewTile[] temp = new PreviewTile[4];    //MapTile array, to be returned
        int index = 0;

        //If the tile is not on the leftmost edge...
        if (x > 0)
        {
            temp[3] = curMap[x - 1, y];
            index += 1;
        }
        //If the tile is not on the rightmost edge...
        if (x < mapSize - 1)
        {
            temp[1] = curMap[x + 1, y];
            index += 1;
        }
        //If the tile is not on the bottommost row...
        if (y > 0)
        {
            temp[0] = curMap[x, y - 1];
            index += 1;
        }
        //If the tile is not on the topmost row...
        if (y < mapSize - 1)
        {
            temp[2] = curMap[x, y + 1];
            index += 1;
        }
        return temp;
    }

}
