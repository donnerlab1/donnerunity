using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Donner;
using Lnrpc;
using System.Threading.Tasks;

public interface EventInvoice {


    void OnInvoicePaid(Invoice invoice,string sender, string[] data);

    Task<string> CreateInvoice(LndRpcBridge lnd, string sender, string[] data);
    
}

[Serializable]
public struct EventInvoicePayload
{
    public string type;
    public string sender;
    public string[] data;

    public EventInvoicePayload(string type, string sender, string[] data)
    {
        this.type = type;
        this.sender = sender;
        this.data = data;
    }
}

   

