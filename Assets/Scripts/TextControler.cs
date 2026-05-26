using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TextController : MonoBehaviour
{
    private List<GameObject> buttonsPool = new List<GameObject>();
    private List<TextMeshProUGUI> responsesTexts = new List<TextMeshProUGUI>();

    private TextAnimation textAnimation;

    public List<Button> currentOptions = new List<Button>();

    [SerializeField] private Button backgroundClickButton;

    [SerializeField] private TextsTemplate template;
    [SerializeField] private TextsTemplate[] arrayTemplates;

    [SerializeField] private int poolAmount = 6;

    [SerializeField] private TextMeshProUGUI mainText;

    [SerializeField] private GameObject responseButtonPrefab;
    [SerializeField] private Transform responseContent;

    [Header("Configuración Cortina Inicial")]
    [SerializeField] private GameObject cortinaCapitulo;
    [SerializeField] private TextMeshProUGUI textoTitulo;

    [Header("Configuración Transición Capítulo 2")]
    [SerializeField] private CanvasGroup faderNegro;
    [SerializeField] private AudioClip sonidoTransicion;
    [SerializeField] private float duracionFade = 1.5f;
    [SerializeField] private string nombreEscenaCapitulo2;

    private bool esperandoClickOpcionUnica = false;
    private bool enTransicionFinal = false;
    private bool esUltimoTextoDelCapitulo = false; // Nueva bandera de control estricto

    private void Awake()
    {
        textAnimation = GetComponent<TextAnimation>();
        ObjectPooling();
    }

    private void Start()
    {
        template = arrayTemplates[0];

        if (faderNegro != null)
        {
            faderNegro.alpha = 0f;
            faderNegro.blocksRaycasts = false;
        }

        if (cortinaCapitulo != null)
        {
            StartCoroutine(SecuenciaCortinaPorTecla("Capítulo 1: Los orígenes"));
        }
        else
        {
            ShowText();
        }
    }

    private void Update()
    {
        if (enTransicionFinal) return;

        if (esperandoClickOpcionUnica && Input.GetMouseButtonDown(0))
        {
            AlDarClickEnFondo();
        }
    }

    private IEnumerator SecuenciaCortinaPorTecla(string nombreCapitulo)
    {
        if (textoTitulo != null)
        {
            textoTitulo.text = nombreCapitulo;
        }

        cortinaCapitulo.SetActive(true);
        yield return null;
        yield return new WaitUntil(() => Input.anyKeyDown);

        cortinaCapitulo.SetActive(false);
        ShowText();
    }

    private void ObjectPooling()
    {
        for (int i = 0; i < poolAmount; i++)
        {
            GameObject buttonChoicePrefab = Instantiate(responseButtonPrefab, responseContent);
            buttonChoicePrefab.SetActive(false);
            buttonsPool.Add(buttonChoicePrefab);
            responsesTexts.Add(buttonChoicePrefab.GetComponentInChildren<TextMeshProUGUI>());
        }
    }

    public GameObject GetObjectInPool()
    {
        foreach (var item in buttonsPool)
        {
            if (!item.activeInHierarchy)
                return item;
        }
        return null;
    }

    private void ShowText()
    {
        mainText.text = template.mainText;

        // Comprobamos si el template que se va a mostrar es el 4 o el 5
        if (template == arrayTemplates[4] || template == arrayTemplates[5])
        {
            esUltimoTextoDelCapitulo = true;
        }
        else
        {
            esUltimoTextoDelCapitulo = false;
        }

        // Si es el último texto (4 o 5) o si es un nodo de opción única estándar
        if (esUltimoTextoDelCapitulo || template.optionAmount == 1)
        {
            DisableButton();

            backgroundClickButton.gameObject.SetActive(true);
            backgroundClickButton.onClick.RemoveAllListeners();
            backgroundClickButton.onClick.AddListener(AlDarClickEnFondo);

            esperandoClickOpcionUnica = true;
        }
        else
        {
            backgroundClickButton.gameObject.SetActive(false);
            esperandoClickOpcionUnica = false;
            CreateResponses(template.optionAmount);
        }

        textAnimation.AnimateText();
    }

    private void AlDarClickEnFondo()
    {
        if (textAnimation.IsDoingAnimation())
        {
            textAnimation.ForzarFinAnimacion();
        }
        else
        {
            // Si la animación ya terminó y estamos parados en el texto 4 o 5:
            if (esUltimoTextoDelCapitulo)
            {
                esperandoClickOpcionUnica = false;
                StartCoroutine(SecuenciaTransicionCapitulo());
            }
            else
            {
                esperandoClickOpcionUnica = false;
                ControlButtons(0);
            }
        }
    }

    private IEnumerator SecuenciaTransicionCapitulo()
    {
        enTransicionFinal = true;
        DisableButton();
        backgroundClickButton.gameObject.SetActive(false);

        Debug.Log("Iniciando Fade In a negro...");

        // 1. FADE IN VISUAL (La pantalla se vuelve negra)
        if (faderNegro != null)
        {
            faderNegro.blocksRaycasts = true;

            float tiempoTranscurrido = 0f;
            while (tiempoTranscurrido < duracionFade)
            {
                tiempoTranscurrido += Time.deltaTime;
                faderNegro.alpha = Mathf.Clamp01(tiempoTranscurrido / duracionFade);
                yield return null;
            }
            faderNegro.alpha = 1f;
        }
        else
        {
            yield return new WaitForSeconds(duracionFade);
        }

        // 2. REPRODUCCIÓN Y FADE OUT DEL SONIDO DEL CAMIÓN
        if (sonidoTransicion != null)
        {
            Debug.Log("Fade visual completado. Reproduciendo sonido...");

            // Creamos el AudioSource temporal para el camión
            AudioSource audioTemporal = Camera.main.gameObject.AddComponent<AudioSource>();
            audioTemporal.clip = sonidoTransicion;
            audioTemporal.volume = 1f;
            audioTemporal.Play();

            // --- DETECTAR LA MÚSICA DE FONDO PRINCIPAL ---
            AudioSource musicaFondo = null;

            // Buscamos primero en el objeto SoundManager que tienes en tu jerarquía
            GameObject managerObjeto = GameObject.Find("SoundManager");
            if (managerObjeto != null)
            {
                musicaFondo = managerObjeto.GetComponent<AudioSource>();
            }

            // Si no estaba ahí, buscamos cualquier AudioSource en la escena que tenga el Loop activo (típico de música)
            if (musicaFondo == null)
            {
                AudioSource[] todosLosAudios = FindObjectsOfType<AudioSource>();
                foreach (var audio in todosLosAudios)
                {
                    if (audio.loop && audio.isPlaying && audio != audioTemporal)
                    {
                        musicaFondo = audio;
                        break;
                    }
                }
            }

            // Guardamos el volumen inicial de la música para desvanecerlo proporcionalmente
            float volumenInicialMusica = (musicaFondo != null) ? musicaFondo.volume : 0f;

            // Esperamos a que el sonido avance y solo falte 1.5 segundos para que termine
            float tiempoParaIniciarFadeAudio = sonidoTransicion.length - 1.5f;

            if (tiempoParaIniciarFadeAudio > 0)
            {
                yield return new WaitForSeconds(tiempoParaIniciarFadeAudio);
            }

            // Hacemos el Fade Out de AMBOS audios simultáneamente
            float tiempoFadeAudio = 1.5f;
            float transcurridoAudio = 0f;
            while (transcurridoAudio < tiempoFadeAudio)
            {
                transcurridoAudio += Time.deltaTime;
                float progreso = transcurridoAudio / tiempoFadeAudio;

                // Bajar volumen del camión
                audioTemporal.volume = Mathf.Lerp(1f, 0f, progreso);

                // Bajar volumen de la música principal si se encontró en la escena
                if (musicaFondo != null)
                {
                    musicaFondo.volume = Mathf.Lerp(volumenInicialMusica, 0f, progreso);
                }

                yield return null;
            }

            // Al terminar el fade, si la música sigue activa, la pausamos o detenemos por completo
            if (musicaFondo != null)
            {
                musicaFondo.Stop();
            }

            Destroy(audioTemporal);
        }

        // 3. ESPERA EXTRA EN SILENCIO Y OSCURIDAD
        Debug.Log("Fade de audio completo. Esperando 1 segundo en silencio...");
        yield return new WaitForSeconds(1.0f);

        Debug.Log("Cargando la escena del Capítulo 2...");

        // 4. CAMBIO DE ESCENA
        if (!string.IsNullOrEmpty(nombreEscenaCapitulo2))
        {
            SceneManager.LoadScene(nombreEscenaCapitulo2);
        }
        else
        {
            cortinaCapitulo.SetActive(true);
            textoTitulo.text = "Capítulo 2: El despertar";
            faderNegro.alpha = 0f;
            faderNegro.blocksRaycasts = false;
            enTransicionFinal = false;
            template = arrayTemplates[0];
            ShowText();
        }
    }

    private void CreateResponses(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            Button response = GetObjectInPool().GetComponent<Button>();

            if (response != null)
            {
                currentOptions.Add(response);
                responsesTexts[i].text = template.responses[i];

                response.onClick.RemoveAllListeners();

                int buttonIndex = i;
                response.onClick.AddListener(
                    () => {
                        ControlButtons(buttonIndex);
                    }
                );

                response.gameObject.SetActive(true);
            }
        }
    }

    private void ControlButtons(int index)
    {
        Debug.Log("Índice presionado: " + index);

        int siguienteIndice = template.arrayReferences[index];

        // Si por alguna razón el índice de referencia es inválido o menor a 0, seguridad ante todo:
        if (siguienteIndice < 0 || siguienteIndice >= arrayTemplates.Length)
        {
            StartCoroutine(SecuenciaTransicionCapitulo());
            return;
        }

        template = arrayTemplates[siguienteIndice];
        DisableButton();

        textAnimation.StopAnimations();
        ShowText();
    }

    private void DisableButton()
    {
        foreach (var item in currentOptions)
        {
            item.gameObject.SetActive(false);
        }
        currentOptions.Clear();
    }
}