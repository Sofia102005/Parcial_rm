using UnityEngine;

/// <summary>
/// Botón de salida. Asígnalo en el OnClick del botón "SALIR" de la escena final.
/// En el editor detiene el Play; en el celular cierra la aplicación.
/// </summary>
public class QuitGame : MonoBehaviour
{
    public void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
