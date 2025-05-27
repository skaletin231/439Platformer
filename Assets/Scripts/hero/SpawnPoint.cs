using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{

    
   [SerializeField] GameObject fireObject;
   [SerializeField] float healCD = 10;
    //Gives a cd to heals
    Dictionary<PlayerHealth, float> timeTracker = new Dictionary<PlayerHealth, float>();
    float timePassed = 0f;

    Animator fireAnimator;

    private void Awake()
    {
        if (fireObject)
        {
            fireAnimator = fireObject.GetComponent<Animator>();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        NewHeroController hero = collision.GetComponent<NewHeroController>();
        PlayerHealth health = collision.GetComponent<PlayerHealth>();
        if (hero)
        {
            fireAnimator.SetInteger("State",1);
            hero.SetNewSpawnPoint(this);  
        }
        if (health)
        {
            if (!timeTracker.ContainsKey(health))
            {
                timeTracker.Add(health, timePassed);
                health.FullHeal();
            }
            else
            {
                if (timeTracker[health] + healCD < timePassed)
                {
                    timeTracker[health] = timePassed;
                    health.FullHeal();
                }
            }            
        }
    }

    private void Update()
    {
        timePassed += Time.deltaTime;
    }

    public void StopAnimation()
    {
        fireAnimator.SetInteger("State", 0);
    }
}
