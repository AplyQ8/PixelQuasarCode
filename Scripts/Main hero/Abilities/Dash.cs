using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dash : MonoBehaviour
{
    [SerializeField] private float dashPower;
    [SerializeField] private float dashingTime;
    [SerializeField] private Rigidbody2D rigidBody;

    private IEnumerator DashLogic(Vector3 direction)
    {
        rigidBody.velocity = new Vector2(direction.x * dashPower, direction.y * dashPower);
        yield return new WaitForSeconds(dashingTime);
    }
}

