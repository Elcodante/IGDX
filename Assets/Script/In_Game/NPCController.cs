using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

// 1. TAMBAHKAN STATUS 'Spawning' DI SINI
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

    public NPCState currentState;
    private Transform targetWaypoint;
    private NPCSpawner mySpawner;
    private int mySlotIndex;

    private SpriteRenderer tandaSeruRenderer;
    private SpriteRenderer npcSpriteRenderer;

    private NPCOrderHandler orderHandler;
    private SpriteRenderer sr;

    [Header("Ekspresi NPC")]
    public Sprite spriteSedih;
    private Sprite spriteNormal;

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
    }

    public void SetEkspresiSedih(bool apakahSedih)
    {
        if (spriteSedih == null || npcSpriteRenderer == null) return;

        // Ganti visual karakter sesuai kondisinya
        npcSpriteRenderer.sprite = apakahSedih ? spriteSedih : spriteNormal;
    }

    public void SetSpawner(NPCSpawner spawner) { mySpawner = spawner; }

    public void InitializeNPC(Transform assignedWaypoint, int slotIndex, MenuData[] menuList, int minVariasi, int maxVariasi)
    {
        targetWaypoint = assignedWaypoint;
        mySlotIndex = slotIndex;

        // 2. KUNCI STATUS KE 'Spawning' AGAR TIDAK LANGSUNG JALAN
        currentState = NPCState.Spawning;

        tandaSeru.SetActive(false);
        if (tandaSeruRenderer != null) tandaSeruRenderer.color = Color.white;

        orderHandler.ResetHandler();
        orderHandler.GenerateRandomOrder(menuList, minVariasi, maxVariasi);

        StartCoroutine(AnimasiMunculLaluJalan(targetWaypoint));
    }

    void Update()
    {
        // Fungsi Update ini otomatis MENGABAIKAN NPC yang statusnya 'Spawning'
        if (currentState == NPCState.WalkToCounter || currentState == NPCState.Leave)
        {
            MoveTowardsTarget();
        }
    }

    private void MoveTowardsTarget()
    {
        transform.position = Vector2.MoveTowards(transform.position, targetWaypoint.position, moveSpeed * Time.deltaTime);

        if (Vector2.Distance(transform.position, targetWaypoint.position) < 0.1f)
        {
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
            if (tandaSeruRenderer != null) tandaSeruRenderer.color = warnaPesananDiambil;

            orderHandler.MulaiTungguPesanan();

            OnPesananDiambil?.Invoke(orderHandler.daftarPesanan, npcSpriteRenderer.sprite);
        }
    
    }

    public bool CobaTerimaMakanan(DraggableItem2D makananPemain, out string orderIdTerhapus)
    {
        // Teruskan data makananPemain secara utuh ke OrderHandler
        bool diterima = orderHandler.CobaTerimaMakanan(makananPemain, out orderIdTerhapus);

        if (diterima)
        {
            if (orderHandler.ApakahSemuaPesananSelesai())
            {
                Pulang();
            }
        }
        return diterima;
    }

    public void Pulang()
    {
        currentState = NPCState.Leave;
        AudioManager.instance.PlaySFXGhost();
        tandaSeru.SetActive(false);
        targetWaypoint = (mySpawner != null && mySpawner.exitPoint != null) ? mySpawner.exitPoint : transform;
        mySpawner.BebaskanSlot(mySlotIndex);
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

        // 3. SETELAH ANIMASI SELESAI, BARU UBAH STATUS KE BERJALAN!
        currentState = NPCState.WalkToCounter;
    }
}