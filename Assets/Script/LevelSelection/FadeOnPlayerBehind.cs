using UnityEngine;

public class FadeOnPlayerBehind : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private float fadeAlpha = 0.4f; 
    [SerializeField] private float fadeSpeed = 5f;   

    private float targetAlpha = 1.0f;
    private Color originalColor;

    void Start()
    {
        
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
        
        originalColor = spriteRenderer.color;
    }

    void Update()
    {
   
        Color currentColor = spriteRenderer.color;
        float newAlpha = Mathf.MoveTowards(currentColor.a, targetAlpha, fadeSpeed * Time.deltaTime);
        spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, newAlpha);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
   
        if (other.CompareTag("Player"))
        {
            targetAlpha = fadeAlpha; 
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
       
        if (other.CompareTag("Player"))
        {
            targetAlpha = 1.0f; 
        }
    }
}