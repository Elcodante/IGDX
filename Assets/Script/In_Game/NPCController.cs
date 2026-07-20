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
    
    public UnityEvent<OrderData, Sprite> OnPesananDiambil;

    public NPCState currentState;
    private Transform targetWaypoint;
    private NPCSpawner mySpawner;
    private int mySlotIndex;

    private SpriteRenderer tandaSeruRenderer;
    private SpriteRenderer npcSpriteRenderer;

    void Awake()
    {
        if(tandaSeru != null)
        {
            tandaSeruRenderer = tandaSeru.GetComponent<SpriteRenderer>();
        }
        npcSpriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void SetSpawner(NPCSpawner spawner)
    {
        mySpawner = spawner;
    }

    public void InitializeNPC(Transform assignedWaypoint, int slotIndex, MenuData[] menuList)
    {
        targetWaypoint = assignedWaypoint;
        mySlotIndex = slotIndex;
        currentState = NPCState.WalkToCounter;

        tandaSeru.SetActive(false);

        if (tandaSeruRenderer != null)
        {
            tandaSeruRenderer.color = Color.white; // Warna default
        }

        GenerateRandomOrder(menuList);
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
            if(currentState == NPCState.WalkToCounter)
            {
                currentState = NPCState.WaitingToOrder;
                tandaSeru.SetActive(true);
            }
            else if(currentState == NPCState.Leave)
            {
                mySpawner.ReturnNPC(this.gameObject);
            }
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
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
            
            if(OrderManager.Instance != null)
            {
                OrderManager.Instance.KirimPesananKeDapur(currentOrder);
            }

            OnPesananDiambil?.Invoke(currentOrder, npcSpriteRenderer.sprite);
        }

        // KONDISI 2: Pesanan sudah pernah diambil, tetapi pemain klik LAGI untuk mengintip resep
        else if (currentState == NPCState.WaitingForFood)
        {
            Debug.Log("Melihat kembali pesanan milik NPC ini: " + currentOrder.idResep);

            // Panggil kembali panel UI untuk menampilkan resep yang sama
            OnPesananDiambil?.Invoke(currentOrder, npcSpriteRenderer.sprite);
        }
    }

    private void GenerateRandomOrder(MenuData[] menuList)
    {
        if(menuList == null || menuList.Length == 0)
        {
            Debug.LogError("Menu list kosong atau null!");
            return;
        }

        MenuData menupilihan = menuList[Random.Range(0, menuList.Length)];

        currentOrder.idResep = menupilihan.menuName;
        currentOrder.isian = (TingkatIsian)Random.Range(0, 3); // Random antara 0 dan 2

        currentOrder.targetManis = (TingkatRasa)Random.Range(0, 4);
        currentOrder.targetLembut = (TingkatRasa)Random.Range(0, 4);
        currentOrder.targetGurih = (TingkatRasa)Random.Range(0, 4);
    }

    public void Pulang()
    {
        currentState = NPCState.Leave;
        tandaSeru.SetActive(false);

        if(mySpawner != null)
        {
            targetWaypoint = mySpawner.exitPoint;
        }
        else
        {
            targetWaypoint = mySpawner.spawnPoint; // Fallback jika mySpawner null, meskipun seharusnya tidak terjadi
        }

        mySpawner.BebaskanSlot(mySlotIndex);
    }
}