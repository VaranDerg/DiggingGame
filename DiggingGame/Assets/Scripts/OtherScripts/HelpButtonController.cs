/*****************************************************************************
// File Name :         HelpButtonController.cs
// Author :            Rudy Wolfer
// Creation Date :     November 21st, 2022
//
// Brief Description : Script to control the Help Menu and given Popups.
*****************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HelpButtonController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator _anims;
    [SerializeField] private TextMeshProUGUI _titleText;
    [SerializeField] private TextMeshProUGUI _descText;
    [SerializeField] private GameObject _helpPopup;

    [Header("Values")]
    [SerializeField] private float _animWaitTime;
    private bool _helpShowing;

    /// <summary>
    /// Disables if the option is set to false.
    /// </summary>
    private void Start()
    {
        _helpPopup.SetActive(false);

        if(!MultiSceneData.s_HelpEnabled)
        {
            gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Shows help.
    /// </summary>
    /// <param name="name">Name of the popup</param>
    /// <param name="desc">Description of the popup.</param>
    public void ShowHelp(string name, string desc)
    {
        if(_helpShowing)
        {
            _titleText.text = name;
            _descText.text = desc;
            return;
        }

        _helpShowing = true;
        _helpPopup.SetActive(true);
        _titleText.text = name;
        _descText.text = desc;
        _anims.Play("HelpShow");
    }

    /// <summary>
    /// Hides help; wrapper for UI.
    /// </summary>
    public void HideHelpWrapper()
    {
        if(!_helpShowing)
        {
            return;
        }

        StartCoroutine(HideHelpPopup());
    }

    /// <summary>
    /// Hides the Help Popup.
    /// </summary>
    private IEnumerator HideHelpPopup()
    {
        _anims.Play("HelpHide");
        yield return new WaitForSeconds(_animWaitTime);
        _helpPopup.SetActive(false); 
        _helpShowing = false;
    }
}
