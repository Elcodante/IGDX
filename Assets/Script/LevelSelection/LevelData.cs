using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelData : MonoBehaviour
{
    [Header("Target Scene")]
    public string namaSceneGameplay = "GameplayScene"; 
    public string levelberapa;

    [Header("Daftar Menu Level Ini")]
    public MenuData[] menuList;

    [Header("Spawner Settings Level Ini")]
    public GameObject[] npcPrefab;
    public float spawnInterval = 5f;
    public int maksimalNPC = 10;
    public int minimalVariasiMenu = 1;
    public int maksimalVariasiMenu = 3;

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            SelectThisLevel();
        }
    }
    
    public void SelectThisLevel()
    {
        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.SetLevelData(this);
            Debug.Log("Level " + levelberapa + " dipilih.");

        }
        else
        {
            Debug.LogError("LevelManager tidak ditemukan! Pastikan LevelManager ada di Scene Map.");
        }
    }
}
