using UnityEngine;

public class NPCQueueManager : MonoBehaviour
{
    [Header("Antrean Kasir")]
    public Transform[] queueWaypoints;
    private bool[] slotOccupied;

    private void Awake()
    {
        slotOccupied = new bool[queueWaypoints.Length];
    }
    
    public int GetEmptySlot()
    {
        for(int i = 0; i < slotOccupied.Length; i++)
        {
            if(!slotOccupied[i])
            {
                return i;
            }
        }
        return -1; // No empty slot found
    }

    public Transform GetWaypoint(int index)
    {
        return queueWaypoints[index];
    }

    public void TempatiSlot(int slotIndex)
    {
        slotOccupied[slotIndex] = true;
    }

    public void BebaskanSlot(int slotIndex)
    {
        slotOccupied[slotIndex] = false;
    }
}
