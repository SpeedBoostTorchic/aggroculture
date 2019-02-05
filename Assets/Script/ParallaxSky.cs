using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxSky : MonoBehaviour {

    public CameraControl cam;
    public Vector3 basicOffset;
    public Vector2 mapMidpoint;
    public float sizeMult;


    [Header("X Parallax Variables")]
    public float maxDist;
    public float imageWidth;
    public float offsetWidth;


    [Header("Y Parallax Variables")]
    public GameObject[] layers;             //Layers are arranged from frontmost to back
    public float heightDiff;
    public float maxHeight;
    public float heightOffset;

	// Use this for initialization
	void Start () {
        //sizeMult = cam.zoomTar / (cam.zoomMax-cam.zoomMin);
	}
	
	// Update is called once per frame
	void Update () {

        //Scales with the way the camera zooms
        //transform.localScale = new Vector3(sizeMult, sizeMult, sizeMult);
        sizeMult = cam.zoomTar / (cam.zoomMax - cam.zoomMin);

        //Basic posiion is determined by the following formula:
        this.transform.position = cam.transform.position + basicOffset + new Vector3(offsetWidth,0f,0f);

        //Offset width is found by calculating the percentage of the screen
        //the camera has currently traveled accross
        float distpercent = (cam.transform.position.x - mapMidpoint.x)/maxDist;
        offsetWidth = imageWidth / 2 * distpercent;

        //Height offset is determined by distance between maxheight and the current cam
        if(cam.transform.position.y < maxHeight && cam.transform.position.y > -maxHeight)
        {
            float tempMult = (cam.transform.position.y + maxHeight) / (maxHeight * 2);
            heightOffset = tempMult * (heightDiff - 1f) + 1f;
        }
        //If camera is greater than or equal to the max, offset is set to maximum
        else if(cam.transform.position.y >= maxHeight)
        {
            heightOffset = heightDiff;
        }
        //If camer is less than or equal to the min, offset is set to minimum
        else if(cam.transform.position.y <= -maxHeight)
        {
            heightOffset = 1f;
        }

        //Then, applies the heigh difference to all layers one by one
        for(int i = 0; i < layers.Length; i++)
        {
            Vector3 tempPos = new Vector3(0f, heightOffset * i, 0.1f * i);
            layers[i].transform.localPosition = tempPos;
        }
    
	}
}
