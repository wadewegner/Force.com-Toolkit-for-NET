namespace Salesforce.Force
{
    public static class Bulk
    {
        public sealed class OperationType
        {
            public static readonly OperationType Insert = new OperationType("insert");
            public static readonly OperationType Query = new OperationType("query");
            public static readonly OperationType Update = new OperationType("update");
            public static readonly OperationType Upsert = new OperationType("upsert");
            public static readonly OperationType Delete = new OperationType("delete");

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

        public sealed class ConcurrencyMode
        {
            public static readonly ConcurrencyMode Parallel = new ConcurrencyMode("Parallel");
            public static readonly ConcurrencyMode Serial = new ConcurrencyMode("Serial");

            private readonly string _value;

            private ConcurrencyMode(string value)
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
