using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public LevelArea levelArea;
    public PlayerHealth playerHealth;

    [SerializeField] float rockMaxSpeed;
    [SerializeField] float paperMaxSpeed;
    [SerializeField] float scissorsMaxSpeed;
    [SerializeField] float currentMaxSpeed;

    Vector3 moveVelocity;
    float maxHorizontalPosition;
    float minHorizontalPosition;
    float maxVerticalPosition;
    float minVerticalPosition;

    void Start()
    {
        playerHealth = GetComponent<PlayerHealth>();
        SetMaxPositions();
    }


    void Update()
    {
        SetSpeedBasedOnCurrentType();
        SetMoveVelocity();
        MovePlayer();
        MovePlayerBackToArea();
    }

    void SetMaxPositions()
    {
        maxHorizontalPosition = levelArea.GetHorizontalAreaLimit();
        minHorizontalPosition = levelArea.GetHorizontalAreaLimit() * -1;
        maxVerticalPosition = levelArea.GetVerticalAreaLimit();
        minVerticalPosition = levelArea.GetVerticalAreaLimit() * -1;
    }

    void SetMoveVelocity()
    {
        Vector3 moveInput = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        moveVelocity = moveInput * currentMaxSpeed;
    }

    void MovePlayer()
    {
        //rb.MovePosition(rb.position + moveVelocity * Time.fixedDeltaTime);
        transform.Translate(moveVelocity * Time.deltaTime);
    }

    void SetSpeedBasedOnCurrentType()
    {
        if (playerHealth.currentType == PlayerHealth.PlayerType.Rock) currentMaxSpeed = rockMaxSpeed;
        if (playerHealth.currentType == PlayerHealth.PlayerType.Paper) currentMaxSpeed = paperMaxSpeed;
        if (playerHealth.currentType == PlayerHealth.PlayerType.Scissors) currentMaxSpeed = scissorsMaxSpeed;
    }

    void MovePlayerBackToArea()
    {
        Vector3 maxHorizontalLimit = new(maxHorizontalPosition, transform.position.y, transform.position.z);
        Vector3 minHorizontalLimit = new(minHorizontalPosition, transform.position.y, transform.position.z);
        if (transform.position.x > maxHorizontalPosition) transform.position = maxHorizontalLimit;
        if (transform.position.x < minHorizontalPosition) transform.position = minHorizontalLimit;


        Vector3 maxVerticalLimit = new(transform.position.x, transform.position.y, maxVerticalPosition);
        Vector3 minVerticalLimit = new(transform.position.x, transform.position.y, minVerticalPosition);
        if (transform.position.z > maxVerticalPosition) transform.position = maxVerticalLimit;
        if (transform.position.z < minVerticalPosition) transform.position = minVerticalLimit;
    }
}
