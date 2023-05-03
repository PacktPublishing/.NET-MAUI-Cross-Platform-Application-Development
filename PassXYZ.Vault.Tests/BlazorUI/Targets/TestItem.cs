using KPCLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PassXYZ.Vault.Tests.BlazorUI.Targets
{
    public class TestItem : Item
    {
        private readonly Guid uid = new();
        public override string Id
        {
            get
            {
                return uid.ToString();
            }
        }
        public override string Name { get; set; } = string.Empty;
        public override string Notes { get; set; } = string.Empty;
        public override bool IsGroup { get => ItemType.Equals("group"); }
        public override DateTime LastModificationTime { get; set; } = default!;
        public override string Description
        {
            get
            {
                return $"{ItemType} | {LastModificationTime.ToString("yyyy'-'MM'-'dd")} | {Notes}".Truncate(50);
            }
        }
        public string ItemType { get; set; } = "Group";
    }
}
