using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    [Header("Data Level Terpilih (Runtime)")]
   
    public string pilihanlevel;
    public MenuData[] currentMenuList;
    public GameObject[] currentNpcPrefab;
    public float currentSpawnInterval;
    public int currentMaksimalNPC;
    public int currentMinimalVariasiMenu;
    public int currentMaksimalVariasiMenu;

    private void Awake()
    {

        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SetLevelData(LevelData levelData)
    {
        pilihanlevel = levelData.levelberapa;
        currentMenuList = levelData.menuList;
        //currentNpcPrefab = levelData.npcPrefab;
        currentSpawnInterval = levelData.spawnInterval;
        currentMaksimalNPC = levelData.maksimalNPC;
        //currentMinimalVariasiMenu = levelData.minimalVariasiMenu;
        //currentMaksimalVariasiMenu = levelData.maksimalVariasiMenu;
    }
}