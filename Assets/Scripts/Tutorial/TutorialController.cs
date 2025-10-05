using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialController : MonoBehaviour
{
    [SerializeField] private List<TutorialBase> tutorials;
    public TutorialBase _currentTutorial;
    private int currentIndex = - 1;

    private void Start()
    {
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
        if (currentIndex >= tutorials.Count - 1)
        {
            CompleteAllTutorials();
            return;
        }

        currentIndex++;
        _currentTutorial = tutorials[currentIndex];

        _currentTutorial.Enter(this);
    }

    private void CompleteAllTutorials()
    {
        _currentTutorial = null;
        Debug.unityLogger.Log("튜토리얼 종료!");

        StartCoroutine(LoadSceneCoroutine("MainScene"));
    }

    private IEnumerator LoadSceneCoroutine(string sceneName)
    {
        // (선택) 약간의 대기
        yield return new WaitForSeconds(0.5f);

        // 비동기 로딩 시작
        AsyncOperation async = SceneManager.LoadSceneAsync(sceneName);

        // 로딩 완료까지 대기
        while (!async.isDone)
        {
            Debug.Log($"Loading progress: {async.progress}");
            yield return null;
        }

        Debug.Log("Scene loaded!");
    }
}
