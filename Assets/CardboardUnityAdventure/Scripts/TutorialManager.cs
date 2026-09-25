using TMPro;
using UnityEngine;

/// <summary>
/// Muestra las instrucciones del tutorial paso a paso y detecta cuándo el jugador
/// completa cada paso:
///   0 = bienvenida (avanza sola por tiempo)
///   1 = teletransportarse (detecta que el Player se movió)
///   2 = tomar el vaso (detecta que GrabManager.heldItem no es null)
///   3 = dejar el vaso en la mesa (lo avisa RecipeStation con su evento OnRecipeComplete -> ShowStep(4))
///   4 = mensaje final
/// Requiere en la escena: GrabManager y TeleportManager.
/// </summary>
public class TutorialManager : MonoBehaviour
{
    [SerializeField] private TMP_Text instructionText;

   [TextArea(2, 5)]
[SerializeField]
private string[] steps =
{
    "¡Bienvenido a la cocina!\nEl punto que ves es tu puntero: se mueve hacia donde mires.",
    "Para moverte, mira un círculo del suelo y mantén la mirada hasta que se llene la barra.",
    "Ahora mira el vaso y mantén la mirada para tomarlo.",
    "Lleva el vaso a la mesa y déjalo allí.",
    "¡Bien! Ahora toma la soda y llévala también a la mesa.",
    "¡Excelente! Combinaste los ingredientes y preparaste tu primera bebida.\nAhora comienza el juego de verdad..."
};

    [SerializeField] private float introTime = 6f;
    [SerializeField] private float moveThreshold = 0.5f;

    private int current = -1;
    private float timer;
    private Vector3 startPosition;
    private GrabManager grabManager;

    private void Start()
    {
        grabManager = FindFirstObjectByType<GrabManager>();
        if (TeleportManager.Instance != null && TeleportManager.Instance.Player != null)
            startPosition = TeleportManager.Instance.Player.transform.position;

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

            case 1:
                if (TeleportManager.Instance != null && TeleportManager.Instance.Player != null)
                {
                    float d = Vector3.Distance(TeleportManager.Instance.Player.transform.position, startPosition);
                    if (d > moveThreshold) ShowStep(2);
                }
                break;

            case 2:
                if (grabManager != null && grabManager.heldItem != null) ShowStep(3);
                break;
        }
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
