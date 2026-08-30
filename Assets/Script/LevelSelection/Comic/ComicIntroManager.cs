using System.Collections;
using UnityEngine;

public class ComicIntroManager : MonoBehaviour
{
    [Header("Data Komik")]
    public GameObject[] panelkomik;

    [Header("Referensi Player")]
    public PlayerMovement playerMovement;

    private int currentPanelIndex = 0;
    private bool sedangBeranimasi = false;

    private static bool sudahPernahLihatKomik = false;

    private void Start()
    {
        if (sudahPernahLihatKomik)
        {
            MulaiGame();
            return; // Hentikan fungsi Start di sini, jangan jalankan kode di bawahnya
        }

        if (playerMovement != null)
        {
            playerMovement.canMove = false;
        }

        foreach (GameObject panel in panelkomik)
        {
            if(panel != null)
            {
                panel.SetActive(false);
            }
        }
    }

    public void KlikLayar()
    {
        if(sedangBeranimasi)
        {
            return;
        }

        if (currentPanelIndex < panelkomik.Length)
        {
            GameObject panelAktif = panelkomik[currentPanelIndex];
            panelAktif.SetActive(true);

            float durasiAnimasi = 0.5f;
            UIanimated uIanimated = panelAktif.GetComponent<UIanimated>();
            if(uIanimated != null)
            {
                durasiAnimasi = uIanimated.duration + uIanimated.delay;
            }

            StartCoroutine(KunciLayarSementara(durasiAnimasi));

            currentPanelIndex++;
        }
        else
        {
            MulaiGame();
        }
    }

    private IEnumerator KunciLayarSementara(float durasi)
    {
        sedangBeranimasi = true;
        yield return new WaitForSecondsRealtime(durasi);
        sedangBeranimasi = false;
    }

    private void MulaiGame()
    {
        sudahPernahLihatKomik = true;

        if(playerMovement != null)
        {
            playerMovement.canMove = true;
        }

        gameObject.SetActive(false);
    }
}
