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

    [Tooltip("Titik tujuan NPC setelah mereka selesai.")]
    public Transform exitPoint;

    [Tooltip("Jumlah total npc yang akan muncul di level ini.")]
    public int maksimalNPC;

    [Tooltip("Batas maksimal variasi menu yang bisa dimiliki NPC.")]
    public int maksimalVariasiMenu;

    [Tooltip("Batas Minimal variasinya menu yang bisa dimiliki NPC.")]
    public int minimalVariasiMenu;

    [Header("Antrean Kasir")]
    public Transform[] queueWaypoints;
    private bool[] slotOccupied;

    [Header("Object Pool")]
    public int poolSize = 6;
    private Queue<GameObject> npcPool;

    [Header("UI Reference")]
    // Tambahkan variabel ini untuk menyimpan referensi UI
    public UIManager uiManager;

    private float timer;

    private int jumlahNPCSudahMuncul = 0;

    void Start()
    {
        LoadLevelData();
        slotOccupied = new bool[queueWaypoints.Length];
        npcPool = new Queue<GameObject>();

        if (npcPrefab.Length == 0 || npcPrefab[0] == null)
        {
            Debug.LogError("NPC Prefab array is empty. Please assign at least one NPC prefab.");
            return;
        }

        if (menuList.Length == 0)
        {
            Debug.LogError("Menu list is empty. Please assign at least one menu item.");
            return;
        }

        for (int i = 0; i < poolSize; i++)
        {
            GameObject prefabTerpilih = npcPrefab[Random.Range(0, npcPrefab.Length)];

            GameObject obj = Instantiate(prefabTerpilih);
            obj.SetActive(false);

            NPCController controller = obj.GetComponent<NPCController>();
            controller.SetSpawner(this);
            controller.OnPesananDiambil.AddListener(uiManager.TampilkanPanelPesanan); // Pastikan UIManager memiliki metode ini

            npcPool.Enqueue(obj);
        }
    }

    void Update()
    {
        if (jumlahNPCSudahMuncul >= maksimalNPC)
        {
            return; // Tidak spawn NPC lagi jika sudah mencapai maksimal
        }

        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            int availableSlot = GetEmptySlot();

            if (availableSlot != -1 && npcPool.Count > 0)
            {
                SpawnNPC(availableSlot);
                timer = 0f;
            }
        }
    }

    private void LoadLevelData()
    {
        if (LevelManager.Instance != null)
        {
            Debug.Log("Ini Level" + LevelManager.Instance.pilihanlevel);
            menuList = LevelManager.Instance.currentMenuList;
            maksimalNPC = LevelManager.Instance.currentMaksimalNPC;

            Debug.Log("Berhasil memuat data level dari LevelManager!");
        }
        else
        {
            Debug.LogWarning("LevelManager tidak ditemukan.");
        }
    }

    private int GetEmptySlot()
    {
        for (int i = 0; i < slotOccupied.Length; i++)
        {
            if (!slotOccupied[i]) return i;
        }
        return -1;
    }

    private void SpawnNPC(int slotIndex)
    {
        slotOccupied[slotIndex] = true;

        GameObject spawnNPC = npcPool.Dequeue();
        spawnNPC.transform.position = spawnPoint.position;
        spawnNPC.SetActive(true);

        NPCController controller = spawnNPC.GetComponent<NPCController>();
        controller.InitializeNPC(queueWaypoints[slotIndex], slotIndex, menuList,minimalVariasiMenu, maksimalVariasiMenu);

        jumlahNPCSudahMuncul++;
        Debug.Log($"NPC spawned. Total NPCs spawned: {jumlahNPCSudahMuncul}/{maksimalNPC}");
    }

    public void BebaskanSlot(int slotIndex)
    {
        slotOccupied[slotIndex] = false;
    }
    public void ReturnNPC(GameObject npc)
    {
        npc.SetActive(false);
        npcPool.Enqueue(npc);
    }

}