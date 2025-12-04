using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum SettingPage
{
    None,
    Menu,
    Key,
    Vidio,
    Sound
}

public class SettingManager : MonoBehaviour
{
    public static SettingManager Instance;

    [Header("Setting Panel")]
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private GameObject keyPanel;
    
    [Header("Setting Button")]
    [SerializeField] private Button keyButton;
    [SerializeField] private Button quitButton;

    public SettingPage currentPage = SettingPage.None;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    private void Start()
    {
        keyButton.onClick.AddListener(OnClick_KeyButton);
        quitButton.onClick.AddListener(OnClick_QuitButton);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (currentPage == SettingPage.None)
            {
                Enter_MenuPage();
            }
            else if (currentPage == SettingPage.Menu)
            {
                Enter_NonePage();
            }
            else if (currentPage == SettingPage.Key)
            {
                Enter_MenuPage();
            }
        }
    }
    
    public void OnClick_KeyButton()
    {
        Debug.Log("Key Button Clicked");
        Enter_KeyPage();
    }

    public void OnClick_QuitButton()
    {
        //GameManager.instance.QuitGame();
    }
    
    public void OnClick_MenuButton()
    {
        Enter_MenuPage();
    }

    private void Enter_NonePage()
    {
        currentPage = SettingPage.None;
        menuPanel.SetActive(false);
        keyPanel.SetActive(false);
    }

    private void Enter_MenuPage()
    {
        currentPage = SettingPage.Menu;
        menuPanel.SetActive(true);
        keyPanel.SetActive(false);
    }

    private void Enter_KeyPage()
    {
        currentPage = SettingPage.Key;
        menuPanel.SetActive(true);
        keyPanel.SetActive(true);
    }


    public bool IsOpenSetting()
    {
        if(currentPage == SettingPage.None)
        {
            return false;
        }
        return true;
    }
}
