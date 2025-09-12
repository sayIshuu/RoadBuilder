using UnityEngine;
using UnityEngine.UI;

public class ThemedImage : ThemedBehaviour
{
    public enum Role { BgCanvas, BgLight, BgDark, BgAccent, Icon, BgSetting }

    [SerializeField] Role role;
    private Image _target;
    private void Reset()
    {
        _target = GetComponent<Image>();
    }

    public override void ApplyTheme(ThemePalette p)
    {
        if (!_target) _target = GetComponent<UnityEngine.UI.Image>();
        if (!_target) return;

        // 현재 색상 알파값
        float currentAlpha = _target.color.a;

        Color newColor = role switch
        {
            Role.BgCanvas  => p.bgCanvas,
            Role.BgLight   => p.bgLight,
            Role.BgDark    => p.bgDark,
            Role.BgAccent  => p.bgAccent,
            Role.Icon      => p.icon,
            Role.BgSetting => p.bgSetting,
            _ => _target.color
        };

        newColor.a = currentAlpha;
        _target.color = newColor;
    }
}
