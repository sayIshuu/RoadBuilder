using System.Collections;
using UnityEngine;

public abstract class ThemedBehaviour : MonoBehaviour, IThemeChangeable
{
    protected virtual void OnEnable()
    {
        if (ThemeManager.Instance != null)
        {
            ThemeManager.Instance.OnThemeChanged += ApplyTheme;
            ApplyTheme(ThemeManager.Instance.Current);
        }
        else
        {
            // 한 프레임 뒤에 재시도
            StartCoroutine(WaitAndSubscribe());
        }
    }

    protected virtual void OnDisable()
    {
        ThemeManager.Instance.OnThemeChanged -= ApplyTheme;
    }

    private IEnumerator WaitAndSubscribe()
    {
        yield return null; // 한 프레임 대기
        if (ThemeManager.Instance != null)
        {
            ThemeManager.Instance.OnThemeChanged += ApplyTheme;
            ApplyTheme(ThemeManager.Instance.Current);
        }
    }

    public abstract void ApplyTheme(ThemePalette p);
}
