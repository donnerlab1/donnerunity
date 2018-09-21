using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleWalletKeyListener : MonoBehaviour {

    bool isActive;
    public GameObject GUI;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.F1))
            ToggleGui();
	}

    void ToggleGui()
    {
        isActive = !isActive;
        GUI.SetActive(isActive);
    }

}
