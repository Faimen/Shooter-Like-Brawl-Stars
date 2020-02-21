using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target;
    
    void Update()
    {      
        if (target != null)
            transform.position = target.transform.position;         
    }
}
