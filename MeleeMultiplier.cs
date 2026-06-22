using System.Text.Json;
using System.Text.Json.Serialization;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.Models.Utils;
using Path = System.IO.Path;

namespace DynamicFleaNamespace;

[Injectable(InjectionType = InjectionType.Singleton)]
public class MeleeMultiplier(
    ISptLogger<MeleeMultiplier> logger
)
{
    public static readonly string ModName = "MeleeMultiplier";
    private static readonly string _configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "user", "mods", "MeleeMultiplier", "Config", "MeleeMultiplierConfig.json5");
    private MeleeMultiplierConfig? _config;
    
    /**
     * Load config or create new one with default value
     */
    public void LoadDynamicFleaConfig()
    {
        try
        {
            if (File.Exists(_configPath))
            {
                var jsonContent = File.ReadAllText(_configPath);
                var loadedConfig = JsonSerializer.Deserialize<MeleeMultiplierConfig>(jsonContent,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                        ReadCommentHandling = JsonCommentHandling.Skip,
                        AllowTrailingCommas = true
                    });

                _config = loadedConfig;
            }
            else
            {
                _config = new MeleeMultiplierConfig()
                {
                    MeleeDamageMultiplier = 10
                };
                
                var fileInfo = new FileInfo(_configPath);
                fileInfo.Directory?.Create();

                var jsonString = JsonSerializer.Serialize(_config, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(_configPath, jsonString);
            }
        }
        catch (Exception ex)
        {
            logger.Error($"{ModName}: error on load config", ex);
            throw;
        }
    }

    public int GetMeleeDamageMultiplier()
    {
        return _config!.MeleeDamageMultiplier;
    }
}

public class MeleeMultiplierConfig
{
    [JsonPropertyName("meleeDamageMultiplier")] 
    public int MeleeDamageMultiplier { get; set; } = 1;
}