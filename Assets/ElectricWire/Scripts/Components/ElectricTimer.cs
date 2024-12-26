
//(c8

using System;
using UnityEngine;

namespace ElectricWire
{
    [Serializable]
    public class ElectricTimerJsonData
    {
        public float delay;
        public float nextTime;

        public ElectricTimerJsonData(float newDelay, float newNextTime)
        {
            delay = newDelay;
            nextTime = newNextTime;
        }
    }
    public class ElectricTimer : ElectricComponent, ISaveJsonData
    {
        public Transform needle;
        public Transform setNeedle;

        // Timing
        public float delay = 5f;
        private float nextTime = 0f;

        private void Start()
        {
            // Check to be sure delay is valid
            if (delay == 0 || delay >= 60)
                delay = 1;
            // Set the needle position
            SyncSetNeedleOnReset();
        }

        #region ISaveJsonData interface

        public string GetJsonData()
        {
            string jsonData = JsonUtility.ToJson(new ElectricTimerJsonData(delay, nextTime));
            return jsonData;
        }

        public void SetupFromJsonData(string jsonData)
        {
            ElectricTimerJsonData electricTimerJsonData = JsonUtility.FromJson<ElectricTimerJsonData>(jsonData);
            if (electricTimerJsonData == null)
            {
                Debug.LogWarning("No json data found for: " + name + ". Resave could fix that.");
                return;
            }

            delay = electricTimerJsonData.delay;
            SyncSetNeedleOnReset();
            nextTime = electricTimerJsonData.nextTime;
            SyncNeedleOnReset();
        }

        #endregion

        private void Update()
        {
            if (IsEnergized() && IsOn())
            {
                nextTime += Time.deltaTime;

                needle.Rotate(0f, 0f, Time.deltaTime * 6);

                if (delay <= nextTime)
                {
                    GetSetIsOn = false;
                    nextTime = 0;
                    SyncNeedleOnReset();
                    ActivateOutput();
                }
            }
        }

        public void SyncNeedleOnReset()
        {
            needle.localRotation = Quaternion.identity;
            needle.Rotate(0f, 0f, nextTime * 6);
        }

        public void SyncSetNeedleOnReset()
        {
            setNeedle.localRotation = Quaternion.identity;
            setNeedle.Rotate(0f, 0f, delay * 6);
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

            // The first input control the energized state
            if (isInput && index == 0)
            {
                GetSetIsEnergized = false;
                ActivateOutput();
            }
        }

        public override void EnergizeByWire(bool onOff, int index)
        {
            // The first input is use as energie for the timer and to energize other component
            // The second input is use to toggle on the timer
            if (index == 0)
            {
                GetSetIsEnergized = onOff;
                ActivateOutput();
            }
            else if (index == 1 && IsEnergized() && !IsOn())
            {
                GetSetIsOn = onOff;
                if (onOff)
                    ActivateOutput();
            }
        }

        #endregion

        private void OnMouseDown()
        {
            if (ElectricManager.electricManager.CanTriggerComponent())
            {
                delay++;
                if (delay >= 60)
                    delay = 1;

                SyncSetNeedleOnReset();
            }
        }
    }
}
