using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Donner;

[RequireComponent(typeof(TwitchIRC))]
public class TwitchLnd : MonoBehaviour
{

    private TwitchIRC IRC;
    public WeatherLndClient weatherClient;
    // Use this for initialization
    void Start()
    {
        IRC = this.GetComponent<TwitchIRC>();
        IRC.messageRecievedEvent.AddListener(OnChatMsgRecieved);
        weatherClient.OnInvoiceSettled += new InvoiceSettledEventHandler(ChangeWeather);
    }

    // Update is called once per frame
    void Update()
    {

    }
    async void OnChatMsgRecieved(string msg)
    {
        int msgIndex = msg.IndexOf("PRIVMSG #");
        string msgString = msg.Substring(msgIndex + IRC.channelName.Length + 11);
        string user = msg.Substring(1, msg.IndexOf('!') - 1);
        Debug.Log(msg);
        Debug.Log(msgString);
        var response = "";
        if (msgString == "!help")
        {
            response = "type !rain, !fire or !wind=X (x is integer amount)";
        }
        else if (msgString == "!rain")
        {
           response = response + await weatherClient.GetWeatherInvoice("rain", 5);
        } else if(msgString == "!fire")
        {
            response = await weatherClient.GetWeatherInvoice("fire", 10);
        } else if(msgString.Contains("wind"))
        {
            var s = msgString.Split('=');
            response = response + await weatherClient.GetWeatherInvoice("wind", int.Parse(s[1]));
            
        }
        if (response != "") {
            GetComponent<TwitchChatExample>().CreateUIMessage("sputnck1", response);
            IRC.SendMsg(response);
        }

    }
    void ChangeWeather(object sender, InvoiceSettledEventArgs e)
    {
        Debug.Log(e.Invoice.Memo);
        var response = "";
        switch (e.Invoice.Memo)
        {
            case ("rain"):
                response = "RAIN ACTIVATED";
                break;
            case ("fire"):
                response = "FIRE ACTIVATED";
                break;
            case ("wind"):
                response = "Wind ACTIVATED AT SPEED " + e.Invoice.Value;
                break;

        }
        if (response != "")
        {
            IRC.SendMsg(response);
            GetComponent<TwitchChatExample>().CreateUIMessage("sputnck1", response);
        }
    }
}
