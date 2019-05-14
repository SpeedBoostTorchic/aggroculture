using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviewTile : MonoBehaviour {

    public int tileNum;
    public int xPos;
    public int yPos;

    [Header("Sprites/Visuals")]
    public Sprite[] grass;
    public Sprite[] rock1;
    public GameObject[] rockToppers;
    private bool rockDeactivated;
    public Sprite water1;
    public Sprite seed;
    public Sprite pit;
    public Sprite pitShadow;

    private int variantNum;         //Determines which variant of the grass tile to use
    private int subVariant;

    public Sprite spring;
    public GameObject waterTop;
    public GameObject springTop;
    public ParticleSystem springPart;

    public GameObject decor;
    public Sprite[] GrassDecor;
    public Sprite[] pitHeads;       //1 - GG, 2 - RR, 3 - GR, 4 - RG

    public SpriteRenderer sr;

    [Header("Water Mats")]
    public SpriteRenderer waterTopSR;
    public Material waterShader;
    public Vector2 waterVector1;
    public Vector2 waterVector2;
                       
    public WaterFall[] waterFalls;          // 0 - NW, 1 - NE, 2 - SE, 3 - SW
    public bool[] assignedFalls = new bool[4];

    public PreviewTile[] neighbors; //Clockwise from top: 0 - NW (Y-1), 1 - NE (X+1), 2 - SE, 3 - SW

    // Use this for initialization
    void Start () {
        sr = gameObject.GetComponent<SpriteRenderer>();
        waterShader = waterTopSR.material;
        neighbors = MapPreview.mp.getNeighbors(xPos, yPos);
        variantNum = (int)Random.Range(0f, grass.Length);
        subVariant = (int)Random.Range(0f, 10f);
    }
	
	// Update is called once per frame
	void Update () {
        updateSprites();
	}

    //Changes the appearance of the tile
    public void updateSprites()
    {
        checkDecor();

        //Sets tile to hole
        if (tileNum == 0)
        {
            sr.sprite = pit;

            //Checks for water
            foreach(PreviewTile t in neighbors)
            {
                if(t != null)
                {
                    if(t.tileNum == 4 || t.tileNum == 5)
                    {
                        tileNum = 5;
                    }
                }
            }
        }

        //Sets tile to grass
        if (tileNum == 1)
        {
            sr.sprite = grass[variantNum];
        }

        //sets tile to crop
        if (tileNum == 2)
        {
            sr.sprite = seed;
        }


        //sets tile to rock
        if (tileNum == 3)
        {
            int tempNum = variantNum;
            //Ensures there will be no OOB error with this list
            while (tempNum >= rock1.Length)
            {
                tempNum -= rock1.Length - 1;
            }

            sr.sprite = rock1[tempNum];         
        }


        //Sets tile to spring
        if (tileNum == 4)
        {
            sr.sprite = spring;
            springTop.SetActive(true);

            if (!springPart.isPlaying)
            {
                springPart.Play();
            }        
        }
        else
        {
            springTop.SetActive(false);
            springPart.Stop();
        }

        //Water draining
        if (tileNum == 6)
        {
            tileNum = 5;
        }

        //Sets tile to water
        if (tileNum == 5)
        {
            sr.sprite = water1;
            waterTop.SetActive(true);

            waterVector1 = new Vector2(-0.5f, 0.5f);
            waterVector2 = new Vector2(0.25f, -0.25f);
            waterShader.SetVector("_UV1PanSpeed", waterVector1);
            waterShader.SetVector("_UV2PanSpeed", waterVector2);
        }
        else
        {
            waterTop.SetActive(false);
        }
    }

    public void checkDecor()
    {
        //If a grass tile...
        if (tileNum == 1)
        {
            //If subvariant is greater than number of decorations, deactivates decor
            if (subVariant >= GrassDecor.Length && decor.active)
            {
                decor.SetActive(false);
            }

            //Otherwise, sets a bit of decor based on subvariant
            else if (subVariant < GrassDecor.Length)
            {
                decor.SetActive(true);
                decor.GetComponent<SpriteRenderer>().sprite = GrassDecor[subVariant];
            }


        }
        //If a pit tile
        else if (tileNum == 0)
        {
            //If NW neighbor is at a higher elevation
            if (neighbors[0] != null && neighbors[0].tileNum != 0 && neighbors[0].tileNum != 4 && neighbors[0].tileNum != 5 && neighbors[0].tileNum != 6)
            {
                decor.SetActive(true);
                decor.GetComponent<SpriteRenderer>().sprite = pitShadow;
            }
            else
            {
                decor.SetActive(false);
            }
        }

        //If a different tile, removes decor
        else
        {
            decor.SetActive(false);
        }
    }
}
