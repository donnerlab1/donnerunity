using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssetManager : MonoBehaviour {

    public Material[] skins;
    public static AssetManager instance;

    void Start()
    {
        instance = this;
    }

    public Material getMaterial(int index)
    {
        return skins[index];
    }

    public Material[] getArray()
    {
        return skins;
    }

}
