using System.Text.Json;
using MoviePicker.Models;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactDev",
        policy => policy.WithOrigins("http://localhost:5173").AllowAnyHeader().AllowAnyMethod()
    );
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Movie Picker API",
        Version = "v1"
    });
});

var app = builder.Build();

app.UseCors("AllowReactDev");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Movie Picker API V1");
    });
}

app.UseDefaultFiles();
app.UseStaticFiles();

var dataDir = Path.Combine(AppContext.BaseDirectory, "data");
Directory.CreateDirectory(dataDir);
var jsonFile = Path.Combine(dataDir, "lists.json");

/* List<SubmittedList> LoadLists()
{
    if (!File.Exists(jsonFile))
    {
        return new List<SubmittedList>();
    }

    var json = File.ReadAllText(jsonFile);
    return string.IsNullOrWhiteSpace(jsonFile) ? new List<SubmittedList>() : JsonSerializer.Deserialize<List<SubmittedList>>(json) ?? new List<SubmittedList>();
} */

List<SubmittedList> LoadLists()
{
    if (!File.Exists(jsonFile))
        return new List<SubmittedList>();

    var json = File.ReadAllText(jsonFile);
    return string.IsNullOrWhiteSpace(json)
        ? new List<SubmittedList>()
        : JsonSerializer.Deserialize<List<SubmittedList>>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        }) ?? new List<SubmittedList>();

}

void SaveLists(List<SubmittedList> lists)
{
    var json = JsonSerializer.Serialize(lists, new JsonSerializerOptions { WriteIndented = true });
    File.WriteAllText(jsonFile, json);
}

app.MapPost("/submit-list", async (HttpRequest req) =>
{
    var lists = LoadLists();

    var newList = await JsonSerializer.DeserializeAsync<SubmittedList>(req.Body, new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true
    });

    if (newList == null) return Results.BadRequest();

    newList.ID = Guid.NewGuid();
    newList.CreatedAt = DateTime.UtcNow;

    lists.Add(newList);
    SaveLists(lists);

    return Results.Created($"/lists/{newList.ID}", newList);
});

app.MapGet("/lists", () =>
{
    var lists = LoadLists();
    return Results.Ok(lists);
});

app.MapGet("/lists/{id}", (Guid id) =>
{
    var lists = LoadLists();
    var list = lists.FirstOrDefault(l => l.ID == id);
    return list is null ? Results.NotFound() : Results.Ok(list);
});

/* app.MapDelete("/lists", () =>
{
    var empty = new List<object>();
    File.WriteAllText("lists.json", JsonSerializer.Serialize(empty, new JsonSerializerOptions { WriteIndented = true }));
    return Results.Ok(new { message = "All lists cleared" });
}); */

app.MapDelete("/lists", (HttpRequest req) =>
{
    /*     var key = req.Query["key"].ToString();
        if (key != "deleteAllAdmin"){
            return Results.Unauthorized();
        }  */

    var dir = Path.GetDirectoryName(jsonFile) ?? AppContext.BaseDirectory;
    Directory.CreateDirectory(dir);

    File.WriteAllText(jsonFile, JsonSerializer.Serialize(new List<SubmittedList>(), new JsonSerializerOptions { WriteIndented = true }));

    return Results.Ok(new { message = "All lists cleared.", file = jsonFile });
});


app.Run();
