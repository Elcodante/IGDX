using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public enum NPCState { Spawning, WalkToCounter, WaitingToOrder, WaitingForFood, Leave }

[RequireComponent(typeof(NPCOrderHandler))]
public class NPCController : MonoBehaviour, IPointerClickHandler
{
    [Header("NPC Settings")]
    public float moveSpeed = 3f;

    [Header("UI / Visuals")]
    public GameObject tandaSeru;
    public Color warnaPesananDiambil = new Color(0.4f, 0.4f, 0.4f, 1f);
    public UnityEvent<List<OrderData>, Sprite> OnPesananDiambil;

    [Header("Ekspresi Karakter")]
    public Sprite spriteSedih;
    private Sprite spriteNormal;
    [Header("Suara Karakter")]
    [SerializeField] private AudioClip sfx;

    // --- VARIABEL JUICING BARU ---
    [Header("Juicing Settings (Animasi Prosedural)")]
    public float kecepatanNapas = 4f;
    public float skalaNapas = 0.03f;
    public float kecepatanGetar = 35f;
    public float jarakGetar = 0.05f;

    private Vector3 skalaAwal;
    private Vector3 posisiVisualAwal;
    private bool sedangSedih = false;
    private bool sedangLompat = false;
    // -----------------------------

    public NPCState currentState;
    private Transform targetWaypoint;
    private NPCSpawner mySpawner;
    private int mySlotIndex;

    private SpriteRenderer tandaSeruRenderer;
    private SpriteRenderer npcSpriteRenderer;
    private NPCOrderHandler orderHandler;
    private SpriteRenderer sr;

    void Awake()
    {
        if (tandaSeru != null) tandaSeruRenderer = tandaSeru.GetComponent<SpriteRenderer>();
        npcSpriteRenderer = GetComponent<SpriteRenderer>();
        sr = GetComponent<SpriteRenderer>();
        orderHandler = GetComponent<NPCOrderHandler>();

        if (npcSpriteRenderer != null)
        {
            spriteNormal = npcSpriteRenderer.sprite;
        }

        // Simpan ukuran dan posisi asli untuk animasi
        skalaAwal = transform.localScale;
    }

    public void InitializeNPC(Transform assignedWaypoint, int slotIndex, MenuData[] menuList, int minVariasi, int maxVariasi)
    {
        targetWaypoint = assignedWaypoint;
        mySlotIndex = slotIndex;
        currentState = NPCState.Spawning;

        tandaSeru.SetActive(false);
        if (tandaSeruRenderer != null) tandaSeruRenderer.color = Color.white;

        sedangSedih = false;
        sedangLompat = false;
        transform.localScale = skalaAwal; // Reset skala
        SetEkspresiSedih(false);

        orderHandler.ResetHandler();
        orderHandler.GenerateRandomOrder(menuList, minVariasi, maxVariasi);

        StartCoroutine(AnimasiMunculLaluJalan(targetWaypoint));
    }

    void Update()
    {
        if (currentState == NPCState.WalkToCounter || currentState == NPCState.Leave)
        {
            MoveTowardsTarget();
        }

        // --- LOGIKA JUICING BERJALAN SAAT DIAM DI MEJA ---
        if ((currentState == NPCState.WaitingToOrder || currentState == NPCState.WaitingForFood) && !sedangLompat)
        {
            AnimasiDiamJuice();
        }
    }

    // --- FUNGSI JUICING (Napas & Gemetar) ---
    private void AnimasiDiamJuice()
    {
        if (sedangSedih)
        {
            // 1. PANIC SHAKE: Bergetar cepat ke kiri dan kanan
            float geserX = Mathf.Sin(Time.time * kecepatanGetar) * jarakGetar;
            transform.position = new Vector3(targetWaypoint.position.x + geserX, transform.position.y, transform.position.z);
            transform.localScale = skalaAwal; // Pastikan skala kembali normal
        }
        else
        {
            // 2. IDLE BREATHING: Membesar dan mengecil perlahan (Squash & Stretch)
            float napas = Mathf.Sin(Time.time * kecepatanNapas) * skalaNapas;
            transform.localScale = new Vector3(skalaAwal.x - napas, skalaAwal.y + napas, skalaAwal.z);
            transform.position = new Vector3(targetWaypoint.position.x, transform.position.y, transform.position.z); // Pastikan posisi X terkunci di waypoint
        }
    }

    private void MoveTowardsTarget()
    {
        // Beri sedikit efek mentul-mentul saat jalan (Opsional)
        float jalanMentul = Mathf.Abs(Mathf.Sin(Time.time * 15f)) * 0.05f;

        Vector3 targetPos = new Vector3(targetWaypoint.position.x, targetWaypoint.position.y + jalanMentul, targetWaypoint.position.z);
        transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);

