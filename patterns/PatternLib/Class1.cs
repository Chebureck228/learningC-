namespace PatternLib;

public class MessageDetails
{
    public string Language {get; set; } = "";
    public string DateTime { get; set; } = "";
    public string Status { get; set; } = "";

    public void Deconstruct(out string lang, out string datetime, out string status)
    {
        lang = Language;
        datetime = DateTime;
        status = Status;
    }
}