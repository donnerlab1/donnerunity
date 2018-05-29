using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Donner;
using System.Threading.Tasks;

public class WeatherLndClient : LndRpcBridge {

    public string hostname;
    public string port;
    string cert;
    string mac;

    public GameObject[] particleEffects;
    public WindZone windZone;
    public GameObject Sphere;

    

    // Use this for initialization
    async void Start () {
        cert = File.ReadAllText(Application.dataPath + "/Resources/tls.cert");

        mac = LndHelper.ToHex(File.ReadAllBytes(Application.dataPath + "/Resources/admin.macaroon"));

        await ConnectToLndWithMacaroon(hostname + ":" + port, cert,mac);
        OnInvoiceSettled += new InvoiceSettledEventHandler(ChangeWeather);

        //ListenInvoices();
        SubscribeInvoices();
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


