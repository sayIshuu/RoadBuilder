using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialManager : MonoBehaviour
{
    private void Awake()
    {
        // 튜토리얼 재생 테스트용 코드
        /*PlayerPrefs.DeleteKey("TutorialCompleted");
        PlayerPrefs.Save();*/

        int hasCompletedTutorial = PlayerPrefs.GetInt("TutorialCompleted", 0);
        if (hasCompletedTutorial == 0)
        {
            SceneManager.LoadScene("TutorialScene");
        }
    }
}
