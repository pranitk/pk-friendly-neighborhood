var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowAll",
            builder =>
            {
                builder
                .AllowAnyOrigin() 
                .AllowAnyMethod()
                .AllowAnyHeader();
            });
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll"); // Apply the CORS policy

app.MapPost("/intention", async context =>
{
    var intentionRequest = await context.Request.ReadFromJsonAsync<IntentionRequest>();

    // Call the Azure OpenAI service to detect the user's intention
    var response = await IntentDetect.RouteByIntentionAsync(intentionRequest);
    await context.Response.WriteAsJsonAsync(response);
})
.WithName("PostIntention")
.WithOpenApi();

app.Run();

