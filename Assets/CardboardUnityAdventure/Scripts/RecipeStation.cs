using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

/// <summary>
/// Estación de receta. Recibe los objetos que el jugador lleva en la mano
/// (usa GrabManager y GrabObject) y, cuando llegan a "requiredCount",
/// los reemplaza por un producto final (la bebida del tutorial o el pie).
///
/// Va en el MISMO GameObject que el Collider (mesa o bowl) y con Tag "Interactable".
/// </summary>
public class RecipeStation : MonoBehaviour
{
    [Header("Referencias")]
    [Tooltip("Punto (objeto vacío) donde se sueltan los ingredientes")]
    [SerializeField] private Transform holder;
    [Tooltip("Resultado final. Colócalo donde quieres verlo y déjalo DESACTIVADO")]
    [SerializeField] private GameObject product;
    [Tooltip("Paso intermedio opcional (ej. huevo roto). Déjalo DESACTIVADO")]
    [SerializeField] private GameObject intermediate;
    [Tooltip("Opcional: texto que muestra 'Ingredientes: 3/7'")]
    [SerializeField] private TMP_Text counterText;

    [Header("Receta")]
    [SerializeField] private int requiredCount = 7;
    [Tooltip("Qué tan dispersos caen los ingredientes dentro del bowl (metros)")]
    [SerializeField] private float scatterRadius = 0.06f;
    [Tooltip("Cuánto sube cada ingrediente respecto al anterior (metros)")]
    [SerializeField] private float heightStep = 0.03f;

    [Header("Al completar")]
    [SerializeField] private float intermediateTime = 2f;
    [SerializeField] private float productRotateSpeed = 30f;
    [SerializeField] private AudioClip completeSound;
    [Tooltip("Índice de la escena a cargar al terminar. -1 = no cambiar de escena")]
    [SerializeField] private int nextSceneIndex = -1;
    [SerializeField] private float delayBeforeNextScene = 8f;
    public UnityEvent OnRecipeComplete;

    private GrabManager grabManager;
    private readonly List<GameObject> placed = new List<GameObject>();
    private bool completed;
    private bool rotateProduct;

    private void Start()
    {
        GameObject gm = GameObject.Find("GrabManager");
        if (gm == null)
        {
            Debug.LogError("RecipeStation: falta un objeto llamado 'GrabManager' en la escena.");
            return;
        }
        grabManager = gm.GetComponent<GrabManager>();

        if (holder == null) holder = transform;
        if (product != null) product.SetActive(false);
        if (intermediate != null) intermediate.SetActive(false);
        UpdateCounter();
    }

    // Lo llama CameraPointerManager (mirada sostenida o botón del visor)
    public void OnPointerClickXR()
    {
        if (completed || grabManager == null) return;

        GameObject held = grabManager.heldItem;
        if (held == null) return; // no lleva nada en la mano

        GrabObject grab = held.GetComponent<GrabObject>();
        if (grab == null) return;

        Vector2 r = Random.insideUnitCircle * scatterRadius;
        Vector3 pos = holder.position + new Vector3(r.x, placed.Count * heightStep, r.y);

        grab.Place(pos);                                // suelta + sonido + heldItem = null
        Collider col = held.GetComponent<Collider>();   // que no se pueda volver a agarrar
        if (col != null) col.enabled = false;

        placed.Add(held);
        UpdateCounter();

        if (placed.Count >= requiredCount)
            StartCoroutine(CompleteRoutine());
    }

    private IEnumerator CompleteRoutine()
    {
        completed = true;

        foreach (GameObject g in placed)
            g.SetActive(false);

        if (intermediate != null)
        {
            intermediate.SetActive(true);
            yield return new WaitForSeconds(intermediateTime);
            intermediate.SetActive(false);
        }

        if (product != null)
        {
            product.SetActive(true);
            rotateProduct = true;
        }

        if (completeSound != null)
            AudioSource.PlayClipAtPoint(completeSound, transform.position);

        OnRecipeComplete?.Invoke();

        if (nextSceneIndex >= 0)
        {
            yield return new WaitForSeconds(delayBeforeNextScene);
            SceneManager.LoadScene(nextSceneIndex);
        }
    }

    private void Update()
    {
        if (rotateProduct && product != null)
            product.transform.Rotate(Vector3.up, productRotateSpeed * Time.deltaTime, Space.World);
    }

    private void UpdateCounter()
    {
        if (counterText != null)
            counterText.text = $"Ingredientes: {placed.Count}/{requiredCount}";
    }
}
