using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingPlatform : MonoBehaviour
{
    [SerializeField] float timeToBreak;
    [SerializeField] float timeToReturn;
    [SerializeField] ParticleSystem smokeCloud;
    [SerializeField] ParticleSystem fallingRocks;
    
    bool isBreaking = false;

    BoxCollider2D boxCollider;
    SpriteRenderer sprite;

    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        sprite = GetComponent<SpriteRenderer>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<NewHeroController>())
        {
            if (!isBreaking)
                StartCoroutine(BreakPlatform());
        }
    }

    IEnumerator BreakPlatform()
    {
        isBreaking = true;
        fallingRocks.Play();
        yield return new WaitForSeconds(timeToBreak);
        Color color = sprite.color;
        color.a = 0;
        sprite.color = color;
        smokeCloud.Play();
        boxCollider.enabled = false;
        StartCoroutine(FixPlatform());
    }

    IEnumerator FixPlatform()
    {
        yield return new WaitForSeconds(timeToReturn);
        Color color = sprite.color;
        color.a = 255;
        sprite.color = color;
        boxCollider.enabled = true;
        isBreaking = false;
    }
}
