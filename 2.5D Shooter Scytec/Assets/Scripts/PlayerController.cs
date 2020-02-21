using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PlayerController : NetworkBehaviour
{
    [SerializeField]
    float fSpeed = 5f;
    [SerializeField]
    float fShootDelay = 0.45f;
    [SerializeField]
    float fCountBullet = 10f;

    public GameObject projectilePrefab;
    public GameObject canvasPlayer;
    [HideInInspector]
    public int killCount = 0;
    
    private Animator animator;
    private string currentAnim = "isMoveForward";
    private FloatingJoystick floatingJoystick;
    private FixedJoystick shootingJoystick;
    private CharacterController controller;
    private float fLastShootTime;

  
    private void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
        fLastShootTime = Time.time;
        gameObject.name = "Player " + GetComponent<NetworkIdentity>().netId;
    }

    public override void OnStartLocalPlayer()
    {
       //Setup player camera
        Camera.main.GetComponentInParent<CameraController>().target = transform;
       //Setup player UI 
        canvasPlayer = Instantiate(canvasPlayer);
        canvasPlayer.SetActive(true);
        floatingJoystick = canvasPlayer.GetComponentInChildren<FloatingJoystick>();
        shootingJoystick = canvasPlayer.GetComponentInChildren<FixedJoystick>();
        canvasPlayer.GetComponentInChildren<Text>().text = "Kills: 0";
    }

    void Update()
    {        
        if (this.isLocalPlayer)
        {
            //Movement
            Vector3 rightMovement = Vector3.right * floatingJoystick.Horizontal * fSpeed * Time.deltaTime;
            Vector3 upMovement = Vector3.forward * floatingJoystick.Vertical * fSpeed * Time.deltaTime;
            Vector3 direction = Vector3.Normalize(rightMovement + upMovement);

            if (direction != Vector3.zero)
                transform.forward = direction;
            if (floatingJoystick.Direction.magnitude > 0.1f)
                controller.Move(rightMovement + upMovement);

            animator.SetBool("isShoot", false);
            //Shooting and aim
            if (shootingJoystick.Direction.magnitude >= 0.5f)
            {
                Aim();
                CmdShoot();
            }
            else if (shootingJoystick.Direction != Vector2.zero)
            {
                AutoAim();
                CmdShoot();
            }

            if (floatingJoystick.Direction.magnitude <= 0.1f)
            {
                animator.SetBool(currentAnim, false);
            }
            else
            {
                animator.SetBool(currentAnim, false);
                currentAnim = GetAnimDirection(transform.forward, direction);
                animator.SetBool(currentAnim, true);
            }
        }        
    }

    private string GetAnimDirection(Vector3 forwardView, Vector3 directionMovement)
    {
        float angle = Vector3.Angle(forwardView, directionMovement);
        angle *= Vector3.Dot(forwardView, directionMovement) < 0 ? -1 : 1;

        if (angle < 45f && angle >= 0f)
        {
            return "isMoveForward";
        }
        else if (angle < -135f)
        {
            return "isMoveBack";
        }
        else if ((angle >= -135f && angle >= 45f) || (angle >= -135f && angle <= -90f))
        {
            return (forwardView.x * directionMovement.z - forwardView.z * directionMovement.z) < 0 ? "isMoveRight" : "isMoveLeft";                       
        }
        return "angle isn't in range";
    }

    [Command]
    private void CmdShoot()
    {
        if (Time.time - fLastShootTime >= fShootDelay)
        {
            fLastShootTime = Time.time;
            animator.SetBool("isShoot", true);
            for(int i = 0; i < fCountBullet; i++)
            {                
                Vector3 dir = new Vector3(Random.Range(transform.forward.x - 0.3f, transform.forward.x + 0.3f), 0.0f,
                                          Random.Range(transform.forward.z - 0.3f, transform.forward.z + 0.3f));

                GameObject projectileFire = Instantiate(projectilePrefab, transform.position +
                                                   transform.forward + projectilePrefab.transform.position,
                                                   Quaternion.identity);
                projectileFire.gameObject.GetComponent<ProjectileMove>().direction = dir;
                projectileFire.gameObject.GetComponent<ProjectileMove>().whoShoot = gameObject.name;
                NetworkServer.Spawn(projectileFire);
                Destroy(projectileFire, 0.6f);
            }     
        }
    }

    private void AutoAim()
    {
        int RaysToShoot = 90; //4 degrees
        float angle = 0;

        Vector3 targetPos = Vector3.zero;
        Vector3 castPoint = new Vector3(transform.position.x, transform.position.y 
                                                              + controller.height / 2, transform.position.z);
        float distance = Mathf.Infinity;

        RaycastHit hit;
        for (int i = 0; i < RaysToShoot; i++)
        {
            float x = Mathf.Sin(angle);
            float z = Mathf.Cos(angle);
            angle += 2 * Mathf.PI / RaysToShoot;

            Vector3 dir = new Vector3(x, transform.position.y, z);
           
            if (Physics.Raycast(transform.position, dir, out hit))
            {                            
                if(hit.transform.gameObject.CompareTag("Player"))
                {                    
                    if(distance > hit.distance)
                    {                        
                        targetPos = hit.point;
                        distance = hit.distance;
                    }
                }
            }
        }
        if(targetPos != Vector3.zero)
            transform.LookAt(targetPos);        
    }

    private void Aim()
    {
        Vector3 rightLook = Vector3.right * shootingJoystick.Horizontal;
        Vector3 upLook = Vector3.forward * shootingJoystick.Vertical;
        Vector3 direction = Vector3.Normalize(rightLook + upLook);

        if (direction != Vector3.zero)
            transform.forward = direction;
    }
}
