using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class AccelerateTowardsPlayerAction : Action
{
    [SerializeField] float acceleration = 30;
    [SerializeField] float maxSpeed = 150;
    [SerializeField] float turnSpeed = 1;
    Vector3 relativePosition;

    public override void Act()
    {
        if (!isActing && !hasActed)
        {
            StartCoroutine(CountMovementDuration(duration));
            isActing = true;
        }
        if (isActing)
        {
                    TurnTowardsPlayer();
        AccelerateForward();
        }
    }

    void TurnTowardsPlayer()
    {
        Vector3 relativePosition = GameManager.GetPlayerPosition() - transform.position;
        Quaternion toRotation = Quaternion.LookRotation(relativePosition);
        transform.rotation = Quaternion.Lerp(transform.rotation, toRotation, turnSpeed * Time.deltaTime);
    }

    void AccelerateForward()
    {
        transform.Translate(0, 0, acceleration * Time.deltaTime);
        if (acceleration > maxSpeed) acceleration += 1;
    }

    IEnumerator CountMovementDuration(float duration)
    {
        yield return new WaitForSeconds(duration);
        isActing = false;
        hasActed = true;
    }
}