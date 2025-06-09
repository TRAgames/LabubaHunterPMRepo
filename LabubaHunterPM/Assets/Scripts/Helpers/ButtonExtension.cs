using TMPro;
using UnityEngine;
using UnityEngine.UI;

public static class ButtonExtension
{
    public static void SetActive(this Button button, TextMeshProUGUI textMeshProUGUI, Color colorButton, Color colorText)
    {
        button.GetComponent<Image>().color = colorButton;
        textMeshProUGUI.color = colorText;
    }

    public static void SetUnactive(this Button button, TextMeshProUGUI textMeshProUGUI, Color color)
    {
        button.GetComponent<Image>().color = color;
        textMeshProUGUI.color = Color.red;
    }

}
