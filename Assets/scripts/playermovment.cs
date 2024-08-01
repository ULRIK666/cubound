using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    public float moveDistance = 5f;
    public float moveDuration = 0.3f;
    public float moveDelay = 0.35f;
    public float rotationResetDelay = 0.1f;

    private bool isMoving = false;
    private LevelManager levelManager;

    void Start()
    {
        levelManager = FindObjectOfType<LevelManager>();
    }

    void Update()
    {
        if (!isMoving)
        {
            HandleMovementInput();
        }
    }

    void HandleMovementInput()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");

        // Prioritize horizontal movement over vertical
        if (Mathf.Abs(moveX) > Mathf.Abs(moveZ))
        {
            moveZ = 0;
        }
        else
        {
            moveX = 0;
        }

        if (moveX != 0 || moveZ != 0)
        {
            Vector3 moveDirection = new Vector3(moveX, 0, moveZ).normalized;
            Vector3 targetPosition = transform.position + moveDirection * moveDistance;

            if (levelManager.CanMove(targetPosition))
            {
                StartCoroutine(Move(targetPosition, moveX, moveZ));
            }
            else
            {
                GameObject box = levelManager.GetBoxAtPosition(targetPosition);
                if (box != null)
                {
                    Vector3 boxTargetPosition = box.transform.position + moveDirection * moveDistance;
                    if (levelManager.CanMove(boxTargetPosition))
                    {
                        StartCoroutine(MoveWithBox(targetPosition, box, boxTargetPosition, moveX, moveZ));
                    }
                }
            }
        }
    }

    IEnumerator Move(Vector3 targetPosition, float moveX, float moveZ)
    {
        isMoving = true;

        Vector3 startPosition = transform.position;
        Quaternion startRotation = transform.rotation;
        Quaternion targetRotation = Quaternion.identity;

        if (moveX != 0)
        {
            float rotationZ = -90f * Mathf.Sign(moveX);
            targetRotation = Quaternion.Euler(0, 0, rotationZ);
        }
        else if (moveZ != 0)
        {
            float rotationX = 90f * Mathf.Sign(moveZ);
            targetRotation = Quaternion.Euler(rotationX, 0, 0);
        }

        float elapsedTime = 0;

        while (elapsedTime < moveDuration)
        {
            transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / moveDuration);
            transform.rotation = Quaternion.Slerp(startRotation, targetRotation, elapsedTime / moveDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPosition;
        transform.rotation = targetRotation;

        yield return new WaitForSeconds(rotationResetDelay);

        transform.rotation = Quaternion.Euler(0, 0, 0);

        yield return new WaitForSeconds(moveDelay);

        isMoving = false;
    }

    IEnumerator MoveWithBox(Vector3 targetPosition, GameObject box, Vector3 boxTargetPosition, float moveX, float moveZ)
    {
        isMoving = true;

        Vector3 startPosition = transform.position;
        Quaternion startRotation = transform.rotation;
        Quaternion targetRotation = Quaternion.identity;

        if (moveX != 0)
        {
            float rotationZ = -90f * Mathf.Sign(moveX);
            targetRotation = Quaternion.Euler(0, 0, rotationZ);
        }
        else if (moveZ != 0)
        {
            float rotationX = 90f * Mathf.Sign(moveZ);
            targetRotation = Quaternion.Euler(rotationX, 0, 0);
        }

        float elapsedTime = 0;

        while (elapsedTime < moveDuration)
        {
            transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / moveDuration);
            box.transform.position = Vector3.Lerp(box.transform.position, boxTargetPosition, elapsedTime / moveDuration);
            transform.rotation = Quaternion.Slerp(startRotation, targetRotation, elapsedTime / moveDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPosition;
        box.transform.position = boxTargetPosition;
        transform.rotation = targetRotation;

        yield return new WaitForSeconds(rotationResetDelay);

        transform.rotation = Quaternion.Euler(0, 0, 0);

        yield return new WaitForSeconds(moveDelay);

        isMoving = false;
    }
}
