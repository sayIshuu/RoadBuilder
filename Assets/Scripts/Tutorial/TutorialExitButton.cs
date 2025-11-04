using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialExitButton : MonoBehaviour
{
    public void ExitTutorial()
    {
        PlayerPrefs.SetInt("TutorialCompleted", 1);
        PlayerPrefs.Save();

        SoundManager.Instance.PlaySelectSound();
        SceneManager.LoadScene("MainScene");

    }
}
