using System.Collections;
using UnityEngine;

public class CortinaEscena : MonoBehaviour
{
    [Header("Configuración de la Cortina")]
    [SerializeField] private GameObject canvasCortina;
    [SerializeField] private float tiempoEspera = 3.0f;

    [Header("Referencia al Sistema de Diálogos")]
    // Aquí arrastraremos el objeto que tiene tu script TextController
    [SerializeField] private TextController textController;

    private void Start()
    {
        // Al iniciar la escena, nos aseguramos de que la cortina esté visible
        if (canvasCortina != null)
        {
            canvasCortina.SetActive(true);

            // Iniciamos la cuenta regresiva en segundo plano
            StartCoroutine(ControlarCortina());
        }
        else
        {
            Debug.LogWarning("¡No has asignado el GameObject del Canvas de la cortina!");
            // Si no hay cortina, que los diálogos empiecen directo por si acaso
            IniciarDialogos();
        }
    }

    private IEnumerator ControlarCortina()
    {
        // El código se detiene aquí durante los segundos que le digas (3 segundos)
        yield return new WaitForSeconds(tiempoEspera);

        // Pasado el tiempo, ocultamos por completo el Canvas de la cortina
        canvasCortina.SetActive(false);

        // Llamamos a la función que arranca tu juego de diálogos
        IniciarDialogos();
    }

    private void IniciarDialogos()
    {
        if (textController != null)
        {
            // NOTA: Como en tu Start() de TextController ya llamas a MostrarIntroduccionCapitulo() 
            // o a ShowText(), aquí podemos encender el componente o invocar su lógica.
            textController.enabled = true;

            // Si prefieres manejarlo por métodos, podrías quitar el ShowText() del Start de TextController
            // y llamarlo directamente aquí usando una función pública.
        }
        else
        {
            Debug.LogError("No se encontró la referencia a TextController en el script de la cortina.");
        }
    }
}