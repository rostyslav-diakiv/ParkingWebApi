namespace Parking.BLL.Dtos
{
    public class Response<T>
    {
        public T Data { get; set; }

        public string ErrorMessage{ get; set; }
    }
}
