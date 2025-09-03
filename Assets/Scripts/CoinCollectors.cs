using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinCollectors : MonoBehaviour
{
    public static int CollectedCoins = 0;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            CollectedCoins++;
            Destroy(gameObject);
        }
    }
}
