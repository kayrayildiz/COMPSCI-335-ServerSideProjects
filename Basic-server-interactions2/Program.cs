using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication;
using quiz.Handler;
using quiz.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services
    .AddAuthentication()
.AddScheme<AuthenticationSchemeOptions, QuizAuthHandler>("QuizAuthentication", null);

builder.Services.AddDbContext<QuizDBContext>(options => options.UseSqlite(builder.Configuration["QuizAPIConnection"]));
builder.Services.AddScoped<IQuizRepo, QuizRepo>();

builder.Services.AddAuthorization(options => {
    options.AddPolicy("AdminOnly", policy => policy.RequireClaim("admin"));
    options.AddPolicy("UserOnly", policy => policy.RequireClaim("userName"));

    options.AddPolicy("UserOrAdmin", policy =>
        policy.RequireAssertion(context =>
            context.User.HasClaim(c => (c.Type == "userName" || c.Type == "admin"))
        ));
});

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseSwagger();
app.UseSwaggerUI();


app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
