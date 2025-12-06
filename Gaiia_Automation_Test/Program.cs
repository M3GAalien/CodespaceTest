using System.Text;
using System.Reflection;
using TextCopy;
using System.Diagnostics;
using Gaiia_Automation_Test;



bool debug = false; // SET FALSE BEFORE BUILDING - not allowed to do threads in github codespaces 
bool slowMode = true; // add timed delays for *asthetic* reasons
bool autoContinue = false; // auto continue to next prompt after delay.
int delay = 2000; // delay to add in miliseconds


ConsoleColor notification = ConsoleColor.DarkGray;
ConsoleColor error = ConsoleColor.DarkRed;
ConsoleColor success = ConsoleColor.Green;


#region intro
string text = "Checkout my github for documentation\n\n## https://github.com/M3GAalien/CodespaceTest ##\n\nPlease enter your name: ";
typeText(text, slowMode);
string agent = debug ? "Michael A" : Console.ReadLine() ?? "NO NAME";

text = "\nSelect workflow";
typeText(text, slowMode);
Console.ForegroundColor = notification;
Console.WriteLine("\n     (1) - Precall\n     (2) - Wellness Check");
Console.ResetColor();
string task = getChoice(2) switch
{
    1 => "precall",
    2 => "wellness check",
    _ => "ERROR"
};
#endregion

