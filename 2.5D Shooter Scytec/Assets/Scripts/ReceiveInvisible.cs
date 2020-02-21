using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ReceiveInvisible : NetworkBehaviour
{
    Dictionary<string, SkinnedMeshRenderer[]> playerMeshes = new Dictionary<string, SkinnedMeshRenderer[]>();

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            SkinnedMeshRenderer[] meshRenderers = other.gameObject.GetComponentsInChildren<SkinnedMeshRenderer>();
            playerMeshes.Add(other.gameObject.name, meshRenderers);

            if (!other.GetComponent<PlayerController>().isLocalPlayer)
            {
                foreach (var mesh in meshRenderers)
                {
                    mesh.enabled = false;
                }
            }
        }        
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            if (other.GetComponent<PlayerController>().isLocalPlayer)
            {
                foreach (KeyValuePair<string, SkinnedMeshRenderer[]> keyValue in playerMeshes)
                {
                    foreach (var mesh in keyValue.Value)
                    {
                        mesh.enabled = true;
                    }
                }
            }
        }       
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            SkinnedMeshRenderer[] meshRenderers = playerMeshes[other.gameObject.name];
            playerMeshes.Remove(other.gameObject.name);

            if (!other.GetComponent<PlayerController>().isLocalPlayer)
            {
                foreach (var mesh in meshRenderers)
                {
                    mesh.enabled = true;
                }
            }
            else
            {
                foreach (KeyValuePair<string, SkinnedMeshRenderer[]> keyValue in playerMeshes)
                {
                    foreach (var mesh in keyValue.Value)
                    {
                        mesh.enabled = false;
                    }
                }
            }
        }
    }        
}
