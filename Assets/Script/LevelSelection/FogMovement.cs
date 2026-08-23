using UnityEngine;

public class FogMovement : MonoBehaviour
{
    [SerializeField] private float speed = 2f;
    [SerializeField] private float destroyXLimit = 15f;
    
    private Vector2 direction;

    public void SetDirection(Vector2 newDirection)
    {
        direction = newDirection.normalized;
    }

    private void Update()
    {
        transform.Translate(direction * (speed * Time.deltaTime));
        if (Mathf.Abs(transform.position.x) > destroyXLimit)
        {
            Destroy(gameObject);
        }
    }
}