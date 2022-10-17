using UnityEngine;

public class Target : MonoBehaviour
{
    public float health = 50f;
    //Target health logic
    public void TakeDamage(float amount)
    {
        health -= amount;
        if (health <= 0f)
        {
            Die();
        }
    }
    // Makes the object/enemy disepear/die
    void Die()
    {
        Destroy(gameObject);
    }
}
