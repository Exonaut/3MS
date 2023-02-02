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

public enum SpawnTriggerType
{
    Timer,
    Button
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

    public SpawnTriggerType spawnTriggerType;
    public SpawnButtonController SpawnButton;
    public float TimeBeforeFirstWave;

    private List<Wave> waves;
    public int CurrentWave { get; private set; }
    public int WaveCount => waves.Count;
    public float CurrentWaveLength => IsActive ? waves[CurrentWave].Length : spawnTriggerType == SpawnTriggerType.Timer ? TimeBeforeFirstWave : 0f;
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
            // 1
            new Wave(40, new List<EnemySpawn> {
                new EnemySpawn(EnemyPrefabs[0], new Vector3(4.19999981f,1.40999997f,-11.5f)),
                new EnemySpawn(EnemyPrefabs[0], new Vector3(-8.80000019f,1.40999997f,3.0999999f)),
                new EnemySpawn(EnemyPrefabs[0], new Vector3(-6.53000021f,1.40999997f,13.1000004f)),
                new EnemySpawn(EnemyPrefabs[0], new Vector3(1.85000002f,1.40999997f,19.6100006f)),
            },
            new List<PickupSpawn>{
                new PickupSpawn(PickupPrefabs[0], new Vector3(15f,1.99000001f,-21.2000008f), 10),
                new PickupSpawn(PickupPrefabs[1], new Vector3(15.2600002f,1.99000001f,25.0300007f), 10),
            }),
            // 2
            new Wave(30, new List<EnemySpawn> {
                new EnemySpawn(EnemyPrefabs[0], new Vector3(47.9199982f,7.73000002f,-69.9800034f)),
                new EnemySpawn(EnemyPrefabs[1], new Vector3(62.3199997f,4.07000017f,-0.50999999f)),
                new EnemySpawn(EnemyPrefabs[0], new Vector3(70.0899963f,3.1400001f,48.0999985f)),
                new EnemySpawn(EnemyPrefabs[1], new Vector3(-23.7999992f,0.879999995f,-37.3499985f)),
            },
            new List<PickupSpawn>{
                new PickupSpawn(PickupPrefabs[0], new Vector3(-12.6999998f,0.5f,-2.58999991f), 20),
                new PickupSpawn(PickupPrefabs[0], new Vector3(-12.6999998f,0.5f,0), 20),
                new PickupSpawn(PickupPrefabs[0], new Vector3(-12.6999998f,0.5f,2.58999991f), 20),
                new PickupSpawn(PickupPrefabs[1], new Vector3(26.2099991f,2.5f,-4.46000004f), 20),
                new PickupSpawn(PickupPrefabs[1], new Vector3(26.2099991f,2.5f,-2.46000004f), 20),
                new PickupSpawn(PickupPrefabs[1], new Vector3(26.2099991f,2.5f,0), 20),
            }),
            // 3
            new Wave(20, new List<EnemySpawn> {
                new EnemySpawn(EnemyPrefabs[0], new Vector3(-58.1599998f,6.59000015f,-26.2600002f)),
                new EnemySpawn(EnemyPrefabs[0], new Vector3(-57.3300018f,19.9899998f,12.1999998f)),
                new EnemySpawn(EnemyPrefabs[1], new Vector3(-17.7399998f,13.6800003f,-63.5200005f)),
                new EnemySpawn(EnemyPrefabs[1], new Vector3(4.88000011f,10.8500004f,36.1199989f)),
                new EnemySpawn(EnemyPrefabs[1], new Vector3(-13.71f,2.18000007f,48.5f)),
                new EnemySpawn(EnemyPrefabs[1], new Vector3(7.0999999f,1.72000003f,-0.170000002f)),
            },
            new List<PickupSpawn>{
                new PickupSpawn(PickupPrefabs[0], new Vector3(-10.6999998f,0.5f,-2.58999991f), 30),
                new PickupSpawn(PickupPrefabs[0], new Vector3(-10.6999998f,0.5f,0), 30),
                new PickupSpawn(PickupPrefabs[0], new Vector3(-10.6999998f,0.5f,2.58999991f), 30),
                new PickupSpawn(PickupPrefabs[1], new Vector3(24.2099991f,2.5f,-4.46000004f), 30),
                new PickupSpawn(PickupPrefabs[1], new Vector3(24.2099991f,2.5f,-2.46000004f), 30),
                new PickupSpawn(PickupPrefabs[1], new Vector3(24.2099991f,2.5f,0), 30),
            }),
            // 4
            new Wave(100, new List<EnemySpawn> {
                new EnemySpawn(EnemyPrefabs[2], new Vector3(7.0999999f,1.72000003f,-0.170000002f)),
            },
            new List<PickupSpawn>{
                new PickupSpawn(PickupPrefabs[0], new Vector3(-8.6999998f,0.5f,-2.58999991f), 50),
                new PickupSpawn(PickupPrefabs[0], new Vector3(-8.6999998f,0.5f,0), 50),
                new PickupSpawn(PickupPrefabs[0], new Vector3(-8.6999998f,0.5f,2.58999991f), 50),
                new PickupSpawn(PickupPrefabs[1], new Vector3(20.2099991f,2.5f,-4.46000004f), 50),
                new PickupSpawn(PickupPrefabs[1], new Vector3(20.2099991f,2.5f,-2.46000004f), 50),
                new PickupSpawn(PickupPrefabs[1], new Vector3(20.2099991f,2.5f,0), 50),
            }),
        };
        Initiate();
    }

    public void Initiate()
    {
        CleanUp();
        CurrentWave = 0;
        IsActive = false;
        isCurrentWaveSpawned = false;
        currentlySpawnedEnemies = new List<Hitable>();
        switch (spawnTriggerType)
        {
            case SpawnTriggerType.Button:
                SpawnButton.OnPush += () => { StartCoroutine(StartSpawning(0)); };
                break;
            case SpawnTriggerType.Timer:
            default:
                StartCoroutine(StartSpawning(TimeBeforeFirstWave));
                break;
        }
    }

    IEnumerator StartSpawning(float time)
    {
        yield return new WaitForSeconds(time);
        IsActive = true;
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
