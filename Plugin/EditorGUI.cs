using KSP.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Trajectories
{
    /// <summary> EditorGUI window handler. </summary>
    [KSPAddon(KSPAddon.Startup.EditorAny, false)]
    class EditorGUI : MonoBehaviour
    {
        public static EditorGUI Instance;
        private List<GUIWindowData> windows = new List<GUIWindowData>();

        public void Awake()
        {
            if (Instance)
                Destroy(Instance);
            Instance = this;
        }

        public void OnDestroy()
        {
            foreach(var w in windows)
            {
                w.Close();
            }
        }

        public void OnGUI()
        {
            for (int i = windows.Count - 1; i >= 0; i--)
            {
                GUIWindowData window = windows[i];
                window.windowRect = GUILayout.Window(window.GuiId, window.windowRect, window.Interface,
                    "Descent Profile Settings - " + window.module.part.name,
                    HighLogic.Skin.window, GUILayout.MinWidth(300), GUILayout.MinHeight(200));
            }
        }

        public static void OpenWindow(TrajectoriesVesselSettings module)
        {
            if (!HighLogic.LoadedSceneIsEditor)
                return;

            GUIWindowData window = new GUIWindowData(module);
            Instance.windows.Add(window);
        }

        public static void CloseWindow(TrajectoriesVesselSettings module)
        {
            if (!HighLogic.LoadedSceneIsEditor)
                return;

            GUIWindowData window = Instance.windows.Find(w => w.module == module);
            if (window != null)
            {
                window.Close();
                Instance.windows.Remove(window);
            }
            else
                Debug.Log("Tried closing a window that isn't open.");
        }

        public static void ToggleWindow(TrajectoriesVesselSettings module)
        {
            if (!HighLogic.LoadedSceneIsEditor)
                return;

            if (Instance.windows.Any(w => w.module == module))
                CloseWindow(module);
            else
                OpenWindow(module);
        }

        public static GUIWindowData GetWindow(TrajectoriesVesselSettings module)
        {
            return Instance.windows.Find(w => w.module == module);
        }

        public class GUIWindowData
        {
            public Rect windowRect;
            public TrajectoriesVesselSettings module;
            public int GuiId;

            private float EntrySliderPos;
            private float HighSliderPos;
            private float LowSliderPos;
            private float GroundSliderPos;

            private const float slider_width = 170.0f;

            public GUIWindowData(TrajectoriesVesselSettings module)
            {
                this.module = module;
                RefreshSliders();
                this.windowRect = new Rect(Screen.width * 2 / 3, Screen.height / 2, 300, 200);
                GuiId = GUIUtility.GetControlID(FocusType.Passive);
            }

            public void Interface(int GuiId)
            {
                bool oldToggle;
                float oldSliderPos;

                GUILayout.BeginVertical();

                // Pro/Retrograde Toggles:
                GUILayout.BeginHorizontal();
                oldToggle = module.ProgradeEntry;
                module.ProgradeEntry = GUILayout.Toggle(module.ProgradeEntry, Localizer.Format("#autoLOC_900597"));
                if (oldToggle != module.ProgradeEntry)
                {
                    if (module.ProgradeEntry)
                        Reset(0d);
                }

                oldToggle = module.RetrogradeEntry;
                module.RetrogradeEntry = GUILayout.Toggle(module.RetrogradeEntry, Localizer.Format("#autoLOC_900607"));
                if (oldToggle != module.RetrogradeEntry)
                {
                    if (module.RetrogradeEntry)
                        Reset();
                }
                GUILayout.EndHorizontal();

                // Entry Settings:
                GUILayout.BeginHorizontal();
                GUILayout.Label(Localizer.Format("#autoLOC_Trajectories_Entry"));
                oldToggle = module.EntryHorizon;
                module.EntryHorizon = GUILayout.Toggle(module.EntryHorizon, module.EntryHorizon ? "Horiz" : "AoA", GUILayout.Width(60f));
                if (oldToggle != module.EntryHorizon)
                    CheckGUI();
                oldSliderPos = EntrySliderPos;
                EntrySliderPos = GUILayout.HorizontalSlider(EntrySliderPos, -1, 1, GUILayout.Width(slider_width));
                if (oldSliderPos != EntrySliderPos)
                {
                    module.EntryAngle = SliderPosToAngle(EntrySliderPos);
                    CheckGUI();
                }
                GUILayout.Label(Angle_txt(module.EntryAngle), GUILayout.Width(36f));
                GUILayout.EndHorizontal();

                // High Settings:
                GUILayout.BeginHorizontal();
                GUILayout.Label(Localizer.Format("#autoLOC_Trajectories_High"));
                oldToggle = module.HighHorizon;
                module.HighHorizon = GUILayout.Toggle(module.HighHorizon, module.HighHorizon ? "Horiz" : "AoA", GUILayout.Width(60f));
                if (oldToggle != module.HighHorizon)
                    CheckGUI();
                oldSliderPos = HighSliderPos;
                HighSliderPos = GUILayout.HorizontalSlider(HighSliderPos, -1, 1, GUILayout.Width(slider_width));
                if (oldSliderPos != HighSliderPos)
                {
                    module.HighAngle = SliderPosToAngle(HighSliderPos);
                    CheckGUI();
                }
                GUILayout.Label(Angle_txt(module.HighAngle), GUILayout.Width(36f));
                GUILayout.EndHorizontal();

                // Entry Settings:
                GUILayout.BeginHorizontal();
                GUILayout.Label(Localizer.Format("#autoLOC_Trajectories_Low"));
                oldToggle = module.LowHorizon;
                module.LowHorizon = GUILayout.Toggle(module.LowHorizon, module.LowHorizon ? "Horiz" : "AoA", GUILayout.Width(60f));
                if (oldToggle != module.LowHorizon)
                    CheckGUI();
                oldSliderPos = LowSliderPos;
                LowSliderPos = GUILayout.HorizontalSlider(LowSliderPos, -1, 1, GUILayout.Width(slider_width));
                if (oldSliderPos != LowSliderPos)
                {
                    module.LowAngle = SliderPosToAngle(LowSliderPos);
                    CheckGUI();
                }
                GUILayout.Label(Angle_txt(module.LowAngle), GUILayout.Width(36f));
                GUILayout.EndHorizontal();

                // Entry Settings:
                GUILayout.BeginHorizontal();
                GUILayout.Label(Localizer.Format("#autoLOC_Trajectories_Ground"));
                oldToggle = module.GroundHorizon;
                module.GroundHorizon = GUILayout.Toggle(module.GroundHorizon, module.GroundHorizon ? "Horiz" : "AoA", GUILayout.Width(60f));
                if (oldToggle != module.GroundHorizon)
                    CheckGUI();
                oldSliderPos = GroundSliderPos;
                GroundSliderPos = GUILayout.HorizontalSlider(GroundSliderPos, -1, 1, GUILayout.Width(slider_width));
                if (oldSliderPos != GroundSliderPos)
                {
                    module.GroundAngle = SliderPosToAngle(GroundSliderPos);
                    CheckGUI();
                }
                GUILayout.Label(Angle_txt(module.GroundAngle), GUILayout.Width(36f));
                GUILayout.EndHorizontal();

                // Other buttons:
                if (GUILayout.Button("Apply to symmetry counterparts"))
                {
                    foreach(Part p in module.part.symmetryCounterparts)
                    {
                        TrajectoriesVesselSettings symmetryModule = p.FindModulesImplementing<TrajectoriesVesselSettings>().FirstOrDefault();
                        if (symmetryModule == null)
                            continue;
                        symmetryModule.ProgradeEntry = module.ProgradeEntry;
                        symmetryModule.RetrogradeEntry = module.RetrogradeEntry;

                        symmetryModule.EntryHorizon = module.EntryHorizon;
                        symmetryModule.EntryAngle = module.EntryAngle;
                        symmetryModule.HighHorizon = module.HighHorizon;
                        symmetryModule.HighAngle = module.HighAngle;
                        symmetryModule.LowHorizon = module.LowHorizon;
                        symmetryModule.LowAngle = module.LowAngle;
                        symmetryModule.GroundHorizon = module.GroundHorizon;
                        symmetryModule.GroundAngle = module.GroundAngle;
                        symmetryModule.Initialized = module.Initialized;

                        GUIWindowData symmetryWindow = EditorGUI.GetWindow(symmetryModule);
                        if (symmetryWindow != null)
                            symmetryWindow.RefreshSliders();
                    }
                }
                if (GUILayout.Button("Close"))
                {
                    EditorGUI.CloseWindow(module);
                }

                GUILayout.EndVertical();
            }

            public void CheckGUI()
            {
                double? AoA = module.EntryHorizon ? (double?)null : module.EntryAngle;

                if (module.HighAngle != AoA || module.HighHorizon)
                    AoA = null;
                if (module.LowAngle != AoA || module.LowHorizon)
                    AoA = null;
                if (module.GroundAngle != AoA || module.GroundHorizon)
                    AoA = null;

                if (!AoA.HasValue)
                {
                    module.ProgradeEntry = false;
                    module.RetrogradeEntry = false;
                }
                else
                {
                    if (Math.Abs(AoA.Value) < 0.00001)
                        module.ProgradeEntry = true;
                    if (Math.Abs((Math.Abs(AoA.Value) - Math.PI)) < 0.00001)
                        module.RetrogradeEntry = true;
                }

                module.Initialized = !(Settings.fetch.DefaultDescentIsRetro ? module.RetrogradeEntry : module.ProgradeEntry);
            }

            public void Reset(double AoA = Math.PI)
            {
                module.EntryAngle = AoA;
                module.EntryHorizon = false;
                module.HighAngle = AoA;
                module.HighHorizon = false;
                module.LowAngle = AoA;
                module.LowHorizon = false;
                module.GroundAngle = AoA;
                module.GroundHorizon = false;

                module.ProgradeEntry = AoA == 0d;
                module.RetrogradeEntry = AoA == Math.PI;

                module.Initialized = !(Settings.fetch.DefaultDescentIsRetro ? module.RetrogradeEntry : module.ProgradeEntry);

                RefreshSliders();
            }

            public void RefreshSliders()
            {
                EntrySliderPos = AngleToSliderPos(module.EntryAngle);
                HighSliderPos = AngleToSliderPos(module.HighAngle);
                LowSliderPos = AngleToSliderPos(module.LowAngle);
                GroundSliderPos = AngleToSliderPos(module.GroundAngle);
            }

            private static float AngleToSliderPos(double Angle)
            {
                float position = (float)Math.Pow(Math.Abs(Angle) / Math.PI, 1d / 3d);
                if (Angle < 0d)
                    return -position;
                else
                    return position;
            }

            private static double SliderPosToAngle(double sliderPos)
            {
                return sliderPos * sliderPos * sliderPos * Math.PI;
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

            public void Close()
            {

            }
        }
    }
}
