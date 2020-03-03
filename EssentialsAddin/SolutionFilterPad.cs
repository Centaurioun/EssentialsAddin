﻿using MonoDevelop.Components;
using MonoDevelop.Ide.Gui;

namespace EssentialsAddin
{
    public class SolutionFilterPad : PadContent
    {
        public override Control Control
            => control ?? (control = new SolutionFilterWidget());
        SolutionFilterWidget control;

        public override string Id => "EssentialsAddin.SolutionFilterPad";

        public static string PROPERTY_KEY = "EssentialsAddin.SolutionFilterPad.Filter";

        protected override void Initialize(IPadWindow window)
        {
            base.Initialize(window);
            //StartListeningForWorkspaceChanges();
            //window.PadHidden += (sender, e) => control.SaveNodeLocationsForSelectedProject();

            //Debug.WriteLine($"Bundle path: {NSBundle.MainBundle.BundlePath}");
            //Debug.WriteLine($"Bundle Resource path: {NSBundle.MainBundle.ResourcePath}");
        }

        void StartListeningForWorkspaceChanges()
        {
            //IdeApp.Workspace.SolutionUnloaded += (sender, e) => control.Clear();
            //IdeApp.Workspace.SolutionLoaded += (sender, e) => control.ReloadProjects();
            //IdeApp.Workspace.ItemAddedToSolution += (sender, e) => control.ReloadProjects();
            //IdeApp.Workspace.ItemRemovedFromSolution += (sender, e) => control.ReloadProjects();
        }
    }
}
