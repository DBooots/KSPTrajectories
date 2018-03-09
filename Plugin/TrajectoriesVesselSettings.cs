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
            if (HighLogic.LoadedSceneIsEditor)
                if (!showGUI)
                {
                    popup_dialog = PopupDialog.SpawnPopupDialog(multi_dialog, true, HighLogic.UISkin, false, "");
                    showGUI = true;
                    this.Initialized = true;
                }
            else
                {
                    popup_dialog.Dismiss();
                    showGUI = false;
                }
        }

        private DialogGUIVerticalLayout descent_page;
        private static DialogGUIBox page_box;
        private static MultiOptionDialog multi_dialog;
        private static PopupDialog popup_dialog;
        private const float width = 370.0f;
        private const float height = 285.0f;

        public override void OnAwake()
        {
            base.OnAwake();
            if (!HighLogic.LoadedSceneIsEditor)
            {
                showGUI = false;
                Events["ConfigureEvent"].guiActive = false;
                Events["ConfigureEvent"].active = false;
            }
            else
            {

                GUILayout.BeginVertical();
                descent_page = new DialogGUIVerticalLayout(false, true, 0, new RectOffset(), TextAnchor.UpperCenter,
                    new DialogGUIHorizontalLayout(false, false, 0, new RectOffset(), TextAnchor.MiddleCenter,
                        new DialogGUIToggle(() => { return this.ProgradeEntry; },
                            Localizer.Format("#autoLOC_900597"), OnButtonClick_Prograde),
                        new DialogGUIToggle(() => { return this.RetrogradeEntry; },
                            Localizer.Format("#autoLOC_900607"), OnButtonClick_Retrograde)),
                    new DialogGUIHorizontalLayout(
                        new DialogGUILabel(Localizer.Format("#autoLOC_Trajectories_Entry"), true),
                        new DialogGUIToggle(() => { return this.EntryHorizon; },
                            () => { return this.EntryHorizon ? "Horiz" : "AoA"; }, OnButtonClick_EntryHorizon, 60f),
                        new DialogGUISlider(() => { return this.EntrySliderPos; },
                            -1f, 1f, false, slider_width, -1, OnSliderSet_EntryAngle),
                        new DialogGUILabel(() => { return Angle_txt(EntryAngle); }, 36f)),
                    new DialogGUIHorizontalLayout(
                        new DialogGUILabel(Localizer.Format("#autoLOC_Trajectories_High"), true),
                        new DialogGUIToggle(() => { return this.HighHorizon; },
                            () => { return this.HighHorizon ? "Horiz" : "AoA"; }, OnButtonClick_HighHorizon, 60f),
                        new DialogGUISlider(() => { return this.HighSliderPos; },
                            -1f, 1f, false, slider_width, -1, OnSliderSet_HighAngle),
                        new DialogGUILabel(() => { return Angle_txt(HighAngle); }, 36f)),
                    new DialogGUIHorizontalLayout(
                        new DialogGUILabel(Localizer.Format("#autoLOC_Trajectories_Low"), true),
                        new DialogGUIToggle(() => { return this.LowHorizon; },
                            () => { return this.LowHorizon ? "Horiz" : "AoA"; }, OnButtonClick_LowHorizon, 60f),
                        new DialogGUISlider(() => { return this.LowSliderPos; },
                            -1f, 1f, false, slider_width, -1, OnSliderSet_LowAngle),
                        new DialogGUILabel(() => { return Angle_txt(LowAngle); }, 36f)),
                    new DialogGUIHorizontalLayout(
                        new DialogGUILabel(Localizer.Format("#autoLOC_Trajectories_Ground"), true),
                        new DialogGUIToggle(() => { return this.GroundHorizon; },
                            () => { return this.GroundHorizon ? "Horiz" : "AoA"; }, OnButtonClick_GroundHorizon, 60f),
                        new DialogGUISlider(() => { return this.GroundSliderPos; },
                            -1f, 1f, false, slider_width, -1, OnSliderSet_GroundAngle),
                        new DialogGUILabel(() => { return Angle_txt(GroundAngle); }, 36f))
                    );

                page_box = new DialogGUIBox(null, -1, -1, () => true, descent_page);

                multi_dialog = new MultiOptionDialog(
               "TrajectoriesMainGUI",
               "",
               Localizer.Format("#autoLOC_Trajectories_Title"),
               HighLogic.UISkin,
               // window origin is center of rect, position is offset from lower left corner of screen and normalized
               // i.e (0.5, 0.5 is screen center)
               new Rect(Settings.fetch.MainGUIWindowPos.x, Settings.fetch.MainGUIWindowPos.y, width, height),
               new DialogGUIBase[]
               {
                   // insert page box
                   page_box
               });

            }
        }

        private bool showGUI = false;
        private Rect windowRect;

        public void OnGUI()
        {
            if (!showGUI) return;
            if (!HighLogic.LoadedSceneIsEditor) return;

            windowRect = GUILayout.Window(GUIUtility.GetControlID(FocusType.Passive), windowRect, ConfigureWindow, "Descent Profile");
        }

        private void ConfigureWindow(int GuiId)
        {

        }

        private static string Angle_txt(double angle)
        {
            string Angle_txt;

            if (Math.Abs(angle) < 0.00001)
                angle = 0d;

            double calc_angle = angle * 180.0 / Math.PI;
            if (calc_angle <= -100d || calc_angle >= 100d)
                Angle_txt = calc_angle.ToString("F1") + "°";
            else if (calc_angle <= -10d || calc_angle >= 10d)
                Angle_txt = calc_angle.ToString("F2") + "°";
            else
                Angle_txt = calc_angle.ToString("F3") + "°";

            return Angle_txt;
        }

        private const float slider_width = 170.0f;

        private void OnButtonClick_Prograde(bool inState)
        {
            if (inState != this.ProgradeEntry)
            {
                this.ProgradeEntry = inState;
                if (inState)
                    this.Reset(0d);
            }
        }

        private void OnButtonClick_Retrograde(bool inState)
        {
            if (inState != this.RetrogradeEntry)
            {
                this.RetrogradeEntry = inState;
                if (inState)
                    this.Reset();
            }
        }

        private void OnButtonClick_EntryHorizon(bool inState)
        {
            this.EntryHorizon = inState;
            DescentProfile.fetch.CheckGUI();
        }

        private float EntrySliderPos;

        private void OnSliderSet_EntryAngle(float invalue)
        {
            this.EntryAngle = invalue * invalue * invalue * Math.PI;
            this.EntrySliderPos = invalue;
            DescentProfile.fetch.CheckGUI();
        }

        private void OnButtonClick_HighHorizon(bool inState)
        {
            this.HighHorizon = inState;
            DescentProfile.fetch.CheckGUI();
        }

        private float HighSliderPos;

        private void OnSliderSet_HighAngle(float invalue)
        {
            this.HighAngle = invalue * invalue * invalue * Math.PI;
            this.HighSliderPos = invalue;
            DescentProfile.fetch.CheckGUI();
        }

        private void OnButtonClick_LowHorizon(bool inState)
        {
            this.LowHorizon = inState;
            DescentProfile.fetch.CheckGUI();
        }

        private float LowSliderPos;

        private void OnSliderSet_LowAngle(float invalue)
        {
            this.LowAngle = invalue * invalue * invalue * Math.PI;
            this.LowSliderPos = invalue;
            DescentProfile.fetch.CheckGUI();
        }

        private void OnButtonClick_GroundHorizon(bool inState)
        {
            this.GroundHorizon = inState;
            DescentProfile.fetch.CheckGUI();
        }

        private float GroundSliderPos;

        private void OnSliderSet_GroundAngle(float invalue)
        {
            this.GroundAngle = invalue * invalue * invalue * Math.PI;
            this.GroundSliderPos = invalue;
            DescentProfile.fetch.CheckGUI();
        }

        public void Reset(double AoA = Math.PI)
        {
            //Debug.Log(string.Format("Resetting vessel descent profile to {0} degrees", AoA));
            EntryAngle = AoA;
            EntryHorizon = false;
            HighAngle = AoA;
            HighHorizon = false;
            LowAngle = AoA;
            LowHorizon = false;
            GroundAngle = AoA;
            GroundHorizon = false;

            ProgradeEntry = AoA == 0d;
            RetrogradeEntry = AoA == Math.PI;

            RefreshSliders();
        }

        public void RefreshSliders()
        {
            EntrySliderPos = getSliderPos(EntryAngle);
            HighSliderPos = getSliderPos(HighAngle);
            LowSliderPos = getSliderPos(LowAngle);
            GroundSliderPos = getSliderPos(GroundAngle);
        }

        private float getSliderPos(double Angle)
        {
            float SliderPos;
            float position = (float)Math.Pow(Math.Abs(Angle) / Math.PI, 1d / 3d);
            if (Angle < 0d)
                SliderPos = -position;
            else
                SliderPos = position;
            return SliderPos;
        }
    }
}
