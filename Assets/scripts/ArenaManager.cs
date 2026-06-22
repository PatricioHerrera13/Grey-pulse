using System.Collections.Generic;
using UnityEngine;

public class ArenaManager : MonoBehaviour
{
    public static ArenaManager Instance;

    [Header("Arena Prefabs")]
    public List<GameObject> arenaPrefabs =
        new List<GameObject>();

    [Header("Spawn")]
    public Transform arenaSpawnPoint;

    private GameObject currentArena;

    private void Awake()
    {
        Instance = this;
    }

    public void SpawnRandomArena()
    {
        if (currentArena != null)
        {
            Destroy(currentArena);
        }

        int randomIndex =
            Random.Range(
                0,
                arenaPrefabs.Count
            );

        currentArena =
    Instantiate(
        arenaPrefabs[randomIndex],
        arenaSpawnPoint.position,
        Quaternion.identity
    );
    }
}