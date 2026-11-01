using Microsoft.EntityFrameworkCore;
using hmcts_test.model;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<TaskContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("TaskContext")));

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();
