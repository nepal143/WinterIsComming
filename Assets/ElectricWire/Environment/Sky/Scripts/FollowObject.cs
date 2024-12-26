
//(c8

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ElectricWire
{
    public class FollowObject : MonoBehaviour
    {
        public Transform localPlayer;

        private void Update()
        {
            if (localPlayer != null)
                transform.position = localPlayer.position;
        }
    }
}
