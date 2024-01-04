using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PRN231_API.Dtos;
using PRN231_API.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
             .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                });
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(opt =>
{
    opt.IdleTimeout = TimeSpan.FromMinutes(5);
});
builder.Services.AddDbContext<PRN231_DBContext>(options =>
  options.UseSqlServer(builder.Configuration.GetConnectionString("Context")));

var config = new MapperConfiguration(cfg =>
{
    cfg.CreateMap<Category, CategoryDTO>();
    cfg.CreateMap<Chapter, ChapterDTO>().ReverseMap();
    cfg.CreateMap<Comic, ComicDTO>().ReverseMap();
    cfg.CreateMap<Page, PageDTO>().ReverseMap();
    cfg.CreateMap<User, UserDTO>().ReverseMap();
});
var mapper = config.CreateMapper();
builder.Services.AddSingleton(mapper);
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseSession();
app.UseAuthorization();
app.UseCors(builder =>
{
    builder
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader();
});
app.MapControllers();

app.Run();
