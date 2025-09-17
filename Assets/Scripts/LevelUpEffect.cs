using UnityEngine;

public class LevelUpEffect : MonoBehaviour
{
    [SerializeField]
    private GameObject goalEffect;
    private Transform canvas;
    private int w;

    private void Awake()
    {
        canvas = FindFirstObjectByType<GameCanvas>().GetComponent<Canvas>().transform;
        w = Screen.width;
    }

    public void CrackerShoot(int level)
    {
        //canvas ??= FindFirstObjectByType<Canvas>().transform;
        if (canvas == null) canvas = FindFirstObjectByType<GameCanvas>().GetComponent<Canvas>().transform;

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
