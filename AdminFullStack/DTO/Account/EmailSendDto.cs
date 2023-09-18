namespace AdminFullStack.DTO.Account
{
    public class EmailSendDto
    {
        public EmailSendDto(string to, string subject, string body)
        {
            this.To = to;
            this.Body = body;
            this.Subject = subject;
        }

        public string To { get; set; }

        public string Subject { get; set; }

        public string Body { get; set; }
    }
}
