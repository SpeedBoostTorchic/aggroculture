using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterFall : MonoBehaviour {

    //This is their parent maptile - this waterfall will be destroyed if it 
    //isn't a water tile
    public MapTile m;

    public ParticleSystem thisP;
    public Vector3 offset;
    public Vector3 rotation;

	// Adjusts position & roation of the object
	void Start () {
        thisP = this.gameObject.GetComponent<ParticleSystem>();
        this.transform.position += offset;
        this.transform.rotation = Quaternion.Euler(rotation);
        thisP.Play();
	}
	
	// Update is called once per frame
	void Update () {

        //If the map tile doesn't exist, or is not water...
		if(m == null || m.tileNum != 5)
        {
            thisP.Stop();
        }

        //If particle and all child particles aren't emitting, destroy this
        if (!thisP.IsAlive(true))
        {
            Destroy(gameObject);
        }
	}
}
