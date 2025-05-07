namespace MODELS.USER.Requests
{
    public class GoogleRegisterRequest
    {
        /// <summary>
        /// Địa chỉ Email
        /// </summary>
        public string Username { get; set; }
        public string HoLot { get; set; }
        public string Ten { get; set; }
        public DateTime DateOfBirth { get; set; }
        public int? Gender { get; set; }
    }
}
