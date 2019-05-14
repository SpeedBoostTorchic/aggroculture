using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//AS many text elements have additional text boxes behind
//them to make them stand out, this bit of code ensures that
//tautomates the process of changing children to match
public class TextChanger : MonoBehaviour {

    //Text box parent and its child
    public Text parent;
    public Text[] children;

    //
    public bool movingText;
    public GameObject movingObj;
    private Vector3 basePos;
    private Vector3 tarPos;
    public float maxDist;
    private float time;

    void Start()
    {
        //Takes default position of the moving text
        if (movingText)
        {
            basePos = gameObject.transform.position;
            tarPos = movingObj.transform.position = basePos + new Vector3(Random.Range(-maxDist, maxDist), Random.Range(-maxDist, maxDist), 0f);
        }
    }

    // Changes all children to match their parent
    void Update () {

		foreach(Text c in children)
        {
            c.text = parent.text;
        }

        //Randomly changes the text position 
        if (movingText)
        {
            basePos = gameObject.transform.position;

            //If it has reached it's position...
            if (Vector3.SqrMagnitude(movingObj.transform.position - tarPos) < 0.1f|| time > 1.0f)
            {
                tarPos = movingObj.transform.position = basePos + new Vector3(Random.Range(-maxDist, maxDist), Random.Range(-maxDist, maxDist), 0f);
                time = 0f;

            }
            else
            {
                movingObj.transform.position = Vector3.Lerp(tarPos, movingObj.transform.position, time);
                time += Time.unscaledDeltaTime;
            }
            
        }
	}

    //Because children are in front of parent, changing
    //text color will go through this function
    public void changeColor(Color c)
    {
        children[0].color = c;
    }
}
