using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class uiWarningFlash : MonoBehaviour {

    //Key variables for the animation
    public Color targetColor;
    public float lerpSpeed;
    public float lifeTime;
    private Image im;

    //Takes current image component
    void Start()
    {
        im = gameObject.GetComponent<Image>();
    }

    // Update is called once per frame
    void Update () {

        //Will destroy object after a certain amount of time
        lifeTime -= Time.deltaTime;
        if(lifeTime <= 0)
        {
            Destroy(gameObject);
        }

        //Ping-Pongs between normal and adjusted color
        im.color = Color.Lerp(Color.white, targetColor, Mathf.PingPong(Time.time * lerpSpeed, 1));
	}
}
