using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// WantedSystem - Sistema de Nível de Procurado (Estrelas)
/// Attach this script to any persistent GameObject (ex: GameManager).
/// Inspirado no sistema de estrelas do GTA.
/// </summary>
public class WantedSystem : MonoBehaviour
{
    [Header("Wanted Settings")]
    public int maxWantedLevel = 5;          // Nível máximo de procurado
    public float wantedDecayTime = 10f;     // Segundos sem infração para reduzir 1 estrela
    public float wantedIncreaseAmount = 1f; // Quanto aumenta por infração

    [Header("UI Elements")]
    public Image[] starIcons;               // Array de ícones de estrela na UI (5 elementos)
    public Color activeStarColor = Color.yellow;
    public Color inactiveStarColor = Color.gray;

    [Header("Police Settings")]
    public GameObject policePrefab;         // Prefab do inimigo/policial
    public Transform[] spawnPoints;         // Pontos de spawn dos policiais
    public float spawnInterval = 8f;        // Intervalo de spawn por estrela

    private int currentWantedLevel = 0;
    private float decayTimer = 0f;
    private float spawnTimer = 0f;
    private bool isBeingChased = false;

    public static WantedSystem Instance;    // Singleton para acesso global

    void Awake()
    {
        // Padrão Singleton
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        UpdateStarUI();
    }

    void Update()
    {
        if (currentWantedLevel > 0)
        {
            // Conta o tempo para reduzir estrelas
            decayTimer += Time.deltaTime;

            if (decayTimer >= wantedDecayTime)
            {
                decayTimer = 0f;
                ReduceWantedLevel();
            }

            // Spawna policiais baseado no nível
            if (policePrefab != null && spawnPoints.Length > 0)
            {
                spawnTimer += Time.deltaTime;
                float currentSpawnInterval = spawnInterval / currentWantedLevel;

                if (spawnTimer >= currentSpawnInterval)
                {
                    spawnTimer = 0f;
                    SpawnPolice();
                }
            }
        }
        else
        {
            decayTimer = 0f;
            spawnTimer = 0f;
        }
    }

    /// <summary>
    /// Aumenta o nível de procurado. Chame este método ao cometer uma infração.
    /// Exemplo: WantedSystem.Instance.IncreaseWanted();
    /// </summary>
    public void IncreaseWanted()
    {
        currentWantedLevel = Mathf.Min(currentWantedLevel + 1, maxWantedLevel);
        decayTimer = 0f; // Reseta o timer de decay ao cometer infração
        UpdateStarUI();

        Debug.Log($"[WantedSystem] Nível de procurado: {currentWantedLevel} estrela(s)");
    }

    /// <summary>
    /// Reduz o nível de procurado em 1.
    /// </summary>
    public void ReduceWantedLevel()
    {
        currentWantedLevel = Mathf.Max(currentWantedLevel - 1, 0);
        UpdateStarUI();
    }

    /// <summary>
    /// Zera completamente o nível de procurado (ex: ao entrar em safe zone).
    /// </summary>
    public void ClearWanted()
    {
        currentWantedLevel = 0;
        decayTimer = 0f;
        UpdateStarUI();
    }

    /// <summary>
    /// Retorna o nível atual de procurado.
    /// </summary>
    public int GetWantedLevel()
    {
        return currentWantedLevel;
    }

    private void UpdateStarUI()
    {
        if (starIcons == null) return;

        for (int i = 0; i < starIcons.Length; i++)
        {
            if (starIcons[i] != null)
            {
                starIcons[i].color = (i < currentWantedLevel)
                    ? activeStarColor
                    : inactiveStarColor;
            }
        }
    }

    private void SpawnPolice()
    {
        if (spawnPoints.Length == 0 || policePrefab == null) return;

        // Escolhe ponto de spawn aleatório
        int randomIndex = Random.Range(0, spawnPoints.Length);
        Transform spawnPoint = spawnPoints[randomIndex];

        Instantiate(policePrefab, spawnPoint.position, spawnPoint.rotation);
    }
}
