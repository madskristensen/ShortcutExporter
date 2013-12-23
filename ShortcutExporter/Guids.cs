// Guids.cs
// MUST match guids.h
using System;

namespace MadsKristensen.ShortcutExporter
{
    static class GuidList
    {
        public const string guidShortcutExporterPkgString = "7b562864-ea1c-40d0-b3a9-55618e1d1e09";
        public const string guidShortcutExporterCmdSetString = "3d801e7c-14d7-487f-9389-06e93e5008cd";

        public static readonly Guid guidShortcutExporterCmdSet = new Guid(guidShortcutExporterCmdSetString);
    };
}