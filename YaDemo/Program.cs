using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using YaDemo;
using YaEcs.MicrosoftDependencyInjectionExtensions;
using YaEngine.Bootstrap;

var configurationBuilder = new ConfigurationBuilder();
configurationBuilder.AddInMemoryCollection(new Dictionary<string, string>
{
    { "WindowConfig:Width", "1280" },
    { "WindowConfig:Height", "720" },
    { "WindowConfig:Title", "YaDemo" },
});
var configuration = configurationBuilder.Build();
var services = new ServiceCollection();

services
    .AddSilk(configuration)
    .AddEcs(configuration)
    .AddDefaultSystems()
    .AddOpenGl()
    .AddOpenAl()
    .AddBulletPhysics();

int scene;
if (args.Length <= 0)
{
    Console.WriteLine("Select demo scene:\n\t1. Animations\n\t2. Physics\nOr press anything else to quit");
    scene = Console.Read() - 48;
}
else
{
    scene = int.Parse(args.FirstOrDefault());
}

switch (scene)
{
    case 1:
        services.AddAnimationsScene(configuration);
        break;
    case 2:
        services.AddPhysicsScene(configuration);
        break;
    default:
        Console.WriteLine("davai, udachi");
        return 0;
}
    
var serviceProvider = services.BuildServiceProvider();
var bootstrapper = serviceProvider.GetService<SilkBootstrapper>();
bootstrapper.Run();
return 0;