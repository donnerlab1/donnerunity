using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Donner;
using System.Diagnostics;
using System;

public class NeutrinoTest : MonoBehaviour {

    Process lnd;
	// Use this for initialization
	void Start () {
        
    }

    public void StartLnd(LndConfig config)
    {
        try
        {
            ProcessStartInfo lndStartinfo = new ProcessStartInfo();
            UnityEngine.Debug.Log(Application.dataPath);
            lndStartinfo.FileName = Application.dataPath + "/Resources/Neutrino/lnd.exe";
            lndStartinfo.WorkingDirectory = Application.dataPath + "/Resources/Neutrino/";
            lndStartinfo.Arguments = "--configfile=test_data_neutrino/lnd.conf --rpclisten="+config.Hostname+":"+config.Port+" --restlisten=localhost:8089 --listen=0.0.0.0:9750";
            lndStartinfo.UseShellExecute = false;
            lndStartinfo.RedirectStandardOutput = true;
            lndStartinfo.RedirectStandardError = true;
            lndStartinfo.WindowStyle = ProcessWindowStyle.Hidden;
            //lndStartinfo.CreateNoWindow = true;
            lnd = Process.Start(lndStartinfo);
            lnd.ErrorDataReceived += Lnd_ErrorDataReceived;
            lnd.OutputDataReceived += Lnd_OutputDataReceived;

            lnd.BeginErrorReadLine();
            lnd.BeginOutputReadLine();

            //
            /*
             * lnd = new Process();
            lnd.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
            //lnd.StartInfo.CreateNoWindow = true;
            lnd.StartInfo.UseShellExecute = false;
            lnd.StartInfo.FileName = Application.dataPath + "/Resources/Neutrino/lndNeutrino.bat";

            //lnd.StartInfo.Arguments = "/c" + path;
            lnd.EnableRaisingEvents = true;
            lnd.StartInfo.RedirectStandardOutput = true;
            lnd.OutputDataReceived += Lnd_OutputDataReceived;
            
            lnd.Start();
            UnityEngine.Debug.Log(lnd.ProcessName);
            lnd.BeginOutputReadLine();
            */

        }
        catch (Exception e)
        {
            UnityEngine.Debug.Log(e);
        }
    }

    private void Lnd_ErrorDataReceived(object sender, DataReceivedEventArgs e)
    {
        UnityEngine.Debug.unityLogger.Log("lnd-neutrino-error", e.Data);
    }

    private void Lnd_OutputDataReceived(object sender, DataReceivedEventArgs e)
    {
       UnityEngine.Debug.unityLogger.Log("lnd-neutrino",e.Data);
    }

    // Update is called once per frame
    void Update () {
        //UnityEngine.Debug.Log(lnd.StandardOutput.ReadLine());
	}

    private void OnApplicationQuit()
    {
        //lnd.CloseMainWindow();
        lnd.Kill();
        //lnd.Close();
        //lnd.Kill();
        //lnd.WaitForExit();
        UnityEngine.Debug.Log("called?");

    }
}
