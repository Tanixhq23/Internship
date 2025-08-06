namespace Common
{
    public class ServiceResponse<T>
    {
        public T? Data { get; set; }
        public bool Success { get; set; } = true;
        public string? Message { get; set; } = "";

        public ServiceResponse<T> Response(bool Success, string? Message, T? Data = default)
        {
            this.Data = Data;
            this.Success = Success;
            this.Message = Message;

            return this;
        }
    }
}