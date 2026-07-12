using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public enum NPCState
{
    WalkToCounter,
    WaitingToOrder,
    WaitingForFood,
    Reacting,
    Leave
}

public class NPCController : MonoBehaviour, IPointerClickHandler
{
    [Header("NPC Settings")]
    public float moveSpeed = 3f;
    public OrderData currentOrder;

    [Header("UI / Visuals")]
    public GameObject tandaSeru;
    public UnityEvent<OrderData> OnPesananDiambil;

    private NPCState currentState;
    private Transform targetWaypoint;
    private NPCSpawner mySpawner;
    private int mySlotIndex;

    public void SetSpawner(NPCSpawner spawner)
    {
        mySpawner = spawner;
    }

    public void InitializeNPC(Transform assignedWaypoint, int slotIndex)
    {
        targetWaypoint = assignedWaypoint;
        mySlotIndex = slotIndex;
        currentState = NPCState.WalkToCounter;

        tandaSeru.SetActive(false);
        GenerateRandomOrder();
    }

    void Update()
    {
        if (currentState == NPCState.WalkToCounter)
        {
            MoveTowardsTarget();
        }
    }

    private void MoveTowardsTarget()
    {
        transform.position = Vector2.MoveTowards(transform.position, targetWaypoint.position, moveSpeed * Time.deltaTime);

        if (Vector2.Distance(transform.position, targetWaypoint.position) < 0.1f)
        {
            currentState = NPCState.WaitingToOrder;
            tandaSeru.SetActive(true);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // PERIKSA SAKELAR: Jika ada panel terbuka, batalkan proses klik!
        if (UIManager.IsPanelOpen)
        {
            Debug.Log("Klik diabaikan karena ada pesanan lain yang sedang diproses.");
            return;
        }

        if (currentState == NPCState.WaitingToOrder)
        {
            tandaSeru.SetActive(false);
            currentState = NPCState.WaitingForFood;

            Debug.Log("Mengirim pesanan ke UI: " + currentOrder.idResep);
            OnPesananDiambil?.Invoke(currentOrder);
        }
    }

    private void GenerateRandomOrder()
    {
        currentOrder.idResep = "Serabi";
        currentOrder.isian = TingkatIsian.Banyak;
        currentOrder.tepung = JenisTepung.Beras;
    }

    public void Pulang()
    {
        mySpawner.ReturnNPC(this.gameObject, mySlotIndex);
    }
}