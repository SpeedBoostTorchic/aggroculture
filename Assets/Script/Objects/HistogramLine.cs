using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HistogramLine : MonoBehaviour {

    //Takes an instance of itself to affect
    private LineRenderer self;

    //These values determine the shape/size of the line
    public float[] values;
    public float maxX;
    public float xAdjust;       //Max Position/Total position count
    public float zPos;
    public float delayMax;

    //Used to time the display
    public int count;
    public float delay;

	// Use this for initialization
	void Start () {
        self = this.gameObject.GetComponent<LineRenderer>();


        //Used for TESTING ONLY!!!!
        values = generateValues(90);
        initialize(values);
	}

    //Called when the line is first spawned by game manager
    public void initialize(float[] v)
    {
        values = v;
        self.positionCount = v.Length;
        xAdjust = maxX / v.Length;
        delay = delayMax;
        count = 0;
    }
	
	// Update is called once per frame
	void Update () {

        //While there are still values to display
        if(count < values.Length)
        {
            //If the timer is below zero
            if (delay <= 0)
            {
                //Multiplies position adjustment by current count, sets Y value to position index, and gives it a constant for zPos
                Vector3 temp = new Vector3(xAdjust * count, values[count], zPos);

                //Changes all positions after current one to match new position so it looks like the line is increasing slowly
                for (int i = count; i < self.positionCount; i++)
                {
                    self.SetPosition(i, temp);
                }

                //Increments count, then resets the timer
                count++;
                delay = delayMax;
            }
            else
            {
                delay -= Time.deltaTime;
            }
        }	
	}

    //USED FOR TESTING ONLY!!!
    public float[] generateValues(int num)
    {
        float[] temp = new float[num];

        for(int i = 0; i < num; i++)
        {
            temp[i] = Random.Range(-1f, 2.3f);
        }

        return temp;
    }
}
