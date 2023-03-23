using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class CreateRoom : MonoBehaviour
{

    private TMP_InputField roomNameField;
    private TMP_InputField passwordField;

    void Start()
    {
        roomNameField = transform.Find("RoomName (field)").GetComponent<TMP_InputField>();
        passwordField = transform.Find("Password (field)").GetComponent<TMP_InputField>();

        roomNameField.characterLimit = 20;
        passwordField.characterLimit = 20;
        roomNameField.onValidateInput = (string text, int charIndex, char addedChar) =>
        {
            return ValidateChar(addedChar);
        };
        passwordField.onValidateInput = (string text, int charIndex, char addedChar) =>
        {
            return ValidateChar(addedChar);
        };
        

    }

    void Update()
    {
        
    }

    private char ValidateChar(char addedChar)
	{
        return char.IsLetterOrDigit(addedChar) ? addedChar : '\0';
	}

}
