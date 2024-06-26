﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ContainerFileSystemWatcher;
using Microsoft.Extensions.DependencyInjection;

namespace ConsoleFileSystemWatcher.Demo;

internal class Program
{
    static void Main(string[] args)
    {
        var host = CreateHostBuilder(args).Build();
        var folderPath = host.Services.GetRequiredService<IConfiguration>()["FOLDER_TO_WATCH"];
        if (string.IsNullOrWhiteSpace(folderPath))
        {
            Console.WriteLine("Please set the FOLDER_TO_WATCH environment variable.");
            return;
        }
        var watcher = host.Services.GetRequiredService<IContainerFileWatcher>();
        watcher.EnableLogging = false;
        watcher.OnFileChanged += (changeType, filePath) =>
        {
            Console.WriteLine($"File changed: {changeType}/{filePath}");
        };

        watcher.AddWatch(folderPath, TimeSpan.FromMilliseconds(500));
        Console.WriteLine($"Watching {folderPath} for changes.");
        Console.WriteLine();
        Console.ReadLine();
    }

    static IHostBuilder CreateHostBuilder(string[] args) 
        => Host.CreateDefaultBuilder(args)
            .ConfigureServices((hostContext, services) =>
            {
                services.AddContainerFileSystemWatcher();
            })
            .ConfigureAppConfiguration((hostingContext, config) =>
            {
                config.AddEnvironmentVariables();
            })
            .ConfigureLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddConsole();
            });
}

