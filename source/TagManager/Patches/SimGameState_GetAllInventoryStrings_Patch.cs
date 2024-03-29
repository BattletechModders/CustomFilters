﻿#nullable disable
// ReSharper disable InconsistentNaming
using BattleTech;
using HBS.Data;

namespace CustomFilters.TagManager.Patches;

[HarmonyPatch(typeof(SimGameState), nameof(SimGameState.GetAllInventoryStrings))]
public static class SimGameState_GetAllInventoryStrings_Patch
{
    public static bool Prepare()
    {
        return TagManagerFeature.Settings.Enabled && TagManagerFeature.Settings.SimGameItemsMinCount > 0;
    }

    [HarmonyPrefix]
    [HarmonyWrapSafe]
    public static void Prefix(ref bool __runOriginal, SimGameState __instance)
    {
        if (!__runOriginal)
        {
            return;
        }

        var feature = TagManagerFeature.Shared;
        var state = __instance;
        var minCount = TagManagerFeature.Settings.SimGameItemsMinCount;

        void AddApplicable<T>(DictionaryStore<T> store) where T : MechComponentDef, new()
        {
            foreach (var def in store.items.Values)
            {
                if (!feature.ComponentIsValidForSkirmish(def, false))
                {
                    continue;
                }

                var id = state.GetItemStatID(def.Description.Id, SimGameState.GetTypeFromComponent(def.ComponentType));
                if (state.companyStats.ContainsStatistic(id))
                {
                    var count = state.companyStats.GetValue<int>(id);
                    if (count < minCount)
                    {
                        state.companyStats.ModifyStat("SimGameState", 0, id, StatCollection.StatOperation.Set, minCount);
                    }
                }
                else
                {
                    state.companyStats.AddStatistic(id, minCount);
                }
            }
        }

        var dataManager = UnityGameInstance.BattleTechGame.DataManager;
        AddApplicable(dataManager.ammoBoxDefs);
        AddApplicable(dataManager.heatSinkDefs);
        AddApplicable(dataManager.jumpJetDefs);
        AddApplicable(dataManager.upgradeDefs);
        AddApplicable(dataManager.weaponDefs);

        foreach (var def in dataManager.mechDefs.items.Values)
        {
            if (!feature.MechIsValidForSkirmish(def, false))
            {
                continue;
            }

            var id = state.GetItemStatID(def.Description.Id, "MECHPART");
            if (state.companyStats.ContainsStatistic(id))
            {
                var count = state.companyStats.GetValue<int>(id);
                if (count < minCount)
                {
                    state.companyStats.ModifyStat("SimGameState", 0, id, StatCollection.StatOperation.Set, minCount);
                }
            }
            else
            {
                state.companyStats.AddStatistic(id, minCount);
            }
        }
    }
}