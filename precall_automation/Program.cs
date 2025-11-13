using System.Diagnostics;

bool debug = true;
Console.WriteLine("Enter your name");
string agent = debug ? "Raiderish" : Console.ReadLine() ?? "No Name";

// Process Excel Data
Console.WriteLine("Enter Excel data to process\nPress esc to exit");
List<UserAccount> data = new List<UserAccount>();
bool readAgain = true;
do
{
    string input = Console.ReadLine() ?? "";
    if (input != "")
    {
        try
        {
            UserAccount account = new UserAccount(input.Split("\t"), agent);
            data.Add(account);
        }
        catch (Exception e)
        {
            Console.WriteLine("Oopsie Poopsie");
        }
    }
    else
    {
        readAgain = false;
    }
} while (readAgain);


string url = "https://app.gaiia.com/iq-fiber/accounts/";
string chrome = @"C:\Program Files\Google\Chrome\Application\chrome.exe";
for(int i = 0; i < data.Count(); i++)
{
    Process.Start(chrome, $"--net-tab {url + data[i].AccountNumber}");
}


Console.WriteLine("Data Entered:");
foreach (UserAccount u in data)
{
    Console.WriteLine(u);
}

