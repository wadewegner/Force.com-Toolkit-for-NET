namespace Salesforce.Force
{
    public static class BulkConstants
    {
        public sealed class OperationType
        {
            public static readonly OperationType Insert = new OperationType("insert");
            public static readonly OperationType Update = new OperationType("update");
            public static readonly OperationType Upsert = new OperationType("upsert");
            public static readonly OperationType Delete = new OperationType("delete");
            public static readonly OperationType HardDelete = new OperationType("hardDelete");

            private readonly string _value;

            private OperationType(string value)
            {
                _value = value;
            }

            public string Value()
            {
                return _value;
            }
        }

        public sealed class BatchState
        {
            public static readonly BatchState Queued = new BatchState("Queued");
            public static readonly BatchState InProgress = new BatchState("InProgress");
            public static readonly BatchState Completed = new BatchState("Completed");
            public static readonly BatchState Failed = new BatchState("Failed");
            public static readonly BatchState NotProcessed = new BatchState("Not Processed");

            private readonly string _value;

            private BatchState(string value)
            {
                _value = value;
            }

            public string Value()
            {
                return _value;
            }
        }

    }
}
