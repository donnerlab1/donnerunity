using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Donner;
using UnityEngine.Networking;
using System;

public class lndPlaysController : LndRpcBridge {

    public string hostname;
    public string port;
    string cert;
    string mac;
    bool pressLastButton;
    public bool lastPressTrigger;
    [SerializeField]
    public string lastButton;

    // Use this for initialization
    async void Start() {
        cert = File.ReadAllText(Application.dataPath + "/Resources/tls.cert");

        mac = LndHelper.ToHex(File.ReadAllBytes(Application.dataPath + "/Resources/admin.macaroon"));

        await ConnectToLndWithMacaroon(hostname + ":" + port, cert, mac);
    }

    // Update is called once per frame
    async void Update()
    {
        if (Input.GetButtonUp("a"))
        {
            lastButton = "a";
            StartCoroutine(GetInvoice("a"));
        }
        else if (Input.GetButtonUp("b"))
        {
            lastButton = "b";
            StartCoroutine(GetInvoice("b"));
        }
        else if (Input.GetButtonUp("select"))
        {
            lastButton = "select";
            StartCoroutine(GetInvoice("select"));

        }
        else if (Input.GetButtonUp("start"))
        {
            lastButton = "start";
            StartCoroutine(GetInvoice("start"));

        }
        else if (Input.GetButtonUp("left"))
        {
            lastButton = "left";
            StartCoroutine(GetInvoice("left"));

        }
        else if (Input.GetButtonUp("right"))
        {
            lastButton = "right";
            StartCoroutine(GetInvoice("right"));

        }
        else if (Input.GetButtonUp("up"))
        {
            lastButton = "up";
            StartCoroutine(GetInvoice("up"));

        }
        else if (Input.GetButtonUp("down"))
        {
            lastButton = "down";
            StartCoroutine(GetInvoice("down"));

        }
        if (lastPressTrigger)
        {
            lastPressTrigger = false;
            pressLastButton = (pressLastButton) ? false : true;

            if (pressLastButton)
            {
                Debug.Log("start coroutine  " + pressLastButton);
                StartCoroutine(ContinuuosPress(lastButton, pressLastButton));
            } else
            {
                Debug.Log("stop coroutine  " + pressLastButton);
                StopCoroutine(ContinuuosPress(lastButton,pressLastButton));
            }
            
            
        }
    }
    IEnumerator GetInvoice(string button)
    {
        
        UnityWebRequest www = UnityWebRequest.Get("http://lnplays.com/getInvoice/" + button);
        yield return www.SendWebRequest();

        if (www.isNetworkError)
        {
            Debug.Log(www.error);
        }
        else
        {
            // Show results as text
            //Debug.Log(www.downloadHandler.text);
            var invoice = JsonUtility.FromJson<InvoiceJson>(www.downloadHandler.text);
            Debug.Log(invoice.data.invoice);

            yield return SendPayment(invoice.data.invoice);

        }
    }

    IEnumerator ContinuuosPress(string button, bool started)
    {
        
            


        while (pressLastButton)
        {
            UnityWebRequest www = UnityWebRequest.Get("http://lnplays.com/getInvoice/" + button);
            yield return www.SendWebRequest();
            var invoice = JsonUtility.FromJson<InvoiceJson>(www.downloadHandler.text);
            yield return SendPayment(invoice.data.invoice);
            yield return new WaitForSeconds(0.1f);
        }
}
}

[Serializable]
public class InvoiceJson
{
    public InvoiceDataJson data;
    public bool succes;
}

[Serializable]
public class InvoiceDataJson
{
    public string invoice;
    public string buttonPressed;
    public int amountInSat;
}
