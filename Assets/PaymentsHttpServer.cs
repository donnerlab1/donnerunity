using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Linq;
using System.Text;
using System;
using UnityEngine;

public class PaymentsHttpServer : MonoBehaviour {
    WebServer ws;
    public WeatherLndClient weatherClient;

    // Use this for initialization
    void Start () {
        ws = new WebServer(SendResponse, "http://*:8081/weather/");
        ws.Run();
        
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
        var response = "Wrong Query, /weather?rain for rain(cost: 5 sat); /weather?fire for fire(cost: 10 sat); /weather?wind='amt' for wind(cost: 'amt' sat);";
        if (request.QueryString[0] == "rain")
        {
            Debug.Log("get rain request");
            response = weatherClient.GetWeatherInvoice("rain", 5).GetAwaiter().GetResult();
            response = "<HTML><script type='text/javascript' src='https://ajax.googleapis.com/ajax/libs/jquery/1.11.0/jquery.min.js'></script><script type='text/javascript' src='https://cdn.rawgit.com/jeromeetienne/jquery-qrcode/master/jquery.qrcode.min.js'></script><BODY><div id = 'qrcode' ><br>" + response+ "</div ></BODY><script>$(document).ready(function () {jQuery('#qrcode').qrcode('" + response + "');});</script></HTML>";
            
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
        }
       
        return response;
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
