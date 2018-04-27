using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Grpc.Core;
using Lnrpc;
using System.IO;
using Donner;

public class LndTestClient : LndRpcBridge
{

    public string hostname;
    public string port;
    public string password;
    public string cert;

    public string[] mnemonic;
    public bool GenSeed;
    public bool UnlockWalletTrigger;



    // Use this for initialization
    void Start()
        {
        //LndHelper.StartLnd(Application.dataPath+"/Resources/lnd/lnd.exe", port);

        var r = File.ReadAllText(Application.dataPath + "/Resources/tls.cert");
        Debug.Log(r);
        ConnectToLnd(hostname + ":" + port, r);
            
        }

        // Update is called once per frame
        async void Update()
        {
            if(GenSeed)
            {
                GenSeed = false;
                mnemonic = await GenerateSeed();

               
            }
            if (UnlockWalletTrigger)
            {
                UnlockWalletTrigger = false;
                UnlockWallet(password, mnemonic);
            }
        }

        void OnApplicationQuit()
        {
            Shutdown();
            //LndHelper.KillLnd();
        }

        

    }

    


