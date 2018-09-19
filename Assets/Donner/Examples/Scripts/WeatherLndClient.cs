using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Donner;
using System.Threading.Tasks;
using System;

public class WeatherLndClient : LndRpcBridge {

    public string confname;
    public string hostname;
    public string port;
    public string certFile;
    public string macaroonFile;
    public string pubkey { get; private set; }
    public bool readConfig;
    string cert;
    string mac;
    public GameObject[] particleEffects;
    public WindZone windZone;
    public GameObject Sphere;

    LndConfig config;

    // Use this for initialization
    async void Start () {
        LndHelper.SetupEnvironmentVariables();
        if (readConfig)
        {
            config = LndHelper.ReadConfigFile(Application.dataPath + "/Resources/"+confname);
        }
        else
        {
            config = new LndConfig { Hostname = hostname, Port = port, MacaroonFile = macaroonFile, TlsFile = certFile };
        }
        if(config.Neutrino)
        {
            var neutrino = gameObject.AddComponent<NeutrinoTest>() as NeutrinoTest;
            neutrino.StartLnd(config);
            NeutrinoUnlock();
        }else
        {

            cert = File.ReadAllText(Application.dataPath + "/Resources/" + config.TlsFile);

            mac = LndHelper.ToHex(File.ReadAllBytes(Application.dataPath + "/Resources/" + config.MacaroonFile));
            await ConnectToLndWithMacaroon(config.Hostname + ":" + config.Port, cert, mac);
            OnInvoiceSettled += new InvoiceSettledEventHandler(ChangeWeather);

            SubscribeInvoices();

            var getInfo = await GetInfo();
            pubkey = getInfo.IdentityPubkey;
        }
        
        

       
        
    }
    public async void NeutrinoUnlock()
    {
        cert = File.ReadAllText(Application.dataPath + "/Resources/" + config.TlsFile);
        mac = "";
        try
        {
            mac = LndHelper.ToHex(File.ReadAllBytes(Application.dataPath + "/Resources/" + config.MacaroonFile));
        }
        catch (Exception e)
        {
            UnityEngine.Debug.Log(e);

        }
        await ConnectToLndWithMacaroon(config.Hostname + ":" + config.Port, cert, mac);
        var seed = await GenerateSeed();
        var s = await UnlockWallet("suchwowmuchhey", seed);


        await ConnectToLndWithMacaroon(config.Hostname + ":" + config.Port, cert, mac);
        var getinfo = await GetInfo();
        Debug.Log(getinfo.IdentityPubkey);
    }
    void ChangeWeather(object sender, InvoiceSettledEventArgs e)
    {
        Debug.Log(e.Invoice.Memo);
        switch (e.Invoice.Memo) {
            case ("rain"):
                ActivateEffect(0);
                break;
            case ("fire"):
                ActivateEffect(1);
                break;
            case ("wind"):
                SetWindZone((int)e.Invoice.Value);
                break;

        }

    }
  
    void ActivateEffect(int index)
    {
        foreach (var ps in particleEffects)
            ps.SetActive(false);
        particleEffects[index].SetActive(true);
    }
    void SetWindZone(int amount)
    {
        windZone.windMain = amount;
    }

	
	// Update is called once per frame
	void Update () {
		
	}

    public async Task<string> GetWeatherInvoice(string weatherType, int satAmount)
    {
        var s = "";
        switch (weatherType)
        {
             
            case ("rain"):
                s = await AddInvoice(5, "rain");
                break;
            case ("fire"):
                s = await AddInvoice(10, "fire");
                break;
            case ("wind"):
                s = await AddInvoice(satAmount, "wind");
                break;
            default:
                s = "error in request";
                break;
        }

        Debug.Log(s);
        return s;
    }


}


