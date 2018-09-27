using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.ComponentModel.Design;
using Microsoft.Win32;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using EnvDTE80;
using EnvDTE;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Xml;

namespace MadsKristensen.ShortcutExporter
{
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(GuidList.guidShortcutExporterPkgString)]
    public sealed class ShortcutExporterPackage : Package
    {
        private DTE2 _dte;

        protected override void Initialize()
        {
            base.Initialize();

            _dte = GetGlobalService(typeof(DTE)) as DTE2;

            OleMenuCommandService mcs = GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (null != mcs)
            {
                CommandID menuCommandID = new CommandID(GuidList.guidShortcutExporterCmdSet, (int)PkgCmdIDList.cmdExportShortcuts);
                MenuCommand menuItem = new MenuCommand(MenuItemCallback, menuCommandID);
                mcs.AddCommand(menuItem);
            }
        }

        private void MenuItemCallback(object sender, EventArgs e)
        {
            var dialog = new SaveFileDialog()
            {
                AddExtension = true,
                DefaultExt = ".xml",
                FileName = GetVersion() + ".xml",
                CheckPathExists = true,
                Filter = "XML Files (*.xml)|*.xml|All files (*.*)|*.*",
            };

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                WriteDocument(dialog.FileName);
            }
        }

        private string GetVersion()
        {
            switch (_dte.Version)
            {
                case "11.0":
                    return "2012";
                case "12.0":
                    return "2013";
                case "14.0":
                    return "2015";
                case "15.0":
                    return "2017";
                case "16.0":
                    return "2019";
            }

            return "2013";
        }

        private void WriteDocument(string fileName)
        {
            using (XmlWriter writer = XmlWriter.Create(fileName))
            {
                writer.WriteStartDocument(true);
                writer.WriteStartElement("commands");

                WriteCommands(writer);

                writer.WriteEndElement();
            }
        }

        private static void Print(XmlWriter writer, EnvDTE.Command command, object[] bindings)
        {
            foreach (object binding in bindings)
            {
                var shortcut = binding.ToString();
                var scopeIndex = shortcut.IndexOf("::");
                string scope = "";
                if (scopeIndex >= 0)
                {
                    scope = shortcut.Substring(0, scopeIndex);
                    shortcut = shortcut.Substring(scopeIndex + 2);
                }

                writer.WriteStartElement("command");
                if (scope.Length > 0)
                {
                    writer.WriteAttributeString("scope", scope);
                }
                writer.WriteAttributeString("shortcut", shortcut);
                writer.WriteAttributeString("name", command.Name);
                writer.WriteEndElement();
            }
        }

        private void WriteCommands(XmlWriter writer)
        {
            List<Command> commands = GetCommands();

            foreach (EnvDTE.Command command in commands.OrderBy(c => c.Name))
            {
                var bindings = command.Bindings as object[];

                if (bindings != null && bindings.Length > 0)
                {
                    Print(writer, command, bindings);
                }
            }
        }

        private List<Command> GetCommands()
        {
            List<Command> commands = new List<Command>();

            foreach (EnvDTE.Command command in _dte.Commands)
            {
                if (!string.IsNullOrEmpty(command.Name))
                    commands.Add(command);
            }

            return commands;
        }
    }
}
