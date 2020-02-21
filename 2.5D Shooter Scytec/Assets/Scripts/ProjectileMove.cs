using UnityEngine;
using UnityEngine.Networking;

public class ProjectileMove : NetworkBehaviour
{
    [SerializeField]
    float fSpeed = 10f;
    [SerializeField]
    float fProjectileDamage = 10f;
    [SerializeField]
    GameObject hitParticle;

    [SyncVar]
    public Vector3 direction;
    [SyncVar]
    public string whoShoot;

    void Update()
    {
        transform.Translate(direction * fSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            CmdSpawnParticle();
            other.gameObject.GetComponent<ReceiveDamage>().TakeDamage(fProjectileDamage, whoShoot);
            Destroy(this.gameObject);
        }
        else if (other.CompareTag("Walls"))
        {
            CmdSpawnParticle();
            Destroy(this.gameObject);
        }
        else if (other.CompareTag("Box"))
        {
            CmdSpawnParticle();
            other.gameObject.GetComponent<ReceiveDamage>().TakeDamage(fProjectileDamage, whoShoot);
            Destroy(this.gameObject);           
        }
    }

    [Command]
    private void CmdSpawnParticle()
    {
        GameObject hitParticleObj = Instantiate(hitParticle, transform.position, Quaternion.identity);
        hitParticleObj.GetComponent<ParticleSystem>().Play();
        NetworkServer.Spawn(hitParticleObj);
        Destroy(hitParticleObj, 0.7f);
    }
}
