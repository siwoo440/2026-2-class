using UnityEngine;
using UnityEngine.SceneManagement; // 씬 관리자 관련 코드
using UnityEngine.UI; // UI 관련 코드

// 필요한 UI에 즉시 접근하고 변경할 수 있도록 허용하는 UI 매니저
public class UIManager : MonoBehaviour 
{
    // 싱글톤 접근용 프로퍼티
    public static UIManager instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = FindFirstObjectByType<UIManager>();
            }

            return m_instance;
        } // 방어 코드
    }

    private static UIManager m_instance; // 싱글톤이 할당될 변수

    public Text ammoText; // 탄약 표시용 텍스트
    public Text scoreText; // 점수 표시용 텍스트
    public Text waveText; // 적 웨이브 표시용 텍스트
    public GameObject gameoverUI; // 게임 오버시 활성화할 UI 

    private Zombie activeBoss;
    private Image bossBar;
    private RectTransform bossFill;
    private Text bossLabel;
    private float displayedBossHealth;

    public void ShowBossHealth(Zombie boss)
    {
        if (bossBar == null)
        {
            CreateBossBar();
        }

        activeBoss = boss;
        displayedBossHealth = float.NaN;
        bossBar.gameObject.SetActive(true);
        RefreshBossBar();
    }

    private void LateUpdate()
    {
        if (bossBar == null || !bossBar.gameObject.activeSelf)
        {
            return;
        }

        if (activeBoss == null || activeBoss.dead)
        {
            HideBossBar();
            return;
        }

        RefreshBossBar();
    }

    private void RefreshBossBar()
    {
        float hp = Mathf.Clamp(activeBoss.health, 0f, activeBoss.startingHealth);
        if (hp == displayedBossHealth)
        {
            return;
        }

        displayedBossHealth = hp;
        float ratio = activeBoss.startingHealth > 0f ? hp / activeBoss.startingHealth : 0f;
        bossFill.anchorMax = new Vector2(ratio, 1f);
        bossLabel.text = $"BOSS   {Mathf.CeilToInt(hp)} / {Mathf.CeilToInt(activeBoss.startingHealth)}";
    }

    private void HideBossBar()
    {
        activeBoss = null;
        if (bossBar != null)
        {
            bossBar.gameObject.SetActive(false);
        }
    }

    private void CreateBossBar()
    {
        // 기존 HUD의 Score Text 아래에 화면 너비에 비례하는 체력바를 배치합니다.
        bossBar = new GameObject("Boss Health", typeof(RectTransform), typeof(Image)).GetComponent<Image>();
        RectTransform track = bossBar.rectTransform;
        track.SetParent(scoreText.canvas.transform, false);
        track.anchorMin = new Vector2(0.25f, 1f);
        track.anchorMax = new Vector2(0.75f, 1f);
        track.pivot = new Vector2(0.5f, 1f);
        track.anchoredPosition = new Vector2(0f, -84f);
        track.sizeDelta = new Vector2(0f, 28f);
        bossBar.color = new Color(0.12f, 0.06f, 0.07f, 0.96f);
        bossBar.raycastTarget = false;

        Image fill = new GameObject("Health Fill", typeof(RectTransform), typeof(Image)).GetComponent<Image>();
        fill.color = new Color(0.72f, 0.12f, 0.16f);
        fill.raycastTarget = false;
        bossFill = fill.rectTransform;
        StretchToBar(bossFill, track);

        bossLabel = new GameObject("Boss HP", typeof(RectTransform), typeof(Text)).GetComponent<Text>();
        StretchToBar(bossLabel.rectTransform, track);
        bossLabel.font = scoreText.font;
        bossLabel.fontSize = 18;
        bossLabel.fontStyle = FontStyle.Bold;
        bossLabel.alignment = TextAnchor.MiddleCenter;
        bossLabel.color = Color.white;
        bossLabel.raycastTarget = false;
    }

    private static void StretchToBar(RectTransform child, RectTransform parent)
    {
        child.SetParent(parent, false);
        child.anchorMin = Vector2.zero;
        child.anchorMax = Vector2.one;
        child.offsetMin = Vector2.zero;
        child.offsetMax = Vector2.zero;
    }

    // 탄약 텍스트 갱신
    public void UpdateAmmoText(int magAmmo, int remainAmmo) 
    {
        ammoText.text = magAmmo + "/" + remainAmmo;
    }

    // 점수 텍스트 갱신
    public void UpdateScoreText(int newScore) 
    {
        scoreText.text = "Score : " + newScore;
    }

    // 적 웨이브 텍스트 갱신
    public void UpdateWaveText(int waves, int count) 
    {
        waveText.text = "Wave : " + waves + "\nEnemy Left : " + count;
    }

    // 게임 오버 UI 활성화
    public void SetActiveGameoverUI(bool active) 
    {
        if (active)
        {
            HideBossBar();
        }
        gameoverUI.SetActive(active);
    }

    // 게임 재시작
    public void GameRestart() 
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}