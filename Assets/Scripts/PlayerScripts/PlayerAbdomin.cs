using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAbdomin : MonoBehaviour
{
    public bool hit;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.name.Equals("EnemyAttackPoint") || collision.name.Equals("HotSteveProjectile(Clone)") || collision.name.Equals("Beam"))
        {
            hit = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.name.Equals("EnemyAttackPoint") || collision.name.Equals("HotSteveProjectile(Clone)") || collision.name.Equals("Beam"))
        {
            hit = false;
        }
    }
}
