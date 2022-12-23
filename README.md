# CustomFilters

Features:
- TODO MechBayFiltering: Allow to specify a custom set of filters similar to MechLabFiltering.
- MechBayScrolling: Improves the performance when scrolling large amounts of mechs (4000+).
  This allows large mod packs like RogueTech to smoothly scroll through 4000+ mechs in the Skirmish MechBay.
  - When CustomUnits is detected, the SimGame Lance Configuration wil be ignored and not patched.
    The amount of mechs in SimGame is low and thus this is not a big issue.
- MechBaySorting: Sorts mechs within the Skirmish MechBay. For now only via json settings.
  - TODO: Allow UI controls to change sort order dynamically.
- MechLabFiltering: Allows to customize the tabs and filters using to list the components in the MechLab.
  This is the original feature of CustomFilters.
  - Optionally can work with CustomComponents and its Categories feature.
  - ❌ If CustomComponents is loaded, it has to be the latest version, older version will break.
- MechLabScrolling: Incorporates own version of MechLab performance fix from BattletechPerformanceFix.
  Fixes the performance issue when too many items are listed in the MechLab.
  - ❌ You must disable the MechLabFixFeature from BattletechPerformanceFix.
- MechLabShopPooling: Fixes a memory leak with a shop being instantiated but never properly collected.
  - ❌ You must disable the MechLabFixFeature from BattletechPerformanceFix.
- TagManager: Allows to modify component and mech tags, and their interpretation in Skirmish and SimGame.
  Also allows to activate a filtering UI that appears before entering the Skirmish MechBay
  to reduce which mechs are loaded and what components are listed in the MechLab. Originally moved from MechEngineer.
