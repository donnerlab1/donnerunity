using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ColorChanger : NetworkBehaviour {

    public Material content;
    public int price;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnCollisionEnter(Collision collision)
    {
        var bullet = collision.gameObject.GetComponent<Bullet>();
        if(bullet != null)
        {
            Debug.Log("hit by bullet");
            var material = GetComponent<MeshRenderer>().material;
            Debug.Log(material.ToString());
            int i = 0;
            foreach(var mat in AssetManager.instance.getArray())
            {
                if(mat.color == material.color)
                {
                    Debug.Log(i);
                    StartCoroutine(bullet.shooter.GetComponentInChildren<PlayerController>().Changematerial(i, price));
                }
                i++;
            }
            
        }
    }
}
