using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialManager : MonoBehaviour
{
    private string _tutorialCompletedKey = "TutorialCompleted";
    private void Awake()
    {
        // 튜토리얼 재생 테스트용 코드
        /*PlayerPrefs.DeleteKey(_tutorialCompletedKey);
        PlayerPrefs.Save();*/

        int hasCompletedTutorial = PlayerPrefs.GetInt(_tutorialCompletedKey, 0);
        if (hasCompletedTutorial == 0)
        {
            SceneManager.LoadScene("TutorialScene");
        }
    }
}
