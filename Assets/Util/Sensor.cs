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

        [Hookable] public event Action<Collider> onSensorEnter;
        private void OnTriggerEnter(Collider other)
        {
            if (sensorMask != (sensorMask | ( 1 << other.gameObject.layer))) return;
            onSensorEnter?.Invoke(other);
        }

        [Hookable] public event Action<Collider> onSensorExit;
        private void OnTriggerExit(Collider other)
        {
            if (sensorMask != (sensorMask | (1 << other.gameObject.layer))) return;
            onSensorExit?.Invoke(other);
        }
    }
}
