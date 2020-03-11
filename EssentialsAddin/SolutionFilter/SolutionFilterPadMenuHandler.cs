﻿using MonoDevelop.Components.Commands;
using MonoDevelop.Ide;
using MonoDevelop.Ide.Gui;

namespace EssentialsAddin.SolutionFilter
{
    public class SolutionFilterPadMenuHandler : CommandHandler
    {
        protected override void Run()
        {
            var pad = IdeApp.Workbench.GetPad<SolutionFilterPad>();
            if (pad !=null)
            {
                pad.Visible = true;
                pad.IsOpenedAutomatically = true;
                pad.BringToFront(true);
            }
        }

        protected override void Update(CommandInfo info)
        {
            info.Enabled = true;//IdeApp.Workbench.ActiveDocument?.Editor != null;
        }
    }
}
