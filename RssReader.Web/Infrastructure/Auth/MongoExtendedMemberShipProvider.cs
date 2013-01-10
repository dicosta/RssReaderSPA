using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.Security;
using Resources;
using RssReader.Model;
using WebMatrix.WebData;

namespace RssReader.Web.Infrastructure.Auth
{
    public class MongoExtendedMemberShipProvider : ExtendedMembershipProvider
    {
        #region delegation
        
        protected internal MongoMemberShipProvider DelegateProvider { get; set; }

        public MongoExtendedMemberShipProvider()
        {
            DelegateProvider = new MongoMemberShipProvider();
        }

        protected internal MongoExtendedMemberShipProvider(MongoMemberShipProvider deletegateProvider)
        {
            DelegateProvider = deletegateProvider;
        }

        public override string ApplicationName
        {
            get { return DelegateProvider.ApplicationName; }
            set { DelegateProvider.ApplicationName = value; }
        }

        public override void Initialize(string name, NameValueCollection config)
        {
            if (null == config)
                throw new ArgumentNullException("config");

            if (String.IsNullOrWhiteSpace(name))
                name = MongoMemberShipProvider.DEFAULT_NAME;

            if (String.IsNullOrEmpty(config["description"]))
            {
                config.Remove("description");
                config.Add("description", InfrastructureResources.MembershipProvider_description);
            }

            base.Initialize(name, config);

            DelegateProvider.InitializeMongoMembershipProviderProvider(name, config);
        }

        public override bool ChangePassword(string username, string oldPassword, string newPassword)
        {
            return DelegateProvider.ChangePassword(username, oldPassword, newPassword);
        }

        public override bool ChangePasswordQuestionAndAnswer(string username, string password, string newPasswordQuestion,
                                                    string newPasswordAnswer)
        {
            return DelegateProvider.ChangePasswordQuestionAndAnswer(username, password, newPasswordQuestion, newPasswordAnswer);
        }

        public override MembershipUser CreateUser(string username, string password, string email, string passwordQuestion, string passwordAnswer,
                                         bool isApproved, object providerUserKey, out MembershipCreateStatus status)
        {
            return DelegateProvider.CreateUser(username, password, email, passwordQuestion, passwordAnswer, isApproved, providerUserKey, out status);
        }

        public override bool DeleteUser(string username, bool deleteAllRelatedData)
        {
            return DelegateProvider.DeleteUser(username, deleteAllRelatedData);
        }

        public override bool EnablePasswordReset
        {
            get { return DelegateProvider.EnablePasswordReset; }
        }

        public override bool EnablePasswordRetrieval
        {
            get { return DelegateProvider.EnablePasswordRetrieval; }
        }

        public override MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            return DelegateProvider.FindUsersByEmail(emailToMatch, pageIndex, pageSize, out totalRecords);
        }

