﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeePassLib;
using KPCLib;
using PassXYZLib;
using Microsoft.AspNetCore.Components;

namespace PassXYZ.Vault.Shared
{
    public static class ItemEx
    {
        public static string GetIcon(this Item item)
        {

            if (item.IsGroup)
            {
                // Group
                if (item is PwGroup group)
                {
                    if (group.CustomData.Exists(PxDefs.PxCustomDataIconName))
                    {
                        return $"/images/{group.CustomData.Get(PxDefs.PxCustomDataIconName)}";
                    }
                }
            }
            else
            {
                // Entry
                if (item is PwEntry entry)
                {
                    if (entry.CustomData.Exists(PxDefs.PxCustomDataIconName))
                    {
                        return $"/images/{entry.CustomData.Get(PxDefs.PxCustomDataIconName)}";
                    }
                }
            }

            // 2. Get custom icon
            return item.GetCustomIcon();
        }

        /// <summary>
        /// Get the action link of an item.
        /// </summary>
        public static string GetActionLink(this Item item, string? action = default)
        {
            string itemType = (item.IsGroup) ? PxConstants.Group : PxConstants.Entry;

            return (action == null) ? $"/{itemType}/{item.Id}" : $"/{itemType}/{action}/{item.Id}";
        }

        /// <summary>
        /// Get the parent link of an item.
        /// </summary>
        public static string? GetParentLink(this Item item)
        {
            Item? parent = default;

            if (item == null) return null;

            if(item.IsGroup)
            {
                PwGroup group = (PwGroup)item;
                if (group.ParentGroup == null) return null;

                parent = group.ParentGroup;
            }
            else 
            {
                PwEntry entry = (PwEntry)item;
                if (entry.ParentGroup == null) return null;

                parent = entry.ParentGroup;
            }

            return $"/{PxConstants.Group}/{parent.Id}";
        }
    }
}
