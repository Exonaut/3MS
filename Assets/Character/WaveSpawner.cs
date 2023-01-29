using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public struct EnemySpawn
{
    public GameObject Prefab { get; }
    public Vector3 Position { get; }
    public EnemySpawn(GameObject prefab, Vector3 position)
    {
        Prefab = prefab;
        Position = position;
    }
}

public struct PickupSpawn
{
    public GameObject Prefab { get; }
    public Vector3 Position { get; }
    public int Amount { get; }
    public PickupSpawn(GameObject prefab, Vector3 position, int amount)
    {
        Prefab = prefab;
        Position = position;
        Amount = amount;
    }
}

public struct Wave
{
    public float Length { get; private set; }
    public List<EnemySpawn> EnemySpawns { get; private set; }
    public List<PickupSpawn> PickupSpawns { get; private set; }
    public Wave(float length, List<EnemySpawn> enemySpawns, List<PickupSpawn> pickupSpawns)
    {
        Length = length;
        EnemySpawns = enemySpawns;
        PickupSpawns = pickupSpawns;
    }
}

public class WaveSpawner : MonoBehaviour
{
    public List<GameObject> EnemyPrefabs;
    public List<GameObject> PickupPrefabs;

    private List<Wave> waves;
    public int CurrentWave { get; private set; }
    public int WaveCount => waves.Count;
    public float CurrentWaveLength => waves[CurrentWave].Length;
    public float WaveStartingTime { get; private set; }
    public bool IsActive { get; private set; }

    private bool isCurrentWaveSpawned;
    private List<Hitable> currentlySpawnedEnemies;

    private void Start()
    {
        IsActive = false;
        isCurrentWaveSpawned = false;
        currentlySpawnedEnemies = new List<Hitable>();
        waves = new List<Wave>
        {
            new Wave(20, new List<EnemySpawn> { new EnemySpawn(EnemyPrefabs[0], new Vector3(-10, 0, 30)) },
            new List<PickupSpawn>{
                new PickupSpawn(PickupPrefabs[0], new Vector3(0, .5f, -10), 10),
                new PickupSpawn(PickupPrefabs[1], new Vector3(2, .5f, -10), 10),
            }),
            new Wave(25, new List<EnemySpawn> {
                new EnemySpawn(EnemyPrefabs[0], new Vector3(0, 0, 30)) },
            new List<PickupSpawn>{
                new PickupSpawn(PickupPrefabs[0], new Vector3(0, .5f, -12), 20),
                new PickupSpawn(PickupPrefabs[1], new Vector3(2, .5f, -12), 20),
            }),
            new Wave(30, new List<EnemySpawn> { new EnemySpawn(EnemyPrefabs[0], new Vector3(10, 0, 30)) }, new List<PickupSpawn>()),
        };
        Initiate();
    }

    public void Initiate()
    {
        CleanUp();
        CurrentWave = 0;
        IsActive = true;
        isCurrentWaveSpawned = false;
        currentlySpawnedEnemies = new List<Hitable>();
    }

    public void CleanUp()
    {
        foreach (var entity in currentlySpawnedEnemies)
            Destroy(entity);
    }

    void Update()
    {
        if (IsActive)
        {
            if (CurrentWave >= WaveCount)
            {
                IsActive = false;
                return;
            }

            if (!isCurrentWaveSpawned)
            {
                SpawnWave(waves[CurrentWave].EnemySpawns, waves[CurrentWave].PickupSpawns);
                WaveStartingTime = Time.time;
                isCurrentWaveSpawned = true;
            }
            else
            {
                if (AreAllEnemiesDead() || Time.time - WaveStartingTime >= CurrentWaveLength)
                {
                    CurrentWave++;
                    isCurrentWaveSpawned = false;
                }
            }
        }
    }

    private void SpawnWave(List<EnemySpawn> enemySpawns, List<PickupSpawn> pickupSpawns)
    {
        foreach (var enemy in enemySpawns)
        {
            var gameObject = Instantiate(enemy.Prefab, enemy.Position, Quaternion.identity);
            var hitable = gameObject.GetComponent<Hitable>();
            if (hitable)
                currentlySpawnedEnemies.Add(hitable);
        }
        foreach (var spawn in pickupSpawns)
        {
            var gameObject = Instantiate(spawn.Prefab, spawn.Position, Quaternion.identity);
            if (gameObject.TryGetComponent<IPickup>(out var pickup))
                pickup.Amount = spawn.Amount;
        }
    }

    private bool AreAllEnemiesDead()
    {
        return currentlySpawnedEnemies.All(entity => entity.health <= 0);
    }
}
