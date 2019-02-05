using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarningMarker : MonoBehaviour {

    //These values determine the location of the warning
    public Transform target;
    public PlayerClass p;
    private Vector2 tarVec;
    private Vector2 orVec;
    private Vector2 dir;
    private float angle;
    public float timer;
    public Color warningColor;
    public float flashSpeed;

    //The Game Objects in question
    public float maxDist;
    public float curDist;
    public GameObject warning;
    public GameObject pointer;

	// Used for testing purposes ONLY
	void Start () {
        if(target != null && p != null)
        {
            tarVec = target.position;
            orVec = p.pCam.transform.position;


            if (p.pNum == 1)
            {
                gameObject.layer = 8;
                pointer.layer = 8;
            }
            else
            {
                gameObject.layer = 9;
                pointer.layer = 9;
            }
        }
    }
	
	// Reallocates to match new position if camera moves
	void Update () {

        //Destroys self after a set period of time
        timer -= Time.deltaTime;
        if(timer <= 0f)
        {
            Debug.Log("Destroyed");
            Destroy(gameObject);
        }

        //Causes the warning message to flash
        gameObject.GetComponent<SpriteRenderer>().color = Color.Lerp(Color.white, warningColor, Mathf.PingPong(Time.time * flashSpeed, 1));
        pointer.GetComponent<SpriteRenderer>().color = gameObject.GetComponent<SpriteRenderer>().color;

        //updates the position of the player's camera
        if (target != null && p != null)
        {
            orVec = p.pCam.transform.position;
            //tarVec = target.position - new Vector3(0f,1f,0f);
        }

        //Finds the distance to the object. Will clamp to MaxDist
        curDist = Vector2.Distance(orVec, tarVec);
        curDist = Mathf.Clamp(curDist, -maxDist, maxDist);

        //Calculates the position of the warning symbol and the pointer according to the current distance
        dir = findMidpoint();
        warning.transform.position = orVec + (dir * curDist / 2);
        pointer.transform.position = orVec + (dir * curDist);
        warning.transform.position = new Vector3(warning.transform.position.x, warning.transform.position.y, -5f);
        pointer.transform.position = new Vector3(pointer.transform.position.x, pointer.transform.position.y, -5f);

        //Adjusts the scale of both symbols to match the current zoom value of the player's camera
        float t = (p.pCam.zoomTar / p.pCam.zoomMin / 1.6f);
        warning.transform.localScale = new Vector3(t,t,t);

        //Changes the angle of the pointer to face the target
        Vector2 pointFace = tarVec - orVec;
        angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        angle = angle - 90f;
        Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);
        pointer.transform.rotation = q;
    }

    //must be called after instantiation
    public void initialize(Transform t, PlayerClass x)
    {
        target = t;
        p = x;
        tarVec = target.position - new Vector3(0f, 1f, 0f);
        orVec = p.pCam.transform.position;

        //Sets the layer to the appropriate player
        if(p.pNum == 1)
        {
            gameObject.layer = 8;
            pointer.layer = 8;
        }
        else
        {
            gameObject.layer = 9;
            pointer.layer = 9;
        }
        dir = findMidpoint();
    }

    //Finds the midpoint between the Player CAM and the target point
    //then returns the normalized vector
    public Vector2 findMidpoint()
    {
       Vector2 temp = tarVec - orVec;
       temp.Normalize();
       return temp;
    }
}
