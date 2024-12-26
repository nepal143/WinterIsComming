
//(c8

using System;
using UnityEngine;

namespace ElectricWire
{
    [Serializable]
    public class ElectricSwitchJsonData
    {
        public bool isOn;

        public ElectricSwitchJsonData(bool newIsOn)
        {
            isOn = newIsOn;
        }
    }

    public class ElectricSwitch : ElectricComponent, ISaveJsonData
    {
        private void Start()
        {
            // When start with already placed prefabs in the scene
            TurningOnOff(IsOn());
        }

        #region ISaveJsonData interface

        public string GetJsonData()
        {
            string jsonData = JsonUtility.ToJson(new ElectricSwitchJsonData(GetSetIsOn));
            return jsonData;
        }

        public void SetupFromJsonData(string jsonData)
        {
            ElectricSwitchJsonData electricSwitchJsonData = JsonUtility.FromJson<ElectricSwitchJsonData>(jsonData);
            if (electricSwitchJsonData == null)
            {
                Debug.LogWarning("No json data found for: " + name + ". Resave could fix that.");
                return;
            }

            TurningOnOff(electricSwitchJsonData.isOn);
        }

        #endregion

        private void TurningOnOff(bool newIsOn)
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
                TurningOnOff(!IsOn());
        }
    }
}
