using System.Text;
using System.Reflection;
using TextCopy;



bool debug = true; // SET FALSE BEFORE BUILDING - not allowed to do threads in github codespaces 
bool slowMode = true; // add timed delays for *asthetic* reasons
int delay = 500; // delay to add in miliseconds

#region intro
Console.WriteLine("this program does blah blah blah...");
Console.Write("your name pls: ");
string agent = debug ? "Michael A" : Console.ReadLine() ?? "NO NAME";
#endregion

#region get info from Excel
if (slowMode) Thread.Sleep(delay);
Console.WriteLine("\nDrop the Excel pls good sir then press Enter to continue");

string input;
List<Account> accounts = new List<Account>();
do
{
    input = Console.ReadLine() ?? "";
    try
    {
        accounts.Add(new Account(input.Split("\t"), agent));
    }
    catch (Exception e)
    {
        if (e.Message != "The input string '' was not in a correct format.")
        {
            Console.WriteLine("Oopsie Poopsies: " + e.Message);
        }
    }
} while (input != "");
#endregion

var client = new HttpClient();
string url = @"https://app.gaiia.com/iq-fiber/accounts/";

foreach (Account a in accounts)
{
    #region Display info to console
    Console.WriteLine("Getting account....");
    if (slowMode) Thread.Sleep(delay);
    Console.Clear();
    Console.WriteLine($"PROGRESS: {accounts.IndexOf(a) + 1} of {accounts.Count()}\n");
    string message = @$"Account Info:
    #############################################
    {url + a.AccountNumber}
    #############################################

    NAME----------{a.FirstName} {a.LastName}
    ADDRESS-------{a.Address}
    INSTALL TIME--{a.reformatedInstallTime()}
    SUBSCRIPTION--{a.Subsciption}";
    Console.WriteLine(message);
    #endregion

    #region Copy number to clipboard to paste in NICE
    if (slowMode) Thread.Sleep(delay);
    string text = a.PhoneNumber;

    if (string.IsNullOrWhiteSpace(text))
    {
        Console.WriteLine("\nError getting phone number." +
                          "\nPlease use Gaiia to get the phone number");
    }
    else
    {

        if (debug)
        {
            Console.WriteLine($"\n{text}");
        }
        else
        {
            await ClipboardService.SetTextAsync(text);
            Console.WriteLine("\nPhone Number copied to clipboard!");
        }
    }

    #endregion

    #region Copy note to leave in Gaiia account
    if (slowMode) Thread.Sleep(delay);
    Console.WriteLine("\nResolution:\n   (1) Confirmed\n   (2) Voicemail");
    a.Resolution = getChoice(1, 2) == 1 ? "Confirmed" : "Voicemail";

    #region Format account note
    text = @"ISSUE: PRE-CALL
	
ACTION: 
*   Vetro ID verified & called the customer
*   ";

    switch (a.Resolution)
    {
        case "Confirmed":
            text += "Confirmed ";
            break;
        case "Voicemail":
            text += "Left voicemail informing of the ";
            break;
        default:
            text += "Informed the customer of the ";
            break;
    }

    text += $@"installation details:
*   {a.Address}
*   {a.reformatedInstallTime()}
*   {a.Subsciption}

RESULT: Pending Installation";
    #endregion

    if (debug)
    {
        Console.WriteLine($"\n{text}");
    }
    else
    {
        await ClipboardService.SetTextAsync(text);
    }
    Console.WriteLine("\nGaiia note copied to clipboard!");
    #endregion

    if (slowMode) Thread.Sleep(delay);
    Console.WriteLine("\nPress ENTER to continue");
    Console.ReadLine();
}

#region Copy results to leave in Excel
Console.Clear();
Console.WriteLine("Getting results for Excel....");

if (slowMode) Thread.Sleep(delay);
StringBuilder excelOuput = new StringBuilder();
foreach (Account a in accounts)
{
    string output = string.Join("\t",
    a.GetType()
    .GetFields(BindingFlags.Public | BindingFlags.Instance)
    .Select(x => x.GetValue(a)?.ToString())) + "\n";
    excelOuput.Append(output);
}
if (debug)
{
    Console.WriteLine(excelOuput);
}
else
{
    await ClipboardService.SetTextAsync(excelOuput.ToString());
}
Console.WriteLine("\nExcel output copied to clipboad.");
#endregion

if (slowMode) Thread.Sleep(delay);
Console.WriteLine("\n\nALL DONE: YIPEEE");
if (slowMode) Thread.Sleep(delay);
Console.WriteLine("Press ENTER to exit");
Console.ReadLine();




int getChoice(int choice1, int choice2)
{
    int choice = -1;
    bool invalidChoice = true;

    do
    {

        try
        {
            choice = int.Parse(Console.ReadLine() ?? "-1");
        }
        catch (Exception e)
        {
            Console.WriteLine("Whoopsie Tootsie: " + e.Message);
            choice = -99;
        }

        if (choice == choice1 || choice == choice2)
        {
            invalidChoice = false;
        }
        else
        {
            Console.WriteLine("Invalide choice: Please try again");
            if (slowMode) Thread.Sleep(delay);

            int linesToErase = choice == -99 ? 2 : 1;
            int errorStart = Console.CursorTop - linesToErase - 1;
            Console.SetCursorPosition(0, errorStart);

            for (int i = 0; i <= linesToErase; i++)
            {
                Console.WriteLine(new string(' ', Console.WindowWidth));
            }

            Console.SetCursorPosition(0, errorStart);

        }
    } while (invalidChoice);
    return choice;
}