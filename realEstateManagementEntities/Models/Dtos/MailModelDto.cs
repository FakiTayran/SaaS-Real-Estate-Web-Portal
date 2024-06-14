using System;
using System.Net.Mail;

namespace realEstateManagementEntities.Models.Dtos
{
	public class MailModelDto
	{
        public required string Subject { get; set; }
        public string? To { get; set; }
        public required string Body { get; set; }
        public MailPriority MailPriority { get; set; } = MailPriority.Normal;
    }

}

