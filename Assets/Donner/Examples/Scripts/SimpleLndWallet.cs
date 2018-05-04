
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using Donner;
using QRCoder;
using System;

public class SimpleLndWallet : LndRpcBridge
{

    public string hostname;
    public string port;
    public string filename;
    string cert;
    string mac;
    public int pixelPerUnit;
    public InputField pwInput;
    public Text balanceOutput;
    public Text chanBalanceOutput;
    public InputField addrOutput;
    public Image addrImg;

    public InputField ChanPeerInput;

    public InputField SatforChannelInput;

    public InputField TxOutput;

    public InputField InvoiceText;

    public InputField SatForInvoice;

    public InputField MemoForInvoice;
    public InputField InvPeerOutput;

    public InputField getInfoPubkey;
    public InputField synced;
    public InputField pendingChannels;
    public InputField activeChannels;
    public InputField numPeers;
    public InputField blockHeight;
    public InputField blockHash;





    // Use this for initialization
    async void Start()
    {
        LndHelper.SetupEnvironmentVariables();
        cert = File.ReadAllText(Application.dataPath + "/Resources/"+filename+".cert");
        
        mac = LndHelper.ToHex(File.ReadAllBytes(Application.dataPath + "/Resources/admin.macaroon")); 

        await ConnectToLndWithMacaroon(hostname + ":" + port, cert,mac);
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
        var payload = new PayloadGenerator.BitcoinAddress(s, 0, null, "pay Unity").ToString();
        var qrCodeGenerator = new QRCodeGenerator();
        var qrCodeData = qrCodeGenerator.CreateQrCode(payload, QRCodeGenerator.ECCLevel.Q);
        var qrCode = new UnityQRCode(qrCodeData).GetGraphic(pixelPerUnit);
        var addrSprite = Sprite.Create(qrCode, new Rect(0, 0, qrCode.width, qrCode.height), Vector2.zero, pixelPerUnit, 0,SpriteMeshType.FullRect, new Vector4(0,1,0,1));

        addrImg.sprite = addrSprite;
        addrImg.preserveAspect = true;
        addrOutput.text = s.ToString();
        
    }

    public void OnConnectPeer()
    {
        var peer = ChanPeerInput.text.Split('@');
        ConnectPeer(peer[0],peer[1]);
    }

    public async void OnOpenChannel() {
        var peer = ChanPeerInput.text.Split('@');
        var s = await OpenChannel(peer[0], int.Parse(SatforChannelInput.text));
        TxOutput.text = s;
                Debug.Log(s);

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

    public async void OnGetInfo()
    {
        var s = await GetInfo();
        getInfoPubkey.text = s.IdentityPubkey;
        synced.text = "synced to chain: "+ s.SyncedToChain.ToString();
        pendingChannels.text = "pending channels: " + s.NumPendingChannels.ToString();
        activeChannels.text = "active channels: "+s.NumActiveChannels.ToString();
        numPeers.text = "peers: "+s.NumPeers.ToString();
        blockHeight.text = "blockheight: "+s.BlockHeight.ToString();
        blockHash.text = "blockhash: "+ s.BlockHash.ToString();

    }
    
    void OnApplicationQuit()
    {
        Shutdown();
    }



}




