﻿using System;
using System.Collections.Generic;
using WhatBreaksIf.DTO;

namespace WhatBreaksIf.TreeViewUIElements
{
    // this base class is used so we can display different types of objects in the treeview. Abstract because we enforce typed implementations
    internal abstract class TreeNodeElementBase
    {
        private readonly Action<NodeUpdateObject> updateNodeUi;

        public string ElementId { get; set; }

        internal abstract IEnumerable<TreeNodeElementBase> ChildObjects { get; }

        internal abstract TreeNodeElementBase Parent { get; }

        public TreeNodeElementBase(Action<NodeUpdateObject> updateNodeUi)
        {
            this.updateNodeUi = updateNodeUi;
        }
    }
}