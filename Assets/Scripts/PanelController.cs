using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Qualcomm.Snapdragon.Spaces;

public class PanelController : MonoBehaviour
{
    public bool FollowGaze = true;
    public float TargetDistance = 1.0f;
    public float MovementSmoothness = 0.2f;

    private Camera _mainCamera;
    private void Start()
    {
        _mainCamera = Camera.main;
    }

    private void Update()
    {
        if (FollowGaze)
        {
            AdjustPanelPosition();
        }
    }

    public void SwitchToScene(string SceneName)
    {
        SceneManager.LoadScene(SceneName);
    }


    /* Ajdusts the position of the Panel if the gaze moves outside of the inner rectangle of the FOV,
     * which is half the length in both axis.
     */
    private void AdjustPanelPosition()
    {
        var headPosition = _mainCamera.transform.position;
        var gazeDirection = _mainCamera.transform.forward;

        var direction = (transform.position - headPosition).normalized;
        var targetPosition = headPosition + gazeDirection * TargetDistance;
        var targetDirection = (targetPosition - headPosition).normalized;

        var eulerAngles = Quaternion.LookRotation(direction).eulerAngles;
        var targetEulerAngles = Quaternion.LookRotation(targetDirection).eulerAngles;

        var verticalHalfAngle = _mainCamera.fieldOfView * 0.8f;
        eulerAngles.x += GetAdjustedDelta(targetEulerAngles.x - eulerAngles.x, verticalHalfAngle);

        var horizontalHalfAngle = _mainCamera.fieldOfView * 0.5f * _mainCamera.aspect;
        eulerAngles.y += GetAdjustedDelta(targetEulerAngles.y - eulerAngles.y, horizontalHalfAngle);

        targetPosition = headPosition + Quaternion.Euler(eulerAngles) * Vector3.forward * TargetDistance;

        transform.position = Vector3.Lerp(transform.position, targetPosition, MovementSmoothness);
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(transform.position - headPosition), MovementSmoothness);
    }

    /* Returns the normalized delta to a certain threshold, if it exceeds that threshold. Otherwise return 0. */
    private float GetAdjustedDelta(float angle, float threshold)
    {
        /* Normalize angle to be between 0 and 360. */
        angle = (540f + angle) % 360f - 180f;
        if (Mathf.Abs(angle) > threshold)
        {
            return -angle / Mathf.Abs(angle) * (threshold - Mathf.Abs(angle));
        }
        else
        {
            return 0f;
        }
    }
}
