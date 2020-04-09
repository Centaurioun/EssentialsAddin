﻿using System;
using System.Threading;
using EssentialsAddin.Helpers;
using EssentialsAddin.Services;
using EssentialsAddin.SolutionFilter;
using MonoDevelop.Core;
using MonoDevelop.Ide;
using MonoDevelop.Ide.Gui.Pads;
using Xwt.GtkBackend;

namespace EssentialsAddin
{
    [System.ComponentModel.ToolboxItem(true)]
    public partial class SolutionFilterWidget : Gtk.Bin
    {
        private Timer timer;

        private ReleaseService _releaseService = new ReleaseService();


        public SolutionFilterWidget()
        {
            this.Build();
            this.ShowAll();
            Setup();
        }

        private void Setup()
        {
            var pad = (SolutionPad)IdeApp.Workbench.Pads.SolutionPad.Content;
            if (pad == null)
                return;

            pad.TreeView.GrabFocus();
            var root = pad.TreeView.GetRootNode();
            if (root != null)
            {
                oneClickCheckbutton.Active = root.Options[FileNodeBuilderExtension.OneClickShowFileOption];
            }

            filterEntry.Text = EssentialProperties.SolutionFilter;
            collapseEntry.Text = EssentialProperties.ExpandFilter;

            newReleaseAvailableButton.SetBackgroundColor(Xwt.Drawing.Colors.DarkRed);
            newReleaseAvailableButton.Visible = false;
            CheckForNewRelease();
        }

        #region Events

        protected void OnFilterEntryChanged(object sender, EventArgs e)
        {
            StartTimer();
        }

        protected void oneClickCheckbutton_Toggled(object sender, EventArgs e)
        {
            EssentialProperties.OneClickShowFile = oneClickCheckbutton.Active;
            FilterSolutionPad();
        }

        protected void collapseButton_Clicked(object sender, EventArgs e)
        {
            var pad = (SolutionPad)IdeApp.Workbench.Pads.SolutionPad.Content;
            if (pad != null)
            {
                EssentialProperties.IsRefreshingTree = true;
                pad.TreeView.CollapseTree();
                var root = pad.TreeView.GetRootNode();
                if (root != null)
                {
                    root.Expanded = false;
                    pad.TreeView.RefreshNode(root);
                    root.Expanded = true;
                    SolutionTreeExtensions.ExpandAll(root);
                }
                EssentialProperties.IsRefreshingTree = false;
            }
            ExpandOnlyCSharpProjects();
        }

        protected void clearButton_Clicked(object sender, EventArgs e)
        {
            filterEntry.Text = "";
            FilterSolutionPad();
        }

        protected void OnEditingDone(object sender, EventArgs e)
        {
        }


        #endregion

        private void StartTimer()
        {
            StopTimer();
            timer = new Timer(OnTimerElapsed, null, 2000, Timeout.Infinite); // dueTime in miliseconds
        }

        private void StopTimer()
        {
            timer?.Dispose();
            timer = null;
        }

        object refreshLock = new Object();
        private void OnTimerElapsed(object state)
        {
            lock (refreshLock)
            {
                StopTimer();
                Runtime.RunInMainThread(FilterSolutionPad);
            }
        }

        private void FilterSolutionPad()
        {
            var SolutionPad = (SolutionFilterPad)IdeApp.Workbench.Pads.Find((p) => p.Id == Constants.SolutionFilterPadId).Content;
            if (SolutionPad != null)
                SolutionPad.Window.IsWorking = true;

            var ctx = IdeApp.Workbench.StatusBar.CreateContext();

            using (ctx)
            {
                ctx.AutoPulse = true;
                ctx.ShowMessage("Filtering...");
                ctx.Pulse();

                EssentialProperties.SolutionFilter = filterEntry.Text;

                if (string.IsNullOrEmpty(filterEntry.Text))
                {
                    ExpandOnlyCSharpProjects();
                    return;
                }

                var pad = (SolutionPad)IdeApp.Workbench.Pads.SolutionPad.Content;
                if (pad == null)
                    return;

                EssentialProperties.IsRefreshingTree = true;
                pad.TreeView.CollapseTree();
                var root = pad.TreeView.GetRootNode();
                if (root != null)
                {
                    root.Expanded = false;
                    pad.TreeView.RefreshNode(root);
                    root.Expanded = true;
                    SolutionTreeExtensions.ExpandAll(root);
                }
                EssentialProperties.IsRefreshingTree = false;
            }
            IdeApp.Workbench.StatusBar.ShowReady();
            //if (SolutionPad != null)
            //   SolutionPad.Window.IsWorking = false;
        }

        private void ExpandOnlyCSharpProjects()
        {
            EssentialProperties.ExpandFilter = collapseEntry.Text;

            var pad = (SolutionPad)IdeApp.Workbench.Pads.SolutionPad.Content;
            if (pad == null)
                return;

            EssentialProperties.IsRefreshingTree = true;
            pad.TreeView.CollapseTree();
            var root = pad.TreeView.GetRootNode();
            if (root != null)
            {
                root.Expanded = false;
                pad.TreeView.RefreshNode(root);
                root.Expanded = true;
                SolutionTreeExtensions.ExpandOnlyCSharpProjects(root);
            }
            EssentialProperties.IsRefreshingTree = false;
        }

        public void OnDocumentClosed()
        {
            if (IdeApp.Workbench.Documents is null || IdeApp.Workbench.Documents.Count == 0)
            {
                FilterSolutionPad();
            }
        }

        private async void CheckForNewRelease()
        {
            if (await _releaseService.CheckForNewRelease())
            {
                newReleaseAvailableButton.Visible = true;
            }
            else
            {
                newReleaseAvailableButton.Visible = false;
            }
        }

        protected void NewReleaseAvailableButton_Clicked(object sender, EventArgs e)
        {
            var url = _releaseService.LatestRelease.Url.OriginalString;
            try
            {
                System.Diagnostics.Process.Start(url);
            }
            catch (Exception)
            {
                string msg = $"Could not open the url {url}";
                MonoDevelop.Ide.MessageService.ShowError(((Gtk.Widget)sender).Toplevel as Gtk.Window, msg);
            }
        }
    }
}