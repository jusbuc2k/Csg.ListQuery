﻿using System;

namespace Csg.ListQuery
{
    [System.AttributeUsage(AttributeTargets.Class | AttributeTargets.Property)]
    public class SortableAttribute : System.Attribute
    {
        public SortableAttribute(bool sortable = true, bool descending = false, bool isDefault = false)
        {
            this.IsSortable = sortable;
            this.Descending = descending;
            this.IsDefault = isDefault;
        }

        public bool IsSortable { get; private set; }

        public bool Descending { get; private set; }

        public bool IsDefault { get; private set; }
    }
}
