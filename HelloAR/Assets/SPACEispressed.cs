using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SPACEispressed : MonoBehaviour {
    public TextMesh infodisplay;
	// Use this for initialization
	void Start () {
        infodisplay = GameObject.Find("infodisplay").GetComponent<TextMesh>();
	}
	
	// Update is called once per frame
	void Update () {
        if(Input.touchCount < 1)
        {
            return;
        }
        Touch touch;
        touch = Input.GetTouch(0);
        if (touch.phase.Equals(TouchPhase.Began))
        {
            var t = GameObject.Find("Telephone");
            float[] myarray = new float[4];
            myarray[0] = touch.position.x;
            myarray[1] = touch.position.y;
            myarray[2] = 0f;
            myarray[3] = 0f;

            t.GetComponent<SendPositionOnUpdate>().SendInformation(myarray);
        }
    }
       
}
