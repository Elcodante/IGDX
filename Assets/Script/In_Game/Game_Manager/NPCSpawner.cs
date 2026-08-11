using UnityEngine;
using System.Collections.Generic;

public class NPCSpawner : MonoBehaviour
{
    [Header("Daftar Menu")]
    public MenuData[] menuList;

    [Header("Spawner Settings")]
    public GameObject[] npcPrefab;
    public Transform spawnPoint;
    public float spawnInterval = 5f;
    public Transform exitPoint;

    public int maksimalNPC;
    public int maksimalVariasiMenu;
    public int minimalVariasiMenu;

    [Header("Object Pool")]
    public int poolSize = 6;
    private Queue<GameObject> npcPool;

    [Header("References")]
    public UIManager uiManager;

    // 1. TAMBAHKAN REFERENSI KE MANAJER ANTREAN
    public NPCQueueManager queueManager;

    private float timer;
    private int jumlahNPCSudahMuncul = 0;

    void Start()
    {
        LoadLevelData();

        // 2. KODE INISIALISASI POOL LEBIH BERSIH
        npcPool = new Queue<GameObject>();

        if (npcPrefab.Length == 0 || npcPrefab[0] == null) return;
        if (menuList.Length == 0) return;

        for (int i = 0; i < poolSize; i++)
        {
            GameObject prefabTerpilih = npcPrefab[Random.Range(0, npcPrefab.Length)];
            GameObject obj = Instantiate(prefabTerpilih);
            obj.SetActive(false);

            NPCController controller = obj.GetComponent<NPCController>();
            controller.SetSpawner(this);
            controller.OnPesananDiambil.AddListener(uiManager.TampilkanPanelPesanan);

            npcPool.Enqueue(obj);
        }
    }

    void Update()
    {
        if (jumlahNPCSudahMuncul >= maksimalNPC) return;

        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            // 3. MINTA INFO SLOT KOSONG DARI QUEUE MANAGER
            int availableSlot = queueManager.GetEmptySlot();

            if (availableSlot != -1 && npcPool.Count > 0)
            {
                SpawnNPC(availableSlot);
                timer = 0f;
            }
        }
    }

    private void SpawnNPC(int slotIndex)
    {
        // 4. BERITAHU QUEUE MANAGER BAHWA SLOT INI DIAMBIL
        queueManager.TempatiSlot(slotIndex);

        GameObject spawnNPC = npcPool.Dequeue();
        spawnNPC.transform.position = spawnPoint.position;
        spawnNPC.SetActive(true);

        NPCController controller = spawnNPC.GetComponent<NPCController>();

        // 5. MINTA TITIK WAYPOINT DARI QUEUE MANAGER
        Transform targetWaypoint = queueManager.GetWaypoint(slotIndex);

        controller.InitializeNPC(targetWaypoint, slotIndex, menuList, minimalVariasiMenu, maksimalVariasiMenu);

        jumlahNPCSudahMuncul++;
        Debug.Log($"NPC spawned. Total NPCs spawned: {jumlahNPCSudahMuncul}/{maksimalNPC}");
    }

    private void LoadLevelData()
    {
        if (LevelManager.Instance != null)
        {
            Debug.Log("Ini Level" + LevelManager.Instance.pilihanlevel);
            menuList = LevelManager.Instance.currentMenuList;
            maksimalNPC = LevelManager.Instance.currentMaksimalNPC;
        }
    }

    // 6. FUNGSI FACADE: Meneruskan perintah dari NPCController ke QueueManager
    public void BebaskanSlot(int slotIndex)
    {
        queueManager.BebaskanSlot(slotIndex);
    }

    public void ReturnNPC(GameObject npc)
    {
        npc.SetActive(false);
        npcPool.Enqueue(npc);
    }
}