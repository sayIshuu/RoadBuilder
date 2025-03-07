using UnityEngine;

public class LevelUpEffect : MonoBehaviour
{
    [SerializeField]
    private GameObject goalEffect;
    public Transform canvas;
    private int w;

    private void Awake()
    {
        if (canvas == null) canvas = FindFirstObjectByType<Canvas>().transform;
        w = Screen.width;
    }

    public void CrackerShoot(int level)
    {
        if(canvas == null) canvas = FindFirstObjectByType<Canvas>().transform;

        for (int i = 0; i < 20 + 10 * level; i++)
        {
            GameObject newEffect = Instantiate(goalEffect, canvas);
            newEffect.transform.SetParent(canvas);
            newEffect.transform.SetAsFirstSibling();

            newEffect.transform.position = new Vector2(Random.Range(0.1f * w, 0.9f * w), -200);

            newEffect.GetComponent<ShootedCracker>().StartcrackerEffect();
        }
    }
}
