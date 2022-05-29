using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using YaDemo;
using YaEngine.Bootstrap;

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", false, false)
    .Build();
var services = new ServiceCollection();

services
    .AddSilk(configuration)
    .AddManagers()
    .AddDefaultSystems()
    .AddOpenGl()
    .AddOpenAl()
    .AddBulletPhysics()
    .AddDefaultLogging(configuration)
    .AddScene<AnimationsSceneProvider>()
    .AddScene<PhysicsSceneProvider>();

var serviceProvider = services.BuildServiceProvider();
var bootstrapper = serviceProvider.GetService<SilkBootstrapper>();

bootstrapper.Run(args.FirstOrDefault());