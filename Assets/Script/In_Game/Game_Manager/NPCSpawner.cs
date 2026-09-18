using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // PENTING: Tambahkan ini untuk mengakses TextMeshPro

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

    public NPCQueueManager queueManager;

    // --- KODE BARU: Referensi UI Penghitung ---
    [Header("UI Progress NPC")]
    public TextMeshProUGUI teksProgressNPC;
    private Coroutine rutinitasAnimasiTeks;
    // ------------------------------------------

    private float timer;
    private int jumlahNPCSudahMuncul = 0;

    void Start()
    {
        LoadLevelData();

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
        queueManager.TempatiSlot(slotIndex);

        GameObject spawnNPC = npcPool.Dequeue();
        spawnNPC.transform.position = spawnPoint.position;
        spawnNPC.SetActive(true);
        AudioManager.instance.PlaySFXGhost();

        NPCController controller = spawnNPC.GetComponent<NPCController>();
        Transform targetWaypoint = queueManager.GetWaypoint(slotIndex);

        controller.InitializeNPC(targetWaypoint, slotIndex, menuList, minimalVariasiMenu, maksimalVariasiMenu);

        jumlahNPCSudahMuncul++;
    }

    private void LoadLevelData()
    {
        if (LevelManager.Instance != null)
        {
            menuList = LevelManager.Instance.currentMenuList;
            maksimalNPC = LevelManager.Instance.currentMaksimalNPC;
        }

        // --- KODE BARU: Set teks ke 0/X di awal permainan tanpa animasi ---
        UpdateProgressUI(false);
    }

    public void BebaskanSlot(int slotIndex)
    {
        queueManager.BebaskanSlot(slotIndex);
    }

    public void ReturnNPC(GameObject npc)
    {
        if (npcPool == null) return;

        npc.SetActive(false);
        npcPool.Enqueue(npc);

        // Tambah hitungan NPC yang sudah beres
        jumlahNPCSelesai++;

        // --- KODE BARU: Perbarui Teks UI dengan animasi Pop! ---
        UpdateProgressUI(true);

        // Cek apakah NPC yang sudah di-spawn mencapai batas, DAN semuanya sudah pulang
        if (jumlahNPCSudahMuncul >= maksimalNPC && jumlahNPCSelesai >= maksimalNPC)
        {
            if (LevelManager.Instance != null)
            {
                LevelManager.Instance.NextLevel();
            }

            if (levelEndManager != null && ScoreManager.Instance != null)
            {
                levelEndManager.TampilkanHasilAkhir(ScoreManager.Instance.totalSkor);
            }
        }
    }

    // ==========================================
    // FUNGSI BARU UNTUK UI PENGHITUNG
    // ==========================================
    private void UpdateProgressUI(bool pakaiAnimasi)
    {
        if (teksProgressNPC != null)
        {
            // Ubah teksnya
            teksProgressNPC.text = $"{jumlahNPCSelesai}/{maksimalNPC}";

            // Jalankan efek jus (animasi) jika diminta
            if (pakaiAnimasi)
            {
                if (rutinitasAnimasiTeks != null) StopCoroutine(rutinitasAnimasiTeks);
                rutinitasAnimasiTeks = StartCoroutine(AnimasiPopTeks());
            }
        }
    }

    private IEnumerator AnimasiPopTeks()
    {
        Vector3 skalaNormal = Vector3.one;
        Vector3 skalaMembesar = new Vector3(1.5f, 1.5f, 1f); // Membesar 1.5x
        float durasi = 0.15f;
        float waktu = 0f;

        // Fase 1: Membesar dengan cepat
        while (waktu < durasi)
        {
            waktu += Time.deltaTime;
            teksProgressNPC.transform.localScale = Vector3.Lerp(skalaNormal, skalaMembesar, waktu / durasi);
            yield return null;
        }

        waktu = 0f;

        // Fase 2: Kembali ke ukuran normal
        while (waktu < durasi)
        {
            waktu += Time.deltaTime;
            teksProgressNPC.transform.localScale = Vector3.Lerp(skalaMembesar, skalaNormal, waktu / durasi);
            yield return null;
        }

        teksProgressNPC.transform.localScale = skalaNormal; // Pastikan posisi akhir presisi
    }
}