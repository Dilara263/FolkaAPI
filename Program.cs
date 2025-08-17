using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models; // Swagger yetkilendirme i�in

var builder = WebApplication.CreateBuilder(args);

// CORS ayarlar�n� mevcut haliyle ekliyoruz
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// SWAGGER'I JWT �LE YETK�LEND�RECEK �EK�LDE AYARLA
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "FolkaAPI", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

// JWT Kimlik Do�rulama Servisini Ekleyin
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true, // �mza anahtar�n� do�rula
            // appsettings.json'daki gizli anahtar� kullan
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8
                .GetBytes(builder.Configuration.GetSection("AppSettings:Token").Value!)),
            ValidateIssuer = false, // Geli�tirme i�in false (ger�ek projede true olmal�)
            ValidateAudience = false // Geli�tirme i�in false (ger�ek projede true olmal�)
        };
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection(); // E�er daha �nce yorumlad�ysan�z bu �ekilde kalabilir

app.UseStaticFiles(); // Mevcut UseStaticFiles() kullan�m� korunuyor

// CORS middleware'ini mevcut politikas�yla kullan�yoruz
app.UseCors("AllowAll");

// Kimlik do�rulama middleware'ini ekleyin (UseRouting/UseCors'dan sonra, UseAuthorization'dan �nce)
app.UseAuthentication();
app.UseAuthorization(); // Mevcut UseAuthorization() s�ras� korunuyor

app.MapControllers();

app.Run();
