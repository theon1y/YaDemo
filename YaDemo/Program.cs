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
switch (args.FirstOrDefault())
{
    case "animations":
        services.AddAnimationsScene(configuration);
        break;
    case "physics":
        services.AddPhysicsScene(configuration);
        break;
    default:
        Console.WriteLine("Hello World");
        return 0;
}
    
var serviceProvider = services.BuildServiceProvider();
var bootstrapper = serviceProvider.GetService<SilkBootstrapper>();
bootstrapper.Run();
return 0;