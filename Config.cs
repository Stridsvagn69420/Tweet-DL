namespace Tweet_DL
{
    public class Config
    {
        public int delay { get; set; }
        public string downloadDir { get; set; }
        public ApiAuth apiAuth { get; set; }
        public UserAuth userAuth { get; set; }
        public class ApiAuth
        {
            public string bearerToken { get; set; }
        }
        public class UserAuth
        {
            public string cookie { get; set; }
            public string csrfToken { get; set; }

        }
    }
}
