using UnityEngine;

public class TriggerLevel : MonoBehaviour
{
    [SerializeField] private GameObject ButtonLevel; 

    void Start()
    {
        ButtonLevel.SetActive(false); 
    }


    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            ButtonLevel.SetActive(true); 
        }
    }

  
    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (ButtonLevel != null) 
            {
                ButtonLevel.SetActive(false);
            }
        }
    }
}