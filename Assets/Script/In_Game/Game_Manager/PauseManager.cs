using UnityEngine;
using UnityEngine.InputSystem;
public class PauseManager : MonoBehaviour
{
    [Header("Referensi UI")]
    public GameObject panelSettings;
    public GameObject settingsButton;
    public GameObject tombolNavigasi;

    public static bool isPaused { get; private set; }
    void Start()
    {
        ResumeGame();
    }

    public void Pausegame()
    {
        isPaused = true;
        panelSettings.SetActive(true);
        settingsButton.SetActive(false);
        tombolNavigasi.SetActive(false);

        Time.timeScale = 0f;
        Debug.Log("Game Paused");
    }

    public void ResumeGame()
    {
        isPaused = false;
        panelSettings.SetActive(false);
        settingsButton.SetActive(true);
        tombolNavigasi.SetActive(true);
        Time.timeScale = 1f;
        Debug.Log("Game Resumed");
    }

    void Update()
    {
        if(Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                Pausegame();
            }
        }
    }

}
