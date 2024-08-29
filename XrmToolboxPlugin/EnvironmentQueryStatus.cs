using System;

namespace FlowOwnershipAudit
{
    public partial class FlowOwnershipAuditControl
    {
        // helper class to hold information about the environment and the status of the queries﻿
        public class EnvironmentQueryStatus﻿
        {
            private bool _flowsQueryCompleted = false;
            private bool _connectionRefsQueryCompleted = false;

            public bool flowsQueryCompleted﻿
            {
                get
                {
                    return _flowsQueryCompleted;
                }
                set
                {
                    _flowsQueryCompleted = value;

                    // raise event and inform that all queries have been completed if necessary﻿
                    if (_flowsQueryCompleted && connectionRefsQueryCompleted)
                    {
                        EnvironmentQueriesCompleted?.Invoke(this, new EventArgs());
                    }
                }
            }
            public bool connectionRefsQueryCompleted﻿
            {
                get => _connectionRefsQueryCompleted;
                set
                {
                    _connectionRefsQueryCompleted = value;

                    // raise event and inform that all queries have been completed if necessary﻿
                    if (_flowsQueryCompleted && connectionRefsQueryCompleted)
                    {
                        EnvironmentQueriesCompleted?.Invoke(this, new EventArgs());
                    }
                }
            }

            public event EventHandler EnvironmentQueriesCompleted;
        }
        #endregion
    }
}