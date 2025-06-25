using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Text.Json.Serialization;
using WebSale.Data;
using WebSale.Dto.Email;
using WebSale.Interfaces;
using WebSale.Models;
using WebSale.Models.Momo;
using WebSale.Respository;
using WebSale.Services.Momo;
using WebSale.Services.Vnpay;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddControllers().AddJsonOptions(x =>
                x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddScoped<ILoginRepository, LoginRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<ITokenService, TokenServiceRespository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ICartRepository, CartRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IFeedBackRepository, FeedBackRepository>();
builder.Services.AddScoped<IImageCategoryRepository, ImageCategoryRepository>();
builder.Services.AddScoped<IImageProductRepository, ImageProductRepository>();
builder.Services.AddScoped<IProductDetailRepository, ProductDetailRepository>();
builder.Services.AddScoped<IOrderProductRepository, OrderProductRespository>();
builder.Services.AddScoped<IImageFeedBackRepository, ImageFeedBackRepository>();
builder.Services.AddScoped<IAddressRepository, AddressRepository>();
builder.Services.AddScoped<IAddressUserRepository, AddressUserRepository>();
builder.Services.AddScoped<IBillRepository, BillRepository>();
builder.Services.AddScoped<IBillDetailRepository, BillDetailRepository>();
builder.Services.AddScoped<AddressDataSeeder>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IVnPayService, VnPayService>();
//Connect MomoApi
builder.Services.Configure<MomoOptionModel>(builder.Configuration.GetSection("MomoAPI"));
builder.Services.AddScoped<IMomoService, MomoService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Name = "Authorization",
        Description = "Bearer Authentication with JWT Token",
        Type = SecuritySchemeType.Http,
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Id = "Bearer",
                    Type = ReferenceType.SecurityScheme,
                }
            },
            new List<string>()
        }
    });
});

var configuration = builder.Configuration;
builder.Services.AddDbContext<DataContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// Configure IdentityOptions for ASP.NET Identity
//builder.Services.AddIdentity<User, Role>(options =>
//{
//    options.SignIn.RequireConfirmedEmail = true;
//})
//.AddEntityFrameworkStores<DataContext>()
//.AddDefaultTokenProviders();

//builder.Services.Configure<IdentityOptions>(options => options.SignIn.RequireConfirmedEmail = true);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddCookie().AddGoogle(options =>
    {
        var clientId = builder.Configuration["GoogleKeys:ClientId"];
        if(clientId == null)
        {
            throw new ArgumentNullException(nameof(clientId));
        }
        var clientSecret = builder.Configuration["GoogleKeys:ClientSecret"];
        if (clientSecret == null)
        {
            throw new ArgumentNullException(nameof(clientSecret));
        }
        options.ClientId = clientId;
        options.ClientSecret = clientSecret;
        options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    })
    .AddFacebook(options =>
    {
        var appId = builder.Configuration["FacebookKeys:AppId"];
        if (appId == null)
        {
            throw new ArgumentNullException(nameof(appId));
        }
        var appSecret = builder.Configuration["FacebookKeys:AppSecret"];
        if (appSecret == null)
        {
            throw new ArgumentNullException(nameof(appSecret));
        }
        options.AppId = appId;
        options.AppSecret = appSecret;
        options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    })
.AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidAudience = builder.Configuration["JWT:ValidAudience"],
        ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Secret"]))
    };
});

// Add Email Configs
var emailConfig = configuration.GetSection("EmailConfiguration").Get<EmailConfiguration>();
builder.Services.AddSingleton(emailConfig);

builder.Services.AddAWSLambdaHosting(LambdaEventSource.RestApi);
//app.UseCors(options =>
//            options.WithOrigins("*")
//            .AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
//app.UseCors(options =>
//    options.WithOrigins("http://localhost:3000")
//           .AllowAnyMethod()
//           .AllowAnyHeader());

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin", policy =>
    {
        policy.WithOrigins("*")
              .AllowAnyMethod()
              //.AllowCredentials()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Add data address
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var seeder = services.GetRequiredService<AddressDataSeeder>();
    seeder.SeedProvinces("Files/thanh-pho");
    seeder.SeedDistricts("Files/quan-huyen");
    seeder.SeedWards("Files/xa-phuong");
}

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
app.UseSwagger();
app.UseSwaggerUI();
//}

app.UseHttpsRedirection();

app.UseCors("AllowSpecificOrigin");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
