using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using Donner;


public class PublicLndInfo : NetworkBehaviour {


    [SyncVar]
    public string pubkey;
    [SyncVar]
    public string externalIp;
    [SyncVar]
    public int port;

    LndRpcClient rpcClient;

    public override void OnStartLocalPlayer()
    {
        GetComponent<LndPlayer>().Connect();
    }
    // Use this for initialization
    void Start () {
        if (!isLocalPlayer)
        {
            Destroy(GetComponent<LndRpcBridge>());
            return;
        }
        Setup();
	}
	
    async void Setup()
    {
        rpcClient = GetComponent<LndRpcClient>();
        var getInfo = await rpcClient.rpc.GetInfo();
        pubkey = getInfo.IdentityPubkey;
        StartCoroutine(GetPublicIP());
    }

    IEnumerator GetPublicIP()
    {
        using (WWW www = new WWW("https://ipv4.myexternalip.com/raw"))
        {
            yield return www;
            externalIp = www.text;

        }
    }

    [TargetRpc]
    public void Target_GetPaymentRequest(NetworkConnection self, NetworkIdentity payer, int amt, string memo)
    {
        var task = rpcClient.rpc.AddInvoice(amt, memo);
        task.RunSynchronously();
        Cmd_PayUserSecondStep(payer, task.Result);
    }

    [TargetRpc]
    public void Target_Pay(NetworkConnection self, string payreq)
    {
        var task = rpcClient.rpc.SendPayment(payreq);
    }

    [Command]
    public void Cmd_PayUser(NetworkIdentity payer, NetworkIdentity payee, int amt, string memo)
    {
        Target_GetPaymentRequest(payee.connectionToServer, payer, amt, memo);
    }
	
    [Command]
   public void Cmd_PayUserSecondStep(NetworkIdentity payer, string payreq)
    {
        Target_Pay(payer.connectionToServer, payreq);
    }
}
