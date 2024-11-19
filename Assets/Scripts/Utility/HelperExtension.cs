using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public static class HelperExtension
{
    public static bool CompareTo(this TMP_InputField value, TMP_InputField input)
    {
        return value.text == input.text;
    }

    public static bool Length(this TMP_InputField value, int count)
    {
        return value.text.Length >= count;
    }

    public static void Clear(this TMP_InputField value)
    {
        value.text = string.Empty;
    }

    public static void Clear(this TMP_Dropdown value)
    {
        value.value = 0;
    }
}
