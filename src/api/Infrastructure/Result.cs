namespace NiceNuget.Api.Infrastructure {
    
    // My new favorite pattern
    public class Result {
        public bool Success { get; private set; }
        public string Error { get; private set; }
        protected Result(bool success, string error) {
            Success = success;
            Error = error;
        }

        public static Result Fail(string error) {
            return new Result(false, error);
        }

        public static Result<T> Fail<T>(string error) {
            return new Result<T>(default(T), false, error);
        }

        public static Result Ok() {
            return new Result(true, null);
        }

        public static Result<T> Ok<T>(T value) {
            return new Result<T>(value, true, null);
        }
    }

    public class Result<T> : Result
    {
        public T Value { get; private set; }
        protected internal Result(T value, bool success, string error) : base(success, error) {
            Value = value;
        }
    }
}
