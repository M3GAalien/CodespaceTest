using System.Text;
using System.Reflection;
using TextCopy;


bool debug = true; // SET FALSE BEFORE BUILDING - not allowed to do threads in github codespaces 
bool slowMode = true; // add timed delays for *asthetic* reasons
int delay = 1000; // delay to add in miliseconds

ConsoleColor notification = ConsoleColor.DarkGray;
ConsoleColor error = ConsoleColor.DarkRed;
ConsoleColor success = ConsoleColor.Green;


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
    Console.ForegroundColor = notification;
    input = Console.ReadLine() ?? "";
    try
    {
        Console.ForegroundColor = notification;
        accounts.Add(new Account(input.Split("\t"), agent));
    }
    catch (Exception e)
    {
        Console.ForegroundColor = error;
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
    Console.ResetColor();
} while (input != "");
#endregion

var client = new HttpClient();
string url = @"https://app.gaiia.com/iq-fiber/accounts/";

foreach (Account a in accounts)
{
    do
    {
        #region Display info to console
        Console.WriteLine("Getting account....");
        if (slowMode) Thread.Sleep(delay);
        Console.Clear();

        Console.WriteLine($"PROGRESS: {accounts.IndexOf(a) + 1} of {accounts.Count()}\n");
        string text = "";
        printAccountInfo(a, url);
        #endregion

        #region Copy phone number to clipboard to paste in NICE
        Console.WriteLine("\nGetting phone number....");
        if (slowMode) Thread.Sleep(delay);
        text = a.PhoneNumber;

        if (string.IsNullOrWhiteSpace(text))
        {
            Console.ForegroundColor = error;
            Console.WriteLine("\tError getting phone number." +
                            "\n\tPlease use Gaiia to get the phone number");
            Console.ResetColor();
        }
        else
        {
            results(debug, slowMode, delay, text);
        }
        #endregion

        #region Copy note to leave in Gaiia account
        Console.WriteLine("\nResolution:");

        // color choices
        Console.ForegroundColor = success;
        Console.WriteLine("   (1) Confirmed");
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("   (2) Voicemail");
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("   (3) Email");
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("   (4) Reschedule");
        Console.ForegroundColor = error;
        Console.WriteLine("   (5) Canceled");
        Console.ResetColor();

        int resolution = getChoice(5);
        if (resolution <= 3)
        {
            a.Resolution = resolution switch
            {
                1 => "Confirmed",
                2 => "Voicemail",
                3 => "Emailed",
                _ => "ERROR"
            };
        }
        else
        {
            a.CXLorRS = resolution switch
            {
                4 => "Rescheduled",
                5 => "Canceled",
                _ => "ERROR"
            };
        }

        #region Format account note
        if (a.CXLorRS != "Rescheduled" && a.CXLorRS != "Canceled")
        {
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
            results(debug, slowMode, delay, text);
            #endregion
        }
        #endregion

        #region Send an email if necessary
        if (a.Resolution.Contains("Emailed"))
        {
            Console.WriteLine("Formatting email....");
            if (slowMode) Thread.Sleep(delay);

            text = @$"Hi {a.FirstName},
Just wanted to confirm the details of your installation 
Where : {a.Address},
When  : {a.reformatedInstallTime()}.
Plan  : {a.Subsciption}.

The technicians will call when they are on the way.
Please make sure someone 18 or older is home for the full appointment, any pets are secured, and any gates needed for access are opened.

If you need to reschedule or have any questions, feel free to call us at 1-800-495-4775.
We look forward to getting you connected!

Best regards,";
            results(debug, slowMode, delay, text);
        }
        #endregion
    } while (goBack());

}

#region Copy results to leave in Excel
Console.WriteLine("Getting results for Excel....");
if (slowMode) Thread.Sleep(delay);
Console.Clear();

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
Console.ForegroundColor = success;
Console.WriteLine("ALL DONE: YIPEEE");
Console.ResetColor();

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
            Console.ForegroundColor = error;
            Console.WriteLine("Whoopsie Tootsie: " + e.Message);
            choice = -99;
        }

        if (choice > 0 && choice <= range)
        {
            invalidChoice = false;
        }
        else
        {
            Console.ForegroundColor = error;
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
        Console.ResetColor();
    } while (invalidChoice);
    return choice;
}

// show results if in debug mode or send text straight to clipboard 
async void results(bool isInDebugMode, bool isInSlowMode, int delay, string text)
{
    Console.ForegroundColor = notification;
    if (isInDebugMode)
    {
        Console.WriteLine($"\n{text}");
    }
    else
    {
        await ClipboardService.SetTextAsync(text);
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine("Copied to clipboard!");
        Console.ResetColor();
    }
    Console.ResetColor();
    Console.WriteLine("Press ENTER to continue");
    Console.ReadLine();
}

// add colors when displaying account info
void printAccountInfo(Account a, string url)
{

    ConsoleColor borderColor = notification;
    ConsoleColor urlColor = ConsoleColor.Blue;
    ConsoleColor labelColor = ConsoleColor.Yellow;
    ConsoleColor dashColor = notification;
    ConsoleColor valueColor = ConsoleColor.Cyan;

    Console.WriteLine("Account Info:");

    writeBorder();
    Console.ForegroundColor = urlColor;
    Console.WriteLine(url + a.AccountNumber);
    Console.ResetColor();
    writeBorder();

    printField("NAME", $"{a.FirstName} {a.LastName}");
    printField("ADDRESS", a.Address);
    printField("INSTALL TIME", a.reformatedInstallTime());
    printField("SUBSCRIPTION", a.Subsciption);

    void writeBorder(char borderCharacter = '#', int borderLength = 45)
    {
        Console.ForegroundColor = borderColor;
        Console.WriteLine(new string(borderCharacter, borderLength));
        Console.ResetColor();
    }

    void printField(string label, string value, int labelBlockWidth = 15)
    {
        Console.ForegroundColor = labelColor;
        Console.Write(label);

        int fillerLength = labelBlockWidth - label.Length;

        Console.ForegroundColor = dashColor;
        Console.Write(new string('-', fillerLength));

        Console.ForegroundColor = valueColor;
        Console.WriteLine(value);
        Console.ResetColor();
    }
}

// check if need to change call resolution in Gaiia
bool goBack()
{
    Console.WriteLine("Need to change resolution?");
    Console.ForegroundColor = notification;
    Console.WriteLine("     (1) NO");
    Console.WriteLine("     (2) YES");
    Console.ResetColor();
    return getChoice(2) == 1 ? false : true;
}