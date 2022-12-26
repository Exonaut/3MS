using UnityEngine;
using Sirenix.OdinInspector;
using System;
using Object = UnityEngine.Object;

namespace Exo.Sensors
{
    [RequireComponent(typeof(Collider))]
    public class Sensor : MonoBehaviour
    {
        [FoldoutGroup("Masks", true)][SerializeField] LayerMask sensorMask;

        [Hookable] public event Action<Object> onSensorEnter;
        [Hookable] public event Action<Collider> onSensorEnterCollider;
        private void OnTriggerEnter(Collider other)
        {
            if (sensorMask != (sensorMask | ( 1 << other.gameObject.layer))) return;
            onSensorEnter?.Invoke(other);
            onSensorEnterCollider?.Invoke(other);
        }

        [Hookable] public event Action<Object> onSensorExit;
        [Hookable] public event Action<Collider> onSensorExitCollider;
        private void OnTriggerExit(Collider other)
        {
            if (sensorMask != (sensorMask | (1 << other.gameObject.layer))) return;
            onSensorExit?.Invoke(other);
            onSensorExitCollider?.Invoke(other);
        }
    }
}
