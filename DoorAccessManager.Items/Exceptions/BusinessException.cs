namespace DoorAccessManager.Items.Exceptions
{
    public class BusinessException : Exception
    {
        public int StatusCode { get; set; }

        public BusinessException(string message, int code = 400) : base(message)
        {
            StatusCode = code;
        }
    }
}
