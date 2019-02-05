using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FloatingText : MonoBehaviour {

    //Basic info regarding the text
    public string text;
    public Text parent;
    public Text[] children;

    public float speed;         //How fast the text moves
    public float lifetime;      //How long the text lives

	// Use this for initialization
	void Start () {

        //Sets all text boxes to match the message
        parent.text = text;
        foreach(Text t in children)
        {
            t.text = text;
        }
	}
	
	// Update is called once per frame
	void Update () {

        //Destroys self after a set period of time
        lifetime -= Time.deltaTime;
        if(lifetime <= 0)
        {
            Destroy(gameObject);
        }

        //Moves self based o speed
        Vector3 newPos = new Vector3(transform.position.x, transform.position.y + speed*Time.deltaTime, transform.position.z);
        transform.position = newPos;
	}
}
