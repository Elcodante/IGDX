using UnityEngine;

public class MusicMainMenu : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        AudioManager.instance.PlayMainMenuMusic();
    }

   
}
