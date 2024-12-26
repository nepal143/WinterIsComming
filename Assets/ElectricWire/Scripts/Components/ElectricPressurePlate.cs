
//(c8

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ElectricWire
{
    [Serializable]
    public class ElectricPressurePlateJsonData
    {
        public float delay;

        public ElectricPressurePlateJsonData(float newDelay)
        {
            delay = newDelay;
        }
    }

    public class ElectricPressurePlate : ElectricComponent, ISaveJsonData
    {
        public float delay = 5f;

        private float maxDelay = 10f;

        private List<string> playersInsideZone = new List<string>();

        #region ISaveJsonData interface

        public string GetJsonData()
        {
            string jsonData = JsonUtility.ToJson(new ElectricPressurePlateJsonData(delay));
            return jsonData;
        }

        public void SetupFromJsonData(string jsonData)
        {
            ElectricPressurePlateJsonData electricPressurePlateJsonData = JsonUtility.FromJson<ElectricPressurePlateJsonData>(jsonData);
            if (electricPressurePlateJsonData == null)
            {
                Debug.LogWarning("No json data found for: " + name + ". Resave could fix that.");
                return;
            }

            delay = electricPressurePlateJsonData.delay;
        }

        #endregion

        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponentInParent<AvatarControlRigidBody>() != null)
            {
                if (IsEnergized())
                {
                    // Dont turn off after time
                    StopAllCoroutines();

                    if (!playersInsideZone.Contains(other.name))
                        playersInsideZone.Add(other.name);
                }
            }

            if (playersInsideZone.Count > 0)
                TryTurnOnOff(true);
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.GetComponentInParent<AvatarControlRigidBody>() != null)
            {
                if (playersInsideZone.Contains(other.name))
                    playersInsideZone.Remove(other.name);
            }

            if (playersInsideZone.Count < 1)
            {
                StopAllCoroutines();
                // Start turn off after time
                StartCoroutine(nameof(ReturnOffAfter), delay);
            }
        }

        private IEnumerator ReturnOffAfter(float secs)
        {
            yield return new WaitForSeconds(secs);

            TryTurnOnOff(false);
        }

        public void TryTurnOnOff(bool newIsOn)
        {
            if (newIsOn)
            {
                if (!IsOn())
                    TurningOnOff(newIsOn);
            }
            else if (!newIsOn)
            {
                if (IsOn())
                    TurningOnOff(newIsOn);
            }
        }

        public void TurningOnOff(bool newIsOn)
        {
            GetSetIsOn = newIsOn;
            ActivateOutput();
        }

        #region IWire interface

        public override void ConnectWire(GameObject wire, bool isInput, int index)
        {
            base.ConnectWire(wire, isInput, index);

            ActivateOutput();
        }

        public override void DisconnectWire(bool isInput, int index)
        {
            base.DisconnectWire(isInput, index);

            if (isInput)
            {
                GetSetIsEnergized = false;
                ActivateOutput();
            }
        }

        public override void EnergizeByWire(bool onOff, int index)
        {
            base.EnergizeByWire(onOff, index);

            ActivateOutput();
        }

        #endregion

        private void OnMouseDown()
        {
            if (ElectricManager.electricManager.CanTriggerComponent())
            {
                delay++;
                if (delay >= maxDelay)
                    delay = 1;
            }
        }
    }
}
