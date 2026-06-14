# 🎮 GTA Unity 3D

**Desenvolvido por:** Igor Takagui Reis  
**Engine:** Unity 3D  
**Disciplina:** Trabalho Individual — 2º Bimestre

---

## 📖 Descrição do Jogo

GTA Unity 3D é um jogo de mundo aberto em terceira pessoa desenvolvido na Unity, inspirado na clássica franquia Grand Theft Auto. O jogador controla um personagem que pode explorar livremente o ambiente urbano, interagir com veículos e enfrentar as consequências de suas ações por meio de um sistema de nível de procurado. O objetivo é explorar o mapa, acumular pontos e sobreviver à perseguição policial.

---

## 🕹️ Instruções de Jogabilidade

| Ação | Controle |
|---|---|
| Mover personagem | `W` `A` `S` `D` |
| Câmera | Mouse |
| Correr | `Shift` |
| Entrar/Sair de veículo | `F` |
| Mostrar/Ocultar minimapa | `M` |
| Pular | `Espaço` |
| Atirar / Atacar | `Botão Esquerdo do Mouse` |
| Pausar | `Esc` |

---

## 🎬 Gameplay — Vídeo

> ⚠️ *Adicione aqui o link do vídeo no YouTube ou embed do vídeo (máx. 5 minutos)*

