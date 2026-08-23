using UnityEngine;

public class FogSpawner : MonoBehaviour
{
    [SerializeField] private GameObject fogPrefab;
    [SerializeField] private Transform[] leftSpawns = new Transform[2];  
    [SerializeField] private Transform[] rightSpawns = new Transform[2];
    [SerializeField] private float spawnInterval = 3f;

    private void Start()
    {
        InvokeRepeating(nameof(SpawnFog), 1f, spawnInterval);
    }

    private void SpawnFog()
    {

        bool spawnFromLeft = Random.value > 0.5f;

        Transform selectedSpawn = spawnFromLeft 
            ? leftSpawns[Random.Range(0, leftSpawns.Length)] 
            : rightSpawns[Random.Range(0, rightSpawns.Length)];

        GameObject fog = Instantiate(fogPrefab, selectedSpawn.position, Quaternion.identity);
   
        Vector2 moveDirection = spawnFromLeft ? Vector2.right : Vector2.left;
        fog.GetComponent<FogMovement>().SetDirection(moveDirection);

        Debug.Log("oy");
    }
}