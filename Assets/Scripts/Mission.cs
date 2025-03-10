using UnityEngine;
using UnityEngine.UI;

public class Mission : MonoBehaviour
{
    public string missionName;
    public int requiredLength;
    private bool isCompleted = false;
    private Image image;

    private void Awake()
    {
        image = GetComponent<Image>();
        isCompleted = PlayerPrefs.GetInt(missionName, 0) == 1;
        if (isCompleted)
        {
            missionCompleted();
        }
    }

    public void CheckLengthMisson(int len)
    {
        if (requiredLength == 0)
        {
            return;
        }

        if (len >= requiredLength)
        {
            isCompleted = true;
            missionCompleted();
        }
    }

    private void missionCompleted()
    {
        transform.SetAsLastSibling();
        // 6F6345
        image.color = new Color32(111, 99, 69, 200);
        PlayerPrefs.SetInt(missionName, 1);
    }
}
