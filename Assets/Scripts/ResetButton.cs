using UnityEngine;
using UnityEngine.SceneManagement;

public class ResetButton : MonoBehaviour
{
    public void Reset()
    {
        SoundManager.Instance.PlayDisplaySound();
        BoardCheck.score = 0;
        BoardCheck.gameover = false;
        SceneManager.LoadScene("MainScene");
    }
}
