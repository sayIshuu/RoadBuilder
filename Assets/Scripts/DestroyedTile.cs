using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DestroyedTile : MonoBehaviour
{
    private Image image;
    private Rigidbody2D rb;
    private ConstantForce2D cf;
    private int h;

    private Color32 orangeColor = new Color32(248, 202, 155, 255);
    private Color32 blueColor = new Color32(155, 165, 248, 255);
    private Color32 redColor = new Color32(255, 83, 110, 255);

    private void Awake()
    {
        image = gameObject.GetComponent<Image>();
        rb = transform.GetComponent<Rigidbody2D>();
        cf = transform.GetComponent<ConstantForce2D>();
        h = Screen.height;
    }

    private void Update()
    {
        if (transform.position.y > h)
        {
            transform.position = new Vector2(transform.position.x, h);
        }
        else if (transform.position.y < -0.2f * h)
        {
            Destroy(gameObject);
        }
    }

    public void StartDestroyEffect()
    {
        StartCoroutine(DestroyEffect());
    }

    public void StartDestroyEffect(int tileType)
    {
        ChangeTilePieceColor(tileType);
        StartCoroutine(DestroyEffect());
    }

    IEnumerator DestroyEffect()
    {
        //cf의 force를 주는 대신 rb 속도의 x값을 바꾸는 방법도 가능
        rb.linearVelocity = new Vector2(0, 3500f);
        cf.force = new Vector2(Random.Range(-5000f, 5000f), 0);
        //rb.AddForce(new Vector2(Random.Range(-5000f, 5000f), 0));
        //rb.AddForce(new Vector2(Random.Range(-5000f, 5000f), 0), ForceMode2D.Impulse);

        cf.torque = Random.Range(-100f, 100f);

        yield return new WaitForSecondsRealtime(0.05f);

        rb.gravityScale = Random.Range(2000f, 3000f);
    }

    private void ChangeTilePieceColor(int tileType)
    {
        // // 부서진 타일 조각 색 변경
        switch (tileType)
        {
            case 10:
            case 5:
            case 6:
            case 12:
            case 9:
            case 3:
                image.color = orangeColor;
                break;
            case 7:
            case 11:
            case 14:
            case 13:
                image.color = blueColor;
                break;
            case 15:
                image.color = redColor;
                break;
            case 0:
                image.color = Color.white;
                break;
        }
    }
}
