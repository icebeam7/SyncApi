using SyncApi.Context;
using SyncApi.Endpoints;
using Microsoft.EntityFrameworkCore;

using SyncApi.Models;
using Microsoft.AspNetCore.Http.Json;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<EventDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("EventDbConnection");
    options.UseSqlServer(connectionString);
});

builder.Services.Configure<JsonOptions>(options =>
    options.SerializerOptions.DefaultIgnoreCondition =
        JsonIgnoreCondition.WhenWritingDefault | JsonIgnoreCondition.WhenWritingNull);

var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapEventEndpoints();

app.UseHttpsRedirection();

app.Run();