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
        InstallTime = accountInfo[6];
    }

    public override string ToString()
    {
        return $"{AccountNumber} {FirstName} {LastName} {PhoneNumber} {Subsciption} {Address} {InstallTime}";
    }
}