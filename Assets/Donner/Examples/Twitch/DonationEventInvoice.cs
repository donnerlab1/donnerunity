using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Donner;
using Lnrpc;
using UnityEngine;

public class DonationEventInvoice : EventInvoice
{
    public async Task<string> CreateInvoice(LndRpcBridge lnd, string sender, string[] data)
    {
        int amt = int.Parse(data[0]);
        return await lnd.AddInvoice(amt, "donate;" + sender);
    }

    public void OnInvoicePaid(Invoice invoice, string sender, string[] data)
    {
        Debug.Log("DONATION BY: " + sender + "FOR: " + invoice.AmtPaidSat + " MESSAGE: " + dataToMessage(data.Slice(1, data.Length)));
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

}
