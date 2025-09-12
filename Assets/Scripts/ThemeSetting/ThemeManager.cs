using System;
using UnityEngine;

public class ThemeManager : MonoBehaviour
{
    public enum ThemeKind { Light = 0, Dark = 1 }

    public static ThemeManager Instance { get; private set; }

    [SerializeField] private ThemePalette lightPalette;
    [SerializeField] private ThemePalette darkPalette;

    public ThemePalette Current { get; private set; }
    public ThemeKind CurrentTheme { get; private set; }

    public event Action<ThemePalette> OnThemeChanged;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        CurrentTheme = ThemeKind.Light;
        ApplyTheme(CurrentTheme, false);
        OnThemeChanged?.Invoke(Current);
    }

    public void ToggleTheme()
    {
        ApplyTheme(CurrentTheme == ThemeKind.Light ? ThemeKind.Dark : ThemeKind.Light);
    }

    public void ApplyTheme(ThemeKind kind, bool invokeEvent = true)
    {
        CurrentTheme = kind;
        Current = (kind == ThemeKind.Dark) ? darkPalette : lightPalette;

        if (invokeEvent) OnThemeChanged?.Invoke(Current);
    }
}

