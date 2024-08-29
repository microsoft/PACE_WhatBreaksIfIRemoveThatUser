using System;
using System.Collections.Generic;
using System.Linq;

namespace FlowOwnershipAudit
{
    public partial class FlowOwnershipAuditControl
    {
        public class EnvironmentCollection : Dictionary<Model.Environment, EnvironmentQueryStatus>
        {
            /// <summary>﻿
            ﻿/// This Event is thrown when the queries of all environments have been completed.﻿
            ﻿/// </summary>﻿
            public event EventHandler AllEnvironmentsQueriesCompleted;

            /// <summary>﻿
            /// This event is thrown when the underlying dictionary changes﻿
            /// </summary>﻿
            public event EventHandler CollectionChanged;

            public EnvironmentCollection() : base()
            { }

            // these overrrides are necessary to implement our own observable event pattern﻿

            public new void Add(Model.Environment key, EnvironmentQueryStatus value)
            {
                // call base method to add item to the dictionary﻿
                base.Add(key, value);

                // invoke collection changed handlers if there are any﻿
                CollectionChanged?.Invoke(this, new EventArgs());

                // subscribe to the event that tells us that this environment finished processing﻿
                value.EnvironmentQueriesCompleted += EnvironmentQueriesCompleted;
            }

            public new void AddRange(IEnumerable<KeyValuePair<Model.Environment, EnvironmentQueryStatus>> items)
            {
                // call base method to add items to the dictionary﻿
                foreach (var item in items)
                {
                    Add(item.Key, item.Value);
                }
            }

            public new void Clear()
            {
                // call base method to clear the dictionary﻿
                base.Clear();

                // invoke collection changed handlers if there are any﻿
                CollectionChanged?.Invoke(this, new EventArgs());
            }

            public new bool Remove(Model.Environment key)
            {
                // call base method to remove the item from the dictionary﻿
                var result = base.Remove(key);

                // invoke collection changed handlers if there are any﻿
                CollectionChanged?.Invoke(this, new EventArgs());

                // TODO : unsubscribe from the event that tells us that this environment finished processing﻿

                return result;
            }

            private void EnvironmentQueriesCompleted(object sender, EventArgs e)
            {
                // check if handler is present before triggering﻿
                if (AllEnvironmentsQueriesCompleted != null)
                {
                    // check whether all environments in this collection have finished both flow and connection reference queries﻿
                    if (this.All(x => x.Value.flowsQueryCompleted && x.Value.connectionRefsQueryCompleted))
                    {
                        AllEnvironmentsQueriesCompleted(this, new EventArgs());
                    }
                }
            }
        }
        #endregion
    }
}