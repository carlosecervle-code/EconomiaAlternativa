using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextController : MonoBehaviour
{
    [SerializeField] private TextsTemplate template;
    [SerializeField] public TextsTemplate[] arrayTemplates;

    [SerializeField] private TextMeshProUGUI mainText;

    [SerializeField] private GameObject responseButtonPrefab;
    [SerializeField] private Transform responseContent;

    private void Start()
    {
        template = arrayTemplates[0];
        ShowText();
    }

    private void ShowText()
    {
        mainText.text = template.mainText;
        CreateResponses(template.optionAmount);
        //TODO: ANIMAR TEXTOS
    }

    private void CreateResponses(int amount)
    {
        for(int i = 0; i < amount; i++)
        {
            GameObject response = Instantiate(responseButtonPrefab, responseContent);
            response.GetComponentInChildren<TextMeshProUGUI>().text = template.responses[i];

        }
    }
}