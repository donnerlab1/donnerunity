using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Donner
{
    public abstract class LndRpcClient : MonoBehaviour
    {
        public LndRpcBridge rpc;

        public string host;
        public int port;

    }
}
