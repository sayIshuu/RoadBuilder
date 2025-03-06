using UnityEngine;
using UnityEngine.SceneManagement;

public class ResetButton : MonoBehaviour
{
    public void Reset()
    {
        SoundManager.Instance.PlayDisplaySound();
        SceneManager.LoadScene("MainScene");
    }
}