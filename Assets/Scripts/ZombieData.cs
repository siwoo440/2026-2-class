using UnityEngine;

// 좀비 생성시 사용할 셋업 데이터
[CreateAssetMenu(menuName = "Scriptable/ZombieData", fileName = "Zombie Data")]
public class ZombieData : ScriptableObject 
{
    public float health = 100f; // 체력
    public float damage = 20f; // 공격력
    public float speed = 2f; // 이동 속도
    [Min(0.01f)] public float scaleMultiplier = 1f; // 프리팹 크기 배율
    public Color skinColor = Color.white; // 피부색
}
