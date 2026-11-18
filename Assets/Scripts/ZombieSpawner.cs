using System.Collections.Generic;
using UnityEngine;

// 좀비 게임 오브젝트를 주기적으로 생성
public class ZombieSpawner : MonoBehaviour {
    public Zombie zombiePrefab; // 생성할 좀비 원본 프리팹

    public ZombieData[] zombieDatas; // 사용할 좀비 셋업 데이터들
    public Transform[] spawnPoints; // 좀비 AI를 소환할 위치들

    [Header("Boss Settings")]
    [SerializeField] private ZombieData bossZombieData;

    private const int BossWaveInterval = 5;
    private readonly List<Zombie> zombies = new List<Zombie>(); // 생성된 좀비들을 담는 리스트
    private readonly List<ZombieData> normalZombieDatas = new List<ZombieData>();
    private int wave; // 현재 웨이브

    private void Awake()
    {
        // 보스가 Inspector의 일반 목록에도 들어간 경우 추첨 대상에서 제외합니다.
        if (zombieDatas != null)
        {
            foreach (ZombieData data in zombieDatas)
            {
                if (data != null && data != bossZombieData)
                {
                    normalZombieDatas.Add(data);
                }
            }
        }

        if (bossZombieData == null || normalZombieDatas.Count == 0)
        {
            Debug.LogError("ZombieSpawner에 Boss Zombie Data와 일반 Zombie Data를 각각 연결하세요.", this);
            enabled = false;
        }
    }

    private void Update() {
        // 게임 오버 상태일때는 생성하지 않음
        if (GameManager.instance != null && GameManager.instance.isGameover)
        {
            return;
        }

        // 좀비를 모두 물리친 경우 다음 스폰 실행
        if (zombies.Count <= 0)
        {
            SpawnWave();
        }

        // UI 갱신
        UpdateUI();
    }

    // 웨이브 정보를 UI로 표시
    private void UpdateUI() {
        // 현재 웨이브와 남은 적 수 표시
        UIManager.instance.UpdateWaveText(wave, zombies.Count);
    }

    // 현재 웨이브에 맞춰 좀비들을 생성
    private void SpawnWave() {
        wave++;
        int spawnCount = Mathf.RoundToInt(wave * 1.5f);
        bool isBossWave = wave % BossWaveInterval == 0;

        for (int i = 0; i < spawnCount; i++)
        {
            CreateZombie(isBossWave && i == 0);
        }
    }

    // 좀비를 생성하고 생성한 좀비에게 추적할 대상을 할당
    private void CreateZombie(bool isBoss) {
        ZombieData zombieData = isBoss
            ? bossZombieData
            : normalZombieDatas[Random.Range(0, normalZombieDatas.Count)];
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        Zombie zombie = Instantiate(zombiePrefab, spawnPoint.position, spawnPoint.rotation);
        zombie.transform.localScale *= Mathf.Max(0.01f, zombieData.scaleMultiplier);
        zombie.Setup(zombieData);
        if (isBoss)
        {
            zombie.gameObject.name = "Boss Zombie";
            UIManager.instance.ShowBossHealth(zombie);
        }

        zombies.Add(zombie);
        zombie.onDeath += () => zombies.Remove(zombie);
        zombie.onDeath += () => Destroy(zombie.gameObject, 10f);
        zombie.onDeath += () => GameManager.instance.AddScore(100);
    }
}
