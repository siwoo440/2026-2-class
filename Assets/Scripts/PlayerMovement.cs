using UnityEngine;

// 화면 기준으로 이동하고, 이동 입력과 독립적으로 마우스 커서를 바라봅니다.
public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float rotateSpeed = 180f;

    private PlayerInput playerInput;
    private Rigidbody playerRigidbody;
    private Animator playerAnimator;
    private Camera gameplayCamera;

    private void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        playerRigidbody = GetComponent<Rigidbody>();
        playerAnimator = GetComponent<Animator>();
        gameplayCamera = Camera.main;
    }

    private void FixedUpdate()
    {
        Move();
        FaceMouse();
    }

    private void Move()
    {
        // 기존 Horizontal(rotate)과 Vertical(move) 입력을 화면의 가로·세로로 사용합니다.
        Vector2 input = Vector2.ClampMagnitude(new Vector2(playerInput.rotate, playerInput.move), 1f);
        playerAnimator.SetFloat("Move", input.magnitude);
        if (input.sqrMagnitude == 0f)
        {
            return;
        }

        Vector3 right = gameplayCamera != null
            ? Vector3.ProjectOnPlane(gameplayCamera.transform.right, Vector3.up).normalized
            : Vector3.right;
        Vector3 forward = Vector3.Cross(right, Vector3.up);
        Vector3 direction = right * input.x + forward * input.y;

        playerRigidbody.MovePosition(playerRigidbody.position + direction * moveSpeed * Time.fixedDeltaTime);
    }

    private void FaceMouse()
    {
        if (gameplayCamera == null)
        {
            return;
        }

        // 몸 중심 높이의 수평면에서 커서 위치를 구하므로 별도 콜라이더가 필요하지 않습니다.
        Vector3 center = playerRigidbody.worldCenterOfMass;
        Ray ray = gameplayCamera.ScreenPointToRay(Input.mousePosition);
        Plane aimPlane = new Plane(Vector3.up, center);
        if (!aimPlane.Raycast(ray, out float distance))
        {
            return;
        }

        Vector3 direction = ray.GetPoint(distance) - center;
        direction.y = 0f;
        // 커서가 몸 중심에 겹치면 현재 방향을 유지합니다.
        if (direction.sqrMagnitude < 0.0025f)
        {
            return;
        }

        Quaternion facing = Quaternion.LookRotation(direction, Vector3.up);
        playerRigidbody.MoveRotation(Quaternion.RotateTowards(
            playerRigidbody.rotation, facing, rotateSpeed * Time.fixedDeltaTime));
    }
}
