using System.Collections;
using UnityEngine;

public class ShootedCracker : MonoBehaviour
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
        if (transform.position.y > 1.2f * h)
        {
            Destroy(gameObject);
        }
        else if (transform.position.y < -0.5f * h)
        {
            Destroy(gameObject);
        }
    }

    public void StartcrackerEffect()
    {
        StartCoroutine(CrackerEffect());
    }

    IEnumerator CrackerEffect()
    {
        yield return new WaitForSecondsRealtime(Random.Range(0, 0.1f));

        rb.linearVelocity = new Vector2(0, 10000f);
        rb.gravityScale = -Random.Range(5000f, 10000f);
        cf.force = new Vector2(Random.Range(-20000f, 20000f), 0);

        cf.torque = Random.Range(-1000f, 1000f);
        
        yield return new WaitForSecondsRealtime(Random.Range(0.2f, 0.3f));

        rb.gravityScale *= -1;
    }
}
