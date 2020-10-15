using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class PlayerNameInput : MonoBehaviour {
  [Header("UI")]
  [SerializeField] private TMP_InputField nameInputField = null;
  [SerializeField] private Button ConfirmButton = null;

  public static string DisplayName { get; private set; }
  private const string PlayerPrefsNameKey = "PlayerName";




  // Start is called before the first frame update
  void Start() => SetUpInputFeild();

  private void SetUpInputFeild() {
    if (!PlayerPrefs.HasKey(PlayerPrefsNameKey)) { return; }

    string defaultName = PlayerPrefs.GetString(PlayerPrefsNameKey);

    nameInputField.text = defaultName;

    SetPlayerName(defaultName);
  }

  public void SetPlayerName(string name) {
    ConfirmButton.interactable = !string.IsNullOrEmpty(name);
  }

  public void SavePlayerName() {
    DisplayName = nameInputField.text;

    PlayerPrefs.SetString(PlayerPrefsNameKey, DisplayName);
  }

  // Update is called once per frame
  void Update() {

  }
}
