using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ResetButton : MonoBehaviour
{
    public void Reset()
    {
        // 목숨부족시 재시작 불가.
        if (LifeManager.Instance.CurrentLives <= 0)
        {
            // todo : 광고유도
            return;
        }

        SoundManager.Instance.PlaySelectSound();
        SceneManager.LoadScene("MainScene");
        LifeManager.Instance.UseLife();
    }
}
