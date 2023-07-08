using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Fakeloading : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI tmpLoading;
    [SerializeField] InventoryField inventoryField;

    private void OnEnable()
    {
        StartCoroutine(IELoadingFX());
        Registry.itemService.GetItemConfig((listItemConfig) =>
        {
            GameData.Instance.GetItemDescriptionDictionary(listItemConfig, () =>
            {
                Invoke(nameof(DelayLoadItems), 2f);
            });

        }, () =>
        {
            Debug.LogError("Get Item Config Failed");
        });
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private IEnumerator IELoadingFX()
    {
        int count = 0;
        int waitTime = 0;
        int randomWait = Random.Range(2, 6);
        while (true)
        {
            int n = count % 4;
            switch (n)
            {
                case 0:
                    tmpLoading.text = "Loading";
                    break;
                case 1:
                    tmpLoading.text = "Loading.";
                    break;
                case 2: tmpLoading.text = "Loading.."; break;
                case 3: tmpLoading.text = "Loading..."; break;
            }
            waitTime++;
            count++;
            yield return new WaitForSeconds(0.2f);
            if (waitTime > randomWait)
            {
                //_imgLoading.fillAmount = 1f;
                break;
            }
        }
    }

    private void DelayLoadItems()
    {
        inventoryField.gameObject.SetActive(true);
        gameObject.SetActive(false);
    }
}
