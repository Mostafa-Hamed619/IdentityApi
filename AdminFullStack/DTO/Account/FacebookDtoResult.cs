namespace AdminFullStack.DTO.Account
{
    public class FacebookDtoResult
    {
        public FacebookData Data { get; set; }
    }

    public class FacebookData
    {
        public bool Is_Valid { get; set; }

        public string User_id { get; set; }
    }
}
