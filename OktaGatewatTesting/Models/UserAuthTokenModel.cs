namespace OktaGatewatTesting.Models
{
    public class UserAuthTokenModel
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string SiteName { get; set; }
        public bool IsAuthenticated { get; set; }
        public string Token { get; set; }
        public DateTime LogingTime { get; set; }
        public bool IsValidated { get; set; }
    }
}
