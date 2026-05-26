using System.Collections;
using TMPro;
using UnityEngine;

public class TextAnimation : MonoBehaviour
{
    private string historyTextContent;
    private bool doingAnimation;
    private IEnumerator animationCoroutine;

    [SerializeField] private TextMeshProUGUI historyText;

    public bool IsDoingAnimation() => doingAnimation;

    public void SetDoingAnimation(bool newAnimationState)
    {
        doingAnimation = newAnimationState;
    }

    public void StopAnimations()
    {
        if (animationCoroutine != null)
        {
            StopCoroutine(animationCoroutine);
        }
        doingAnimation = false;
    }

    public void AnimateText()
    {
        // Detenemos cualquier animación previa por seguridad antes de iniciar una nueva
        StopAnimations();

        animationCoroutine = AnimateHistoryText();
        StartCoroutine(animationCoroutine);
    }

    private IEnumerator AnimateHistoryText()
    {
        doingAnimation = true;
        historyTextContent = historyText.text;
        historyText.text = string.Empty;

        char[] historyLetters = historyTextContent.ToCharArray();

        for (int i = 0; i < historyLetters.Length; i++)
        {
            historyText.text += historyLetters[i];
            yield return new WaitForSeconds(0.03f);
        }

        // Al terminar el bucle de forma natural, la animación cambia de estado
        doingAnimation = false;
    }

    // Esta función la llama TextController si el usuario hace clic a mitad de la animación
    public void ForzarFinAnimacion()
    {
        StopAnimations();
        historyText.text = historyTextContent; // Muestra el texto completo de golpe
        doingAnimation = false;
    }
}