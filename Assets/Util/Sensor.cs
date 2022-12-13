using Exo.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace Exo.Sensors
{
    [RequireComponent(typeof(Collider))]
    public class Sensor : MonoBehaviour
    {
        [FoldoutGroup("Masks", true)][SerializeField] LayerMask sensorMask;

        public readonly HookableEvent onSensorEnter = new HookableEvent("SensorEnter");
        private void OnTriggerEnter(Collider other)
        {
            if (sensorMask != (sensorMask | ( 1 << other.gameObject.layer))) return;
            onSensorEnter.Invoke(other.GetComponent<MonoBehaviour>());
        }

        public readonly HookableEvent onSensorExit = new HookableEvent("SensorExit");
        private void OnTriggerExit(Collider other)
        {
            if (sensorMask != (sensorMask | (1 << other.gameObject.layer))) return;
            onSensorExit.Invoke(other.GetComponent<MonoBehaviour>());
        }
    }
}
