using TextCopy;


bool debug = true;
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
    string message = @$"Account Info:
    {url + a.AccountNumber}
    -----------------------------------------
    NAME----------{a.FirstName} {a.LastName}
    ADDRESS-------{a.Address}
    INSTALL TIME--{a.InstallTime}
    SUBSCRIPTION--{a.Subsciption}";

    Console.Clear();
    Console.WriteLine(message);

    string text = a.PhoneNumber;
    if (!debug) // not allowed to do threads in github codespaces #########DISABLE FOR PRODUCTION##########
    {
        await ClipboardService.SetTextAsync(text);
    }
    else
    {
        Console.WriteLine($"Output copied to clipboard: {text}");
    }

    Console.WriteLine(@"Resolution:
    (1) Confirmed
    (2) Voicemail");
    int choice = getChoice(1, 2);

#region Format account note
    text = @"ISSUE: PRE-CALL
	
ACTION: 
*   Vetro ID verified & called the customer
*   ";

    switch (choice)
    {
        case 1:
            text += "Confirmed ";
            break;
        case 2:
            text += "Left voicemail informing of";
            break;
        default:
            text += "Informed the customer of the";
            break;
    }

    text += $@"installation details:
*   {a.Address}
*   {a.InstallTime}
*   {a.Subsciption}

RESULT: Pending Installation";
#endregion

    Thread.Sleep(1000);
    Console.WriteLine("\nPress Enter to continue");
    Console.ReadLine();
}

Console.WriteLine("Press Enter to exit");
Console.ReadLine();

int getChoice(int choice1, int choice2)
{
    int choice;
    bool invalidChoice = true;
    do
    {
        choice = int.Parse(Console.ReadLine() ?? $"-99999999");
        if (choice == choice1 || choice == choice2)
        {
            invalidChoice = false;
        }
        else
        {
            Console.WriteLine("Invalide choice: Please try again");
            Thread.Sleep(1000);

            // clear last message
            Console.SetCursorPosition(0, 11);
            Console.WriteLine(new string(' ', 100));
            Console.Write(new string(' ', 33));
            Console.SetCursorPosition(0, 11);
        }
    } while (invalidChoice);
    return choice;
}