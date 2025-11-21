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
            if (slowMode) Thread.Sleep(1000);
            Console.SetCursorPosition(0, Console.CursorTop - 2);
            Console.WriteLine(new string(' ', Console.WindowWidth));
            Console.WriteLine(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, Console.CursorTop - 2);
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
    string text = @$"Account Info:
    #############################################
    {url + a.AccountNumber}
    #############################################

    NAME----------{a.FirstName} {a.LastName}
    ADDRESS-------{a.Address}
    INSTALL TIME--{a.reformatedInstallTime()}
    SUBSCRIPTION--{a.Subsciption}";
    Console.WriteLine(text);
    #endregion

    #region Copy number to clipboard to paste in NICE
    Console.WriteLine("Getting phone number....");
    if (slowMode) Thread.Sleep(delay);
    text = a.PhoneNumber;

    if (string.IsNullOrWhiteSpace(text))
    {
        Console.WriteLine("\nError getting phone number." +
                          "\nPlease use Gaiia to get the phone number");
    }
    else
    {
        results(debug, slowMode, delay, text);
    }

    #endregion

    #region Copy note to leave in Gaiia account
    if (slowMode) Thread.Sleep(delay);
    Console.WriteLine("\nResolution:\n   (1) Confirmed\n   (2) Voicemail\n   (3) Email");
    a.Resolution = getChoice(3) switch
    {
        1 => "Confirmed",
        2 => "Voicemail",
        3 => "Emailed",
        _ => "ERROR"
    };

    #region Format account note
    Console.WriteLine("Formating note for Gaiia....");
    if (slowMode) Thread.Sleep(delay);
    text = @"ISSUE: PRE-CALL
	
ACTION: 
    Vetro ID verified & called the customer
    ";

    switch (a.Resolution)
    {
        case "Confirmed":
            text += "Confirmed ";
            break;
        case "Voicemail":
            text += "Left voicemail informing of ";
            break;
        case "Emailed":
            text += "Sent an email informing of ";
            break;
        default:
            text += "Informed the customer of ";
            break;
    }

    text += $@"the installation details:
    *   {a.Address}
    *   {a.reformatedInstallTime()}
    *   {a.Subsciption}

RESULT: Pending Installation";
    #endregion

    results(debug, slowMode, delay, text);
    #endregion

    #region Send an email if necessary
    Console.WriteLine("Formatting email....");
    text = @$"Hi {a.FirstName},
Just wanted to confirm the details of your installation 
    Where : {a.Address},
    When  : {a.reformatedInstallTime}.
    Plan  : {a.Subsciption}.

The technicians will call when they are on the way.
Please make sure someone 18 or older is home for the full appointment, any pets are secured, and any gates needed for access are opened.
 
If you need to reschedule or have any questions, feel free to call us at 1-800-495-4775.
We look forward to getting you connected!
 
Best regards,";
    if (slowMode) Thread.Sleep(delay);

    results(debug, slowMode, delay, text);
    #endregion
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



// select a choice from 1 to range
int getChoice(int range)
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

        if (choice > 0 && choice <= range)
        {
            invalidChoice = false;
        }
        else
        {
            Console.WriteLine("Invalide choice: Please try again");
            if (slowMode) Thread.Sleep(1000);

            int linesToErase = choice == -99 ? 2 : 1;
            int errorStart = Console.CursorTop - linesToErase - 1 < 0 ? 0 : Console.CursorTop - linesToErase - 1; //prevents negative value error
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

async void results(bool isInDebugMode, bool isInSlowMode, int delay, string text)
{
    if (isInDebugMode)
    {
        Console.WriteLine($"\n{text}");
    }
    else
    {
        await ClipboardService.SetTextAsync(text);
        Console.WriteLine("\nCopied to clipboard!");
    }
    if (isInSlowMode) Thread.Sleep(delay);
    Console.WriteLine("\nPress ENTER to continue");
    Console.ReadLine();
}