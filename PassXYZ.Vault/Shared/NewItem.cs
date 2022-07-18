using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeePassLib;
using KPCLib;
using PassXYZLib;

namespace PassXYZ.Vault.Shared
{
    public class NewItem : Item
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
        public override bool IsGroup { get => (SubType == ItemSubType.Group); }
        public override DateTime LastModificationTime { get; set; } = default!;
        public override string Description
        {
            get
            {
                return $"{ItemType} | {LastModificationTime.ToString("yyyy'-'MM'-'dd")} | {Notes}".Truncate(50);
            }
        }
        public ItemSubType SubType { get; set; } = ItemSubType.Group;
        public string ItemType
        { 
            get => SubType.ToString();
            set
            {
                SubType = SubType.GetItemSubType(value);
            }
        }
    }
}
