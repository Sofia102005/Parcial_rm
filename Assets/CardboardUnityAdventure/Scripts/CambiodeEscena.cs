using UnityEngine;
using UnityEngine.SceneManagement;

public class CambiodeEscena : MonoBehaviour
{
    public void CambiarEscena(int numeroEscena)
    {
        Debug.Log("¡El botón funciona! Intentando cargar escena número: " + numeroEscena);
        SceneManager.LoadScene(numeroEscena);
    }
}