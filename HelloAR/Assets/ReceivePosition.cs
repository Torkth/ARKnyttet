using UnityEngine;
using System.Collections;
using GoogleARCore;
using GoogleARCore.HelloAR;

public class ReceivePosition : MonoBehaviour {
    
   	public OSC osc;

    // Use this for initialization
    void Start () {
	   osc.SetAddressHandler( "/Message" , UnpackMessage );
        osc.SetAddressHandler("/Ready", UnpackReady);
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    void UnpackReady(OscMessage message)
    {
        int x = message.GetInt(0);
        //GameObject.Find("ExampleController").GetComponent<HelloARController>().getReady(x);

    }

    void UnpackMessage(OscMessage message){

        float w = message.GetFloat(0);

        float x = message.GetFloat(1);
        float y = message.GetFloat(2);
        float z = message.GetFloat(3);
        GameObject.Find("ExampleController").GetComponent<HelloARController>().CreateItemFromMultiplayer(w,x,y,z);

	}
    
}
