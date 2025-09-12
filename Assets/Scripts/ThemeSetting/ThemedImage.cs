using UnityEngine;
using UnityEngine.UI;

public class ThemedImage : ThemedBehaviour
{
    public enum Role { BgCanvas, BgLight, BgDark, BgAccent, Icon }

    [SerializeField] Role role;
    private Image _target;
    private void Reset()
    {
        _target = GetComponent<Image>();
    }

    public override void ApplyTheme(ThemePalette p)
    {
        if (_target == null) _target = GetComponent<Image>();
        if (_target == null) return;

        switch (role)
        {
            case Role.BgCanvas: _target.color = p.bgCanvas; break;
            case Role.BgLight:  _target.color = p.bgLight;   break;
            case Role.BgDark:   _target.color = p.bgDark;    break;
            case Role.BgAccent: _target.color = p.bgAccent;  break;
            case Role.Icon:     _target.color = p.icon;      break;
        }
    }
}
