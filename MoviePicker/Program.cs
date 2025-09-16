using System.Text.Json;
using MoviePicker.Models;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "Hello World!");      //TODO: Continue here

app.Run();
