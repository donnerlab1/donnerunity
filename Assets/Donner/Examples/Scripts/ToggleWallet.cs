using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Donner;
using System;
using UnityEngine.UI;
using System.IO;

public class ToggleWallet : MonoBehaviour {

    public LndRpcBridge wallet;
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

    public string pubkey { get; private set; }

    LndConfig config;




    // Use this for initialization
    async void Start()
    {
        
        
    }


    public async void OnUnlockWallet()
    {
        
        var s = await wallet.UnlockWallet(pwInput.text, new string[] { "" });
        Debug.Log(s);
    }

    public async void OnGetBalance()
    {
        var s = await wallet.WalletBalance();
        Debug.Log(s.ToString());
        balanceOutput.text = s.ToString();
    }

    public async void OnGetChannelBalance()
    {
        var s = await wallet.ChannelBalance();
        Debug.Log(s.ToString());
        chanBalanceOutput.text = s.ToString();
    }

    public async void OnGenerateAddress()
    {
        var s = await wallet.NewWitnessAdress();
        Debug.Log(s.ToString());
        //var payload = new PayloadGenerator.BitcoinAddress(s, 0, null, "pay Unity").ToString();
        //var qrCodeGenerator = new QRCodeGenerator();
        //var qrCodeData = qrCodeGenerator.CreateQrCode(payload, QRCodeGenerator.ECCLevel.Q);
        //var qrCode = new UnityQRCode(qrCodeData).GetGraphic(pixelPerUnit);
        //var addrSprite = Sprite.Create(qrCode, new Rect(0, 0, qrCode.width, qrCode.height), Vector2.zero, pixelPerUnit, 0,SpriteMeshType.FullRect, new Vector4(0,1,0,1));

        //addrImg.sprite = addrSprite;
        //addrImg.preserveAspect = true;
        addrOutput.text = s.ToString();

    }

    public void OnConnectPeer()
    {
        var peer = ChanPeerInput.text.Split('@');
        wallet.ConnectPeer(peer[0], peer[1]);
    }

    public async void OnOpenChannel()
    {
        var peer = ChanPeerInput.text.Split('@');
        var s = await wallet.OpenChannel(peer[0], int.Parse(SatforChannelInput.text));
        TxOutput.text = s;
        Debug.Log(s);

    }

    public async void OnPayInvoice()
    {
        var s = await wallet.SendPayment(InvoiceText.text);
        Debug.Log(s.ToString());
    }

    public async void OnCreateInvoice()
    {
        var s = await wallet.AddInvoice(int.Parse(SatForInvoice.text), MemoForInvoice.text);
        Debug.Log(s);
        InvoiceText.text = s;
    }

    public async void OnDecodeInvoice()
    {
        var s = await wallet.DecodePaymentRequest(InvoiceText.text);
        InvPeerOutput.text = s.Destination;
        SatForInvoice.text = s.NumSatoshis.ToString();
        MemoForInvoice.text = s.Description;
    }

    public async void OnGetInfo()
    {
        var s = await wallet.GetInfo();
        getInfoPubkey.text = s.IdentityPubkey;
        synced.text = "synced to chain: " + s.SyncedToChain.ToString();
        pendingChannels.text = "pending channels: " + s.NumPendingChannels.ToString();
        activeChannels.text = "active channels: " + s.NumActiveChannels.ToString();
        numPeers.text = "peers: " + s.NumPeers.ToString();
        blockHeight.text = "blockheight: " + s.BlockHeight.ToString();
        blockHash.text = "blockhash: " + s.BlockHash.ToString();

    }



}
