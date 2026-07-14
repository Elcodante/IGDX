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

    [Header("Warna tanda seru saat pesanan diambil")]
    public Color warnaPesananDiambil = new Color(0.4f, 0.4f, 0.4f, 1f);
    
    public UnityEvent<OrderData> OnPesananDiambil;

    private NPCState currentState;
    private Transform targetWaypoint;
    private NPCSpawner mySpawner;
    private int mySlotIndex;

    private SpriteRenderer tandaSeruRenderer;

    void Awake()
    {
        if(tandaSeru != null)
        {
            tandaSeruRenderer = tandaSeru.GetComponent<SpriteRenderer>();
        }
    }

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

        if (tandaSeruRenderer != null)
        {
            tandaSeruRenderer.color = Color.white; // Warna default
        }

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
        // PEMBATAS GLOBAL: Jika ada panel pesanan LAIN yang sedang terbuka, abaikan klik
        if (UIManager.IsPanelOpen)
        {
            Debug.Log("Klik diabaikan karena sedang ada panel aktif.");
            return;
        }

        // KONDISI 1: Pesanan baru diambil pertama kali
        if (currentState == NPCState.WaitingToOrder)
        {
            // PENTING: Jangan di-deactivate, melainkan ubah warnanya jadi gelap
            if (tandaSeruRenderer != null)
            {
                tandaSeruRenderer.color = warnaPesananDiambil;
            }

            currentState = NPCState.WaitingForFood; // Status berubah menunggu makanan

            Debug.Log("Mengambil pesanan pertama kali: " + currentOrder.idResep);
            OnPesananDiambil?.Invoke(currentOrder);
        }

        // KONDISI 2: Pesanan sudah pernah diambil, tetapi pemain klik LAGI untuk mengintip resep
        else if (currentState == NPCState.WaitingForFood)
        {
            Debug.Log("Melihat kembali pesanan milik NPC ini: " + currentOrder.idResep);

            // Panggil kembali panel UI untuk menampilkan resep yang sama
            OnPesananDiambil?.Invoke(currentOrder);
        }
    }

    private void GenerateRandomOrder()
    {
        string[] contohKue = { "Serabi", "Putu Ayu" };
        currentOrder.idResep = contohKue[Random.Range(0, contohKue.Length)];

        currentOrder.isian = (TingkatIsian)Random.Range(0, 3);
        currentOrder.tepung = (JenisTepung)Random.Range(0, 4);

        currentOrder.targetManis = Random.Range(20, 90);
        currentOrder.targetLembut = Random.Range(20, 90);
        currentOrder.targetGurih = Random.Range(20, 90);
    }

    public void Pulang()
    {
        mySpawner.ReturnNPC(this.gameObject, mySlotIndex);
    }
}