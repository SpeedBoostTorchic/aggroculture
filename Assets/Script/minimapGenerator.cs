using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class minimapGenerator : MonoBehaviour {

    //The current position;
    public int mapSize;
    public float tileSize;
    public float baseX;
    public float baseY;
    public Vector3 rotationOffset;

    public GameObject parentCanvas;
    public GameObject tilePrefab;
    public GameObject[,] tiles;

    [Header("Sprites")]
    public Sprite[] tileSprites;         //0 - Neutral, 1 - P1Controlled, 2 - P2Controlled, 3 - Uncontrolled Farm
                                        //4 - Spring, 5 - Water, 6 - Ash, 7 - Fire
    public Sprite[] overlaySprites;
    

	// Use this for initialization
	void Start () {
        mapSize = GameManager.gm.mapSize;
        tiles = new GameObject[mapSize, mapSize];

        //Offsets intended position by tile size
        if(baseX != 0)
        {
            baseX -= (float)mapSize * (tileSize / 2) - (tileSize / 2);
        }
        //baseY += (float)mapSize * tileSize;

        //Generates the minimap
        GenerateMap(mapSize);
        this.transform.rotation = Quaternion.Euler(rotationOffset);
    }

    // Update is called once per frame
    void Update () {
        updateMap(GameManager.gm.curMap);
	}

    //Creates the minimap
    public void GenerateMap(int size)
    {
        for(int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                //Instantiates a maptile and set's it's parent to the canvas
                GameObject pf = Instantiate(tilePrefab, this.transform);
                pf.transform.parent = this.transform;

                //Offsets position 
                Vector3 pos = new Vector3(baseX + tileSize * i, baseY + tileSize * (size-j), -1f);
                pf.transform.localPosition = pos;

                //Adds the tile to the tile list
                tiles[i, j] = pf;
            }
        }
    }

    public void updateMap(MapTile[,] curMap)
    {
        //...Iterates over the current map...
        for(int i = 0; i < mapSize; i++)
        {
            for(int j = 0; j < mapSize; j++)
            {
                //If a worker is currently assigned to this tile...
                if (curMap[i, j].workerAssigned > 0)
                {
                    //Assigns one of the player control sprites
                    if (curMap[i, j].controllingPlayer == 1)
                    {
                        tiles[i, j].GetComponent<Image>().sprite = tileSprites[1];
                    }
                    else
                    {
                        tiles[i, j].GetComponent<Image>().sprite = tileSprites[2];
                    }
                }
                else
                {
                    //If the tile is a farm w/ no workers
                    if (curMap[i, j].tileNum == 2)
                    {
                        tiles[i, j].GetComponent<Image>().sprite = tileSprites[3];
                    }
                    //If the tile is a water source
                    else if (curMap[i, j].tileNum == 4 || (curMap[i, j].edge && curMap[i,j].tileNum == 5))
                    {
                        tiles[i, j].GetComponent<Image>().sprite = tileSprites[4];
                    }
                    //If the tile is currently a water source
                    else if (curMap[i, j].tileNum == 5 || curMap[i, j].tileNum == 6)
                    {
                        tiles[i, j].GetComponent<Image>().sprite = tileSprites[5];
                    }
                    //If the tile is currently on Fire
                    else if (curMap[i, j].tileNum == 7)
                    {
                        tiles[i, j].GetComponent<Image>().sprite = tileSprites[7];
                    }
                    //If the tile is Ash
                    else if (curMap[i, j].tileNum == 8)
                    {
                        tiles[i, j].GetComponent<Image>().sprite = tileSprites[6];
                    }
                    //Otherwise, the tile is neutral
                    else
                    {
                        tiles[i, j].GetComponent<Image>().sprite = tileSprites[0];
                    }
                }
            }
        }
    }
}
