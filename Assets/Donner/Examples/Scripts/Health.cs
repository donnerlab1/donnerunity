using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
public class Health : NetworkBehaviour
{
    public const int maxHealth = 100;
    [SyncVar(hook = "OnChangeHealth")]
    public int currentHealth = maxHealth;

    public RectTransform healthBar;

    private NetworkStartPosition[] spawnPoints;

    void Start()
    {
        if (isLocalPlayer)
        {
            spawnPoints = FindObjectsOfType<NetworkStartPosition>();
        }
    }

    public bool TakeDamage(int amount)
    {
        if (!isServer)
        {
            return false;
        }
        currentHealth -= amount;
        Debug.Log(currentHealth);
        if (currentHealth <= 0)
        {
            currentHealth = maxHealth;

            RpcRespawn();
            return true;
        }
        return false;
    }

    void ReleaseBounty(NetworkConnection sender, int bounty)
    {
        Target_GetBounty(sender, bounty);
    }
    [TargetRpc]
    public async void Target_GetBounty(NetworkConnection target, int amt)
    {
       var s = await GetComponent<LndPlayer>().AddInvoice(amt);
        CmdReleaseBounty(s);
    }
    IEnumerator DonnerlabPay(string invoice)
    {
        using (WWW www = new WWW("http://donnerlab.com/pay_invoice/"+invoice))
        {
            yield return www;
            var payment = www.text;
            Debug.Log(payment);

        }
    }

    void CmdReleaseBounty(string invoice)
    {

        StartCoroutine(DonnerlabPay(invoice));
    }
    void OnChangeHealth(int health)
    {
        healthBar.sizeDelta = new Vector2(health, healthBar.sizeDelta.y);
    }
    [ClientRpc]
    void RpcRespawn()
    {
        if (isLocalPlayer)
        {
            Vector3 spawnPoint = Vector3.zero;
            if (spawnPoints != null && spawnPoints.Length > 0)
            {
                spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)].transform.position;
            }
            transform.position = spawnPoint;
        }
        GetComponent<PlayerController>().bounty = 0;
    }

    
}