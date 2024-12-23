using FluentValidation;
using LiteDB;
using MarioTiscareno.Football.Api.Core;
using MarioTiscareno.Football.Api.Market;
using MarioTiscareno.Football.Api.Players;
using MarioTiscareno.Football.Api.Teams;

namespace MarioTiscareno.Football.Api;

public class Program
{
    public static void Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddAuthorization();

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(opt =>
        {
            opt.TagActionsBy(d => new[] { d.RelativePath.Split('/')[2] });
        });

        builder.Services.AddScoped(svc => new LiteDatabase("football.db"));
        builder.Services.AddScoped<IPlayerDb, PlayerDb>();
        builder.Services.AddScoped<ITeamDb, TeamDb>();
        builder.Services.AddScoped<IMarketDb, MarketDb>();
        builder.Services.AddSingleton<RequestPipeline>();
        builder.Services.AddRequestHandlers();
        builder.Services.AddValidators();

        builder
            .Services.AddApiVersioning(opt =>
            {
                opt.DefaultApiVersion = new(1, 0);
                opt.ReportApiVersions = true;
            })
            .AddApiExplorer(opt =>
            {
                opt.GroupNameFormat = "'v'V";
                opt.SubstituteApiVersionInUrl = true;
            });

        WebApplication app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapPlayerEndpoints();
        app.MapTeamEndpoints();

        // Disable FluentValidation localized error messages
        ValidatorOptions.Global.LanguageManager.Enabled = false;

        app.SeedDatabase();

        app.Run();
    }
}

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder SeedDatabase(this IApplicationBuilder app)
    {
        var appLifetime = app.ApplicationServices.GetRequiredService<IHostApplicationLifetime>();

        appLifetime.ApplicationStarted.Register(() =>
        {
            using (var db = new LiteDatabase("football.db"))
            {
                var players = new PlayerDb(db);
                var teams = new TeamDb(db);
                var market = new MarketDb(db);

                using (var idGenerator = new IdGenerator())
                {
                    var alkmaar = new Team(
                        Id: idGenerator.Generate("AZ Alkmaar"),
                        Name: "AZ Alkmaar",
                        Country: "Netherlands",
                        League: "Eredivisie"
                    );

                    var ajax = new Team(
                        Id: idGenerator.Generate("Ajax"),
                        Name: "Ajax",
                        Country: "Netherlands",
                        League: "Eredivisie"
                    );

                    teams.Upsert(alkmaar);
                    teams.Upsert(ajax);

                    var alkmaarPlayers = new Player[]
                    {
                        new(
                            Id: idGenerator.Generate("Rome Jayden Owusu-Oduro"),
                            Name: "Rome Jayden Owusu-Oduro",
                            HeightInCm: 190,
                            Age: 20,
                            Nationality: "Netherlands"
                        ),
                        new(
                            Id: idGenerator.Generate("Troy Parrot"),
                            Name: "Troy Parrot",
                            HeightInCm: 185,
                            Age: 20,
                            Nationality: "Ireland"
                        ),
                        new(
                            Id: idGenerator.Generate("Bruno Martins Indi"),
                            Name: "Bruno Martins Indi",
                            HeightInCm: 185,
                            Age: 32,
                            Nationality: "Netherlands"
                        ),
                        new(
                            Id: idGenerator.Generate("Ruben van Bommel"),
                            Name: "Ruben van Bommel",
                            HeightInCm: 192,
                            Age: 20,
                            Nationality: "Netherlands"
                        ),
                        new(
                            Id: idGenerator.Generate("Ibrahim Sadiq"),
                            Name: "Ibrahim Sadiq",
                            HeightInCm: 167,
                            Age: 37,
                            Nationality: "Ghana"
                        )
                    };

                    foreach (var player in alkmaarPlayers)
                    {
                        players.Upsert(player);
                        market.SignPlayer(player, alkmaar);
                    }

                    var ajaxPlayers = new Player[]
                    {
                        new(
                            Id: idGenerator.Generate("Remko Pasveer"),
                            Name: "Remko Pasveer",
                            HeightInCm: 188,
                            Age: 41,
                            Nationality: "Netherlands"
                        ),
                        new(
                            Id: idGenerator.Generate("Mika Godts"),
                            Name: "Mika Godts",
                            HeightInCm: 176,
                            Age: 19,
                            Nationality: "Belgium"
                        ),
                        new(
                            Id: idGenerator.Generate("Brian Brobbey"),
                            Name: "Brian Brobbey",
                            HeightInCm: 182,
                            Age: 22,
                            Nationality: "Netherlands"
                        ),
                        new(
                            Id: idGenerator.Generate("Bertrand Traoré"),
                            Name: "Bertrand Traoré",
                            HeightInCm: 181,
                            Age: 29,
                            Nationality: "Burkina Faso"
                        ),
                        new(
                            Id: idGenerator.Generate("Kenneth Taylor"),
                            Name: "Kenneth Taylor",
                            HeightInCm: 182,
                            Age: 22,
                            Nationality: "Netherlands"
                        )
                    };

                    foreach (var player in ajaxPlayers)
                    {
                        players.Upsert(player);
                        market.SignPlayer(player, ajax);
                    }
                }
            }
        });

        return app;
    }
}
