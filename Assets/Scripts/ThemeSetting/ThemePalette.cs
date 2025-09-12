using UnityEngine;

[CreateAssetMenu(fileName = "ThemePalette", menuName = "Scriptable Objects/ThemePalette")]
public class ThemePalette : ScriptableObject
{
    [Header("Backgrounds")]
    public Color bgCanvas;   // 캔버스 배경
    public Color bgLight;    // 연한 배경 (타일, 업적 등)
    public Color bgDark;     // 진한 배경 (스코어, 스크롤뷰 등)
    public Color bgAccent;   // 강조 배경 (재시작 버튼, 슬라이더 핸들 등)
    public Color bgSetting;  // 설정창 배경

    [Header("Texts")]
    public Color textPrimary;   // 기본 텍스트(흰 글씨)
    public Color textSecondary; // 진한 텍스트(라이트에선 어두운 글씨, 다크에선 회색)
    public Color textWarning;   // 경고 텍스트(빨강 톤)
    public Color textSuccess;   // 성공 텍스트(초록 톤)

    [Header("Icons")]
    public Color icon;          // 아이콘(설정, 뒤로가기, 테마 등)
}
