using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireController : MonoBehaviour {

    public Animator anim;
    public MapTile parent;

	// Use this for initialization
	void Start () {
        anim = gameObject.GetComponent<Animator>();
        anim.SetBool("Dying",false);
	}
	
	// Update is called once per frame
	void Update () {

        //...If the fire in the parent has burned out...
		if(parent != null && parent.tileNum != 7)
        {
            anim.SetBool("Dying", true);
        }

        //...If the animation has reached it's final stage...
        if(anim.GetCurrentAnimatorStateInfo(0).IsName("Empty"))
        {
            Destroy(gameObject);
        }
	}
}