        if (Vector2.Distance(new Vector2(transform.position.x, transform.position.y), new Vector2(targetWaypoint.position.x, targetWaypoint.position.y)) < 0.1f)
        {
            // Kunci posisi y agar tidak melayang setelah jalan
            transform.position = new Vector3(transform.position.x, targetWaypoint.position.y, transform.position.z);

            if (currentState == NPCState.WalkToCounter)
            {
                currentState = NPCState.WaitingToOrder;
                tandaSeru.SetActive(true);
            }
            else if (currentState == NPCState.Leave)
            {
                mySpawner.ReturnNPC(this.gameObject);
            }
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (UIManager.IsPanelOpen) return;

        if (currentState == NPCState.WaitingToOrder)
        {
            currentState = NPCState.WaitingForFood;
            AudioManager.instance.PlaySFX(sfx);
            if (tandaSeruRenderer != null) tandaSeruRenderer.color = warnaPesananDiambil;
            orderHandler.MulaiTungguPesanan();
            OnPesananDiambil?.Invoke(orderHandler.daftarPesanan, npcSpriteRenderer.sprite);
        }
    }

    public bool CobaTerimaMakanan(DraggableItem2D makananPemain, out string orderIdTerhapus)
    {
        bool diterima = orderHandler.CobaTerimaMakanan(makananPemain, out orderIdTerhapus);

        if (diterima)
        {
            // --- JUICING: LOMPAT KEGIRANGAN SAAT MAKANAN BENAR ---
            StartCoroutine(AnimasiLompatBahagia());

            if (orderHandler.ApakahSemuaPesananSelesai())
            {
                Pulang();
            }
        }
        return diterima;
    }

    // --- COROUTINE JUICING LOMPAT ---
    private IEnumerator AnimasiLompatBahagia()
    {
        sedangLompat = true;
        float durasiLompat = 0.3f;
        float waktu = 0f;

        Vector3 posisiAwalLompat = transform.position;
        Vector3 posisiPuncak = posisiAwalLompat + new Vector3(0, 0.5f, 0); // Lompat setengah unit ke atas

        // Squash sebelum lompat (Menunduk)
        transform.localScale = new Vector3(skalaAwal.x * 1.2f, skalaAwal.y * 0.8f, skalaAwal.z);
        yield return new WaitForSeconds(0.05f);

        // Melayang ke atas
        while (waktu < durasiLompat / 2)
        {
            waktu += Time.deltaTime;
            transform.position = Vector3.Lerp(posisiAwalLompat, posisiPuncak, waktu / (durasiLompat / 2));
            transform.localScale = new Vector3(skalaAwal.x * 0.9f, skalaAwal.y * 1.1f, skalaAwal.z); // Stretch saat di udara
            yield return null;
        }

        waktu = 0f;

        // Mendarat ke bawah
        while (waktu < durasiLompat / 2)
        {
            waktu += Time.deltaTime;
            transform.position = Vector3.Lerp(posisiPuncak, posisiAwalLompat, waktu / (durasiLompat / 2));
            yield return null;
        }

        // Squash saat mendarat
        transform.position = posisiAwalLompat;
        transform.localScale = new Vector3(skalaAwal.x * 1.1f, skalaAwal.y * 0.9f, skalaAwal.z);
        yield return new WaitForSeconds(0.05f);

        transform.localScale = skalaAwal;
        sedangLompat = false;
    }

    public void Pulang()
    {
        currentState = NPCState.Leave;
        AudioManager.instance.PlaySFXGhost();
        AudioManager.instance.PlaySFX(sfx);
        tandaSeru.SetActive(false);
        targetWaypoint = (mySpawner != null && mySpawner.exitPoint != null) ? mySpawner.exitPoint : transform;
        mySpawner.BebaskanSlot(mySlotIndex);
    }

    public void SetEkspresiSedih(bool apakahSedih)
    {
        if (spriteSedih == null || npcSpriteRenderer == null) return;

        sedangSedih = apakahSedih; // Simpan status sedih untuk trigger getaran
        npcSpriteRenderer.sprite = apakahSedih ? spriteSedih : spriteNormal;
    }

    private IEnumerator AnimasiMunculLaluJalan(Transform targetWaypoint)
    {
        if (sr != null)
        {
            Color warna = sr.color;
            warna.a = 0f;
            sr.color = warna;

            float waktuFade = 0f;
            float durasiFade = 2f;

            while (waktuFade < durasiFade)
            {
                waktuFade += Time.deltaTime;
                warna.a = Mathf.Lerp(0f, 1f, waktuFade / durasiFade);
                sr.color = warna;
                yield return null;
            }

            warna.a = 1f;
            sr.color = warna;
        }

        currentState = NPCState.WalkToCounter;
    }

    public void SetSpawner(NPCSpawner spawner)
    {
        mySpawner = spawner;
    }
}