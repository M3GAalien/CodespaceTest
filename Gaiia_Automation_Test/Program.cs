using TextCopy;

Console.WriteLine("Drop the Excel pls good sir");
Console.WriteLine("Press Enter to continue");

string input;
List<Account> accounts = new List<Account>();
do
{
    input = Console.ReadLine() ?? "";
    try
    {
        accounts.Add(new Account(input.Split("\t")));
    }
    catch (Exception e)
    {
        if (e.Message != "The input string '' was not in a correct format.")
        {
            Console.WriteLine("Oopsie Poopsies: " + e.Message);
        }
    }
} while (input != "");

var client = new HttpClient();

string url = @"https://app.gaiia.com/iq-fiber/accounts/";

foreach (Account a in accounts)
{
    Console.WriteLine($"Navigating to site:\n{url + a.AccountNumber}");
    string text = $"Hello {a.FirstName}'s World!";
    await ClipboardService.SetTextAsync(text);

    Console.WriteLine($"Output copied to clipboard: {text}");
}