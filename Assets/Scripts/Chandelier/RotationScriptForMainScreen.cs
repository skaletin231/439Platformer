using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationScriptForMainScreen : MonoBehaviour
{
    [SerializeField] GameObject objectToRotate;
    [SerializeField] float rotationgAmountPerFrame = 1f;
    [SerializeField] bool hitLeft = false;

    Rigidbody2D rb;

    private void Awake()
    {
        rb = objectToRotate.GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (hitLeft)
        {
            if (objectToRotate.transform.rotation.eulerAngles.z < 2 || objectToRotate.transform.rotation.eulerAngles.z > -2 && rb.velocity.x <= 0.1)
            {
                rb.AddRelativeForce(Vector2.left * rotationgAmountPerFrame * Time.deltaTime, ForceMode2D.Impulse);
            }
        }
        else
        {
            if (objectToRotate.transform.rotation.eulerAngles.z < 2 || objectToRotate.transform.rotation.eulerAngles.z > -2 && rb.velocity.x >= -0.1)
            {
                rb.AddRelativeForce(-Vector2.left * rotationgAmountPerFrame * Time.deltaTime, ForceMode2D.Impulse);
            }
        }
    }
}
