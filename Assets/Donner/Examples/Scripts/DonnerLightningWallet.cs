using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Donner;
using System.IO;

public class DonnerLightningWallet : LndRpcBridge {

    public string pubkey;
    string cert;
    string mac;
    LndConfig config;
    // Use this for initialization
    async void Start() {
        config = new LndConfig{Hostname = "localhost", Port = "10006"};
        var appdata = System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData);
        Debug.Log(appdata);
        cert = File.ReadAllText(appdata + "/lightning-app/lnd/tls.cert");
        mac = LndHelper.ToHex(File.ReadAllBytes(appdata + "/lightning-app/lnd/data/chain/bitcoin/testnet/admin.macaroon"));
        await ConnectToLndWithMacaroon(config.Hostname + ":" + config.Port, cert, mac);

        SubscribeInvoices();

        var getInfo = await GetInfo();
        pubkey = getInfo.IdentityPubkey;
        Debug.Log(pubkey);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
