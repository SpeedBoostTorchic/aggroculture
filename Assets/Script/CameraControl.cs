using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour {


    public int camNum;
    public Camera thisCam;
    public float camHeight;

    //Camera movement speed conrtrols
    public float zoomSpeed;
    public float zoomTar;
    public float smoothSpeed;
    public float zoomMin;
    public float zoomMax;
    public float moveSpeed;

    //Used for the lerpCam function
    public Vector2 orig;
    public Vector3 orig3;
    public float speed;
    public bool l;

    //Used for end-round scrolling
    public Vector3 neutralPos;
    public Vector3 endPos;
    private Transform topTile;
    public float height;
    public float width;
    private float prevSize;

    private void Awake()
    {
        thisCam = this.gameObject.GetComponent<Camera>();
    }

    // Use this for initialization
    void Start () {
        Camera.main.orthographicSize = zoomMax - zoomMin;
        zoomTar = Camera.main.orthographicSize;
      

        topTile = GameManager.gm.curMap[0, GameManager.gm.mapSize - 1].gameObject.transform;
	}
	
	// Update is called once per frame
	void Update () {

        if (!l)
        {
            orig = transform.position;
            orig3 = transform.position;
        }

        /*/Zooms the camera in if the player uses the mouse wheel
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if(scroll != 0f)
        {
            zoomTar -= zoomSpeed * scroll;
            zoomTar = Mathf.Clamp(zoomTar, zoomMin, zoomMax); //Clamps amount of zoom
        }*/

        //Adjusts camera zoom to meet the zoom target
        thisCam.orthographicSize = Mathf.MoveTowards(Camera.main.orthographicSize, zoomTar, smoothSpeed * Time.deltaTime);

        //Adjusts end positions if the camera's orthographic size changes
        if(prevSize != thisCam.orthographicSize)
        {
            height = thisCam.orthographicSize * 2;
            width = height * thisCam.aspect;

            //Position differs depending on which player's cam it is
            if (camNum == 1)
            {
                neutralPos = new Vector3(topTile.position.x - width / 2, -topTile.position.y, camHeight);
                endPos = new Vector3(topTile.position.x - width / 2, topTile.position.y, camHeight);
            }
            else
            {
                neutralPos = new Vector3(topTile.position.x + width / 2, -topTile.position.y, camHeight);
                endPos = new Vector3(topTile.position.x + width / 2, topTile.position.y, camHeight);
            }

            //Adjusts previous size
            prevSize = thisCam.orthographicSize;
        }

    }

    public void lerpCam(Vector2 dest)
    {

        //(Vector2.Distance(orig,dest)-Vector2.Distance(transform.position,dest))+0.1f
        Vector2 temp = Vector2.Lerp(transform.position, dest, speed * Time.deltaTime);
        gameObject.transform.position = new Vector3(temp.x, temp.y, camHeight);
    }
}
