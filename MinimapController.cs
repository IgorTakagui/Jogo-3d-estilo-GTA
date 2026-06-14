using UnityEngine;

/// <summary>
/// MinimapController - Funcionalidade de Minimapa
/// Attach this script to a GameObject called "MinimapCamera" in your scene.
/// Requires a second Camera component configured as a minimap overlay.
/// </summary>
public class MinimapController : MonoBehaviour
{
    [Header("Minimap Settings")]
    public Transform player;           // Arraste o GameObject do jogador aqui
    public float heightOffset = 20f;   // Altura da câmera do minimapa acima do jogador
    public float zoomLevel = 30f;      // Nível de zoom (menor = mais próximo)
    public KeyCode toggleKey = KeyCode.M; // Tecla para mostrar/ocultar o minimapa

    [Header("UI Reference")]
    public GameObject minimapPanel;    // Painel da UI do minimapa (Canvas > MinimapPanel)

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

    /// <summary>
    /// Aumenta o zoom do minimapa (scroll up)
    /// </summary>
    public void ZoomIn()
    {
        zoomLevel = Mathf.Max(5f, zoomLevel - 5f);
        if (minimapCamera != null)
            minimapCamera.orthographicSize = zoomLevel;
    }

    /// <summary>
    /// Diminui o zoom do minimapa (scroll down)
    /// </summary>
    public void ZoomOut()
    {
        zoomLevel = Mathf.Min(100f, zoomLevel + 5f);
        if (minimapCamera != null)
            minimapCamera.orthographicSize = zoomLevel;
    }
}
