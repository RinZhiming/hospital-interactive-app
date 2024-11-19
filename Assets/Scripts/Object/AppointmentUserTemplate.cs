using System;

[Serializable]
public class AppointmentUser
{
    public string doctorId;
    public bool isExamination;
    public string date;
    public string time;
}

[Serializable]
public class AppointmentUserClone
{
    public string doctorId;
    public bool isExamination;
    public DateTime date;
    public TimeSpan startTime;
    public TimeSpan endTime;
    public string key;
}