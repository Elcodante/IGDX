using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public enum NPCState { WalkToCounter, WaitingToOrder, WaitingForFood, Leave }

[RequireComponent(typeof(NPCOrderHandler))] // Memastikan script Order otomatis terpasang
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

    // Referensi ke "Buku Catatan" NPC
    private NPCOrderHandler orderHandler;

    void Awake()
    {
        if (tandaSeru != null) tandaSeruRenderer = tandaSeru.GetComponent<SpriteRenderer>();
        npcSpriteRenderer = GetComponent<SpriteRenderer>();

        // Ambil komponen handler
        orderHandler = GetComponent<NPCOrderHandler>();
    }

    public void SetSpawner(NPCSpawner spawner) { mySpawner = spawner; }

    public void InitializeNPC(Transform assignedWaypoint, int slotIndex, MenuData[] menuList, int minVariasi, int maxVariasi)
    {
        targetWaypoint = assignedWaypoint;
        mySlotIndex = slotIndex;
        currentState = NPCState.WalkToCounter;

        tandaSeru.SetActive(false);
        if (tandaSeruRenderer != null) tandaSeruRenderer.color = Color.white;

        // Suruh handler mereset dan mengacak pesanan
        orderHandler.ResetHandler();
        orderHandler.GenerateRandomOrder(menuList, minVariasi, maxVariasi);
    }

    void Update()
    {
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
            if (tandaSeruRenderer != null) tandaSeruRenderer.color = warnaPesananDiambil;
            currentState = NPCState.WaitingForFood;

            // Suruh handler mengirim tiket ke dapur
            orderHandler.KirimKeDapur();

            OnPesananDiambil?.Invoke(orderHandler.daftarPesanan, npcSpriteRenderer.sprite);
        }
        else if (currentState == NPCState.WaitingForFood)
        {
            OnPesananDiambil?.Invoke(orderHandler.daftarPesanan, npcSpriteRenderer.sprite);
        }
    }

    // Fungsi Facade (Jembatan) untuk NPCDropTarget
    public bool CobaTerimaMakanan(string idMakananDiberikan)
    {
        // Lempar tugas pengecekan ke orderHandler
        bool diterima = orderHandler.CobaTerimaMakanan(idMakananDiberikan);

        if (diterima)
        {
            // Cek apakah NPC ini sudah kenyang (semua pesanan terpenuhi)
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
        tandaSeru.SetActive(false);
        targetWaypoint = (mySpawner != null && mySpawner.exitPoint != null) ? mySpawner.exitPoint : transform;
        mySpawner.BebaskanSlot(mySlotIndex);
    }
}