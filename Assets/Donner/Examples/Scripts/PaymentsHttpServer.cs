using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Linq;
using System.Text;
using System;
using Donner;
using UnityEngine;
using System.Text.RegularExpressions;

public class PaymentsHttpServer : MonoBehaviour {
    WebServer ws;
    public WeatherLndClient weatherClient;
    string externalIp;
    public string webserverPort = "8079";
    string port = "9735";
    // Use this for initialization
    void Start () {
        ws = new WebServer(SendResponse, "http://*:"+webserverPort+"/weather/");
        ws.Run();
        
        StartCoroutine(GetPublicIP());
    }
	
	// Update is called once per frame
	void Update () {

	}

    void OnApplicationQuit()
    {
        

        ws.Stop();
    }

    public string SendResponse(HttpListenerRequest request)
    {
        var response = "Wrong Query, /weather?channel for channel information; /weather?rain for rain(cost: 5 sat); /weather?fire for fire(cost: 10 sat); /weather?wind='amt' for wind(cost: 'amt' sat);";
        if (request.QueryString[0] == "rain")
        {
            Debug.Log("get rain request");
            response = weatherClient.GetWeatherInvoice("rain", 5).GetAwaiter().GetResult();
            response = "<HTML><script type='text/javascript' src='https://ajax.googleapis.com/ajax/libs/jquery/1.11.0/jquery.min.js'></script><script type='text/javascript' src='https://cdn.rawgit.com/jeromeetienne/jquery-qrcode/master/jquery.qrcode.min.js'></script><BODY><div id = 'qrcode' ><br>" + response + "</div ></BODY><script>$(document).ready(function () {jQuery('#qrcode').qrcode('" + response + "');});</script></HTML>";

        }
        else if (request.QueryString[0] == "fire")
        {
            Debug.Log("get fire request");
            response = weatherClient.GetWeatherInvoice("fire", 10).GetAwaiter().GetResult();
            response = "<HTML><script type='text/javascript' src='https://ajax.googleapis.com/ajax/libs/jquery/1.11.0/jquery.min.js'></script><script type='text/javascript' src='https://cdn.rawgit.com/jeromeetienne/jquery-qrcode/master/jquery.qrcode.min.js'></script><BODY><div id = 'qrcode' ><br>" + response + "</div ></BODY><script>$(document).ready(function () {jQuery('#qrcode').qrcode('" + response + "');});</script></HTML>";
        }
        else if (request.QueryString.AllKeys.Contains("wind"))
        {
            Debug.Log("get wind request");
            Debug.Log(request.QueryString[0]);
            response = weatherClient.GetWeatherInvoice("wind", int.Parse(request.QueryString[0])).GetAwaiter().GetResult();
            response = "<HTML><script type='text/javascript' src='https://ajax.googleapis.com/ajax/libs/jquery/1.11.0/jquery.min.js'></script><script type='text/javascript' src='https://cdn.rawgit.com/jeromeetienne/jquery-qrcode/master/jquery.qrcode.min.js'></script><BODY><div id = 'qrcode' ><br>" + response + "</div ></BODY><script>$(document).ready(function () {jQuery('#qrcode').qrcode('" + response + "');});</script></HTML>";
        } else if (request.QueryString[0] == "channel")
        {
            Debug.Log("get channel request");
            response = weatherClient.pubkey + "@" + externalIp + ":" + port;
            //response = "02e9dc06de4ba3f2aa52f91e4239df011a3af80c83cf5b4f4aa54da1fac6865547@87.122.44.34:9735";
            response = "<HTML><script type='text/javascript' src='https://ajax.googleapis.com/ajax/libs/jquery/1.11.0/jquery.min.js'></script><script type='text/javascript' src='https://cdn.rawgit.com/jeromeetienne/jquery-qrcode/master/jquery.qrcode.min.js'></script><BODY><div id = 'qrcode' ><br>" + response + "</div ></BODY><script>$(document).ready(function () {jQuery('#qrcode').qrcode('" + response + "');});</script></HTML>";
        }
       
        return response;
    }
    IEnumerator GetPublicIP()
    {
        using (WWW www = new WWW("https://ipv4.myexternalip.com/raw"))
        {
            yield return www;
            var temp = www.text;
            Regex.Replace(temp, @"\s+", "");
            externalIp = LndHelper.RemoveWhitespace(temp);
            Debug.Log(www.text);

        }
    }
}


public class WebServer
{
    HttpListener _listener = new HttpListener();
   

    Func<HttpListenerRequest, string> _responderMethod;

    public WebServer(string[] prefixes, Func<HttpListenerRequest, string> method)
    {
        // "http://localhost:8080/index/".
        if (prefixes == null || prefixes.Length == 0)
            throw new ArgumentException("prefixes");

        // A responder method is required
        if (method == null)
            throw new ArgumentException("method");

        foreach (string s in prefixes)
            _listener.Prefixes.Add(s);

        _responderMethod = method;
        _listener.Start();
    }
    public WebServer(Func<HttpListenerRequest, string> method, params string[] prefixes)
            : this(prefixes, method) { }

    public void Run()
    {
        ThreadPool.QueueUserWorkItem((o) =>
        {
            Debug.Log("Webserver running...");
            try
            {
                while (_listener.IsListening)
                {
                    ThreadPool.QueueUserWorkItem((c) =>
                    {
                        var ctx = c as HttpListenerContext;
                        try
                        {
                            string rstr = _responderMethod(ctx.Request);
                            byte[] buf = Encoding.UTF8.GetBytes(rstr);
                            ctx.Response.ContentLength64 = buf.Length;
                            ctx.Response.OutputStream.Write(buf, 0, buf.Length);
                        }
                        catch { } // suppress any exceptions
                        finally
                        {
                            // always close the stream
                            ctx.Response.OutputStream.Close();
                        }
                    }, _listener.GetContext());
                }
            }
            catch { } // suppress any exceptions
        });
    }
    public void Stop()
    {

        _listener.Stop();
        _listener.Close();


    }
}
