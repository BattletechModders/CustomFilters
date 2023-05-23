using System;
using BattleTech;
using Newtonsoft.Json;

namespace CustomFilters.TagManager;

internal class TagManagerSettings
{
    [JsonProperty]
    internal const string EnabledDescription =
        $"Manipulates tags on Components, Mechs, Pilots and Lances by adding or removing {MechValidationRules.Tag_Blacklisted}:" +
        $" {nameof(TagsTransformer.Blacklist)} is applied after {nameof(TagsTransformer.Whitelist)}." +
        $" Also allows filtering by tags when entering the skirmish mech lab:" +
        $" {nameof(TagsFilter.NotContainsAny)} has precedence over {nameof(TagsFilter.ContainsAny)}." +
        $" Empty allow can be used to match nothing, while null means to allow everything.";
    [JsonProperty]
    internal bool Enabled = true;

    [JsonProperty]
    internal const string SimGameItemsMinCountDescription = "Set the owned minimum count of each mech component in SimGame.";
    [JsonProperty]
    internal int SimGameItemsMinCount = 0;

    [JsonProperty]
    internal const string LostechStockWeaponVariantDescription = "Fixes lostech variant weapon tagging by checking if id ends with -STOCK.";
    [JsonProperty]
    internal bool LostechStockWeaponVariantFix = false;

    [JsonProperty]
    internal const string SkirmishMechDefQueryFilterDescription =
        "UnitDef filter when searching for MechDefs by tags via MDDB." +
        " CustomUnits converts VehicleDefs (2) to MechDefs (1) but does not add them to the MDDB" +
        ", set the filter to \"(d.UnitTypeID = 1 OR d.UnitTypeID = 2)\" in that case.";
    [JsonProperty]
    internal string SkirmishMechDefQueryFilter = "(d.UnitTypeID = 1)";

    [JsonProperty]
    internal const string SkirmishOverloadWarningDescription = "Warn the user before loading into the SkirmishMechBay if too many 'Mech will be loaded.";
    [JsonProperty]
    internal int SkirmishOverloadWarningCount = 500;

    [JsonProperty]
    internal const string SkirmishDefaultDescription = "The default settings used when no options panel is shown and the user enters the skirmish 'Mech bay directly.";
    [JsonProperty(Required = Required.DisallowNull)]
    internal TagsFilterSet SkirmishDefault = new()
    {
        Label = "Default",
        Components = new()
        {
            ContainsAny = new[] { MechValidationRules.ComponentTag_Stock },
            NotContainsAny = new[] { MechValidationRules.Tag_Blacklisted }
        },
        Mechs = new()
        {
            ContainsAny = new[] { MechValidationRules.MechTag_Released },
            NotContainsAny = new[] { MechValidationRules.Tag_Blacklisted, MechValidationRules.MechTag_Unlocked }
        },
        Pilots = new()
        {
            ContainsAny = new[] { MechValidationRules.PilotTag_Released }
        },
        Lances = new()
        {
            ContainsAny = new[] { MechValidationRules.LanceTag_Skirmish }
        }
    };

    [JsonProperty]
    internal const string SkirmishOptionsShowDescription = "Shows or hides the skirmish options panel before entering the skirmish 'Mech bay.";
    [JsonProperty]
    internal bool SkirmishOptionsShow = false;

    [JsonProperty]
    internal const string SkirmishPresetsDescription = "Presets allow to quickly select a custom filter-combination.";
    [JsonProperty(Required = Required.DisallowNull)]
    internal TagsFilterSet[]? SkirmishOptionsPresets =
    {
        new()
        {
            Label = "Stock",
            Components = new()
            {
                ContainsAny = new[] { MechValidationRules.ComponentTag_Stock },
                NotContainsAny = new[] { MechValidationRules.Tag_Blacklisted }
            },
            Mechs = new()
            {
                ContainsAny = new[] { MechValidationRules.MechTag_Released },
                NotContainsAny = new[] { MechValidationRules.Tag_Blacklisted, MechValidationRules.MechTag_Unlocked }
            }
        },
        new()
        {
            Label = "Variants",
            Components = new()
            {
                ContainsAny = new[] { MechValidationRules.ComponentTag_Stock, MechValidationRules.ComponentTag_Variant, MechValidationRules.ComponentTag_LosTech },
                NotContainsAny = new[] { MechValidationRules.Tag_Blacklisted }
            },
            Mechs = new()
            {
                ContainsAny = new[] { MechValidationRules.MechTag_Released },
                NotContainsAny = new[] { MechValidationRules.Tag_Blacklisted, MechValidationRules.MechTag_Unlocked }
            }
        }
    };

    [JsonProperty]
    internal const string SkirmishOptionsDefaultDescription = "Filters that are always active regardless of what the user selects in the options panel.";
    [JsonProperty(Required = Required.DisallowNull)]
    internal TagsFilterSet SkirmishOptionsDefault = new()
    {
        Components = new()
        {
            ContainsAny = new string[] { },
            NotContainsAny = new[] { MechValidationRules.Tag_Blacklisted }
        },
        Mechs = new()
        {
            ContainsAny = new[] { MechValidationRules.MechTag_Released },
            NotContainsAny = new[] { MechValidationRules.Tag_Blacklisted, MechValidationRules.MechTag_Unlocked }
        },
        Pilots = new()
        {
            ContainsAny = new[] { MechValidationRules.PilotTag_Released }
        },
        Lances = new()
        {
            ContainsAny = new[] { MechValidationRules.LanceTag_Skirmish }
        }
    };

