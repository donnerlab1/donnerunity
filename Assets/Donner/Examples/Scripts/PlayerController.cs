using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System;
using System.Collections;
using System.Threading.Tasks;

[NetworkSettings(channel = 2, sendInterval = 0.05f)]
public class PlayerController : NetworkBehaviour
{
    public float forwardForce;
    public float turnForce;
    public float shootForce;

    public GameObject bulletPrefab;
    public Transform bulletSpawn;
    public Text bountyText;

    [SyncVar(hook = "OnChangeBounty")]
    public int bounty = 0;

    LndPlayer lnd;

    public NetworkConnection cts;



    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();

        GetComponentInChildren<Renderer>().material.color = Color.blue;
        GameObject.FindGameObjectWithTag("MainCamera").transform.parent = transform;
        GameObject.FindGameObjectWithTag("MainCamera").transform.rotation = Quaternion.Euler(15, 0, 0);
        GameObject.FindGameObjectWithTag("MainCamera").transform.localPosition = new Vector3(0, 5, -10);

        lnd = GetComponent<LndPlayer>();

        cts = connectionToServer;
    }
    void Update()
    {
        if (!isLocalPlayer)
        {
            return;
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(Fire());
        }


        var x = Input.GetAxis("Horizontal") * Time.deltaTime * turnForce;
        var z = Input.GetAxis("Vertical") * Time.deltaTime * forwardForce;
        
        transform.Rotate(0, x, 0);
        transform.Translate(0, 0, z);
    }

    async void SendPayment(string payment, int amount)
    {


        var preimage = await lnd.SendPayment(payment, amount);
        
        if (preimage.PaymentError == "")
        {

            bounty += amount;
           
            CmdFire();
        } else
        {
            Debug.Log(preimage.PaymentError);
        }
    }
    
    public void CollectBounty(int amount)
    {

    }
    
    IEnumerator Fire()
    {
        string payment;
        using (WWW www = new WWW("http://donnerlab.com/get_invoice/"))
        {
            yield return www;
            payment = www.text;
            SendPayment(payment,1);
            

        }
        


    }


    async void OnChangeBounty(int bounty)
    {
        
        bountyText.text = "Bounty: " + bounty;
    }

    public IEnumerator Changematerial(int mat, int amount)
    {
    string payment;
        using (WWW www = new WWW("http://donnerlab.com/get_invoice/"))
        {
            yield return www;
            payment = www.text;
            Debug.Log(payment);
            Debug.Log("start");
            changeSkin(payment, mat, amount);
            Debug.Log("after");
        }


    }

    async void changeSkin(string payment, int mat, int price)
    {

        var preimage = await lnd.SendPayment(payment, price);
        if (preimage.PaymentError == "")
        {
            Debug.Log(preimage.PaymentPreimage.ToBase64());
            bounty += price;
            CmdChangeSkin(mat);
        }
    }

    [Command]
    void CmdChangeSkin(int index)
    {
        Debug.Log("changeSkin");
        RpcChangeSkin(index);
    }
    [ClientRpc]
    void RpcChangeSkin(int index)
    {
        GetComponentInChildren<MeshRenderer>().material = AssetManager.instance.getMaterial(index);
    }



    [Command]
    void CmdFire()
{
    // Create the Bullet from the Bullet Prefab
    var bullet = (GameObject)Instantiate(
        bulletPrefab,
        bulletSpawn.position,
        bulletSpawn.rotation);
    // Add velocity to the bullet
    bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * shootForce;
        bullet.GetComponent<Bullet>().shooter = this;

        NetworkServer.Spawn(bullet);
        // Destroy the bullet after 2 seconds
        Destroy(bullet, 2.0f);
}
    

}