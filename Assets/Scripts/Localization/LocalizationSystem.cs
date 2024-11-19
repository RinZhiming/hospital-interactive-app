using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Language
{
    Thai,
    English,
}

public static class LocalizationSystem
{
    public static Language LocalizationLanguage { get; set; }
}
