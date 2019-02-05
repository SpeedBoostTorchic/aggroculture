using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteChanger : MonoBehaviour {

    public SpriteRenderer parent;
    public SpriteRenderer[] children;
    public Color lastColor;
	
	// Changes color of children to match parent
	void Update () {

        //If there ARE children, and their color doesn't match the parents'
		if(children.Length > 0 && parent.color != lastColor)
        {
            //Changes the color of each of the children
            foreach (SpriteRenderer s in children)
            {
                s.color = parent.color;
            }

            //Marks the previous color used, so this isn't called many times
            lastColor = parent.color;
        }
	}
}
