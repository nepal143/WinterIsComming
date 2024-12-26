
//(c8

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ElectricWire
{
    public class UILoadMenu : MonoBehaviour
    {
        public static UILoadMenu uiLoadMenu;

        public GameObject fileNameSlotPrefab;
        public GameObject fileNameContent;

        public GameObject panel;

        private List<GameObject> allSlots = new List<GameObject>();

        private void OnDisable()
        {
            ClearPanelList();
        }

        private void Awake()
        {
            if (uiLoadMenu != null && uiLoadMenu != this) Destroy(gameObject); else uiLoadMenu = this;
        }

        public void LoadMenu(string[] fileNames)
        {
            ClearPanelList();
            panel.SetActive(true);

            for (int i = 0; i < fileNames.Length; i++)
            {
                // Remove the extension
                string pathAndFile = fileNames[i];
                string fileName = pathAndFile.Substring(pathAndFile.LastIndexOf("\\") + 1);
                string fileNameNoExt = fileName.Substring(0, fileName.LastIndexOf("."));
                fileNames[i] = pathAndFile.Substring(pathAndFile.LastIndexOf("\\") + 1);
                GameObject newGameObject = Instantiate(fileNameSlotPrefab);
                newGameObject.transform.SetParent(fileNameContent.transform, false);
                newGameObject.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = fileNameNoExt;// pathAndFile.Substring(pathAndFile.LastIndexOf("\\") + 1);
                newGameObject.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(() => { LoadFileName(pathAndFile); });
                newGameObject.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(() => { RemoveFileName(newGameObject, pathAndFile); });
                allSlots.Add(newGameObject);
            }
        }

        private void ClearPanelList()
        {
            for (int i = 0; i < allSlots.Count; i++)
                Destroy(allSlots[i]);
        }

        public void LoadFileName(string fileName)
        {
            ElectricManager.electricManager.SpawnWithJsonData(fileName);
        }

        public void RemoveFileName(GameObject newGameObject, string fileName)
        {
            allSlots.Remove(newGameObject);
            Destroy(newGameObject);
            ElectricManager.electricManager.RemoveSaveFile(fileName);
        }
    }
}
