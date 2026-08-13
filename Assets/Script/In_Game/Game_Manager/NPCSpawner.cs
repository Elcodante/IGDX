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

    [Header("Level Completion")]
    public LevelEndManager levelEndManager;
    private int jumlahNPCSelesai = 0;

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
        // CEK TERSANGKA 1: Apakah Sistem Pool Kosong?
        if (npcPool == null)
        {
            Debug.LogError("<color=red>[ERROR 1]</color> npcPool KOSONG! Ini biasanya terjadi karena ada error merah lain saat game baru saja di-Play (di fungsi Start).");
            return; // Hentikan sistem agar tidak crash
        }

        npc.SetActive(false);
        npcPool.Enqueue(npc);

        // Tambah hitungan NPC yang sudah beres
        jumlahNPCSelesai++;

        // Cek apakah NPC yang sudah di-spawn mencapai batas, DAN semuanya sudah pulang
        if (jumlahNPCSudahMuncul >= maksimalNPC && jumlahNPCSelesai >= maksimalNPC)
        {
            Debug.Log("Level Selesai! Semua NPC sudah pulang.");

            // CEK TERSANGKA 2: Apakah LevelEndManager belum dimasukkan?
            if (levelEndManager == null)
            {
                Debug.LogError("<color=red>[ERROR 2]</color> levelEndManager KOSONG! Anda belum menarik objek LevelEnd_Manager ke dalam kolom Spawner di Inspector.");
                return; // Hentikan sistem agar tidak crash
            }

            // CEK TERSANGKA 3: Apakah ScoreManager hilang dari Scene?
            if (ScoreManager.Instance == null)
            {
                Debug.LogError("<color=red>[ERROR 3]</color> ScoreManager KOSONG! Pastikan objek yang memiliki script ScoreManager ada menyala di dalam Scene.");
                return; // Hentikan sistem agar tidak crash
            }

            // Jika semua aman, panggil Panel Hasil!
            levelEndManager.TampilkanHasilAkhir(ScoreManager.Instance.totalSkor);
        }
    }
}