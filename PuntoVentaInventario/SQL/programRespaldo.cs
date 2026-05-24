//using Microsoft.AspNetCore.Authentication.JwtBearer;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.IdentityModel.Tokens;
//using Microsoft.OpenApi.Models;
//using PuntoVentaInventario.Authorization;
//using PuntoVentaInventario.Data;
//using PuntoVentaInventario.Models.Entities;
//using System.Text;

//var builder = WebApplication.CreateBuilder(args);

//builder.Services.AddDbContext<AppDbContext>(options =>
//    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

//builder.Services
//    .AddIdentity<ApplicationUser, IdentityRole>(options =>
//    {
//        options.Password.RequiredLength = 6;
//        options.Password.RequireDigit = false;
//        options.Password.RequireUppercase = false;
//        options.Password.RequireLowercase = false;
//        options.Password.RequireNonAlphanumeric = false;
//        options.User.RequireUniqueEmail = false;
//    })
//    .AddEntityFrameworkStores<AppDbContext>()
//    .AddDefaultTokenProviders();

//var jwtKey = builder.Configuration["Jwt:Key"]!;
//var jwtIssuer = builder.Configuration["Jwt:Issuer"]!;
//var jwtAudience = builder.Configuration["Jwt:Audience"]!;

//builder.Services.AddAuthentication(options =>
//{
//    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
//    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
//})
//.AddJwtBearer(options =>
//{
//    options.RequireHttpsMetadata = true;
//    options.SaveToken = true;
//    options.TokenValidationParameters = new TokenValidationParameters
//    {
//        ValidateIssuer = true,
//        ValidateAudience = true,
//        ValidateLifetime = true,
//        ValidateIssuerSigningKey = true,
//        ValidIssuer = jwtIssuer,
//        ValidAudience = jwtAudience,
//        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
//        ClockSkew = TimeSpan.Zero
//    };
//});

//builder.Services.AddAuthorization(options =>
//{
//    foreach (var permission in Permissions.All)
//    {
//        options.AddPolicy(permission, policy =>
//            policy.RequireAssertion(context =>
//                context.User.IsInRole("Administrador") ||
//                context.User.HasClaim("permission", permission)));
//    }
//});
//builder.Services.AddControllers();
//builder.Services.AddEndpointsApiExplorer();

//builder.Services.AddSwaggerGen(options =>
//{
//    options.SwaggerDoc("v1", new OpenApiInfo
//    {
//        Title = "PuntoVentaInventario API",
//        Version = "v1"
//    });

//    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
//    {
//        Name = "Authorization",
//        Type = SecuritySchemeType.Http,
//        Scheme = "bearer",
//        BearerFormat = "JWT",
//        In = ParameterLocation.Header,
//        Description = "Ingresa solo el token JWT. Swagger agregará automáticamente 'Bearer'."
//    });

//    options.AddSecurityRequirement(new OpenApiSecurityRequirement
//    {
//        {
//            new OpenApiSecurityScheme
//            {
//                Reference = new OpenApiReference
//                {
//                    Type = ReferenceType.SecurityScheme,
//                    Id = "Bearer"
//                }
//            },
//            Array.Empty<string>()
//        }
//    });
//});

//var allowedOrigins = builder.Configuration.GetValue<string>("AllowedOrigins")!;

//builder.Services.AddCors(options =>
//{
//    options.AddDefaultPolicy(policy =>
//    {
//        policy.WithOrigins(allowedOrigins)
//              .AllowAnyMethod()
//              .AllowAnyHeader();
//    });
//});

//var app = builder.Build();

//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}

//app.UseCors();
//app.UseHttpsRedirection();
//app.UseAuthentication();
//app.UseAuthorization();

//app.MapControllers();

//using (var scope = app.Services.CreateScope())
//{
//    var services = scope.ServiceProvider;
//    await SeedData.InitializeAsync(services);
//}

//app.Run();