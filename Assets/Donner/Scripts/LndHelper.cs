using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using UnityEngine;

namespace Donner
{
    public static class LndHelper
    {

        public static string ToHex(byte[] input)
        {
            return BitConverter.ToString(input).Replace("-", string.Empty);
        }

        public static void SetupEnvironmentVariables() {
                    Environment.SetEnvironmentVariable("GRPC_SSL_CIPHER_SUITES", "ECDHE-RSA-AES128-GCM-SHA256:ECDHE-RSA-AES128-SHA256:ECDHE-RSA-AES256-SHA384:ECDHE-RSA-AES256-GCM-SHA384:ECDHE-ECDSA-AES128-GCM-SHA256:ECDHE-ECDSA-AES128-SHA256:ECDHE-ECDSA-AES256-SHA384:ECDHE-ECDSA-AES256-GCM-SHA384");
        }
        
        public static LndConfig ReadConfigFile(string url)
        {
            if (File.Exists(url))
            {
                var dataString = File.ReadAllText(url);
                UnityEngine.Debug.Log(dataString);
                return JsonUtility.FromJson<LndConfig>(dataString);
            }
            else { return null; }
            
        }

        public static string RemoveWhitespace(this string input)
        {
            return new string(input.ToCharArray()
                .Where(c => !Char.IsWhiteSpace(c))
                .ToArray());
        }

        public static void StartLndWithNeutrino()
        {
            try
            {
                Process lnd = new Process();
                lnd.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                //lnd.StartInfo.CreateNoWindow = true;
                //lnd.StartInfo.UseShellExecute = true;
                lnd.StartInfo.FileName = Application.dataPath + "/Resources/Neutrino/lndNeutrino.cmd";
                lnd.OutputDataReceived += Lnd_OutputDataReceived;
                lnd.Start();

            } catch(Exception e)
            {
                UnityEngine.Debug.Log(e);
            }
        }

        private static void Lnd_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            UnityEngine.Debug.Log(e.Data);
        }
    }
}
