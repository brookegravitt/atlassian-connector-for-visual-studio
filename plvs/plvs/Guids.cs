// Guids.cs
// MUST match guids.h
using System;

namespace Atlassian.plvs
{
    static class GuidList
    {
        public const string guidplvsPkgString = "36fa5f7f-2b5d-4cec-8c06-10c483683a16";
        public const string guidplvsCmdSetString = "85a7fbbb-c60c-4329-9f0f-2fdf1fa122e6";
        public const string guidToolWindowPersistanceString = "06c81945-10ef-4d72-8daf-32d29f7e9573";

        public static readonly Guid guidplvsCmdSet = new Guid(guidplvsCmdSetString);
    };
}