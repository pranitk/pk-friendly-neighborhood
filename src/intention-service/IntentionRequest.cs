
// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle


// Configure the HTTP request pipeline.







public class IntentionRequest
{
    public required string Message { get; set; }
    public required string Userid { get; set; }
    public required string Appstate { get; set; }
}
