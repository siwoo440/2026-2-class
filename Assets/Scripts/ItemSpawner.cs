using UnityEngine;
using UnityEngine.AI; // 내비메쉬 관련 코드

// 주기적으로 아이템을 플레이어 근처에 생성하는 스크립트
public class ItemSpawner : MonoBehaviour {
    public GameObject[] items; // 생성할 아이템들
    public Transform playerTransform; // 플레이어의 트랜스폼

    public float maxDistance = 5f; // 플레이어 위치로부터 아이템이 배치될 최대 반경

    public float timeBetSpawnMax = 7f; // 최대 시간 간격
    public float timeBetSpawnMin = 2f; // 최소 시간 간격
    private float timeBetSpawn; // 생성 간격

    private float lastSpawnTime; // 마지막 생성 시점

    private void Start() {
        // 다른 씬에 프리팹을 직접 배치한 경우에도 플레이어를 연결합니다.
        if (playerTransform == null)
        {
            PlayerHealth player = FindFirstObjectByType<PlayerHealth>();
            if (player != null)
            {
                playerTransform = player.transform;
            }
        }

        if (playerTransform == null || items == null || items.Length == 0)
        {
            Debug.LogError("Item Spawner에 플레이어와 아이템 프리팹을 연결하세요.", this);
            enabled = false;
            return;
        }

        // 생성 간격과 마지막 생성 시점 초기화
        timeBetSpawn = Random.Range(timeBetSpawnMin, timeBetSpawnMax);
        lastSpawnTime = Time.time;
    }

    // 주기적으로 아이템 생성 처리 실행
    private void Update() {
        // 현재 시점이 마지막 생성 시점에서 생성 주기 이상 지남
        // && 플레이어 캐릭터가 존재함
        if (Time.time >= lastSpawnTime + timeBetSpawn && playerTransform != null)
        {
            // 마지막 생성 시간 갱신
            lastSpawnTime = Time.time;
            // 생성 주기를 랜덤으로 변경
            timeBetSpawn = Random.Range(timeBetSpawnMin, timeBetSpawnMax);
            // 아이템 생성 실행
            Spawn();
        }
    }

    // 실제 아이템 생성 처리
    private void Spawn() 
    {
        // 플레이어 근처에서 내비메시 위의 랜덤 위치 가져오기
        if (!TryGetSpawnPosition(playerTransform.position, out Vector3 spawnPosition))
        {
            return;
        }
        // 바닥에서 0.5만큼 위로 올리기
        spawnPosition += Vector3.up * 0.5f;

        // 아이템 중 하나를 무작위로 골라 랜덤 위치에 생성
        GameObject selectedItem = items[Random.Range(0, items.Length)];
        GameObject item = Instantiate(selectedItem, spawnPosition, Quaternion.identity);

        // 생성된 아이템을 5초 뒤에 파괴
        Destroy(item, 5f);
    }

    // 바닥의 유효한 지점만 사용하고, 통로 가장자리에서는 다른 위치를 시도합니다.
    private bool TryGetSpawnPosition(Vector3 center, out Vector3 position)
    {
        for (int attempt = 0; attempt < 8; attempt++)
        {
            Vector2 offset = Random.insideUnitCircle * maxDistance;
            Vector3 candidate = center + new Vector3(offset.x, 0f, offset.y);
            if (NavMesh.SamplePosition(candidate, out NavMeshHit hit, 1f, NavMesh.AllAreas) &&
                (hit.position - center).sqrMagnitude <= maxDistance * maxDistance)
            {
                position = hit.position;
                return true;
            }
        }

        position = default;
        return false;
    }
}
