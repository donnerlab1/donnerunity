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
        private static Process lndProcess;
        public static int RpcListenPort { get; private set; }
        public const int ListenPortv4 = 9737;
        public const int ListenPortv6 = 9738;


        public static void StartLnd(string path, int rpcport)
        {
            RpcListenPort = rpcport;
            lndProcess = new Process();
            lndProcess.StartInfo = new ProcessStartInfo();


            lndProcess.StartInfo.FileName = path;

            lndProcess.StartInfo.Arguments = "--bitcoin.active --bitcoin.testnet --debuglevel=debug --bitcoin.node=neutrino --neutrino.connect=faucet.lightning.community --no-macaroons --datadir=lnd_data --logdir=lnd_log --tlscertpath=tls.cert --tlskeypath=tls.key --listen=0.0.0.0:"+ListenPortv4+
                " --listen=[::1]:"+ListenPortv6+" --rpclisten=localhost:"+rpcport;
            //lndProcess.StartInfo.UseShellExecute = true;
            lndProcess.OutputDataReceived += LndProcess_OutputDataReceived;
            lndProcess.ErrorDataReceived += LndProcess_OutputDataReceived;
            //lndProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            //lndStartInfo.WindowStyle = ProcessWindowStyle.Minimized;
            lndProcess.Start();
            

        }

        public static string ToHex(byte[] input)
        {
            return BitConverter.ToString(input).Replace("-", string.Empty);
        }

        public static void SetupEnvironmentVariables() {
                    Environment.SetEnvironmentVariable("GRPC_SSL_CIPHER_SUITES", "ECDHE-RSA-AES128-GCM-SHA256:ECDHE-RSA-AES128-SHA256:ECDHE-RSA-AES256-SHA384:ECDHE-RSA-AES256-GCM-SHA384:ECDHE-ECDSA-AES128-GCM-SHA256:ECDHE-ECDSA-AES128-SHA256:ECDHE-ECDSA-AES256-SHA384:ECDHE-ECDSA-AES256-GCM-SHA384");
        }

        private static void LndProcess_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            UnityEngine.Debug.Log(e.Data);
        }

        public static void KillLnd()
        {
            lndProcess.Kill();
        }
    }
}
