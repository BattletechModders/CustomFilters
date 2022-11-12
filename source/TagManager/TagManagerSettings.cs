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
    internal const string SimGameItemsMinCountDescription = $"Set the owned minimum count of each mech component in SimGame.";
    [JsonProperty]
    internal int SimGameItemsMinCount = 0;

    [JsonProperty]
    internal const string LostechStockWeaponVariantDescription = "Fixes lostech variant weapon tagging by checking if id ends with -STOCK.";
    [JsonProperty]
    internal bool LostechStockWeaponVariantFix = false;

    [JsonProperty]
    internal const string SkirmishOverloadWarningDescription = "Warn the user before loading into the SkirmishMechBay if too many 'Mech will be loaded.";
    [JsonProperty]
    internal int SkirmishOverloadWarningCount = 500;

    [JsonProperty]
    internal const string SkirmishDefaultDescription = "The default settings used when no options panel is shown and the user enters the skirmish 'Mech bay directly.";
    [JsonProperty]
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
    [JsonProperty]
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
    [JsonProperty]
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
        public string Label = "<null>";
        public TagOption[] Options = Array.Empty<TagOption>();
    }

    internal class TagOption
    {
        public string Label = "<null>";
        public string[]? ContainsAny;
        public string[]? NotContainsAny;
        public bool OptionActive = false;
        public bool OptionBreakLineBefore = false;
    }

    internal class TagsFilterSet
    {
        public string Label = "<null>";
        public TagsFilter Components = new();
        public TagsFilter Mechs = new();
        public TagsFilter Pilots = new();
        public TagsFilter Lances = new();
    }
    internal class TagsFilter
    {
        public string[]? ContainsAny;
        public string[]? NotContainsAny;
        [fastJSON.JsonIgnore]
        internal string? OptionsSearch;
        [fastJSON.JsonIgnore]
        internal TagOptionsGroup[]? OptionsGroups;
    }

    [JsonProperty]
    internal TagsTransformer Components = new();
    [JsonProperty]
    internal TagsTransformer Mechs = new();
    [JsonProperty]
    internal TagsTransformer Pilots = new();
    [JsonProperty]
    internal TagsTransformer Lances = new();

    internal class TagsTransformer
    {
        public string[] Whitelist = { };
        public string[] Blacklist = { };
    }
}