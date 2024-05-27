using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SaveSlot : MonoBehaviour
{
    private Button button;
    private TextMeshProUGUI buttonText;

    public int slotNumber;

    public GameObject alertUI;
    Button yesButton;
    Button noButton;

    private void Awake()
    {
        button = GetComponent<Button>();
        buttonText = transform.GetChild(0).GetComponent<TextMeshProUGUI>();

        yesButton = alertUI.transform.Find("YesButton").GetComponent<Button>();
        noButton = alertUI.transform.Find("NoButton").GetComponent<Button>();
    }

    private void Start()
    {
        button.onClick.AddListener(() =>
        {
            if (SaveManager.Instance.IsSlotEmpty(slotNumber))
            {
                SaveGameConfirmed();
            }
            else
            {
                DisplayOverwriteAlert();
            }
        });
    }

    private void Update()
    {
        if (SaveManager.Instance.IsSlotEmpty(slotNumber)) {
            buttonText.text = "Empty";
        }
        else
        {
            buttonText.text = PlayerPrefs.GetString("Slot" + slotNumber + "Description");
        }
    }

    public void DisplayOverwriteAlert()
    {
        alertUI.SetActive(true);

        yesButton.onClick.AddListener(() =>
        {
            SaveGameConfirmed();
            alertUI.SetActive(false);
        });

        noButton.onClick.AddListener(() =>
        {
            alertUI.SetActive(false);
        });
    }

    private void SaveGameConfirmed()
    {
        SaveManager.Instance.SaveGame(slotNumber);

        DateTime dt = DateTime.Now;
        string time = dt.ToString("yyyy-MM-dd HH:mm");

        string description = "Saved Game " + slotNumber + " | " + time;

        buttonText.text = description;
        PlayerPrefs.SetString("Slot" + slotNumber + "Description", description);
        PlayerPrefs.Save();

        SaveManager.Instance.DeselectButton();
    }
}
