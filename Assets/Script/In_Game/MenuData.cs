using UnityEngine;

[CreateAssetMenu(fileName = "MenuData", menuName = "Kasir/Data Menu Makanan")]
public class MenuData : ScriptableObject
{
    [Header("Informasi Menu")]
    public string menuName; // Nama menu
    public Sprite menuImage; // Gambar menu

    [Header("Bahan & Syarat")]
    public JenisTepung jenisTepung; // Jenis tepung yang digunakan
}