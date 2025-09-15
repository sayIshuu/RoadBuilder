using TMPro;
using UnityEngine;

public class ThemedText : ThemedBehaviour
{
    public enum Role { Primary, Secondary, Warning, Success }

    [SerializeField] Role role;
    private TMP_Text _target;

    void Awake()
    {
        _target   = GetComponent<TMP_Text>();
    }

    public override void ApplyTheme(ThemePalette p)
    {
        var c = role switch
        {
            Role.Primary  => p.textPrimary,
            Role.Secondary=> p.textSecondary,
            Role.Warning  => p.textWarning,
            Role.Success  => p.textSuccess,
            _             => p.textPrimary
        };

        _target.color = c;
    }
}
