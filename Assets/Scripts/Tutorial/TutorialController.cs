using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialController : MonoBehaviour
{
    private List<TutorialBase> _tutorials;
    private TutorialBase _currentTutorial;
    private int currentIndex = -1;

    private void Start()
    {
        _tutorials = GetComponentsInChildren<TutorialBase>().OrderBy(c => c.transform.GetSiblingIndex()).ToList();
        SetNextTutorial();
    }

    private void Update()
    {
        if (_currentTutorial != null )
        {
            _currentTutorial.Execute(this);
        }
    }

    public void SetNextTutorial()
    {
        if (_currentTutorial != null)
        {
            _currentTutorial.Exit();
        }

        // 튜토리얼 종료
        if (currentIndex >= _tutorials.Count - 1)
        {
            CompleteAllTutorials();
            return;
        }

        currentIndex++;
        _currentTutorial = _tutorials[currentIndex];

        _currentTutorial.Enter(this);
    }

    private void CompleteAllTutorials()
    {
        _currentTutorial = null;

        PlayerPrefs.SetInt("TutorialCompleted", 1);
        PlayerPrefs.Save();

        StartCoroutine(LoadSceneCoroutine("MainScene", 1f));
    }

    private IEnumerator LoadSceneCoroutine(string sceneName, float interval = 0f)
    {
        yield return new WaitForSeconds(interval);

        AsyncOperation async = SceneManager.LoadSceneAsync(sceneName);

        while (!async.isDone)
        {
            yield return null;
        }
    }
}
