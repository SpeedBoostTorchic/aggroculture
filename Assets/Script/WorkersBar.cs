using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class WorkersBar : MonoBehaviour {

    [Header("Essential Attributes")]
    public int numWorkers;          //Maximum number of workers
    public float curWorkers;          //Current used worker count
    public int usedWorkers;
    public bool adjustPos;

    //Transform adjustment information
    [Header("Mid-Round Changes")]
    public float scaleSize;
    public float scaleSpeed;
    public float curWidth;
    public float defaultWidth;
    public Vector2 defaultPos;
    public Vector2 curPos;
    public Vector2 pipStartPos;
    private int numPips;
    public List<GameObject> pipList = new List<GameObject>();

    //Visuals/GameObjects
    [Header("Sprites/Visuals")]
    public Image end;               //The end of the bar
    public GameObject pip;          //Representation of a single woker on the bar
    public Image bar;

    //Sprites
    public Image pic;
    public Sprite norm;
    public Sprite empty;

    //End of each round...
    [Header("End-of-Round Changes")]
    public float growTimer;
    private float t;
    public float size;
    public Vector2 trans;
    public bool p1;

	// Grabs default width, and uses it as reference
	void Awake () {
        defaultWidth = bar.rectTransform.sizeDelta.x;
        defaultPos = bar.rectTransform.localPosition;
        growTimer = 3f;
        size = 1.0f;
        trans = gameObject.GetComponent<RectTransform>().anchoredPosition;
	}
	
	// Update is called once per frame
	void Update () {

        if (adjustPos)
        {
            //Slowly adjusts current number of workers (thus adjusting bar)
            if((int) curWorkers < numWorkers)
            {
                curWorkers += Time.deltaTime * scaleSpeed;

                //If the number of workers exceeds the pip amount, increase them
                if((int) curWorkers > numPips)
                {
                    //Vector3 temp = new Vector3 (scaleSize * numPips, 0f, -1f) + (Vector3) defaultPos;
                    Transform p = Instantiate(pip, this.transform).transform;

                    //If there are workers used, it will be set to dark
                    if (usedWorkers <= 0)
                    {
                        p.GetComponent<Pip>().active = true;
                    }

                    //Sets the this to pips parent, and draws it appropriately
                    p.SetParent(this.transform);
                    p.GetComponent<RectTransform>().anchoredPosition = new Vector2(scaleSize * numPips, 0f) + pipStartPos;
                    pipList.Add(p.gameObject);
                    numPips += 1;
                }
            }
            else if ( Mathf.Ceil(curWorkers) -1 > numWorkers)
            {
                curWorkers -= Time.deltaTime * scaleSpeed;

                //Otherwise, remove them from the list
                if ((int)curWorkers < numPips)
                {

                    //pipList.Count-1
                    GameObject temp = pipList[pipList.Count - 1];
                    Destroy(temp);
                    pipList.RemoveAt(pipList.Count - 1);
                    numPips -= 1;
                }
            }

            //Changes dimentions of the worker bar to fit the number of workers
            curWidth = scaleSize * (curWorkers+1.8f);
            curPos = new Vector2(curWidth / 2, defaultPos.y);
            bar.rectTransform.anchoredPosition = curPos;
            bar.rectTransform.sizeDelta = new Vector2(curWidth, bar.rectTransform.sizeDelta.y);
            end.rectTransform.anchoredPosition = new Vector2(16 + curPos.x + curWidth / 2, end.rectTransform.anchoredPosition.y);
        }

        //Deactivates pips equal to the number of used workers
        int num = 0;  
        for (int i = pipList.Count - 1; i >= 0; i--)
        {
             if(num < usedWorkers)
            {
                pipList[i].GetComponent<Pip>().active = false;
                num++;
            }
            else
            {
                pipList[i].GetComponent<Pip>().active = true;
            } 
        }

        //Changes appearnce based on number of workers
        if(usedWorkers == numWorkers)
        {
            pic.sprite = empty;
        }
        else
        {
            pic.sprite = norm;
        }

        //Resets the growth timer
        if (!GameManager.gm.gameStart && GameManager.gm.roundFinish)
        {
            growTimer = 3f + 0.5f* (GameManager.gm.numGames - 2);
        }

        //Initiates growth animation
        if (!GameManager.gm.roundFinish && !GameManager.gm.gameStart)
        {
            //At the end of each round, has the thing grow large and then shrink
            if(t < 1.0f && growTimer > 0)
            {
                t += Time.deltaTime * scaleSpeed;
                size = Mathf.Lerp(1.0f, 3.0f - 0.33f*(GameManager.gm.numGames -2), t);
            }
            else if (t >- 1.0f && growTimer > 0)
            {
                growTimer -= Time.deltaTime;
            }
            else if (growTimer <= 0 && t > 0)
            {
                t -= Time.deltaTime * scaleSpeed;
                size = Mathf.Lerp(1.0f, 3.0f - 0.33f * (GameManager.gm.numGames - 2), t);
            }else if (t < 0)
            {
                t = 0f;
            }

            //Scales the size of all associated objects below
            gameObject.transform.localScale = new Vector3(size, size, 1f);

            //Adjusts position of the new bar to match its new size
            if (p1)
            {
                gameObject.GetComponent<RectTransform>().anchoredPosition = trans + new Vector2((size-1) * -scaleSize * 4, (size - 1) * 8);
            }
            else
            {
                gameObject.GetComponent<RectTransform>().anchoredPosition = trans + new Vector2((size-1) * scaleSize * 4, (size - 1) * 8);
            }
        }

        //Resets back to normal position
        else
        {
            gameObject.GetComponent<RectTransform>().anchoredPosition = trans;
        }
	}
}
