using KSP.Localization;
using System;
using UnityEngine;

namespace Trajectories
{
    class TrajectoriesVesselSettings: PartModule
    {
        [KSPField(isPersistant = true, guiActive = false)]
        public bool Initialized = false;

        [KSPField(isPersistant = true, guiActive = false)]
        public double EntryAngle = Math.PI;

        [KSPField(isPersistant = true, guiActive = false)]
        public bool EntryHorizon;

        [KSPField(isPersistant = true, guiActive = false)]
        public double HighAngle = Math.PI;

        [KSPField(isPersistant = true, guiActive = false)]
        public bool HighHorizon;

        [KSPField(isPersistant = true, guiActive = false)]
        public double LowAngle = Math.PI;

        [KSPField(isPersistant = true, guiActive = false)]
        public bool LowHorizon;

        [KSPField(isPersistant = true, guiActive = false)]
        public double GroundAngle = Math.PI;

        [KSPField(isPersistant = true, guiActive = false)]
        public bool GroundHorizon;

        [KSPField(isPersistant = true, guiActive = false)]
        public bool ProgradeEntry;

        [KSPField(isPersistant = true, guiActive = false)]
        public bool RetrogradeEntry = true;

        [KSPField(isPersistant = true, guiActive = false)]
        public string TargetBody = "";

        [KSPField(isPersistant = true, guiActive = false)]
        public double TargetPosition_x = 0;

        [KSPField(isPersistant = true, guiActive = false)]
        public double TargetPosition_y = 0;

        [KSPField(isPersistant = true, guiActive = false)]
        public double TargetPosition_z = 0;

        [KSPField(isPersistant = true, guiActive = false)]
        public string ManualTargetTxt = "";

        [KSPEvent(guiName = "Configure Reentry Profile", guiActiveEditor = true, guiActive = false)]
        public void ConfigureEvent()
        {
            EditorGUI.ToggleWindow(this);
        }

        public override void OnAwake()
        {
            base.OnAwake();
            if (!HighLogic.LoadedSceneIsEditor)
            {
                Events["ConfigureEvent"].guiActive = false;
                Events["ConfigureEvent"].active = false;
            }
        }
    }
}