        public override MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            return DelegateProvider.FindUsersByName(usernameToMatch, pageIndex, pageSize, out totalRecords);
        }

        public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
        {
            return DelegateProvider.GetAllUsers(pageIndex, pageSize, out totalRecords);
        }

        public override int GetNumberOfUsersOnline()
        {
            return DelegateProvider.GetNumberOfUsersOnline();
        }

        public override string GetPassword(string username, string answer)
        {
            return DelegateProvider.GetPassword(username, answer);
        }

        public override MembershipUser GetUser(string username, bool userIsOnline)
        {
            return DelegateProvider.GetUser(username, userIsOnline);
        }

        public override MembershipUser GetUser(object providerUserKey, bool userIsOnline)
        {
            return DelegateProvider.GetUser(providerUserKey, userIsOnline);
        }

        public override string GetUserNameByEmail(string email)
        {
            return DelegateProvider.GetUserNameByEmail(email);
        }

        public override int MaxInvalidPasswordAttempts
        {
            get { return DelegateProvider.MaxInvalidPasswordAttempts; }
        }

        public override int MinRequiredNonAlphanumericCharacters
        {
            get { return DelegateProvider.MinRequiredNonAlphanumericCharacters; }
        }

        public override int MinRequiredPasswordLength
        {
            get { return DelegateProvider.MinRequiredPasswordLength; }
        }

        public override int PasswordAttemptWindow
        {
            get { return DelegateProvider.PasswordAttemptWindow; }
        }

        public override MembershipPasswordFormat PasswordFormat
        {
            get { return DelegateProvider.PasswordFormat; }
        }

        public override string PasswordStrengthRegularExpression
        {
            get { return DelegateProvider.PasswordStrengthRegularExpression; }
        }

        public override bool RequiresQuestionAndAnswer
        {
            get { return DelegateProvider.RequiresQuestionAndAnswer; }
        }

        public override bool RequiresUniqueEmail
        {
            get { return DelegateProvider.RequiresUniqueEmail; }
        }

        public override string ResetPassword(string username, string answer)
        {
            return DelegateProvider.ResetPassword(username, answer);
        }

        public override bool UnlockUser(string userName)
        {
            return DelegateProvider.UnlockUser(userName);
        }

        public override void UpdateUser(MembershipUser user)
        {
            DelegateProvider.UpdateUser(user);
        }

        public override bool ValidateUser(string username, string password)
        {
            return DelegateProvider.ValidateUser(username, password);
        }

        #endregion delegation

        public override ICollection<OAuthAccountData> GetAccountsForUser(string userName)
        {
            return new Collection<OAuthAccountData>();
        }

        public override string CreateUserAndAccount(string userName, string password, bool requireConfirmation, IDictionary<string, object> values)
        {
            return CreateAccount(userName, password, requireConfirmation);
        }

        public override string CreateAccount(string userName, string password, bool requireConfirmationToken)
        {
            MembershipCreateStatus status;
            CreateUser(userName, password, null, null, null, !requireConfirmationToken, null, out status);
            
            if (status.Equals(MembershipCreateStatus.Success) && requireConfirmationToken)
            {
                var user = DelegateProvider._userService.GetByUserName(userName);
                user.ConfirmationToken = Guid.NewGuid().ToString();
                DelegateProvider._userService.Update(user);
                
                return user.ConfirmationToken;
            }

            if (status != MembershipCreateStatus.Success)
                throw new MembershipCreateUserException(status);

            return null;
        }

        public override bool ConfirmAccount(string userName, string accountConfirmationToken)
        {
            var user = DelegateProvider._userService.GetByUserName(userName);
            return ConfirmAccount(accountConfirmationToken, user);
        }

        public override bool ConfirmAccount(string accountConfirmationToken)
        {
            var user = DelegateProvider._userService.GetByConfirmationToken(accountConfirmationToken);
            return ConfirmAccount(accountConfirmationToken, user);
        }

        private bool ConfirmAccount(string accountConfirmationToken, User user)
        {
            if (user == null)
            {
                return false;
            }
            user.IsApproved = user.ConfirmationToken == accountConfirmationToken;
            DelegateProvider._userService.Update(user);
            return user.IsApproved;
        }

        public override bool DeleteAccount(string userName)
        {
            var user = DelegateProvider._userService.GetByUserName(userName);
            if (user != null)
            {
                DelegateProvider._userService.Delete(user.Id);
                return true;
            }
            return false;
        }

        public override string GeneratePasswordResetToken(string userName, int tokenExpirationInMinutesFromNow)
        {
            var user = DelegateProvider._userService.GetByUserName(userName);
            if (user != null)
            {
                user.PasswordResetToken = Guid.NewGuid().ToString();
                user.PasswordResetTokenExpiry = DateTime.Now.Add(TimeSpan.FromMinutes(tokenExpirationInMinutesFromNow));
                DelegateProvider._userService.Update(user);
                return user.PasswordResetToken;
            }
            return null;
        }

        public override int GetUserIdFromPasswordResetToken(string token)
        {
            throw new Exception("no puedo devolverte un entero, mis user son Guid Keyed!");
            /*
            var user = DelegateProvider._userService.GetByPasswordResetToken(token);
            return user != null ? user.Id : -1;
            */
        }

        public override bool IsConfirmed(string userName)
        {
            var user = DelegateProvider._userService.GetByUserName(userName);
            return user != null && user.IsApproved;
        }

        public override bool ResetPasswordWithToken(string token, string newPassword)
        {
            var user = DelegateProvider._userService.GetByPasswordResetToken(token);
            if (user != null)
            {
                user.Password = DelegateProvider.EncodePassword(newPassword, 
                    (MembershipPasswordFormat)Enum.Parse(typeof(MembershipPasswordFormat),  user.PasswordFormat), 
                        user.PasswordSalt);
                user.PasswordResetTokenExpiry = DateTime.MinValue;
                user.LastPasswordChangedDate = DateTime.Now;
                DelegateProvider._userService.Update(user);
                return true;
            }
            return false;
        }

        public override int GetPasswordFailuresSinceLastSuccess(string userName)
        {
            var user = DelegateProvider._userService.GetByUserName(userName);
            return user != null ? user.FailedPasswordAttemptCount : 0;
        }

        public override DateTime GetCreateDate(string userName)
        {
            var user = DelegateProvider._userService.GetByUserName(userName);
            return user != null ? user.CreateDate : DateTime.MinValue;
        }

        public override DateTime GetPasswordChangedDate(string userName)
        {
            var user = DelegateProvider._userService.GetByUserName(userName);
            return user != null ? user.LastPasswordChangedDate : DateTime.MinValue;
        }

        public override DateTime GetLastPasswordFailureDate(string userName)
        {
            var user = DelegateProvider._userService.GetByUserName(userName);

            //use another variable for this?
            return user != null ? user.FailedPasswordAnswerAttemptWindowStart : DateTime.MinValue;
        }

        public override bool HasLocalAccount(int userId)
        {
            throw new Exception("no puedo buscar por enteros, mis user son Guid Keyed!");
            //return BirdBrainHelper.GetUserById(userId, session) != null;
        }
    }
}