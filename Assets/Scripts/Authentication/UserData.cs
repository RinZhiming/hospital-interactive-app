using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserData
{
    public string username;
    public int callname;
    public string firstname;
    public string lastname;
    public string birthday;
    public int age;
    public int iconProfile = -1;
}

public class UserVerify
{
    public bool isCardVerify;
    public bool isFaceVerify;
    public bool isPatientVerify;
}

public class UserDevice
{
    public string device;
}