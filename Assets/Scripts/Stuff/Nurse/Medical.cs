using System;

[Serializable]
public class Medical
{
    public string name;
    public int number;
    public float cost;
}

[Serializable]
public class MedicalSummary
{
    public string patientId;
    public string patientName;
    public string doctorName;
    public string doctorId;
    public string dateTime;
    public string time;
    public float totalCost;
    public Medical[] medicals;
}

[Serializable]
public class MedicalSummaryClone : IComparable<MedicalSummaryClone>
{
    public string patientId;
    public string patientName;
    public string doctorName;
    public string doctorId;
    public DateTime dateTime;
    public TimeSpan startTime;
    public TimeSpan endTime;
    public float totalCost;
    public Medical[] medicals;


    public int CompareTo(MedicalSummaryClone other)
    {
        if (ReferenceEquals(this, other)) return 0;
        if (ReferenceEquals(null, other)) return 1;
        var dateTimeComparison = dateTime.CompareTo(other.dateTime);
        if (dateTimeComparison != 0) return dateTimeComparison;
        var startTimeComparison = startTime.CompareTo(other.startTime);
        if (startTimeComparison != 0) return startTimeComparison;
        return endTime.CompareTo(other.endTime);
    }
}