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
        if(msgString == "!help")
        {
            IRC.SendMsg("type !rain, !fire or !rain=X (x is integer amount)");
        }
       else if (msgString == "!rain")
        {
           response = await weatherClient.GetWeatherInvoice("rain", 5);
           IRC.SendMsg("let it rain: " + response);
        } else if(msgString == "!fire")
        {
            response = await weatherClient.GetWeatherInvoice("fire", 10);
            IRC.SendMsg("BUUUURN: " + response);
        } else if(msgString.Contains("wind"))
        {
            var s = msgString.Split('=');
            foreach(var str in s)
                Debug.Log(str);
           response = await weatherClient.GetWeatherInvoice("wind", int.Parse(s[1]));
            IRC.SendMsg("Blowing in the wind: " + response);
        }
        GetComponent<TwitchChatExample>().CreateUIMessage("sputnck1", response);
    }
    void ChangeWeather(object sender, InvoiceSettledEventArgs e)
    {
        Debug.Log(e.Invoice.Memo);
        switch (e.Invoice.Memo)
        {
            case ("rain"):
                IRC.SendMsg("RAIN ACTIVATED");
                break;
            case ("fire"):
                IRC.SendMsg("FIRE ACTIVATED");
                break;
            case ("wind"):
                IRC.SendMsg("Wind ACTIVATED AT SPEED " + e.Invoice.Value);
                break;

        }

    }
}
