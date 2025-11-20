public class Account
{
    public int AccountNumber;
    public string FirstName;
    public string LastName;
    public string PhoneNumber;
    public string Subsciption;
    public string Address;
    public string InstallTime;
    public string? Resolution;
    public string CXLorRS = "";
    public string Agent; 

    public Account(string[] accountInfo, string agent)
    {
        AccountNumber = int.Parse(accountInfo[0]);
        FirstName = accountInfo[1];
        LastName = accountInfo[2];
        PhoneNumber = accountInfo[3];
        Subsciption = accountInfo[4];
        Address = accountInfo[5];
        InstallTime = accountInfo[6];
        Agent = agent;
    }

    public override string ToString()
    {
        return $"{AccountNumber} {FirstName} {LastName} {PhoneNumber} {Subsciption} {Address} {InstallTime}";
    }

    public string reformatedInstallTime()
    {
        DateTime start = DateTime.Parse(InstallTime);
        DateTime end = start.AddHours(3);

        // Build formatted string
        string dayOfWeek = start.ToString("dddd");
        string month = start.ToString("MMM");
        string day = GetOrdinal(start.Day);

        string startTime = start.ToString("h:mmtt").ToLower();
        string endTime = end.ToString("h:mmtt").ToLower();

        return $"{dayOfWeek} {month}. {day}, {startTime} - {endTime}";
    }

    private string GetOrdinal(int day)
    {
        if (day % 10 == 1 && day != 11) return day + "st";
        if (day % 10 == 2 && day != 12) return day + "nd";
        if (day % 10 == 3 && day != 13) return day + "rd";
        return day + "th";
    }
}