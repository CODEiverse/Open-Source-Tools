using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CODEiverse.OST.Lib.Install
{
    [RunInstaller(true)]
    public partial class SetupInstaller : System.Configuration.Install.Installer
    {
        public SetupInstaller()
        {
            InitializeComponent();
        }

        public override void Commit(IDictionary savedState)
        {
            base.Commit(savedState);
            String progFiles = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
            String finalPath = Path.Combine(progFiles, @"CODEiverse.com\CODEiverse Open Source Tools");
            AddPathSegments(finalPath);
        }

        /// <summary>
        /// Adds an environment path segments (the PATH varialbe).
        /// </summary>
        /// <param name="pathSegment">The path segment.</param>
        public static void AddPathSegments(string pathSegment)
        {
            string allPaths = Environment.GetEnvironmentVariable("PATH", EnvironmentVariableTarget.Machine);
            if (allPaths != null)
            {
                if (!allPaths.Contains(pathSegment))
                {
                    allPaths = pathSegment + "; " + allPaths;
                }
            }
            else allPaths = pathSegment;
            Environment.SetEnvironmentVariable("PATH", allPaths, EnvironmentVariableTarget.Machine);
        }

    }
}
