namespace Gaiia_Automation_Test;

public class WellnessCheckForm
{
    // Gaiia Account Status
    public bool accountIsActive;
    public bool hasPaymentMethod;
    public bool hasWIFIMan;
    public bool hasTechNotes;

    // PONMON Status
    public string lightLevels = "";
    public string errorsOnService = "";

    // Eero Account Status
    public string channelUtilization = "";
    public string averageUtilization = "";
    public string noiseLevel = "";

    // Customer Feedback
    public string customerFeedback = "";
}
