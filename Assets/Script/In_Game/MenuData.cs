using UnityEngine;

[CreateAssetMenu(fileName = "MenuData", menuName = "Kasir/Data Menu Makanan")]
public class MenuData : ScriptableObject
{
    [Header("Informasi Menu")]
    public string menuName; // Nama menu
    public OrderData order;

}