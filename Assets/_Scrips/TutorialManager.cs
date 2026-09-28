using TMPro;
using UnityEngine;

/// <summary>
/// Muestra las instrucciones del tutorial paso a paso y detecta cuándo el jugador
/// completa cada uno:
///   0 = bienvenida                          (avanza sola por tiempo)
///   1 = primer teletransporte (al estante)  (detecta que el Player se movió)
///   2 = tomar la gaseosa                    (detecta GrabManager.heldItem != null)
///   3 = segundo teletransporte (a la mesa)  (detecta que el Player se movió otra vez)
///   4 = servir la gaseosa en el vaso        (lo avisa RecipeStation con OnRecipeComplete -> ShowStep(5))
///   5 = mensaje final, mientras RecipeStation ya cuenta el tiempo para cargar el juego real
///
/// Requiere en la escena: GrabManager y TeleportManager (con su Player asignado).
/// El paso 4 -> 5 NO lo dispara este script: se conecta el evento
/// RecipeStation.OnRecipeComplete al método ShowStep(5) desde el Inspector.
/// </summary>
public class TutorialManager : MonoBehaviour
{
    [SerializeField] private TMP_Text instructionText;

    [TextArea(2, 5)]
    [SerializeField]
    private string[] steps =
    {
        "¡Bienvenido a la cocina!\nEl tu puntero: se mueve hacia donde mires y se activa cuando detecta cosas.",
        "Mira un círculo en el suelo y mantén la mirada hasta llenar la barra:\nasí te teletransportas hacia el estante.",
        "Ahora mira la gaseosa en la mesa y mantén la mirada para tomarla.",
        "Llevas la gaseosa en la mano.\nllevalo al meson.",
        "Mira el vaso y mantén la mirada para servir la gaseosa.",
        "¡Perfecto! Preparaste tu primera bebida.\nAhora comienza el juego de verdad..."
    };

    [SerializeField] private float introTime = 6f;
    [SerializeField] private float moveThreshold = 0.5f;

    private int current = -1;
    private float timer;
    private Vector3 lastPosition;
    private GrabManager grabManager;

    private void Start()
    {
        grabManager = FindFirstObjectByType<GrabManager>();
        if (TeleportManager.Instance != null && TeleportManager.Instance.Player != null)
            lastPosition = TeleportManager.Instance.Player.transform.position;

        ShowStep(0);
    }

    private void Update()
    {
        switch (current)
        {
            case 0:
                timer += Time.deltaTime;
                if (timer >= introTime) ShowStep(1);
                break;

            case 1: // esperando el PRIMER teletransporte
                if (PlayerMoved()) ShowStep(2);
                break;

            case 2: // esperando que tome la gaseosa
                if (grabManager != null && grabManager.heldItem != null) ShowStep(3);
                break;

            case 3: // esperando el SEGUNDO teletransporte
                if (PlayerMoved()) ShowStep(4);
                break;

            // El paso 4 (servir la gaseosa) lo cierra RecipeStation llamando a ShowStep(5)
            // desde su evento "On Recipe Complete ()" en el Inspector.
        }
    }

    /// <summary>True la primera vez que el Player se movió más de moveThreshold desde la última vez que se llamó.</summary>
    private bool PlayerMoved()
    {
        if (TeleportManager.Instance == null || TeleportManager.Instance.Player == null) return false;

        Vector3 currentPos = TeleportManager.Instance.Player.transform.position;
        if (Vector3.Distance(currentPos, lastPosition) > moveThreshold)
        {
            lastPosition = currentPos;
            return true;
        }
        return false;
    }

    /// <summary>Público para poder llamarlo desde un UnityEvent (ej. RecipeStation.OnRecipeComplete).</summary>
    public void ShowStep(int index)
    {
        if (index < 0 || index >= steps.Length) return;
        current = index;
        if (instructionText != null)
            instructionText.text = steps[index];
    }
}