using UnityEngine;

public class Gun : MonoBehaviour
{
    [Header("References")]
    public GameObject bulletPrefab; 
    public Transform shootPoint;    
    public Transform playerCamera;
    public LayerMask whatToIgnore;
    void Update()
    {
        
        if (Input.GetMouseButtonDown(0))
        {
            Shoot();
        }
    }

    void Shoot()
    {
        Ray ray = new Ray(playerCamera.position, playerCamera.forward);
        RaycastHit hit;

        Vector3 targetPoint;
        //                                                                  
        if (Physics.Raycast(ray, out hit, 999f, ~whatToIgnore)) 
        {
            targetPoint = hit.point;
        }
        else
        {
            targetPoint = ray.GetPoint(100);
        }

        Vector3 direction = (targetPoint - shootPoint.position).normalized;
        Quaternion bulletRotation = Quaternion.LookRotation(direction);

        Instantiate(bulletPrefab, shootPoint.position, bulletRotation);
    }
}