using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelayedFallChandelier : MonoBehaviour
{
    public GameObject breakPoint;
    public float breakTime;
    bool breaking = false;
    
    public void StartTimer()
    {
        breaking = true;
        StartCoroutine(breakTimer());
    }

    private void BreakChain()
    {
        Destroy(breakPoint);
    }

    IEnumerator breakTimer()
    {
        yield return new WaitForSeconds(breakTime);
        BreakChain();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!breaking)
        {
            if (collision.gameObject.GetComponent<NewHeroController>())
            {
                StartTimer();
            }   
        }
        
    }
}
