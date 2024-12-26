
//(c8

using System;
using UnityEngine;

namespace ElectricWire
{
    [Serializable]
    public class ElectricLampJsonData
    {
        public bool isOn;

        public ElectricLampJsonData(bool newIsOn)
        {
            isOn = newIsOn;
        }
    }

    public class ElectricLamp : ElectricComponent, ISaveJsonData
    {
        #region ISaveJsonData interface

        public string GetJsonData()
        {
            string jsonData = JsonUtility.ToJson(new ElectricLampJsonData(GetSetIsOn));
            return jsonData;
        }

        public void SetupFromJsonData(string jsonData)
        {
            ElectricLampJsonData electricLampJsonData = JsonUtility.FromJson<ElectricLampJsonData>(jsonData);
            if (electricLampJsonData == null)
            {
                Debug.LogWarning("No json data found for: " + name + ". Resave could fix that.");
                return;
            }

            GetSetIsOn = electricLampJsonData.isOn;
        }

        #endregion

        #region IWire interface

        public override void DisconnectWire(bool isInput, int index)
        {
            base.DisconnectWire(isInput, index);

            // We unenergize by ourself
            GetSetIsEnergized = false;
        }

        #endregion

        private void OnMouseDown()
        {
            if (ElectricManager.electricManager.CanTriggerComponent())
                GetSetIsOn = !IsOn();
        }
    }
}
