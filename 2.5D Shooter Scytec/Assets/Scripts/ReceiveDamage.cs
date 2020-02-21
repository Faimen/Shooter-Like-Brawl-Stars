using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class ReceiveDamage : NetworkBehaviour
{    
    [SyncVar]
    [HideInInspector]
    public float fHealthCurrent;
    [SerializeField]
    private float fHealthMax = 100f;
    [SyncVar]
    private Vector3 spawnPos;

    void Start()
    {
        fHealthCurrent = fHealthMax;
        spawnPos = gameObject.transform.position;
    }

    public void TakeDamage(float count, string whoShoot)
    {
        if(this.isServer)
        {
            fHealthCurrent -= count;   
        }
        if (fHealthCurrent <= 0f)
        {
            if(gameObject.CompareTag("Player"))
            {
                GameObject killer = GameObject.Find(whoShoot);
                SetKillsCount(killer);
                gameObject.SetActive(false);
                Invoke("RpcRespawn", 3f);
            }
            else
            {
                Destroy(gameObject);
            }           
        }
    }

    void SetKillsCount(GameObject player)
    {
        player.GetComponent<PlayerController>().killCount++;
        player.GetComponent<PlayerController>().canvasPlayer.GetComponentInChildren<Text>().text =
                                               "Kills: " + player.GetComponent<PlayerController>().killCount;
    }

    [ClientRpc]
    void RpcRespawn()
    {
        fHealthCurrent = fHealthMax;
        transform.position = spawnPos;
        gameObject.SetActive(true);
    }
}
