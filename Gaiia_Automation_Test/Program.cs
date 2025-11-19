using TextCopy;


bool debug = true;// not allowed to do threads in github codespaces #########SET FALSE BEFORE BUILDING##########
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
    // Display info to console
    Console.Clear();
    Console.WriteLine($"PROGRESS: {accounts.IndexOf(a) + 1} of {accounts.Count()}\n");
    string message = @$"Account Info:
    #############################################
    {url + a.AccountNumber}
    #############################################

    NAME----------{a.FirstName} {a.LastName}
    ADDRESS-------{a.Address}
    INSTALL TIME--{a.InstallTime}
    SUBSCRIPTION--{a.Subsciption}";
    Console.WriteLine(message);

    // Copy number to clipboard to paste in NICE
    string text = a.PhoneNumber;
    if (debug){
        Console.WriteLine($"Output copied to clipboard:\n{text}");
    } else {
        await ClipboardService.SetTextAsync(text);
    }

    // Copy note to leave in Gaiia account
    Console.WriteLine("\nResolution:\n\t(1) Confirmed\n\t(2) Voicemail");
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
            text += "Left voicemail informing of ";
            break;
        default:
            text += "Informed the customer of the ";
            break;
    }

    text += $@"installation details:
*   {a.Address}
*   {a.InstallTime}
*   {a.Subsciption}

RESULT: Pending Installation";
    #endregion

    if (debug)
    {
        Console.WriteLine($"Output copied to clipboard:\n{text}");
    }
    else
    {
        await ClipboardService.SetTextAsync(text);
    }

    Thread.Sleep(1000);
    Console.WriteLine("\nPress any key to continue");
    Console.ReadKey();
}
Console.WriteLine("\n\nALL DONE: YIPEEE");
Thread.Sleep(1000);
Console.WriteLine("Press any key to exit");
Console.ReadKey();



int getChoice(int choice1, int choice2)
{
    int choice = -1;
    bool invalidChoice = true;
    do
    {
        try
        {
            choice = int.Parse(Console.ReadKey(true).KeyChar.ToString() ?? "-1");
        }catch(Exception e)
        {
            Console.WriteLine("Whoopsie Tootsie: " + e.Message);
        }
        if (choice == choice1 || choice == choice2)
        {
            invalidChoice = false;
        }
        else
        {
            Console.WriteLine("Invalide choice: Please try again");
            Thread.Sleep(1000);

            // clear last message
            Console.SetCursorPosition(0, 14);
            Console.WriteLine(new string(' ', 100));
            Console.SetCursorPosition(0, 14);
        }
    } while (invalidChoice);
    return choice;
}