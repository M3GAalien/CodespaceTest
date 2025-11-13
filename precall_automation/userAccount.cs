public class UserAccount
{
    public int AccountNumber;
    public string FirstName;
    public string LastName;
    public string PhoneNumber;
    public string Subsciption;
    public string Address;
    public string InstallTime;
    public string Resolution = "";
    public string Agent;

    public UserAccount(string[] fields, string agent)
    {
        AccountNumber   = int.Parse(fields[0]);
        FirstName       = fields[1];
        LastName        = fields[2];
        PhoneNumber     = fields[3];
        Subsciption     = fields[4];
        Address         = fields[5];
        InstallTime     = fields[6];
        Agent           = agent;
    }

    public override string ToString()
    {
        return $"{AccountNumber}\t{FirstName} {LastName} {PhoneNumber} {Subsciption} {Address} {InstallTime} {Agent}";
    }
}