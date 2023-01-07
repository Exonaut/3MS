using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class WeaponSway : MonoBehaviour
{
    [FoldoutGroup("Dependencies", expanded: true)]
    [FoldoutGroup("Dependencies")][SerializeField] PlayerMovementController playerController;

    [FoldoutGroup("Settings", expanded: true)]
    [FoldoutGroup("Settings")][SerializeField][Range(0f, 0.5f)] float swayMagnitude = .01f;
    [FoldoutGroup("Settings")][SerializeField][Range(0f, 0.5f)] float swaySpeed = .01f;
    [FoldoutGroup("Settings")][SerializeField] float swingMagnitude = 10f;
    [FoldoutGroup("Settings")][SerializeField] float swingSpeed = 2f;

    float rotateAmount = 0f;

    // Update is called once per frame
    void Update()
    {
        // Sway
        float offsetX = Mathf.PerlinNoise(Time.time * swaySpeed, 0) - 0.5f;
        float offsetY = Mathf.PerlinNoise(0, Time.time * swaySpeed) - 0.5f;
        transform.localPosition = new Vector3(offsetX, offsetY, 0) * swayMagnitude;

        // Swing
        Vector3 velocity = transform.InverseTransformDirection(playerController.prevSpeed.normalized);
        Quaternion targetRotation = Quaternion.Euler(velocity.y * swingMagnitude, -velocity.x * swingMagnitude, 0f);
        transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, Time.deltaTime * swingSpeed);
    }
}
