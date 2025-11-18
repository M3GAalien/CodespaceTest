using System.Diagnostics;

Console.WriteLine("View commercial (1) or Residential (2)");
int choice = int.Parse(Console.ReadLine() ?? "0");
if(choice == 0)
{
    Console.WriteLine("An error occured");
    return;
}

string url = "https://app.gaiia.com/iq-fiber/accounts?filters=%7B";
string option = choice == 1 ? """customTypeIds"%3A%5B"6421ab73-c467-4f79-ac17-ba91ad3ee0c4"%5D%7D""" : """customTypeIds"%3A%5B"919285ef-deb8-4c1b-8fc9-e36f6eb00932"%5D%7D""";

Process.Start(new ProcessStartInfo
{
    FileName = url + option,
    UseShellExecute = true,
    Verb = "open"
});

Console.ReadLine();