using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PopupNotEnough : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _text;

    public TextMeshProUGUI Text { get => _text; set => _text = value; }

    private void OnEnable()
    {
        StartCoroutine(Fade());
    }

    private IEnumerator Fade()
    {
        yield return new WaitForSecondsRealtime(1f);
        gameObject.SetActive(false);
    }

    public void SetPosition(string value)
    {
        gameObject.SetActive(true);
        _text.text = LocalizationManager.Instance.GetLocalizedValue(value);
    }

    public void SetPosition()
    {
        gameObject.SetActive(true);
        _text.text = LocalizationManager.Instance.GetLocalizedValue("notenough");
    }
}
