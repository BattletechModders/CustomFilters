using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BattleTech.UI;
using Harmony;
using UnityEngine;

namespace CustomFilters
{
    public class InventoryHelper
    {
        public MechLabInventoryWidget Widget { get; private set; }

        private Traverse main;

        public Traverse<HBSDOTweenToggle> tabAllToggleObj;
        public Traverse<HBSDOTweenToggle> tabWeaponsToggleObj;
        public Traverse<HBSDOTweenToggle> tabEquipmentToggleObj;
        public Traverse<HBSDOTweenToggle> tabMechPartToggleObj;

        public Traverse<HBSRadioSet> filterRadioSet;

        public Traverse<GameObject> filterBtnAll;
        public Traverse<GameObject> filterBtnWeaponBallistic;
        public Traverse<GameObject> filterBtnWeaponEnergy;
        public Traverse<GameObject> filterBtnWeaponMissile;
        public Traverse<GameObject> filterBtnWeaponSmall;
        public Traverse<GameObject> filterBtnEquipmentJumpjet;
        public Traverse<GameObject> filterBtnEquipmentHeatsink;
        public Traverse<GameObject> filterBtnEquipmentUpgrade;

        public InventoryHelper(MechLabInventoryWidget widget)
        {
            Widget = widget;
            main = new Traverse(widget);

            tabAllToggleObj = main.Field<HBSDOTweenToggle>("tabAllToggleObj");
            tabWeaponsToggleObj = main.Field<HBSDOTweenToggle>("tabWeaponsToggleObj");
            tabEquipmentToggleObj = main.Field<HBSDOTweenToggle>("tabEquipmentToggleObj");
            tabMechPartToggleObj = main.Field<HBSDOTweenToggle>("tabMechPartToggleObj");

            filterRadioSet = main.Field<HBSRadioSet>("filterRadioSet");


            filterBtnAll = main.Field<GameObject>("filterBtnAll");
            filterBtnWeaponBallistic = main.Field<GameObject>("filterBtnWeaponBallistic");
            filterBtnWeaponEnergy = main.Field<GameObject>("filterBtnWeaponEnergy");
            filterBtnWeaponMissile = main.Field<GameObject>("filterBtnWeaponMissile");
            filterBtnWeaponSmall = main.Field<GameObject>("filterBtnWeaponSmall");
            filterBtnEquipmentJumpjet = main.Field<GameObject>("filterBtnEquipmentJumpjet");
            filterBtnEquipmentHeatsink = main.Field<GameObject>("filterBtnEquipmentHeatsink");
            filterBtnEquipmentUpgrade = main.Field<GameObject>("filterBtnEquipmentUpgrade");

        }
    }
}
