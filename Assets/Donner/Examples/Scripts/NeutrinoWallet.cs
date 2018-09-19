using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Donner;
using System.IO;
using System;

public class NeutrinoWallet : LndRpcBridge {

    LndConfig config;
    public NeutrinoTest lnd;
    public string[] seed;

    public bool getInfoTrigger;
	// Use this for initialization
	async void Start () {
        config = new LndConfig()
        {
            Hostname = "localhost",
            Port = "10013",
            MacaroonFile = "/Neutrino/neutrinoadmin.macaroon",
            TlsFile = "/Neutrino/neutrino.cert"
        };

        string cert = File.ReadAllText(Application.dataPath + "/Resources/" + config.TlsFile);
        string mac = "";
        try
        {
            mac = LndHelper.ToHex(File.ReadAllBytes(Application.dataPath + "/Resources/" + config.MacaroonFile));
        } catch(Exception e)
        {
            UnityEngine.Debug.Log(e);
            
        }
        lnd.StartLnd(config);

        await ConnectToLndWithMacaroon(config.Hostname + ":" + config.Port, cert, mac);
        seed = await GenerateSeed();
        var s = await UnlockWallet("suchwowmuchhey", seed);
        Debug.Log("s");

        await ConnectToLndWithMacaroon(config.Hostname + ":" + config.Port, cert, mac);
        var getinfo = await GetInfo();
        Debug.Log(getinfo.IdentityPubkey);
    }
	
	// Update is called once per frame
	async void Update () {
		if(getInfoTrigger)
        {
            getInfoTrigger = false;
            var getinfo = await GetInfo();
            Debug.Log(getinfo.ToString());
        }
	}
}
