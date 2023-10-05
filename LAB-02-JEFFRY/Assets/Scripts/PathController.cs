using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathController : MonoBehaviour
{
    [SerializeField] public PathManager pathManager;

    List<Waypoint> thePath;
    Waypoint target;

    public float moveSpeed;
    public float rotateSpeed;

    public Animator animator;
    bool isWalking;

    public bool isBlockable;

    // Start is called before the first frame update
    void Start()
    {
        isWalking = false;
        animator.SetBool("isWalking", isWalking);

        thePath = pathManager.GetPath();

        if (thePath != null && thePath.Count > 0)
        {
            // set starting target to the first waypoint
            target = thePath[0];
        }
    }

    void RotateTowardsTarget()
    {
        float stepSize = rotateSpeed * Time.deltaTime;

        Vector3 targetDirection = target.position - transform.position;
        Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, stepSize, 0.0f);
        transform.rotation = Quaternion.LookRotation(newDirection);
    }

    void MoveForward()
    {
        float stepSize = Time.deltaTime * moveSpeed;
        float distanceToTarget = Vector3.Distance(transform.position, target.position);
        if (distanceToTarget < stepSize)
        {
            // we will overshoot the target,
            // so we should do something smarter here
            return;
        }
        // take a step forward
        Vector3 moveDirection = Vector3.forward;
        transform.Translate(moveDirection * stepSize);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.anyKeyDown)
        {
            // toggle if any key is pressed
            isWalking = !isWalking;
            animator.SetBool("isWalking", isWalking);
        }
        if (isWalking)
        {
            RotateTowardsTarget();
            MoveForward();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Object") && isBlockable)
        {
            isWalking = false;
            animator.SetBool("isWalking", isWalking);
        }

        // switch to next target
        target = pathManager.GetNextTarget();
    }
}
