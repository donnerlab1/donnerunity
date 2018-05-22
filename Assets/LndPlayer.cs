using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Donner;

public class LndPlayer : LndRpcBridge {

    public string hostname;
    public string port;
    public string filename;
    string cert;
    string mac;
    // Use this for initialization
    async void Start () {
        
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public async void Connect()
    {
        LndHelper.SetupEnvironmentVariables();
        cert = File.ReadAllText(Application.dataPath + "/Resources/" + filename + ".cert");

        mac = LndHelper.ToHex(File.ReadAllBytes(Application.dataPath + "/Resources/admin.macaroon"));

        await ConnectToLndWithMacaroon(hostname + ":" + port, cert, mac);
        var s = await GetInfo();
        Debug.Log(s.ToString());
    }
}
