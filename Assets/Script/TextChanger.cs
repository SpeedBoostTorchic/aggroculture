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

	// Changes all children to match their parent
	void Update () {
		foreach(Text c in children)
        {
            c.text = parent.text;
        }
	}

    //Because children are in front of parent, changing
    //text color will go through this function
    public void changeColor(Color c)
    {
        children[0].color = c;
    }
}
