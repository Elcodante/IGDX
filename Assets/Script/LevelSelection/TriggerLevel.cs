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
            LevelData levelData = GetComponent<LevelData>();
            ButtonLevel.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(levelData.OnButtonClicked);
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