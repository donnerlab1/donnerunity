using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Bounty : NetworkBehaviour {


    public Text bountyText;

    [SyncVar(hook = "OnChangeBounty")]
    public int bounty = 0;

    void OnChangeBounty(int bounty)
    {

        bountyText.text = "Bounty: " + bounty;
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
