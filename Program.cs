using Microsoft.EntityFrameworkCore;
using hmcts_test.model;
using hmcts_test.endpoints;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<TaskContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("TaskContext")));

builder.Services.AddEndpointsApiExplorer();
var app = builder.Build();

app.MapTaskEndpoints();

app.Run();