<!-- Exemplo embed YouTube:
[![Gameplay GTA Unity](https://img.youtube.com/vi/SEU_VIDEO_ID/0.jpg)](https://www.youtube.com/watch?v=SEU_VIDEO_ID)
-->

---

## 📸 Prints do Jogo

### Menu Principal
> ⚠️ *Adicione aqui o print do menu (arraste a imagem diretamente no GitHub ao editar o README)*

<!-- ![Menu Principal](prints/menu.png) -->

### Gameplay — Print 1
> ⚠️ *Adicione aqui um print do jogo em funcionamento*

<!-- ![Gameplay 1](prints/gameplay1.png) -->

### Gameplay — Print 2
> ⚠️ *Adicione aqui um segundo print do jogo em funcionamento*

<!-- ![Gameplay 2](prints/gameplay2.png) -->

---

## ⚙️ Funcionalidades Desenvolvidas

### 1. 🗺️ Sistema de Minimapa

Foi desenvolvido um minimapa funcional que acompanha o jogador em tempo real, exibido no canto superior direito da tela. O minimapa utiliza uma segunda câmera ortográfica posicionada acima do personagem que segue seus movimentos. O jogador pode ativar ou desativar o minimapa pressionando a tecla `M`, e o sistema suporta zoom dinâmico. Essa funcionalidade aumenta significativamente a imersão e a orientação espacial no mundo aberto.

**Como funciona:**
- Uma `Camera` secundária é posicionada acima do jogador com projeção ortográfica
- A câmera segue o jogador via `LateUpdate()` para evitar tremidos
- Um `RenderTexture` exibe a visão da câmera em um painel da UI (Canvas)
- A tecla `M` alterna a visibilidade do painel e habilita/desabilita a câmera

```csharp
using UnityEngine;

public class MinimapController : MonoBehaviour
{
    [Header("Minimap Settings")]
    public Transform player;
    public float heightOffset = 20f;
    public float zoomLevel = 30f;
    public KeyCode toggleKey = KeyCode.M;

    [Header("UI Reference")]
    public GameObject minimapPanel;

    private Camera minimapCamera;
    private bool isVisible = true;

    void Start()
    {
        minimapCamera = GetComponent<Camera>();

        if (minimapCamera != null)
        {
            minimapCamera.orthographic = true;
            minimapCamera.orthographicSize = zoomLevel;
        }

        if (minimapPanel != null)
            minimapPanel.SetActive(true);
    }

    void LateUpdate()
    {
        // Segue o jogador mantendo a altura fixa
        if (player != null)
        {
            Vector3 newPos = player.position;
            newPos.y = player.position.y + heightOffset;
            transform.position = newPos;
        }

        // Toggle visibilidade com tecla M
        if (Input.GetKeyDown(toggleKey))
        {
            isVisible = !isVisible;

            if (minimapPanel != null)
                minimapPanel.SetActive(isVisible);

            if (minimapCamera != null)
                minimapCamera.enabled = isVisible;
        }
    }

    public void ZoomIn()
    {
        zoomLevel = Mathf.Max(5f, zoomLevel - 5f);
        if (minimapCamera != null)
            minimapCamera.orthographicSize = zoomLevel;
    }

    public void ZoomOut()
    {
        zoomLevel = Mathf.Min(100f, zoomLevel + 5f);
        if (minimapCamera != null)
            minimapCamera.orthographicSize = zoomLevel;
    }
}
```

> ⚠️ *Adicione aqui um print mostrando o minimapa no canto da tela durante o jogo*

<!-- ![Minimapa](prints/minimap.png) -->

---

### 2. ⭐ Sistema de Nível de Procurado (Wanted System)

Foi desenvolvido um sistema de estrelas de procurado inspirado no GTA original. Ao cometer infrações no jogo (atropelar NPCs, bater em objetos, atirar), o nível de procurado do jogador aumenta de 1 a 5 estrelas. Cada estrela adicional aumenta a frequência de spawn de policiais que perseguem o jogador. Se o jogador ficar um tempo sem cometer infrações, as estrelas reduzem gradualmente até zerar. O sistema utiliza o padrão **Singleton** para ser acessado de qualquer script do projeto com `WantedSystem.Instance.IncreaseWanted()`.

**Como funciona:**
- Um `int currentWantedLevel` controla o nível atual (0 a 5)
- Um `timer` de decay conta o tempo sem infração — ao atingir o limite, reduz 1 estrela
- Policiais são instanciados via `Instantiate()` em pontos de spawn com frequência crescente conforme o nível
- A UI exibe ícones de estrela (`Image[]`) que mudam de cor conforme o nível atual

```csharp
using UnityEngine;
using UnityEngine.UI;

public class WantedSystem : MonoBehaviour
{
    [Header("Wanted Settings")]
    public int maxWantedLevel = 5;
    public float wantedDecayTime = 10f;

    [Header("UI Elements")]
    public Image[] starIcons;
    public Color activeStarColor = Color.yellow;
    public Color inactiveStarColor = Color.gray;

    [Header("Police Settings")]
    public GameObject policePrefab;
    public Transform[] spawnPoints;
    public float spawnInterval = 8f;

    private int currentWantedLevel = 0;
    private float decayTimer = 0f;
    private float spawnTimer = 0f;

    public static WantedSystem Instance;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Update()
    {
        if (currentWantedLevel > 0)
        {
            decayTimer += Time.deltaTime;

            if (decayTimer >= wantedDecayTime)
            {
                decayTimer = 0f;
                ReduceWantedLevel();
            }

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
    }

    public void IncreaseWanted()
    {
        currentWantedLevel = Mathf.Min(currentWantedLevel + 1, maxWantedLevel);
        decayTimer = 0f;
        UpdateStarUI();
    }

    public void ReduceWantedLevel()
    {
        currentWantedLevel = Mathf.Max(currentWantedLevel - 1, 0);
        UpdateStarUI();
    }

    public void ClearWanted()
    {
        currentWantedLevel = 0;
        decayTimer = 0f;
        UpdateStarUI();
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

        int randomIndex = Random.Range(0, spawnPoints.Length);
        Transform spawnPoint = spawnPoints[randomIndex];
        Instantiate(policePrefab, spawnPoint.position, spawnPoint.rotation);
    }
}
```

> ⚠️ *Adicione aqui um print mostrando as estrelas de procurado ativas na HUD do jogo*

<!-- ![Wanted System](prints/wanted.png) -->

---

## 🛠️ Tecnologias Utilizadas

- **Unity 3D** — Engine principal
- **C#** — Linguagem de programação dos scripts
- **Git + GitHub** — Versionamento do projeto

---

## 📁 Estrutura do Projeto

```
Assets/
├── Scripts/
│   ├── MinimapController.cs
│   ├── WantedSystem.cs
│   └── ...
├── Scenes/
│   ├── MainMenu.unity
│   └── GameScene.unity
├── Prefabs/
├── Audio/
└── ...
```

---

*Trabalho Individual — 2º Bimestre | Igor Takagui Reis*
