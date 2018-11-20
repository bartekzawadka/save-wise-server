namespace SaveWise.Api
{
    public class ErrorResult
    {
        private readonly string _message;

        public ErrorResult(string message)
        {
            _message = message;
        }

        public object ToJson()
        {
            return new
            {
                message = _message
            };
        }
        
        public override string ToString()
        {
            return ToJson().ToString();
        }
    }
}