using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;


public class UserMessageStore
{
    private static readonly string FilePath = "UserMessages.json";

    public static string[] GetMessages(string userid, string appstate)
    {
        var userMessages = ReadFromFile();
        return userMessages.Where(m => m.Userid == userid && m.Appstate == appstate).Select(m => m.Message).ToArray();
    }
    public static void AddMessage(string userid, string appstate, string message)
    {
        var userMessages = ReadFromFile();
        userMessages.Add(new IntentionRequest { Userid = userid, Appstate = appstate,  Message = message });
        WriteToFile(userMessages);
    }

    private static List<IntentionRequest> ReadFromFile()
    {
        if (!File.Exists(FilePath))
        {
            return new List<IntentionRequest>();
        }

        var json = File.ReadAllText(FilePath);
        return JsonSerializer.Deserialize<List<IntentionRequest>>(json);
    }

    private static void WriteToFile(List<IntentionRequest> userMessages)
    {
        var json = JsonSerializer.Serialize(userMessages);
        File.WriteAllText(FilePath, json);
    }
}