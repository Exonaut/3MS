using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Interaction
{
    public class Interactor : MonoBehaviour
    {
        [FoldoutGroup("Dependencies", expanded: true)]
        [FoldoutGroup("Dependencies")][SerializeField, Required] protected Logger logger;
    }
}
