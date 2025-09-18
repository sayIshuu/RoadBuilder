using TMPro;
using UnityEngine;

public class CurrentLifeText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _currentLifeText;

    private void Start()
    {
        _currentLifeText.text = LifeManager.Instance.CurrentLives.ToString();
        LifeManager.Instance.OnCurrentLivesChange += UpdateCurrentLifeText;
    }

    public void UpdateCurrentLifeText(int cur)
    {
        _currentLifeText.text = cur.ToString();
    }

    private void OnDestroy()
    {
        LifeManager.Instance.OnCurrentLivesChange -= UpdateCurrentLifeText;
    }
}
