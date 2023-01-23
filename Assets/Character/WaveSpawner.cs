using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public struct SpawnEntity
{
    public GameObject Prefab;
    public Vector3 Position;
    public SpawnEntity(GameObject prefab, Vector3 position)
    {
        Prefab = prefab;
        Position = position;
    }
}

public struct Wave
{
    public float Length { get; private set; }
    public List<SpawnEntity> SpawnEntities { get; private set; }
    public Wave(float length, List<SpawnEntity> spawnEntities)
    {
        Length = length;
        SpawnEntities = spawnEntities;
    }
}
public class WaveSpawner : MonoBehaviour
{
    public List<GameObject> EnemyPrefabs;

    private List<Wave> waves;
    public int currentWave { get; private set; }
    public int waveCount { get { return waves.Count; } }
    public float currentWaveLength { get { return waves[currentWave].Length; } }
    public float waveStartingTime { get; private set; }
    public bool isActive { get; private set; }
    private bool isCurrentWaveSpawned;
    private List<Hitable> currentlySpawnedEntities;

    private void Start()
    {
        isActive = false;
        isCurrentWaveSpawned = false;
        currentlySpawnedEntities = new List<Hitable>();
        waves = new List<Wave>
        {
            new Wave(20, new List<SpawnEntity> { new SpawnEntity(EnemyPrefabs[0], new Vector3(-10, 0, 30)) }),
            new Wave(20, new List<SpawnEntity> { new SpawnEntity(EnemyPrefabs[0], new Vector3(0, 0, 30)) }),
            new Wave(20, new List<SpawnEntity> { new SpawnEntity(EnemyPrefabs[0], new Vector3(10, 0, 30)) }),
        };
        Initiate();
    }

    public void Initiate()
    {
        CleanUp();
        currentWave = 0;
        isActive = true;
        isCurrentWaveSpawned = false;
        currentlySpawnedEntities = new List<Hitable>();
    }

    public void CleanUp()
    {
        foreach (var entity in currentlySpawnedEntities)
            Destroy(entity);
    }

    void Update()
    {
        if (isActive)
        {
            if (currentWave >= waveCount)
            {
                isActive = false;
                return;
            }

            if (!isCurrentWaveSpawned)
            {
                SpawnWave(waves[currentWave].SpawnEntities);
                waveStartingTime = Time.time;
                isCurrentWaveSpawned = true;
            }
            else
            {
                if (AreAllEnemiesDead() || Time.time - waveStartingTime >= currentWaveLength)
                {
                    currentWave++;
                    isCurrentWaveSpawned = false;
                }
            }
        }
    }

    private void SpawnWave(List<SpawnEntity> spawnEntities)
    {
        foreach (var entity in spawnEntities)
            currentlySpawnedEntities.Add(Instantiate(entity.Prefab, entity.Position, Quaternion.identity).GetComponent<Hitable>());
    }

    private bool AreAllEnemiesDead()
    {
        return currentlySpawnedEntities.All(entity => entity.health <= 0);
    }
}
