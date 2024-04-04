using Azure;
using Azure.AI.OpenAI;
using intention_service;
using Newtonsoft.Json;
using System;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;

public class IntentDetect
{
    public static async Task<IntentionResponse> RouteByIntentionAsync(IntentionRequest intentionRequest)
    {
        switch (intentionRequest.Appstate)
        {
            case "initial":
                return await DetectIntentionAsync(intentionRequest);

            case "families":
                return await FamiliesIntention(intentionRequest);
                break;
            case "resources":
                return await ResourcesIntention(intentionRequest);
                break;
            /*case "diagnosis":
                Console.WriteLine("I'm sorry you feel that way. How can I help you today?");
                break;*/
            default:
                return new IntentionResponse { Message = "I'm sorry, I don't understand. Can you clarify?", UserId = intentionRequest.Userid, Appstate = "initial" };
        }
    }

    public async static Task<string> GetSummaryAsync(string sumamryPromptFile, string[] userMesssages)
    {
        string prompt = File.ReadAllText(sumamryPromptFile);
        prompt = prompt.Replace("<<user_message>>", string.Join(" \r\n", userMesssages));
        Console.WriteLine(prompt);
        var response = await CallAzureOpenAIAsync(prompt);
        return response;// new IntentionResponse { Message = chatResponse, UserId = intentionRequest.Userid, Appstate = intentionRequest.Appstate };
    }

    public async static Task<IntentionResponse> FamiliesIntention(IntentionRequest intentionRequest)
    {
        UserMessageStore.AddMessage(intentionRequest.Userid, intentionRequest.Appstate, intentionRequest.Message);
        string[] userMesssages = UserMessageStore.GetMessages(intentionRequest.Userid, intentionRequest.Appstate);
        //get detention intent prompt and merge it with the user message and submit to Azure OpenAI and get the response
        //load prompt from text file
        string prompt = File.ReadAllText("prompts/families-prompt.txt");
        prompt = prompt.Replace("<<user_message>>", string.Join(" \r\n", userMesssages));
        Console.WriteLine(prompt);
        var response = await CallAzureOpenAIAsync(prompt);
        string? chatResponse;
        if (response == "CONTINUE")
        {
            var summary = await GetSummaryAsync("prompts/families-result.txt", userMesssages);
            chatResponse = "Thank you, here are some families that match what you're looking for:  \r\n" + await FindFamilies(summary);
        }
        else
        {
            chatResponse = response;
        }
        return new IntentionResponse { Message = chatResponse, UserId = intentionRequest.Userid, Appstate = intentionRequest.Appstate };
    }

    public async static Task<string> FindFamilies(string promptSummary)
    {
        //make httprequest response to the families service
        var client = new HttpClient();
        //remove special characters (but keep spaces and periods) from the promptSummary using regex
        promptSummary = Regex.Replace(promptSummary, "[^a-zA-Z0-9. ]", "");

        var requestString =  "{\"query_text\": \"" + promptSummary +"\"}";

        var buffer = System.Text.Encoding.UTF8.GetBytes(requestString);
        var byteContent = new ByteArrayContent(buffer);
        byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");


        var response = await client.PostAsync("https://usecase4.azurewebsites.net/search-similar", byteContent);
        var families = await response.Content.ReadAsStringAsync();
        //convert families to collection of SearchResult objects
        var familiesList = JsonConvert.DeserializeObject<List<SearchResult>>(families);
        var familyList = familiesList.Select(f => $"{f.AutismDiagnosisSummary} - Contact: {f.Email}".ToList()).Take(5).ToList();
        string prompt = File.ReadAllText("prompts/families-result.txt");
        prompt = prompt.Replace("<<user_message>>", promptSummary);
        prompt = prompt.Replace("<<family_list>>", string.Join(" \r\n", familyList));
        Console.WriteLine(prompt);
        var openAIResponse = await CallAzureOpenAIAsync(prompt);
        return openAIResponse;
    }

    public async static Task<IntentionResponse> ResourcesIntention(IntentionRequest intentionRequest)
    {
        UserMessageStore.AddMessage(intentionRequest.Userid, intentionRequest.Appstate, intentionRequest.Message);
        string[] userMesssages = UserMessageStore.GetMessages(intentionRequest.Userid, intentionRequest.Appstate);
        //get detention intent prompt and merge it with the user message and submit to Azure OpenAI and get the response
        //load prompt from text file
        string prompt = File.ReadAllText("prompts/resources-prompt.txt");
        prompt = prompt.Replace("<<user_message>>", string.Join(" \r\n", userMesssages));
        Console.WriteLine(prompt);
        var response = await CallAzureOpenAIAsync(prompt);
        string? chatResponse;
        if (response == "CONTINUE")
        {
            var summary = await GetSummaryAsync("prompts/resources-summary.txt", userMesssages);
            chatResponse = "Thank you, let me find you some resources based on what you've told me: \r\n" + summary; ;
        }
        else
        {
            chatResponse = response;
        }
        return new IntentionResponse { Message = chatResponse, UserId = intentionRequest.Userid, Appstate = intentionRequest.Appstate };
    }


    public async static Task<IntentionResponse> DetectIntentionAsync(IntentionRequest intentionRequest)
    {
        //get detention intent prompt and merge it with the user message and submit to Azure OpenAI and get the response
        //load prompt from text file
        string prompt = File.ReadAllText("prompts/intention-prompt.txt");
        prompt = prompt.Replace("<<user_message>>", intentionRequest.Message);
        var response = await CallAzureOpenAIAsync(prompt);
        string? chatResponse;
        if (CheckIfIntention(response))
        {
            chatResponse = "I detected the intention of the user as " + response + ". Tell me more about what you are looking for.";
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

    public static async Task<string> CallAzureOpenAIAsync(string prompt)
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
