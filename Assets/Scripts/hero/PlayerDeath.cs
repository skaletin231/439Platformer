using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerDeath : MonoBehaviour
{
    NewHeroController heroController;
    heroAttackController attackController;

    Animator animator;

    private void Awake()
    {
        heroController = GetComponent<NewHeroController>();
        attackController = GetComponent<heroAttackController>();
        animator = GetComponent<Animator>();
    }

    public void OnDeath()
    {
        heroController.enabled = false;
        attackController.enabled = false;
        StartCoroutine(DeathAnimation());
    }

    IEnumerator DeathAnimation()
    {
        animator.SetTrigger("Death");
        yield return new WaitForSeconds(3);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
