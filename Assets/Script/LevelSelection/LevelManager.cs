using UnityEngine;

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
    }

    // Ambil data level dari PlayerPrefs
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