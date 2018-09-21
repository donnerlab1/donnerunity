using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Donner;
using Lnrpc;
using System.Threading.Tasks;

public interface EventInvoice {


    void OnInvoicePaid(Invoice invoice, string[] data);

    Task<string> CreateInvoice(LndRpcBridge lnd, string sender, string[] data);
    
}

[Serializable]
public struct EventInvoicePayload
{
    public string type;
    public string[] data;

    public EventInvoicePayload(string type, string[] data)
    {
        this.type = type;
        this.data = data;
    }
}

   

