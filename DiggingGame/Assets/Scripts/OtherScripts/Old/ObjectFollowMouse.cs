using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectFollowMouse : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject _objFollowing;
    [SerializeField] private GameObject _objToFollow;

    [Header("Values")]
    [SerializeField] private bool _followMouse;
    [SerializeField] private float _objMoveSpeed;
    [SerializeField] private float _objMoveDist;

    /// <summary>
    /// Updates the camera's position and calculates the mouse position.
    /// </summary>
    private void FixedUpdate()
    {
        if(_followMouse)
        {
            MoveCameraTowards(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        }
        else
        {
            MoveCameraTowards(_objToFollow.transform.position);
        }
    }

    /// <summary>
    /// Moves the camera towards the target.
    /// </summary>
    /// <param name="targetPos">Target's position.</param>
    private void MoveCameraTowards(Vector3 targetPos)
    {
        _objFollowing.transform.position = Vector2.Lerp(_objFollowing.transform.position, targetPos, _objMoveSpeed * Time.deltaTime);
        float clampX = Mathf.Clamp(_objFollowing.transform.position.x, -_objMoveDist, _objMoveDist);
        float clampY = Mathf.Clamp(_objFollowing.transform.position.y, -_objMoveDist, _objMoveDist);
        _objFollowing.transform.position = new Vector3(clampX, clampY, -10);
    }
}
