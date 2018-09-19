using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class LndConfig {

    public bool Neutrino;
    public string Hostname;
    public string Port;
    public string TlsFile;
    public string MacaroonFile;
}
