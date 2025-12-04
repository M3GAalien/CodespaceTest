namespace Gaiia_Automation_Test;

public class Account
{
    public int AccountNumber;
    public string FirstName;
    public string LastName;
    public string PhoneNumber;
    public string Subsciption;
    public string Address;
    public string InstallTime;
    public string Resolution;
    public string CXLorRS;
    public string Agent;
    public string BLANK = ""; // always blank
    public string WellnessCheckResolution = "";
    public string WellnessCheckAgent;
    public string WellnessCheckStatus = "";

    public Account(string[] accountInfo, string agent, string task)
    {
        AccountNumber = int.Parse(accountInfo[0]);
        FirstName = accountInfo[1];
        LastName = accountInfo[2];
        PhoneNumber = accountInfo[3];
        Subsciption = reformatSubscription(accountInfo[4]);
        Address = accountInfo[5];
        InstallTime = accountInfo[6];
        Resolution = task.Contains("precall") ? "" : accountInfo[7];
        CXLorRS = task.Contains("precall") ? "" : accountInfo[8];
        Agent = task.Contains("precall") ? agent : accountInfo[9];
        WellnessCheckAgent = task.Contains("wellness check") ? agent : "";
    }

    public override string ToString()
    {
        return @$"{AccountNumber} {FirstName} {LastName} {PhoneNumber} {Subsciption} 
        {Address}
        {InstallTime} 
        {Resolution} {CXLorRS} 
        {Agent} 
        {WellnessCheckResolution} 
        {WellnessCheckAgent} 
        {WellnessCheckStatus}";
    }

    public string reformatedInstallTime()
    {
        DateTime start = DateTime.Parse(InstallTime);
        DateTime end = start.AddHours(3);

        // Build formatted string
        string dayOfWeek = start.ToString("dddd");
        string month = start.ToString("MMM");
        string day = GetOrdinal(start.Day);
        string timeWindow = start.Hour switch
        {
            < 11 => "8:00am - 11:00am",
            >= 11 and < 14 => "11:00am - 2:00pm",
            >= 14 and < 17 => "2:00pm - 5:00pm",
            _ => "CHECK GAIIA"

        };

        return $"{dayOfWeek} {month}. {day}, {timeWindow}";
    }

    private string GetOrdinal(int day)
    {
        if (day % 10 == 1 && day != 11) return day + "st";
        if (day % 10 == 2 && day != 12) return day + "nd";
        if (day % 10 == 3 && day != 13) return day + "rd";
        return day + "th";
    }

    private string reformatSubscription(string plan)
    {
        if (plan.Contains("250"))
        {
            return "250Mbps. for $65/Month";
        }
        if (plan.Contains("500"))
        {
            return "500Mbps. for $75/Month";
        }
        if (plan.Contains("1"))
        {
            if (plan.ToLower().Contains("pro"))
            {
                return "1 Gig. Pro for $150/Month";
            }
            return "1 Gig. for $85/Month";
        }
        if (plan.Contains("2"))
        {
            return "2 Gig. for $95/Month";
        }
        if (plan.Contains("5"))
        {
            if (plan.ToLower().Contains("pro"))
            {
                return "5 Gig. Pro for $250/Month";
            }
            return "5 Gig. for $125/Month";
        }
        return "";
    }
}