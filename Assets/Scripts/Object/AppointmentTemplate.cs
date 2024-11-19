using System;

[Serializable]
public class Appointment
{
    public int sort;
    public string time;
    public string date;
    public string channelName;
    public string doctorId;
    public string patientId;
    public bool hasAppointment;
    public bool hasEnd;
}

[Serializable]
public class AppointmentClone : IComparable<AppointmentClone>
{
    public int sort;
    public DateTime date;
    public TimeSpan startTime;
    public TimeSpan endTime;
    public string channelName;
    public string doctorId;
    public string patientId;
    public bool hasAppointment;
    public bool hasEnd;
    public string key;

    public int CompareTo(AppointmentClone other)
    {
        if (ReferenceEquals(this, other)) return 0;
        if (ReferenceEquals(null, other)) return 1;
        var dateComparison = date.CompareTo(other.date);
        if (dateComparison != 0) return dateComparison;
        return startTime.CompareTo(other.startTime);
    }
}