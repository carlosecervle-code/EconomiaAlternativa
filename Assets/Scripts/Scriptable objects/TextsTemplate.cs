using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Text template object")]
public class TextsTemplate : ScriptableObject
{
    [TextArea(5,15)]
    [Tooltip("Cadena que contiene el texto de la historia cuando se va a tomar una desición.")]
    public string mainText;

    [Space(50)]
    [Header("Options section")]
    [Space(10)]
    public int optionAmount;

    [Tooltip("Lista de texto de las respuestas que puede tomar el jugador.")]
    public List<string> responses = new List<string>();

    [Tooltip("Lista de índices de a que parte de la historia ir cuando se toma una decisión.")]
    public int[] arrayReferences = new int[3];

    public bool quitButtons;

    public bool chargeAnotherOptions;

    [Space(50)]
    [Header("Dialogs section")]
    [Space(10)]
    [Tooltip("Lista que contiene los dialogos de la sección actual.")]
    public List<string> dialogs = new List<string>();

    [Tooltip("Entero que indica el indice de a que parte de la historia ir al terminar los diálogos.")]
    public int optionsIndex;

    public bool endGame;
}
