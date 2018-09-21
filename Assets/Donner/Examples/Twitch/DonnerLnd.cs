using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Donner;
using System.IO;

public class DonnerLnd : LndRpcBridge {

    public string confname;
    public string hostname;
    public string port;
    public string certFile;
    public string macaroonFile;
    public string pubkey { get; private set; }
    public bool readConfig;

    LndConfig config;
    string cert;
    string mac;
    // Use this for initialization
    async void Start () {
        LndHelper.SetupEnvironmentVariables();
        if (readConfig)
        {
            config = LndHelper.ReadConfigFile(Application.dataPath + "/Resources/" + confname);
        }
        else
        {
            config = new LndConfig { Hostname = hostname, Port = port, MacaroonFile = macaroonFile, TlsFile = certFile };
        }
        

            cert = File.ReadAllText(Application.dataPath + "/Resources/" + config.TlsFile);

            mac = LndHelper.ToHex(File.ReadAllBytes(Application.dataPath + "/Resources/" + config.MacaroonFile));
            await ConnectToLndWithMacaroon(config.Hostname + ":" + config.Port, cert, mac);
            
            SubscribeInvoices();

            var getInfo = await GetInfo();
            pubkey = getInfo.IdentityPubkey;
        Debug.Log(pubkey);
    }
	
}
