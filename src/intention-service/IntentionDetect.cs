using Azure;
using Azure.AI.OpenAI;

public class IntentDetect
{
    public static IntentionResponse RouteByIntention(IntentionRequest intentionRequest)
    {
        switch (intentionRequest.Appstate)
        {
            case "iniital":
                return DetectIntention(intentionRequest);

            /*case "families":
                Console.WriteLine("Goodbye! Have a great day!");
                break;
            case "diagnosis":
                Console.WriteLine("You're welcome!");
                break;
            case "resources":
                Console.WriteLine("I'm sorry you feel that way. How can I help you today?");
                break;*/
            default:
                return new IntentionResponse { Message = "I'm sorry, I don't understand. Can you clarify?", UserId = intentionRequest.Userid, Appstate = "initial" };
        }
    }

    public static IntentionResponse DetectIntention(IntentionRequest intentionRequest)
    {
        //get detention intent prompt and merge it with the user message and submit to Azure OpenAI and get the response
        //load prompt from text file
        string prompt = File.ReadAllText("intention-prompt.txt");
        prompt = prompt.Replace("<<user_message>>", intentionRequest.Message);
        var response = CallAzureOpenAI(prompt).Result;
        var chatResponse = string.Empty;
        if (CheckIfIntention(response))
        {
            chatResponse = "I detected the intention of the user as " + response + ". What can I do for you?";
        }
        else
        {
            chatResponse=  "Not sure what you want to do. Can you clarify for me?";
        }
        return new IntentionResponse { Message = chatResponse, UserId = intentionRequest.Userid, Appstate = response};
    }

    private static bool CheckIfIntention(string response)
    {
        //check if the response = "diagnosis", "resources", "families" or "unknown"
        //if it is, return true
        //else return false
        if (response == "diagnosis" || response == "resources" || response == "families")
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public static async Task<string> CallAzureOpenAI(string prompt)
    {
        Uri azureOpenAIResourceUri = new("https://usecase4openai.openai.azure.com/");
        AzureKeyCredential azureOpenAIApiKey = new("5d7e790180a94d4bb728bb2f56c8b813");
        OpenAIClient client = new(azureOpenAIResourceUri, azureOpenAIApiKey);

        var chatCompletionsOptions = new ChatCompletionsOptions()
        {
            DeploymentName = "gpt-4-32k", // Use DeploymentName for "model" with non-Azure clients
            Messages =
    {
        // The system message represents instructions or other guidance about how the assistant should behave
        new ChatRequestSystemMessage("You are an AI Assistant that will try to detect the intention of a user."),
        // User messages represent current or historical input from the end user
        new ChatRequestUserMessage(prompt),

    }
        };

        Response<ChatCompletions> response = await client.GetChatCompletionsAsync(chatCompletionsOptions);
        ChatResponseMessage responseMessage = response.Value.Choices[0].Message;
        var chatResponse = $"{responseMessage.Content}";
        Console.WriteLine(chatResponse);
        return chatResponse;
    }
}
