using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models; // Swagger yetkilendirme için

var builder = WebApplication.CreateBuilder(args);

// CORS ayarlarýný mevcut haliyle ekliyoruz
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

// SWAGGER'I JWT ÝLE YETKÝLENDÝRECEK ÞEKÝLDE AYARLA
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

// JWT Kimlik Doðrulama Servisini Ekleyin
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true, // Ýmza anahtarýný doðrula
            // appsettings.json'daki gizli anahtarý kullan
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8
                .GetBytes(builder.Configuration.GetSection("AppSettings:Token").Value!)),
            ValidateIssuer = false, // Geliþtirme için false (gerçek projede true olmalý)
            ValidateAudience = false // Geliþtirme için false (gerçek projede true olmalý)
        };
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection(); // Eðer daha önce yorumladýysanýz bu þekilde kalabilir

app.UseStaticFiles(); // Mevcut UseStaticFiles() kullanýmý korunuyor

// CORS middleware'ini mevcut politikasýyla kullanýyoruz
app.UseCors("AllowAll");

// Kimlik doðrulama middleware'ini ekleyin (UseRouting/UseCors'dan sonra, UseAuthorization'dan önce)
app.UseAuthentication();
app.UseAuthorization(); // Mevcut UseAuthorization() sýrasý korunuyor

app.MapControllers();

app.Run();
