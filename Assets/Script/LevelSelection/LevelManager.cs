using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    [Header("Level Info")]
    [SerializeField] private int levelNow;
    public int LevelNow => levelNow;

    [Header("Data Level Terpilih (Runtime)")]
    public string pilihanlevel;
    public MenuData[] currentMenuList;
    public GameObject[] currentNpcPrefab;
    public float currentSpawnInterval;
    public int currentMaksimalNPC;
    public int currentMinimalVariasiMenu;
    public int currentMaksimalVariasiMenu;
    public HashSet<RecipeData> currentActiveRecipes;

    private const string LEVEL_KEY = "PlayerLevel";

    private void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        LoadLevel();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    // Ambil data level dari PlayerPrefs


    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"[LEVEL] Scene '{scene.name}' selesai load, coba hitung ulang resep aktif...");
        GenerateActiveRecipes();
    }

    public void GenerateActiveRecipes()
    {
        if (currentMenuList == null || currentMenuList.Length == 0)
        {
            Debug.LogWarning("[LEVEL] currentMenuList masih kosong, skip generate resep.");
            return;
        }

        if (RecipeDatabase.Instance == null)
        {
            Debug.LogWarning($"[LEVEL] RecipeDatabase belum ada di scene '{SceneManager.GetActiveScene().name}', tunggu scene berikutnya.");
            return;
        }

        currentActiveRecipes = RecipeDatabase.Instance.GetActiveRecipes(currentMenuList);
        Debug.Log($"[LEVEL] Berhasil! currentActiveRecipes = {currentActiveRecipes.Count} resep, dihitung di scene '{SceneManager.GetActiveScene().name}'");
    }

    public void LoadLevel()
    {
        levelNow = PlayerPrefs.GetInt(LEVEL_KEY, 1);
    }

    // Tambah level saat ini dan simpan ke PlayerPrefs
    public void NextLevel(int amount = 1)
    {
        levelNow += amount;
        PlayerPrefs.SetInt(LEVEL_KEY, levelNow);
        PlayerPrefs.Save();
    }

    // Mengatur data level yang sedang aktif
    public void SetLevelData(LevelData levelData)
    {
        pilihanlevel = levelData.levelberapa;
        currentMenuList = levelData.menuList;
        // currentNpcPrefab = levelData.npcPrefab;
        currentSpawnInterval = levelData.spawnInterval;
        currentMaksimalNPC = levelData.maksimalNPC;
        GenerateActiveRecipes();
        // currentMinimalVariasiMenu = levelData.minimalVariasiMenu;
        // currentMaksimalVariasiMenu = levelData.maksimalVariasiMenu;
    }


    public void ResetLevel()
    {
        levelNow = 1;
        PlayerPrefs.SetInt(LEVEL_KEY, levelNow);
        PlayerPrefs.Save();
    }
}