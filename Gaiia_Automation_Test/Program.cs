using System.Text;
using System.Reflection;
using TextCopy;


bool debug = true; // SET FALSE BEFORE BUILDING - not allowed to do threads in github codespaces 
bool slowMode = false; // add timed delays for *asthetic* reasons
int delay = 1000; // delay to add in miliseconds

ConsoleColor notification = ConsoleColor.DarkGray;
ConsoleColor error = ConsoleColor.DarkRed;
ConsoleColor success = ConsoleColor.Green;


#region intro
string text = "this program does blah blah blah...\nyour name pls: ";
typeText(text, slowMode);
string agent = debug ? "Michael A" : Console.ReadLine() ?? "NO NAME";

text = "\nWe doing precalls or wellness checks?";
typeText(text, slowMode);
Console.WriteLine("\n     (1) - precall\n     (2) - wellness check");
string task = getChoice(2) switch
{
    1 => "precall",
    2 => "wellness check",
    _ => "ERROR"
};
#endregion

#region get info from Excel
if (slowMode) Thread.Sleep(delay);
text = "\nDrop the Excel pls good sir then press Enter to continue\n";
typeText(text, slowMode);

List<Account> accounts = new List<Account>();
do
{
    Console.ForegroundColor = notification;
    text = Console.ReadLine() ?? "";
    try
    {
        Console.ForegroundColor = notification;
        accounts.Add(new Account(text.Split("\t"), agent, task));
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
} while (text != "");
#endregion

foreach (Account a in accounts)
{
    #region Display info to console
    text = "Getting account....\n";
    typeText(text, slowMode);
    if (slowMode) Thread.Sleep(delay);
    Console.Clear();

    Console.WriteLine($"PROGRESS: {accounts.IndexOf(a) + 1} of {accounts.Count()}\n");
    printAccountInfo(a, @"https://app.gaiia.com/iq-fiber/accounts/");
    #endregion

    #region Copy phone number to clipboard to paste in NICE
    text = "\nGetting phone number....\n";
    typeText(text, slowMode);
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
    do
    {
        switch (task)
        {
            case "precall":
                precall(a);
                break;
            case "wellness check":
                wellnessCheck(a);
                break;
            default:
                Console.WriteLine("Error processing request");
                break;
        }
    } while (goBack(task, a));

}

#region Copy results to leave in Excel
text = "Getting results for Excel....\n";
typeText(text, slowMode);
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
results(debug, slowMode, delay, excelOuput.ToString());
#endregion

Console.ForegroundColor = success;
text = "ALL DONE: YIPEEE\n";
typeText(text, slowMode);
Console.ResetColor();

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
        typeText(text, slowMode);
    }
    else
    {
        await ClipboardService.SetTextAsync(text);
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine("Copied to clipboard!");
        Console.ResetColor();
    }
    Console.ResetColor();
    Console.WriteLine("\nPress ENTER to continue");
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
    if (string.IsNullOrWhiteSpace(a.Subsciption))
    {
        Console.ForegroundColor = error;
        Console.WriteLine("\tError getting subsciption." +
                        "\n\tPlease use Gaiia to get the subscription");
        Console.ResetColor();
    }
    else
    {
        printField("SUBSCRIPTION", a.Subsciption);
    }

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
bool goBack(string task, Account account)
{
    typeText("Need to change resolution?\n", slowMode);
    Console.ForegroundColor = notification;
    Console.WriteLine("     (1) NO");
    Console.WriteLine("     (2) YES");
    Console.ResetColor();
    bool result = getChoice(2) == 1 ? false : true;
    if (result)
    {
        switch (task) // reset resolution to default settings
        {
            case "precall":
                account.Resolution = "";
                account.CXLorRS = "";
                break;
            case "wellness check":
                account.WellnessCheckResolution = "";
                account.WellnessCheckStatus = "";
                break;
            default:
                Console.ForegroundColor = error;
                Console.WriteLine("An error occurred while removing results");
                break;
        }
    }
    return result;
}

void precall(Account account)
{
    #region Copy note to leave in Gaiia
    typeText("\nResolution:\n", slowMode);

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
        account.Resolution = resolution switch
        {
            1 => "Confirmed",
            2 => "Voicemail",
            3 => "Emailed",
            _ => "ERROR"
        };
    }
    else
    {
        account.CXLorRS = resolution switch
        {
            4 => "Rescheduled",
            5 => "Canceled",
            _ => "ERROR"
        };
    }

    #region Format account note
    string text = "";
    if (account.CXLorRS != "Rescheduled" && account.CXLorRS != "Canceled")
    {
        text = "Formating note for Gaiia account....\n";
        typeText(text, slowMode);
        if (slowMode) Thread.Sleep(delay);
        text = @"ISSUE: PRE-CALL

ACTION: 
Vetro ID verified & called the customer
";


        switch (account.Resolution)
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
*   {account.Address}
*   {account.reformatedInstallTime()}
*   {account.Subsciption}

RESULT: Pending Installation";
        results(debug, slowMode, delay, text);
        #endregion
    }
    #endregion

    #region Send an email if necessary
    if (account.Resolution.Contains("Emailed"))
    {
        text = "Formatting email....\n";
        typeText(text, slowMode);
        if (slowMode) Thread.Sleep(delay);

        text = @$"Hi {account.FirstName},
Just wanted to confirm the details of your installation 
Where : {account.Address},
When  : {account.reformatedInstallTime()}.
Plan  : {account.Subsciption}.

The technicians will call when they are on the way.
Please make sure someone 18 or older is home for the full appointment, any pets are secured, and any gates needed for access are opened.

If you need to reschedule or have any questions, please reach out.
We look forward to getting you connected!

Best regards,";
        results(debug, slowMode, delay, text);
    }
    #endregion

}

void wellnessCheck(Account account)
{
    #region Copy note to leave in Gaiia
    Console.WriteLine("\nResolution:");

    // color choices
    Console.ForegroundColor = success;
    Console.WriteLine("   (1) Satisfied");
    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.WriteLine("   (2) Emailed + VM");
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine("   (3) Rescheduled");
    Console.ForegroundColor = error;
    Console.WriteLine("   (4) Canceled");
    Console.ResetColor();

    int resolution = getChoice(4);
    if (resolution <= 2)
    {
        account.WellnessCheckResolution = resolution switch
        {
            1 => "Satisfied",
            2 => "Emailed + VM",
            _ => "ERROR"
        };
    }
    else
    {
        account.WellnessCheckStatus = resolution switch
        {
            3 => "Rescheduled",
            4 => "Canceled",
            _ => "ERROR"
        };
    }
    #endregion

    #region Send an email if necessary
    string text = "";
    if (account.WellnessCheckResolution.Contains("Emailed + VM"))
    {
        text = "Formatting email....\n";
        typeText(text, slowMode);
        if (slowMode) Thread.Sleep(delay);

        text = @$"Hi {account.FirstName},

We recently installed your IQ Fiber service and tried giving you a call but reached your voicemail — so we wanted to follow up via email.

    We reach out to our new customers proactively to make sure the installation went smoothly and to see if you have any questions or concerns.
    Please let us know how your experience was and if you'd like us to follow up directly.
    We truly value you as an IQ Fiber customer. 

Again, welcome — we're excited to have you with us!

Best regards,";
        results(debug, slowMode, delay, text);
    }
    #endregion
}

void typeText(string text, bool slowMode)
{
    if (slowMode)
    {
        foreach (char c in text)
        {
            Console.Write(c);
            Thread.Sleep(10);
        }
    }
    else
    {
        Console.Write(text);
    }
}