#region get info from Excel
if (slowMode) Thread.Sleep(delay);
text = "\nPaste Excel data and press enter to continue:\n";
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
    printAccountInfo(a);
    #endregion

    if (task.Contains("wellness check"))
    {
        createNewTicket(a);
    }

    do
    {
        if (task.Contains("precall"))
        {
            precall(a);
        }
        else
        {
            wellnessCheck(a);
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
results(debug, excelOuput.ToString());
#endregion

#region outro
Console.ForegroundColor = success;
text = "ALL DONE: YIPEEE\n";
typeText(text, slowMode);
Console.ResetColor();

Console.WriteLine("Press ENTER to exit");
Console.ReadLine();
#endregion

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
async void results(bool isInDebugMode, string text, bool autoContinue = false)
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
    if (autoContinue)
    {
        Thread.Sleep(delay);
        Console.WriteLine();
        return;
    }
    ;

    Console.WriteLine("\nPress ENTER to continue");
    Console.ReadLine();
}

// add colors when displaying account info
void printAccountInfo(Account a)
{

    ConsoleColor borderColor = notification;
    ConsoleColor urlColor = ConsoleColor.Blue;
    ConsoleColor labelColor = ConsoleColor.Yellow;
    ConsoleColor dashColor = notification;
    ConsoleColor valueColor = ConsoleColor.Cyan;

    Console.WriteLine("Account Info:");

    writeBorder();
    Console.ForegroundColor = urlColor;
    Console.WriteLine(@"https://app.gaiia.com/iq-fiber/accounts/" + a.AccountNumber);
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

// precall workflow
void precall(Account account)
{
    autoOpenLink(@$"https://app.gaiia.com/iq-fiber/accounts/{account.AccountNumber}");
    
    callCX(account);

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
        text = "ISSUE: PRE-CALL" +
          "\n\nACTION:\nVetro ID verified & called the customer. ";

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

        text += $"the installation details:" +
                "\n*\t{account.Address}" +
                "\n*\t{account.reformatedInstallTime()}" +
                "\n*\t{account.Subsciption}" +
                "\n\nRESULT: Pending Installation";
        results(debug, text);
    }
    #endregion

    #endregion

    #region Send an email if necessary
    if (account.Resolution.Contains("Emailed"))
    {
        text = "Formatting email....\n";
        typeText(text, slowMode);
        if (slowMode) Thread.Sleep(delay);

        try
        {
            text = File.ReadAllText("../Program/precall_email.txt");
        }
        catch (Exception e)
        {
            Console.ForegroundColor = error;
            Console.WriteLine(e.Message);
            Console.ResetColor();
        }

        text = replaceText(text, account);
        results(debug, text);
    }
    #endregion
}

// wellness check workflow
void wellnessCheck(Account account)
{
    WellnessCheckForm form = new WellnessCheckForm();
    #region fill out form
    // Account status: Account is active and payment method is on file. 
    autoOpenLink(@$"https://app.gaiia.com/iq-fiber/accounts/{account.AccountNumber}/billing/transactions");
    string text = "Account Active\n";
    typeText(text, slowMode);
    Console.ForegroundColor = notification;
    Console.WriteLine("\t(1) - YES\n\t(2) - NO");
    Console.ResetColor();
    form.accountIsActive = getChoice(2) == 1 ? true : false;

    
    text = "Payment method on file\n";
    typeText(text, slowMode);
    Console.ForegroundColor = notification;
    Console.WriteLine("\t(1) - YES\n\t(2) - NO");
    Console.ResetColor();
    form.hasPaymentMethod = getChoice(2) == 1 ? true : false;

    // Install triage: Wi-Fi Man is attached and Tech notes are completed.
    autoOpenLink(@$"https://app.gaiia.com/iq-fiber/accounts/{account.AccountNumber}/work-orders");
    text = "Wi-Fi Man\n";
    typeText(text, slowMode);
    Console.ForegroundColor = notification;
    Console.WriteLine("\t(1) - YES\n\t(2) - NO");
    Console.ResetColor();
    form.hasWIFIMan = getChoice(2) == 1 ? true : false;

    text = "Tech Notes\n";
    typeText(text, slowMode);
    Console.ForegroundColor = notification;
    Console.WriteLine("\t(1) - YES\n\t(2) - NO");
    Console.ResetColor();
    form.hasTechNotes = getChoice(2) == 1 ? true : false;

    // Light Levels:  “Good” Less than 21Dbs / “Borderline” between 21 and 24Dbs / “Bad” above 24Dbs. 
    // Error on service: TX/RX errors, FEC errors or Multiple errors
    autoOpenLink(@$"https://ponmon.iqfiber.com/ontstatus.php?serialnumber={account.AccountNumber}");
    text = "Light Levels\n";
    typeText(text, slowMode);
    Console.ForegroundColor = notification;
    Console.WriteLine("\t(1) - [Good] Less than 21Dbs" +
                    "\n\t(2) - [Borderline] between 21 and 24Dbs" +
                    "\n\t(3) - [Bad] above 24Dbs");
    Console.ResetColor();
    form.lightLevels = getChoice(3) switch
    {
        1 => "Good",
        2 => "Borderline",
        3 => "Bad",
        _ => "ERROR GETTING LIGHT LEVEL"
    };

    text = "Errors on Service\n";
    typeText(text, slowMode);
    Console.ForegroundColor = notification;
    Console.WriteLine("\t(1) - None " +
                    "\n\t(2) - TX/RX Errors" +
                    "\n\t(3) - FEC Errors" +
                    "\n\t(4) - Multiple Errors");
    Console.ResetColor();
    form.errorsOnService = getChoice(4) switch
    {
        1 => "None",
        2 => "TX/RX Errors",
        3 => "FEC Errors",
        4 => "Multiple Errors",
        _ => "ERROR GETTING ERRORS ON SERVICE"
    };

    // eero: Channel utilization, Average utilization and noise levels. 
    autoOpenLink(@$"https://insight.eero.com/search?query={account.FirstName}%20{account.LastName}");

    text = "Channel Utilization: ";
    typeText(text, slowMode);
    form.channelUtilization = Console.ReadLine() ?? "ERROR GETTING CHANNEL UTILIZATION\n";

    text = "Average Utilization: ";
    typeText(text, slowMode);
    form.averageUtilization = Console.ReadLine() ?? "ERROR GETTING AVERAGE UTILIZATION\n";

   text = "Noise Levels: ";
    typeText(text, slowMode);
    form.noiseLevel = Console.ReadLine() ?? "ERROR GETTING NOISE LEVEL\n";

    callCX(account);

    text = "Customer Feedback: ";
    typeText(text, slowMode);
    form.customerFeedback = Console.ReadLine() ?? "ERROR GETTING CUSTOMER FEEDBACK\n";
    #endregion

    #region resolution
    Console.WriteLine("\nResolution:");

    // color choices
    Console.ForegroundColor = success;
    Console.WriteLine("   (1) Satisfied");
    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.WriteLine("   (2) Emailed + VM");
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine("   (3) Service Call");
    Console.WriteLine("   (4) Rescheduled");
    Console.ForegroundColor = error;
    Console.WriteLine("   (5) Canceled");
    Console.ResetColor();

    int resolution = getChoice(5);
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
            3 => "Service Call",
            4 => "Rescheduled",
            5 => "Canceled",
            _ => "ERROR"
        };
    }
    #endregion

    #region fill out internal comment
    if (account.WellnessCheckStatus != "Rescheduled" && account.WellnessCheckStatus != "Canceled")
    {
        text = "Formating interneal note for Gaiia ticket....\n";
        typeText(text, slowMode);
        if (slowMode) Thread.Sleep(delay);

        text = @$"ISSUE: INSTALL WELLNESS CHECK
Account status:
*   {(form.accountIsActive == true ? "ACTIVE" : "INACTIVE")}
*   {(form.hasPaymentMethod == true ? "Payment method on file" : "No payment on file")}

Triage:
WORKORDER
*   has Wi-Fi Man: {(form.hasWIFIMan == true ? "Yes" : "No")}
*   has tech notes: {(form.hasTechNotes == true ? "Yes" : "No")}

PONMON
*   Light Levels: {form.lightLevels}
*   Errors on Service: {form.errorsOnService}

ROUTER
*   Channel Utilization: {form.channelUtilization}
*   Average Utilization: {form.averageUtilization}
*   Noise Level: {form.noiseLevel}

CUSTOMER FEEDBACK
*   {form.customerFeedback}

RESULT: DONE";

        results(debug, text);
    }
    #endregion

    #region fill out external comment
    text = "Formating external note for Gaiia ticket....\n";
        typeText(text, slowMode);
        if (slowMode) Thread.Sleep(delay);

    text = $"Hello {account.FirstName},\n";
    if (account.WellnessCheckResolution.Contains("Satisfied"))
    {
        text += @"It was a pleasure speaking with you today.
Thank you for sharing your feedback on the installation—we truly appreciate your input. 
Please know that our team is available 24/7 should you need any assistance or support in the future. 
You can reach us anytime by creating a new ticket in the customer portal or by phone.

Have a wonderful day!";
    }

    if (account.WellnessCheckResolution.Contains("Emailed + VM"))
    {
        text += @"Welcome to the Fiberhood!
Unfortunately, it appears we were unable to reach you via phone call to hear about your 
installation experience with us and ensure your service is living up to our high expectations. 
Please know that our team is available 24/7 should you need any assistance or support. 

Have a wonderful day!";
    }

    if (account.WellnessCheckStatus.Contains("Service Call"))
    {
        text += @"It was a pleasure speaking with you today.
Thank you for sharing your feedback on the installation—we truly appreciate your input. 
We are disappointed to hear about those challenges with your new service, and are dedicated
to addressing these concerns.

As the next step, a follow-up appointment with a technician has been scheduled.
We appreciate the opportunity to make this right and look forward to resolving your issue.

Have a wonderful day!";
    }
    results(debug, text);
    #endregion

}

