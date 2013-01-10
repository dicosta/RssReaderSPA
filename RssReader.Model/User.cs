using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RssReader.Model
{
    public class User : IGuidKeyedEntity
    {
        public Guid Id { get; set; }

        public string Username { get; set; }

        /*
        public string LowercaseUsername { get; set; }
        */

        public string Email { get; set; }

        /*
        public string LowercaseEmail { get; set; }
        */

        public string DisplayName { get; set; }

        public string Comment { get; set; }

        public string Password { get; set; }

        public string PasswordFormat { get; set; }

        public string PasswordSalt { get; set; }

        public string PasswordQuestion { get; set; }

        public string PasswordAnswer { get; set; }

        public bool IsApproved { get; set; }

        public DateTime LastActivityDate { get; set; }

        public DateTime LastLoginDate { get; set; }

        public DateTime LastPasswordChangedDate { get; set; }

        public DateTime CreateDate { get; set; }

        public bool IsLockedOut { get; set; }

        public DateTime LastLockedOutDate { get; set; }

        public int FailedPasswordAttemptCount { get; set; }

        public DateTime FailedPasswordAttemptWindowStart { get; set; }

        public int FailedPasswordAnswerAttemptCount { get; set; }

        public DateTime FailedPasswordAnswerAttemptWindowStart { get; set; }

        public List<string> Roles { get; set; }

        public string ConfirmationToken { get; set; }

        public string PasswordResetToken { get; set; }

        public DateTime PasswordResetTokenExpiry { get; set; }

        public IList<string> Tags { get; set; }

        public IList<Guid> Feeds { get; set; }

        public User()
        {
            this.Tags = new List<string>();
            this.Feeds = new List<Guid>();
        }
    }
}
