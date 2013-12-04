using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using EnvDTE;
using EnvDTE80;

namespace SassyStudio
{
    static class InteropHelper
    {
        // Utility method from VSWebEssentials, to help out with TFS
        internal static void CheckOut(string file)
        {
            try
            {
                var dte = SassyStudioPackage.Instance.DTE;
                if (File.Exists(file) && dte.Solution.FindProjectItem(file) != null)
                {
                    if (dte.SourceControl.IsItemUnderSCC(file) && !dte.SourceControl.IsItemCheckedOut(file))
                        dte.SourceControl.CheckOutItem(file);
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex, "Failed to check out file.");
            }
        }

        internal static void AddNestedFile(DTE2 dte, string parent, string child, BuildActionType type)
        {
            // ignore if child doesn't exist
            if (!File.Exists(child)) return;
            if (dte == null) return;

            // if we can't load parent or child already part of solution, don't attempt to change anything
            ProjectItem parentItem, childItem;
            if (!TryGetProjectItem(dte.Solution, parent, out parentItem) || TryGetProjectItem(dte.Solution, child, out childItem))
                return;

            // add the child item and save project
            childItem = parentItem.ProjectItems.AddFromFile(child);
            childItem.ContainingProject.Save();

            // schedule call to change build action
            // this is a work around since it seems to ignore property changes until after file saved
            // and even after that it still ignores it, so async makes it better
            Dispatcher.CurrentDispatcher.BeginInvoke(new Action(() => ChangeBuildActionType(dte, child, type)), DispatcherPriority.Background);
        }

        private static void ChangeBuildActionType(DTE2 dte, string path, BuildActionType type)
        {
            ProjectItem item;
            if (TryGetProjectItem(dte.Solution, path, out item))
            {
                var vsBuildAction = (int)type;
                var vsType = type.ToString();

                var actionProperty = item.Properties.Item("BuildAction");
                var typeProperty = item.Properties.Item("ItemType");

                // stop if no changes
                if (vsBuildAction.Equals(actionProperty.Value) && vsType.Equals(typeProperty.Value))
                    return;

                actionProperty.Value = vsBuildAction;
                typeProperty.Value = vsType;
                item.ContainingProject.Save();
            }
        }

        static bool TryGetProjectItem(Solution solution, string path, out ProjectItem item)
        {
            item = null;

            if (solution != null)
                item = solution.FindProjectItem(path);

            return item != null;
        }

        internal enum BuildActionType : int
        {
            None = 0,
            Content = 2
        }
    }
}
