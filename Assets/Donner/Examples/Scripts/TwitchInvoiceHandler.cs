using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Donner;
using UnityEngine.UI;

[RequireComponent(typeof (TwitchIRC))]
public class TwitchInvoiceHandler : MonoBehaviour {


    public DonnerLnd lnd;
    public Text DonationNameText;
    Dictionary<string,EventInvoice> EventDict;
    Dictionary<string, EventInvoicePayload> activeInvoices;
    private TwitchIRC IRC;
    private string[] commandNames;

    void Start () {
        EventDict = new Dictionary<string, EventInvoice>();
        activeInvoices = new Dictionary<string, EventInvoicePayload>();
        IRC = this.GetComponent<TwitchIRC>();
        IRC.messageRecievedEvent.AddListener(OnChatMsgReceived);
        EventDict.Add("message", new MessageEventInvoice());
        EventDict.Add("donate", new DonationEventInvoice(DonationNameText));
        lnd.OnInvoiceSettled += new InvoiceSettledEventHandler(HandlePaidInvoice);
    }
	
    async void OnChatMsgReceived(string msg)
    {
        int msgIndex = msg.IndexOf("PRIVMSG #");
        string msgString = msg.Substring(msgIndex + IRC.channelName.Length + 11);
        string user = msg.Substring(1, msg.IndexOf('!') - 1);
        string command = msgString.Split('!')[1].Split(' ')[0];
        string[] data = msgString.Split(' ');
        data = data.Slice(1, data.Length);
        var response = "";

        if (msgString == "!help")
        {
            response = "type !rain, !fire or !wind=X (x is integer amount)";
        }else if (EventDict.ContainsKey(command))
        {
            var invoice = await EventDict[command].CreateInvoice(lnd, user, data);
            activeInvoices.Add(invoice, new EventInvoicePayload(command, user, data));
            response = invoice;
        }
        if (response != "")
        {
            //GetComponent<TwitchChatExample>().CreateUIMessage("sputnck1", response);
            IRC.SendMsg(response);
        }


    }

    public void HandlePaidInvoice(object sender, InvoiceSettledEventArgs e)
    {
        if(activeInvoices.ContainsKey(e.Invoice.PaymentRequest))
        {
            EventDict[activeInvoices[e.Invoice.PaymentRequest].type].OnInvoicePaid(e.Invoice, activeInvoices[e.Invoice.PaymentRequest].sender, activeInvoices[e.Invoice.PaymentRequest].data);
        }
    }
    
        
}
public static class Extensions
{
    /// <summary>
    /// Get the array slice between the two indexes.
    /// ... Inclusive for start index, exclusive for end index.
    /// </summary>
    public static T[] Slice<T>(this T[] source, int start, int end)
    {
        // Handles negative ends.
        if (end < 0)
        {
            end = source.Length + end;
        }
        int len = end - start;

        // Return new array.
        T[] res = new T[len];
        for (int i = 0; i < len; i++)
        {
            res[i] = source[i + start];
        }
        return res;
    }
}
