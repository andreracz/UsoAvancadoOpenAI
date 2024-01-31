// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.DotNet.Interactive;
using InteractiveKernel = Microsoft.DotNet.Interactive.Kernel;

// ReSharper disable InconsistentNaming

public static class Settings
{
    private const string DefaultConfigFile = "config/settings.json";
    private const string ModelKey = "model";
    private const string EndpointKey = "endpoint";
    private const string SecretKey = "apikey"; 
    private const bool StoreConfigOnFile = true;

    
    // Load settings from file
    public static (string model, string azureEndpoint, string apiKey)
        LoadFromFile(string configFile = DefaultConfigFile)
    {
        if (!File.Exists(configFile))
        {
            Console.WriteLine("Configuration not found: " + configFile);
            throw new Exception("Configuration not found: " + configFile);
        }

        try
        {
            var config = JsonSerializer.Deserialize<Dictionary<string, string>>(File.ReadAllText(configFile));
            string model = config[ModelKey];
            string azureEndpoint = config[EndpointKey];
            string apiKey = config[SecretKey];


            return (model, azureEndpoint, apiKey);
        }
        catch (Exception e)
        {
            Console.WriteLine("Something went wrong: " + e.Message);
            return ("", "", "");
        }
    }

    
}