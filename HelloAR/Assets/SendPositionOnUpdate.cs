using UnityEngine;
using System.Collections;

public class SendPositionOnUpdate : MonoBehaviour {

	public OSC osc;

	// Use this for initialization
	void Start () {
	
	}

    // Update is called once per frame

    public void SendInformation(int ReadyStatus){

        OscMessage message = new OscMessage();
        message.address = "/Ready";
        message.values.Add(ReadyStatus);
        osc.Send(message);

    }


    public void SendInformation (float[] InfoToSend) {
        OscMessage message = new OscMessage();
        print(InfoToSend.Length.ToString());
        message.address = "/Message";
        message.values.Add(InfoToSend[0]);
        message.values.Add(InfoToSend[1]);
        message.values.Add(InfoToSend[2]);
        message.values.Add(InfoToSend[3]);
        osc.Send(message);

    }


}
