using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class attack : MonoBehaviour
{
    public float attackPower = 1.0f;
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "Enemy")
        {
            slimeController slime = col.gameObject.GetComponent<slimeController>();
            slime.takeDamage(attackPower);
        }
    }
}
