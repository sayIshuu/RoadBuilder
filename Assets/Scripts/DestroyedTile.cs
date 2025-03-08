using System.Collections;
using UnityEngine;

public class DestroyedTile : MonoBehaviour
{
    Rigidbody2D rb;
    ConstantForce2D cf;
    private int h;

    private void Awake()
    {
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

    IEnumerator DestroyEffect()
    {
        //cf�� force�� �ִ� ��� rb �ӵ��� x���� �ٲٴ� ����� ����
        rb.linearVelocity = new Vector2(0, 3500f);
        cf.force = new Vector2(Random.Range(-5000f, 5000f), 0);
        //rb.AddForce(new Vector2(Random.Range(-5000f, 5000f), 0));
        //rb.AddForce(new Vector2(Random.Range(-5000f, 5000f), 0), ForceMode2D.Impulse);

        cf.torque = Random.Range(-100f, 100f);

        yield return new WaitForSecondsRealtime(0.05f);

        rb.gravityScale = Random.Range(2000f, 3000f);
    }
}
