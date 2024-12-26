
//(c8

using System;
using UnityEngine;

namespace ElectricWire
{
    [Serializable]
    public class ElectricSwitch3WayJsonData
    {
        public bool isOn;

        public ElectricSwitch3WayJsonData(bool newIsOn)
        {
            isOn = newIsOn;
        }
    }

    public class ElectricSwitch3Way : ElectricComponent, ISaveJsonData
    {
        private void Start()
        {
            // When start with already placed prefabs in the scene
            TurningOnOff(IsOn());
        }

        #region ISaveJsonData interface

        public string GetJsonData()
        {
            string jsonData = JsonUtility.ToJson(new ElectricSwitch3WayJsonData(GetSetIsOn));
            return jsonData;
        }

        public void SetupFromJsonData(string jsonData)
        {
            ElectricSwitch3WayJsonData electricSwitch3WayJsonData = JsonUtility.FromJson<ElectricSwitch3WayJsonData>(jsonData);
            if (electricSwitch3WayJsonData == null)
            {
                Debug.LogWarning("No json data found for: " + name + ". Resave could fix that.");
                return;
            }

            TurningOnOff(electricSwitch3WayJsonData.isOn);
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

        public override void ActivateOutput()
        {
            if (IsOn())
            {
                if (wireOutput[0] != null)
                    wireOutput[0].GetComponent<WireControl>().wireConnectorInput.EnergizeByWire(IsEnergized());
                if (wireOutput[1] != null)
                    wireOutput[1].GetComponent<WireControl>().wireConnectorInput.EnergizeByWire(false);
            }
            else
            {
                if (wireOutput[0] != null)
                    wireOutput[0].GetComponent<WireControl>().wireConnectorInput.EnergizeByWire(false);
                if (wireOutput[1] != null)
                    wireOutput[1].GetComponent<WireControl>().wireConnectorInput.EnergizeByWire(IsEnergized());
            }

        }

        private void OnMouseDown()
        {
            if (ElectricManager.electricManager.CanTriggerComponent())
                TurningOnOff(!IsOn());
        }
    }
}
