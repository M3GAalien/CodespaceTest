public class Account
{
    public int AccountNumber;
    public string FirstName;
    public string LastName;
    public string PhoneNumber;
    public string Subsciption;
    public string Address;
    public string InstallTime;

    public Account(string[] accountInfo)
    {
        AccountNumber = int.Parse(accountInfo[0]);
        FirstName = accountInfo[1];
        LastName = accountInfo[2];
        PhoneNumber = accountInfo[3];
        Subsciption = accountInfo[4];
        Address = accountInfo[5];
        InstallTime = reformatInstallTime(accountInfo[6]);
    }

    public override string ToString()
    {
        return $"{AccountNumber} {FirstName} {LastName} {PhoneNumber} {Subsciption} {Address} {InstallTime}";
    }

    private string reformatInstallTime(string installTime)
    {
        DateTime start = DateTime.Parse(installTime);
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