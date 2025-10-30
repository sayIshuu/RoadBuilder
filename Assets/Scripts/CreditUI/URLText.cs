using System;
using UnityEngine;
using UnityEngine.UI;

public class URLText : MonoBehaviour
{
    [SerializeField] private string url;
    private Button _button;

    private void Awake()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(OpenURL);
    }

    private void OpenURL()
    {
        Application.OpenURL(url);
    }
}