// types each character to terminal
void typeText(string text, bool slowMode = true)
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

// search text for keywords to replace with account info
string replaceText(string text, Account account)
{
    text = text.Replace("#ACCNUM", account.AccountNumber.ToString());
    text = text.Replace("#FNAME", account.FirstName);
    text = text.Replace("#LNAME", account.LastName);
    text = text.Replace("#PHONE", account.PhoneNumber);
    text = text.Replace("#PLAN", account.Subsciption);
    text = text.Replace("#PLACE", account.Address);
    text = text.Replace("#TIME", account.reformatedInstallTime());
    return text;
}

void autoOpenLink(string url)
{
    if (debug)
    {
        Console.ForegroundColor = ConsoleColor.Blue;
        Console.WriteLine("\n" + url);
        Console.ResetColor();
    }
    else
    {
        Process.Start(new ProcessStartInfo(url)
        {
            UseShellExecute = true
        });
    }
}

void callCX(Account account)
{
    text = "\nGetting phone number....\n";
    typeText(text, slowMode);
    if (slowMode) Thread.Sleep(delay);
    text = account.PhoneNumber;

    if (string.IsNullOrWhiteSpace(text))
    {
        Console.ForegroundColor = error;
        Console.WriteLine("\tError getting phone number." +
                        "\n\tPlease use Gaiia to get the phone number");
        Console.ResetColor();
    }
    else
    {
        results(debug, text);
    }
}

void createNewTicket(Account account)
{
    autoOpenLink(@"https://app.gaiia.com/iq-fiber/accounts/" + account.AccountNumber + "/tickets/new");

    typeText("Getting ticket title....\n", slowMode);
    results(debug, "Technical Support", autoContinue);

    typeText("Getting ticket description....\n", slowMode);
    results(debug, "Install Wellness", autoContinue);

    typeText("Getting ticket assigned users....\n", slowMode);
    results(debug, agent, autoContinue);

    typeText("Getting ticket category....\n", slowMode);
    results(debug, "CA - Order Assistance", autoContinue);
}