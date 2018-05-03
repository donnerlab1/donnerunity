
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using Donner;

public class SimpleLndWallet : LndRpcBridge
{

    public string hostname;
    public string port;
    public string filename;
    string cert;
    public InputField pwInput;
    public Text balanceOutput;
    public Text chanBalanceOutput;
    public InputField addrOutput;

    public InputField ChanPeerInput;

    public InputField SatforChannelInput;

    public InputField TxOutput;

    public InputField InvoiceText;

    public InputField SatForInvoice;

    public InputField MemoForInvoice;
    public InputField InvPeerOutput;




    // Use this for initialization
    async void Start()
    {
        LndHelper.SetupEnvironmentVariables();
        cert = File.ReadAllText(Application.dataPath + "/Resources/"+filename+".cert");
        Debug.Log(cert);
        await ConnectToLnd(hostname + ":" + port, cert);
    }

    public async void OnUnlockWallet()
    {
        await ConnectToLnd(hostname + ":" + port, cert);
        var s = await UnlockWallet(pwInput.text, new string[]{""});
        Debug.Log(s);
    }

    public async void OnGetBalance()
    {
        var s = await WalletBalance();
        Debug.Log(s.ToString());
        balanceOutput.text = s.ToString();
    }

    public async void OnGetChannelBalance()
    {
        var s = await ChannelBalance();
        Debug.Log(s.ToString());
        chanBalanceOutput.text = s.ToString();
    }

    public async void OnGenerateAddress()
    {
        var s = await NewWitnessAdress();
        Debug.Log(s.ToString());
        addrOutput.text = s.ToString();
        
    }

    public void OnConnectPeer()
    {
        var peer = ChanPeerInput.text.Split('@');
        foreach(var s in peer)
            Debug.Log(s);
                ConnectPeer(peer[0],peer[1]);
    }

    public async void OnOpenChannel() {
        var peer = ChanPeerInput.text.Split('@');
        var s = await OpenChannel(peer[0], int.Parse(SatforChannelInput.text));
        TxOutput.text = s;
                Debug.Log(s.ToString());

    }

    public async void OnPayInvoice() {
        var s = await SendPayment(InvoiceText.text);
        Debug.Log(s.ToString());
    }

    public async void OnCreateInvoice() {
        var s = await AddInvoice(int.Parse(SatForInvoice.text), MemoForInvoice.text);
        Debug.Log(s);
        InvoiceText.text = s;
    }

    public async void OnDecodeInvoice() {
        var s = await DecodePaymentRequest(InvoiceText.text);
        InvPeerOutput.text = s.Destination;
        SatForInvoice.text = s.NumSatoshis.ToString();
        MemoForInvoice.text = s.Description;
    }

    
    void OnApplicationQuit()
    {
        Shutdown();
    }



}




