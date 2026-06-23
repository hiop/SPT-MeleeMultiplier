using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using SPTarkov.Server.Core.Models.Spt.Mod;
using SPTarkov.Server.Core.Models.Utils;
using SPTarkov.Server.Core.Services;
using Range = SemanticVersioning.Range;
using Version = SemanticVersioning.Version;

namespace DynamicFleaNamespace;

public record ModMetadata : AbstractModMetadata
{
    public override string ModGuid { get; init; } = "hiop.melee.multiplier";
    public override string Name { get; init; } = MeleeMultiplier.ModName;
    public override string Author { get; init; } = "HioP";
    public override List<string>? Contributors { get; init; }
    public override Version Version { get; init; } = new("0.0.2");
    public override Range SptVersion { get; init; } = new("~4.0.7");


    public override List<string>? Incompatibilities { get; init; }
    public override Dictionary<string, Range>? ModDependencies { get; init; }
    public override string? Url { get; init; }
    public override bool? IsBundleMod { get; init; }
    public override string? License { get; init; } = "MIT";
}

[Injectable(TypePriority = OnLoadOrder.PostDBModLoader)]
public class MeleeMultiplierOnLoad(
    ISptLogger<MeleeMultiplierOnLoad> logger,
    MeleeMultiplier meleeMultiplier,
    DatabaseService databaseService
) : IOnLoad
{
    public Task OnLoad()
    {
        meleeMultiplier.LoadDynamicFleaConfig();
        foreach (KeyValuePair<MongoId, TemplateItem> keyValuePair in databaseService.GetTemplates().Items)
        {
            if (
                meleeMultiplier.GetExcludedMeleeItems()?.Contains(keyValuePair.Value.Id) == true ||
                keyValuePair.Value.Properties == null
            )
                continue;

            if (keyValuePair.Value.Properties.KnifeHitStabDam != null)
            {
                keyValuePair.Value.Properties.KnifeHitStabDam *= meleeMultiplier.GetMeleeDamageMultiplier();
                keyValuePair.Value.Properties.KnifeHitSlashDam *= meleeMultiplier.GetMeleeDamageMultiplier();
            }
        }

        logger.Success($"[{MeleeMultiplier.ModName}] damage multiplier x{meleeMultiplier.GetMeleeDamageMultiplier()}!");

        return Task.CompletedTask;
    }
}