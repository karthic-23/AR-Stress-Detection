using System.Collections;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private bool _hasHit = false;

    public void Init(float lifetime)
    {
        StartCoroutine(DestroyAfterDelay(lifetime));
    }

    void OnCollisionEnter(Collision collision)
    {
        if (_hasHit) return;

        Target enemy = collision.gameObject.GetComponentInParent<Target>(); // changed
        if (enemy != null)
        {
            _hasHit = true;
            enemy.OnHit();
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (_hasHit) return;

        Target enemy = other.GetComponentInParent<Target>(); // changed
        if (enemy != null)
        {
            _hasHit = true;
            enemy.OnHit();
            Destroy(gameObject);
        }
    }

    IEnumerator DestroyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (gameObject != null)
            Destroy(gameObject);
    }
}
