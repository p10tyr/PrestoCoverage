using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PrestoCoverage.Extensions
{
    public static class VSExtensions
    {
        private static IVsSolution GetSolution(this IServiceProvider serviceProvider)
        {
            return (IVsSolution)serviceProvider.GetService(typeof(SVsSolution));
        }

        public static IEnumerable<IVsProject> GetLoadedProjects(this IServiceProvider serviceProvider)
        {
            return serviceProvider.GetSolution().GetLoadedProjects();
        }

        public static string GetSolutionDirectory(this IServiceProvider serviceProvider)
        {
            return serviceProvider.GetSolution().GetSolutionDirectory();
        }

        public static bool HasFile(this IServiceProvider serviceProvider, string file)
        {
            return serviceProvider.GetLoadedProjects().Any(p => p.HasFile(file));
        }

        public static IEnumerable<IVsProject> GetLoadedProjects(this IVsSolution solution)
        {
            return solution.EnumerateLoadedProjects(__VSENUMPROJFLAGS.EPF_LOADEDINSOLUTION).OfType<IVsProject>();
        }

        public static string GetProjectName(this IVsProject project)
        {
            return project.GetPropertyValue(__VSHPROPID.VSHPROPID_Name, VSConstants.VSITEMID.Root) as string;
        }

        public static string GetProjectDir(this IVsProject project)
        {
            return project.GetPropertyValue(__VSHPROPID.VSHPROPID_ProjectDir, VSConstants.VSITEMID.Root) as string;
        }

        public static IEnumerable<IVsHierarchy> EnumerateLoadedProjects(this IVsSolution solution, __VSENUMPROJFLAGS enumFlags)
        {
            var prjType = Guid.Empty;

            var hr = solution.GetProjectEnum((uint)enumFlags, ref prjType, out IEnumHierarchies ppHier);
            if (ErrorHandler.Succeeded(hr) && ppHier != null)
            {
                uint fetched = 0;
                var hierarchies = new IVsHierarchy[1];
                while (ppHier.Next(1, hierarchies, out fetched) == VSConstants.S_OK)
                {
                    yield return hierarchies[0];
                }
            }
        }

        public static string GetSolutionDirectory(this IVsSolution solution)
        {
            if (solution.GetSolutionInfo(out string solutionDir, out string solutionFile, out string userOpsFile) == VSConstants.S_OK)
            {
                return solutionDir;
            }
            return null;
        }

        public static bool HasFile(this IVsSolution solution, string file)
        {
            return solution.GetLoadedProjects().Any(p => p.HasFile(file));
        }

        public static IEnumerable<string> GetProjectItems(this IVsProject project)
        {
            // Each item in VS OM is IVSHierarchy. 
            return GetProjectItems((IVsHierarchy)project, VSConstants.VSITEMID_ROOT);
        }

        public static IEnumerable<string> GetProjectItems(IVsHierarchy project, uint itemId)
        {
            object pVar = GetPropertyValue(project, (int)__VSHPROPID.VSHPROPID_FirstChild, itemId);

            uint childId = GetItemId(pVar);
            while (childId != VSConstants.VSITEMID_NIL)
            {
                string childPath = GetCanonicalName(childId, project);
                yield return childPath;

                foreach (var childNodePath in GetProjectItems(project, childId)) yield return childNodePath;

                pVar = GetPropertyValue(project, (int)__VSHPROPID.VSHPROPID_NextSibling, childId);
                childId = GetItemId(pVar);
            }
        }

        public static bool HasFile(this IVsProject project, string file)
        {
            var priority = new VSDOCUMENTPRIORITY[1];
            if (ErrorHandler.Succeeded(project.IsDocumentInProject(file, out int found, priority, out uint projectItemID)))
            {
                return found != 0;
            }
            return false;
        }

        public static uint GetItemId(object pvar)
        {
            if (pvar == null) return VSConstants.VSITEMID_NIL;
            if (pvar is int) return (uint)(int)pvar;
            if (pvar is uint) return (uint)pvar;
            if (pvar is short) return (uint)(short)pvar;
            if (pvar is ushort) return (uint)(ushort)pvar;
            if (pvar is long) return (uint)(long)pvar;
            return VSConstants.VSITEMID_NIL;
        }

        public static object GetPropertyValue(this IVsProject project, __VSHPROPID propid, VSConstants.VSITEMID itemId = VSConstants.VSITEMID.Root)
        {
            return GetPropertyValue((IVsHierarchy)project, propid, itemId);
        }

        public static object GetPropertyValue(this IVsHierarchy vsHierarchy, __VSHPROPID propid, VSConstants.VSITEMID itemId = VSConstants.VSITEMID.Root)
        {
            return GetPropertyValue(vsHierarchy, (int)propid, (uint)itemId);
        }

        public static object GetPropertyValue(this IVsHierarchy vsHierarchy, int propid, uint itemId)
        {
            if (itemId == VSConstants.VSITEMID_NIL)
            {
                return null;
            }

            try
            {
                ErrorHandler.ThrowOnFailure(vsHierarchy.GetProperty(itemId, propid, out object o));

                return o;
            }
            catch (System.NotImplementedException)
            {
                return null;
            }
            catch (System.Runtime.InteropServices.COMException)
            {
                return null;
            }
            catch (System.ArgumentException)
            {
                return null;
            }
        }

        public static string GetCanonicalName(uint itemId, IVsHierarchy hierarchy)
        {
            string strRet = string.Empty;
            int hr = hierarchy.GetCanonicalName(itemId, out strRet);

            if (hr == VSConstants.E_NOTIMPL)
            {
                // Special case E_NOTIMLP to avoid perf hit to throw an exception.
                return string.Empty;
            }
            else
            {
                try
                {
                    ErrorHandler.ThrowOnFailure(hr);
                }
                catch (System.Runtime.InteropServices.COMException)
                {
                    strRet = string.Empty;
                }

                // This could be in the case of S_OK, S_FALSE, etc.
                return strRet;
            }
        }
    }
}