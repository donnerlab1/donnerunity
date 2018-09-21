using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Donner;
using Lnrpc;
using UnityEngine;


public class MessageEventInvoice : EventInvoice {

    
    public void OnInvoicePaid(Invoice invoice, string sender, string[] data)
    {
        Debug.Log(dataToMessage(data));
    }

    public async Task<string> CreateInvoice(LndRpcBridge lnd, string sender, string[] data)
    {
        var Invoice = await lnd.AddInvoice(calculatePrice(data), "message;" + sender);
        return Invoice;
    }

    string dataToMessage(string[] data)
    {
        var message = "";
        foreach (var str in data)
        {
            message += str + " ";
        }
        return message;
    }
    int calculatePrice(string[] data)
    {
        int price = 0;
        var message = dataToMessage(data);
        foreach(char c in message)
        {
            price += 1;
        }

        return price;

    }
}
