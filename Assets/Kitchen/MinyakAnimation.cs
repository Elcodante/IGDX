using UnityEngine;

public class MinyakAnimation : MonoBehaviour
{
    public static MinyakAnimation Instance { get; private set; }

    [SerializeField] private GameObject _MinyakAnimation;
    [SerializeField] private float durasiAnimasi = 1f;

    private CookingAppliance targetAppliance; // BARU: simpen siapa yang minta

    private void Awake()
    {
        Instance = this;
    }

    // GANTI: sekarang minta parameter appliance target
    public void Minyak(CookingAppliance appliance)
    {
        targetAppliance = appliance;

        _MinyakAnimation.SetActive(false);
        _MinyakAnimation.SetActive(true);

        CancelInvoke(nameof(MatikanMinyak));
        Invoke(nameof(MatikanMinyak), durasiAnimasi);
    }

    public void MatikanMinyak()
    {
        _MinyakAnimation.SetActive(false);

        // BARU: begitu animasi tuang selesai, nyalain sprite minyak statis di wajan
        if (targetAppliance != null)
        {
            targetAppliance.TampilkanMinyakDiWajan();
        }
    }
}