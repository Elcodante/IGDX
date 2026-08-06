using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] Transform target; 
    [SerializeField] private  float smoothSpeed = 0.125f;
    [SerializeField] private Vector2 offset;

   
    [SerializeField] private float minX, maxX;
    [SerializeField] private float minY, maxY;

    void LateUpdate()
    {
        if (target == null) return;

       
        float targetX = target.position.x + offset.x;
        float targetY = target.position.y + offset.y;

      
        float clampedX = Mathf.Clamp(targetX, minX, maxX);
        float clampedY = Mathf.Clamp(targetY, minY, maxY);

    
        Vector3 targetPosition = new Vector3(clampedX, clampedY, transform.position.z);

       
        transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed);
    }
}