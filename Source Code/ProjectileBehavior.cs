using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBehavior : MonoBehaviour
{
    public float Speed = 50f;
    readonly private float _killBoundaryX = 35f;
    readonly private float _killBoundaryNegX = -35f;

    private void Update()
    {
        if (transform.position.x >= _killBoundaryX || transform.position.x <= _killBoundaryNegX)
            Destroy(gameObject);
    }

    // FixedUpdate is called once per fixed framerate frame
    private void FixedUpdate()
    {
        transform.position += Speed * Time.deltaTime * transform.right;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Destroy(gameObject);
    }
}
