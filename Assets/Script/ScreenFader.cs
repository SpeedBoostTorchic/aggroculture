using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class ScreenFader : MonoBehaviour {

    //Variables used for fading.
    public float curAlpha;
    public float fadeSpeed;
    public bool fadeIn;
    public Image im;

    //This is the scene this thing will transition to...
    public int targetScene;

	// Use this for initialization
	void Awake () {
        im = gameObject.GetComponent<Image>();

        //If fading in, sets alpha above 1f
        if (fadeIn)
        {
            //curAlpha = 1.5f;
        }
	}
	
	// Update is called once per frame
	void Update () {

        //Sets image color to the current alpha
        im.color = new Color(im.color.r, im.color.g, im.color.b, curAlpha);

        //Fading into this scene from another scene...
        if (fadeIn)
        {
            //If fader still visible, lower the alpha
            if(curAlpha > 0f)
            {
                curAlpha -= Time.deltaTime * fadeSpeed;
            }
        }

        //Fading out from this scene to go to another
        else if(!fadeIn && targetScene != null)
        {
            //Gradually raises alpha
            if (curAlpha < 1.5f)
            {
                curAlpha += Time.deltaTime * fadeSpeed;
            }
            else
            {
                SceneManager.LoadScene(targetScene);
            }
        }
		
	}
}