    [JsonProperty]
    internal const string SkirmishOptionsComponentDescription =
        "Component options that can be selected in the options panel, they don't influence what is loaded into the 'Mech bay only what is shown later on in the 'Mech lab inventory." +
        " (Alpha) All active ContainsAny are combined with an OR, all active NotContainsAny are combined with an OR." +
        " Afterwards NotContainsAny has anything removed that exists in ContainsAny." +
        " Then anything from the defaults are added to each." +
        " Then the ContainsAny and NotContainsAny are combined with an AND.";
    [JsonProperty]
    internal TagOptionsGroup? SkirmishOptionsComponentGroup = new()
    {
        Label = "Components",
        Options = new TagOption[]
        {
            new()
            {
                Label = "Stock",
                ContainsAny = new[] { MechValidationRules.ComponentTag_Stock },
                NotContainsAny = new[] { MechValidationRules.ComponentTag_Variant, MechValidationRules.ComponentTag_LosTech },
                OptionActive = true
            },
            new()
            {
                Label = "Variants & LosTech",
                ContainsAny = new[] { MechValidationRules.ComponentTag_Variant, MechValidationRules.ComponentTag_LosTech },
                NotContainsAny = new[] { MechValidationRules.ComponentTag_Stock },
                OptionActive = true
            }
        }
    };

    [JsonProperty]
    internal const string SkirmishOptionsMechGroupsDescription =
        "Filter which 'Mech get loaded. Each group is combined with an AND, each toggle within a group is combined with an OR.";
    [JsonProperty]
    internal TagOptionsGroup[]? SkirmishOptionsMechGroups =
    {
        new()
        {
            Label = "Tonnage",
            Options = new TagOption[]
            {
                new()
                {
                    Label = "Light",
                    ContainsAny = new[] { "unit_light" },
                    OptionActive = true
                },
                new()
                {
                    Label = "Medium",
                    ContainsAny = new[] { "unit_medium" },
                    OptionActive = true
                },
                new()
                {
                    Label = "Heavy",
                    ContainsAny = new[] { "unit_heavy" },
                    OptionActive = true,
                    OptionBreakLineBefore = true
                },
                new()
                {
                    Label = "Assault",
                    ContainsAny = new[] { "unit_assault" },
                    OptionActive = true
                }
            }
        },
        new()
        {
            Label = "Source",
            Options = new TagOption[]
            {
                new()
                {
                    Label = "Base",
                    NotContainsAny = new[] { "unit_dlc" },
                    OptionActive = true
                },
                new()
                {
                    Label = "DLC",
                    ContainsAny = new[] { "unit_dlc" },
                    OptionActive = true
                }
            }
        }
    };

    internal class TagOptionsGroup
    {
        [JsonProperty(Required = Required.DisallowNull)]
        internal string Label = "<null>";

        [JsonProperty(Required = Required.DisallowNull)]
        internal TagOption[] Options = Array.Empty<TagOption>();
    }

    internal class TagOption
    {
        [JsonProperty(Required = Required.DisallowNull)]
        internal string Label = "<null>";

        [JsonProperty]
        internal string[]? ContainsAny;

        [JsonProperty]
        internal string[]? NotContainsAny;

        [JsonProperty]
        internal bool OptionActive = false;

        [JsonProperty]
        internal bool OptionBreakLineBefore = false;
    }

    internal class TagsFilterSet
    {
        [JsonProperty(Required = Required.DisallowNull)]
        internal string Label = "<null>";

        [JsonProperty(Required = Required.DisallowNull)]
        internal TagsFilter Components = new();

        [JsonProperty(Required = Required.DisallowNull)]
        internal TagsFilter Mechs = new();

        [JsonProperty(Required = Required.DisallowNull)]
        internal TagsFilter Pilots = new();

        [JsonProperty(Required = Required.DisallowNull)]
        internal TagsFilter Lances = new();
    }
    internal class TagsFilter
    {
        [JsonProperty]
        internal string[]? ContainsAny;

        [JsonProperty]
        internal string[]? NotContainsAny;

        [JsonIgnore]
        internal string? OptionsSearch;

        [JsonIgnore]
        internal TagOptionsGroup[]? OptionsGroups;
    }

    [JsonProperty(Required = Required.DisallowNull)]
    internal TagsTransformer Components = new();

    [JsonProperty(Required = Required.DisallowNull)]
    internal TagsTransformer Mechs = new();

    [JsonProperty(Required = Required.DisallowNull)]
    internal TagsTransformer Pilots = new();

    [JsonProperty(Required = Required.DisallowNull)]
    internal TagsTransformer Lances = new();

    internal class TagsTransformer
    {
        public string[] Whitelist = { };
        public string[] Blacklist = { };
    }
}