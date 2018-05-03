using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using Donner;

public class PlayerLnd : LndRpcBridge {

    public string hostname;
    public string port;
    string cert;

	public Text lastMessage;
	public Text channelBalance;

	public Transform instantiatePoint;

	public GameObject TipSphere;

	// Use this for initialization
	async void Start () {
		cert = File.ReadAllText(Application.dataPath + "/Resources/tls.cert");
        Debug.Log(cert);
        await ConnectToLnd(hostname + ":" + port, cert);
		OnInvoiceSettled += new InvoiceSettledEventHandler(PaymentReceived);
		SubscribeInvoices();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public async void PayInvoice(string request) {
		var s = await SendPayment(request);
		UpdateUI();
		Debug.Log(s);
	}

	void PaymentReceived(object sender, InvoiceSettledEventArgs e) {
		Debug.Log(e.Invoice);
		SpawnInvoice();
		UpdateLastMessage(e.Invoice.Memo);
		UpdateUI();
	}

	void SpawnInvoice() {
		var pos = instantiatePoint.position;
		pos.x += Random.Range(-5,5);
		pos.y += Random.Range(3,-10);
		pos.z += Random.Range(-5,5);
		var obj = Instantiate(TipSphere, pos, Quaternion.identity);
	}

	async void UpdateUI() {
		var s = await ChannelBalance();
		channelBalance.text = "Balance: "+ s.ToString();
	}

	void UpdateLastMessage(string text) {
		lastMessage.text = text;
	}
}
