using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ResetButton : MonoBehaviour
{
    //[SerializeField] private Toggle resetToggle;
    public void Reset()
    {
        SoundManager.Instance.PlayDisplaySound();
        SceneManager.LoadScene("MainScene");
    }
}