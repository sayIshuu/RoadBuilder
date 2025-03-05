using System.Collections;
using UnityEngine;

public class TileDestroyCheck : MonoBehaviour
{
    Rigidbody2D rb;
    ConstantForce2D cf;

    public void Awake()
    {
        rb = transform.GetComponent<Rigidbody2D>();
        cf = transform.GetComponent<ConstantForce2D>();

        //초기화가 안됨, 생성된 타일에서는 안발생하는 버그로 추측
        rb.gravityScale = 0f;
        cf.force = Vector2.zero;
        cf.torque = 0f;
    }

    public void Update()
    {
        if (transform.position.y < -500)
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
        //cf의 force를 주는 대신 rb 속도의 x값을 바꾸는 방법도 가능
        rb.linearVelocity = new Vector2(0, 3500f);
        cf.force = new Vector2(Random.Range(-5000f, 5000f), 0);
        
        cf.torque = Random.Range(-100f, 100f);

        yield return new WaitForSecondsRealtime(0.05f);

        rb.gravityScale = Random.Range(2000f, 3000f);
    }
}
