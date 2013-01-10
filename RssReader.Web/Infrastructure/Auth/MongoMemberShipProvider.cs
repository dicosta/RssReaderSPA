using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Configuration.Provider;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.Security;
using Resources;
using RssReader.Model;
using RssReader.Services.Contracts;
using RssReader.Web.Infrastructure;

namespace RssReader.Web.Infrastructure.Auth
{
    public class MongoMemberShipProvider : MembershipProvider
    {
        #region Custom Public Properties
        
        public const string DEFAULT_NAME = "MongoMemberShipProvider";
        public const string DEFAULT_INVALID_CHARACTERS = ",%";
        public const int NEW_PASSWORD_LENGTH = 8;
        public const int MAX_USERNAME_LENGTH = 256;
        public const int MAX_PASSWORD_LENGTH = 128;
        public const int MAX_PASSWORD_ANSWER_LENGTH = 128;
        public const int MAX_EMAIL_LENGTH = 256;
        public const int MAX_PASSWORD_QUESTION_LENGTH = 256;

        //
        // If false, exceptions are thrown to the caller. If true,
        // exceptions are written to the event log.
        //
        public bool WriteExceptionsToEventLog { get; set; }
        public string InvalidUsernameCharacters { get; protected set; }
        public string InvalidEmailCharacters { get; protected set; }

        //set in global asax
        public Func<IUserService> UserServiceProvider;

        #endregion

        #region Protected Properties

        protected MachineKeySection _machineKey;

        protected int _newPasswordLength = 8;
        protected string _eventSource = DEFAULT_NAME;
        protected string _eventLog = "Application";
        protected string _exceptionMessage = InfrastructureResources.ProviderException;
        
        protected string ProviderName { get; set; }

        public IUserService _userService
        {
            get
            {
                return DependencyResolver.Current.GetService<IUserService>();
            }
        }

        #endregion

        #region MembershipProvider properties

        // System.Web.Security.MembershipProvider properties.

        protected string _applicationName;
        protected bool _enablePasswordReset;
        protected bool _enablePasswordRetrieval;
        protected bool _requiresQuestionAndAnswer;
        protected bool _requiresUniqueEmail;
        protected int _maxInvalidPasswordAttempts;
        protected int _passwordAttemptWindow;
        protected MembershipPasswordFormat _passwordFormat;
        protected int _minRequiredNonAlphanumericCharacters;
        protected int _minRequiredPasswordLength;
        protected string _passwordStrengthRegularExpression;

        /// <summary>
        /// The name of the application using the custom membership provider.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// The name of the application using the custom membership provider.
        /// </returns>
        public override string ApplicationName
        {
            get { return _applicationName; }
            set
            {
                _applicationName = value;
            }
        }

        /// <summary>
        /// Indicates whether the membership provider is configured to allow users to reset their passwords.
        /// </summary>
        /// <value></value>
        /// <returns>true if the membership provider supports password reset; otherwise, false. The default is true.
        /// </returns>
        public override bool EnablePasswordReset
        {
            get { return _enablePasswordReset; }
        }

        /// <summary>
        /// Indicates whether the membership provider is configured to allow users to retrieve their passwords.
        /// </summary>
        /// <value></value>
        /// <returns>true if the membership provider is configured to support password retrieval; otherwise, false. The default is false.
        /// </returns>
        public override bool EnablePasswordRetrieval
        {
            get { return _enablePasswordRetrieval; }
        }
        
        /// <summary>
        /// Gets a value indicating whether the membership provider is configured to require the user to answer a password question for password reset and retrieval.
        /// </summary>
        /// <value></value>
        /// <returns>true if a password answer is required for password reset and retrieval; otherwise, false. The default is true.
        /// </returns>
        public override bool RequiresQuestionAndAnswer
        {
            get { return _requiresQuestionAndAnswer; }
        }

        /// <summary>
        /// Gets a value indicating whether the membership provider is configured to require a unique e-mail address for each user name.
        /// </summary>
        /// <value></value>
        /// <returns>true if the membership provider requires a unique e-mail address; otherwise, false. The default is true.
        /// </returns>
        public override bool RequiresUniqueEmail
        {
            get { return _requiresUniqueEmail; }
        }

        /// <summary>
        /// Gets the number of invalid password or password-answer attempts allowed before the membership user is locked out.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// The number of invalid password or password-answer attempts allowed before the membership user is locked out.
        /// </returns>
        public override int MaxInvalidPasswordAttempts
        {
            get { return _maxInvalidPasswordAttempts; }
        }

        /// <summary>
        /// Gets the number of minutes in which a maximum number of invalid password or password-answer attempts are allowed before the membership user is locked out.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// The number of minutes in which a maximum number of invalid password or password-answer attempts are allowed before the membership user is locked out.
        /// </returns>
        public override int PasswordAttemptWindow
        {
            get { return _passwordAttemptWindow; }
        }

        /// <summary>
        /// Gets a value indicating the format for storing passwords in the membership data store.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// One of the <see cref="T:System.Web.Security.MembershipPasswordFormat"/> values indicating the format for storing passwords in the data store.
        /// </returns>
        public override MembershipPasswordFormat PasswordFormat
        {
            get { return _passwordFormat; }
        }

        /// <summary>
        /// Gets the minimum number of special characters that must be present in a valid password.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// The minimum number of special characters that must be present in a valid password.
        /// </returns>
        public override int MinRequiredNonAlphanumericCharacters
        {
            get { return _minRequiredNonAlphanumericCharacters; }
        }

        /// <summary>
        /// Gets the minimum length required for a password.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// The minimum length required for a password.
        /// </returns>
        public override int MinRequiredPasswordLength
        {
            get { return _minRequiredPasswordLength; }
        }

        /// <summary>
        /// Gets the regular expression used to evaluate a password.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// A regular expression used to evaluate a password.
        /// </returns>
        public override string PasswordStrengthRegularExpression
        {
            get { return _passwordStrengthRegularExpression; }
        }

        #endregion

        public void InitializeMongoMembershipProviderProvider(string name, NameValueCollection config)
        {
            this.ProviderName = name;
            _applicationName = config["applicationName"] ?? System.Web.Hosting.HostingEnvironment.ApplicationVirtualPath;
            _maxInvalidPasswordAttempts = GetConfigValue(config["maxInvalidPasswordAttempts"], 5);
            _passwordAttemptWindow = GetConfigValue(config["passwordAttemptWindow"], 10);
            _minRequiredNonAlphanumericCharacters = GetConfigValue(config["minRequiredNonAlphanumericCharacters"], 1);
            _minRequiredPasswordLength = GetConfigValue(config["minRequiredPasswordLength"], 7);
            _passwordStrengthRegularExpression = GetConfigValue(config["passwordStrengthRegularExpression"], "");
            _enablePasswordReset = GetConfigValue(config["enablePasswordReset"], true);
            _enablePasswordRetrieval = GetConfigValue(config["enablePasswordRetrieval"], false);
            _requiresQuestionAndAnswer = GetConfigValue(config["requiresQuestionAndAnswer"], false);
            _requiresUniqueEmail = GetConfigValue(config["requiresUniqueEmail"], true);
            InvalidUsernameCharacters = GetConfigValue(config["invalidUsernameCharacters"], DEFAULT_INVALID_CHARACTERS);
            InvalidEmailCharacters = GetConfigValue(config["invalidEmailCharacters"], DEFAULT_INVALID_CHARACTERS);
            WriteExceptionsToEventLog = GetConfigValue(config["writeExceptionsToEventLog"], true);

            ValidatePwdStrengthRegularExpression();

            if (_minRequiredNonAlphanumericCharacters > _minRequiredPasswordLength)
                throw new ProviderException(InfrastructureResources.MinRequiredNonalphanumericCharacters_can_not_be_more_than_MinRequiredPasswordLength);

            string temp_format = config["passwordFormat"];
            if (null == temp_format)
            {
                temp_format = "Hashed";
            }

            switch (temp_format.ToLowerInvariant())
            {
                case "hashed":
                    _passwordFormat = MembershipPasswordFormat.Hashed;
                    break;
                case "encrypted":
                    _passwordFormat = MembershipPasswordFormat.Encrypted;
                    break;
                case "clear":
                    _passwordFormat = MembershipPasswordFormat.Clear;
                    break;
                default:
                    throw new ProviderException(InfrastructureResources.Provider_bad_password_format);
            }

            if ((PasswordFormat == MembershipPasswordFormat.Hashed) && EnablePasswordRetrieval)
            {
                throw new ProviderException(InfrastructureResources.Provider_can_not_retrieve_hashed_password);
            }

            // Check for invalid parameters in the config

            config.Remove("enablePasswordRetrieval");
            config.Remove("enablePasswordReset");
            config.Remove("requiresQuestionAndAnswer");
            config.Remove("applicationName");
            config.Remove("requiresUniqueEmail");
            config.Remove("maxInvalidPasswordAttempts");
            config.Remove("passwordAttemptWindow");
            config.Remove("commandTimeout");
            config.Remove("passwordFormat");
            config.Remove("name");
            config.Remove("minRequiredPasswordLength");
            config.Remove("minRequiredNonAlphanumericCharacters");
            config.Remove("passwordStrengthRegularExpression");
            config.Remove("writeExceptionsToEventLog");
            config.Remove("invalidUsernameCharacters");
            config.Remove("invalidEmailCharacters");

            if (config.Count > 0)
            {
                string key = config.GetKey(0);
                if (!string.IsNullOrEmpty(key))
                {
                    throw new ProviderException(String.Format(InfrastructureResources.Provider_unrecognized_attribute, key));
                }
            }

            // Get encryption and decryption key information from the configuration.

            Configuration cfg =
              WebConfigurationManager.OpenWebConfiguration(System.Web.Hosting.HostingEnvironment.ApplicationVirtualPath);
            _machineKey = (MachineKeySection)ConfigurationManager.GetSection("system.web/machineKey");

            if (_machineKey.ValidationKey.Contains("AutoGenerate"))
                if (PasswordFormat != MembershipPasswordFormat.Clear)
                    throw new ProviderException(InfrastructureResources.Provider_can_not_autogenerate_machine_key_with_encrypted_or_hashed_password_format);       
        }

        public override void Initialize(string name, NameValueCollection config)
        {
            if (null == config)
                throw new ArgumentNullException("config");

            if (String.IsNullOrWhiteSpace(name))
                name = DEFAULT_NAME;
                        
            if (String.IsNullOrEmpty(config["description"]))
            {
                config.Remove("description");
                config.Add("description", InfrastructureResources.MembershipProvider_description);
            }

            base.Initialize(name, config);

            InitializeMongoMembershipProviderProvider(name, config);           
        }

        private static T GetConfigValue<T>(string configValue, T defaultValue)
        {
            if (String.IsNullOrEmpty(configValue))
                return defaultValue;

            return ((T)Convert.ChangeType(configValue, typeof(T)));
        }

        public override bool ValidateUser(string username, string password)
        {
            if (!SecUtility.ValidateParameter(ref username, true, true, InvalidUsernameCharacters, MAX_USERNAME_LENGTH) || !SecUtility.ValidateParameter(ref password, true, true, null, MAX_PASSWORD_LENGTH))
            {
                return false;
            }

            User user = GetUserByName(username, "ValidateUser");
            if (null == user || user.IsLockedOut || !user.IsApproved)
            {
                return false;
            }


            bool passwordsMatch = ComparePasswords(password, user.Password, user.PasswordSalt, 
                (MembershipPasswordFormat)Enum.Parse(typeof(MembershipPasswordFormat), user.PasswordFormat));
            if (!passwordsMatch)
            {
                // update invalid try count
                UpdateFailureCount(user, "password", isAuthenticated: false);
                return false;
            }

            // User is authenticated. Update last activity and last login dates and failure counts.

            user.LastActivityDate = DateTime.UtcNow;
            user.LastLoginDate = DateTime.UtcNow;
            user.FailedPasswordAnswerAttemptCount = 0;
            user.FailedPasswordAttemptCount = 0;
            user.FailedPasswordAnswerAttemptWindowStart = DateTime.MinValue;
            user.FailedPasswordAttemptWindowStart = DateTime.MinValue;

            var msg = String.Format("Error updating User '{0}'s last login date while validating", username);
            Save(user, msg, "ValidateUser");
            return true;
        }
        
        /// <summary>
        /// Processes a request to update the password for a membership user.
        /// </summary>
        /// <param name="username">The user to update the password for.</param>
        /// <param name="oldPassword">The current password for the specified user.</param>
        /// <param name="newPassword">The new password for the specified user.</param>
        /// <returns>
        /// true if the password was updated successfully; otherwise, false.
        /// </returns>
        public override bool ChangePassword(string username, string oldPassword, string newPassword)
        {
            SecUtility.CheckParameter(ref username, true, true, InvalidUsernameCharacters, MAX_USERNAME_LENGTH, "username");
            SecUtility.CheckParameter(ref oldPassword, true, true, null, MAX_PASSWORD_LENGTH, "oldPassword");
            SecUtility.CheckParameter(ref newPassword, true, true, null, MAX_PASSWORD_LENGTH, "newPassword");

            User user = GetUserByName(username, "ChangePassword");
            if (!CheckPassword(user, oldPassword, true))
                return false;

            if (newPassword.Length < this.MinRequiredPasswordLength)
            {
                throw new ArgumentException(String.Format(CultureInfo.CurrentCulture,
                    InfrastructureResources.Password_too_short,
                    "newPassword", this.MinRequiredPasswordLength), "newPassword");
            }

            if (this.MinRequiredNonAlphanumericCharacters > 0)
            {
                int numNonAlphaNumericChars = 0;
                for (int i = 0; i < newPassword.Length; i++)
                {
                    if (!char.IsLetterOrDigit(newPassword, i))
                    {
                        numNonAlphaNumericChars++;
                    }
                }
                if (numNonAlphaNumericChars < this.MinRequiredNonAlphanumericCharacters)
                {
                    throw new ArgumentException(String.Format(CultureInfo.CurrentCulture,
                        InfrastructureResources.Password_need_more_non_alpha_numeric_chars,
                        "newPassword",
                        this.MinRequiredNonAlphanumericCharacters), "newPassword");
                }
            }

            if ((this.PasswordStrengthRegularExpression.Length > 0) && !Regex.IsMatch(newPassword, this.PasswordStrengthRegularExpression))
            {
                throw new ArgumentException(String.Format(CultureInfo.CurrentCulture,
                    InfrastructureResources.Password_does_not_match_regular_expression,
                    "newPassword"), "newPassword");
            }
            
            // Raise event to let others check new username/password

            ValidatePasswordEventArgs args = new ValidatePasswordEventArgs(username, newPassword, false);
            OnValidatingPassword(args);
            if (args.Cancel)
            {
                if (args.FailureInformation != null)
                    throw args.FailureInformation;
                else
                    throw new MembershipPasswordException(InfrastructureResources.Membership_Custom_Password_Validation_Failure);
            }


            // Save new password

            string encodedPwd = EncodePassword(newPassword, PasswordFormat, user.PasswordSalt);

            user.Password = encodedPwd;
            user.PasswordFormat = PasswordFormat.ToString();
            user.LastPasswordChangedDate = DateTime.UtcNow;

            var msg = String.Format("Unable to save new password for user '{0}'", username);
            Save(user, msg, "ChangePassword");

            return true;
        }

        /// <summary>
        /// Processes a request to update the password question and answer for a membership user.
        /// </summary>
        /// <param name="username">The user to change the password question and answer for.</param>
        /// <param name="password">The password for the specified user.</param>
        /// <param name="newPasswordQuestion">The new password question for the specified user.</param>
        /// <param name="newPasswordAnswer">The new password answer for the specified user.</param>
        /// <returns>
        /// true if the password question and answer are updated successfully; otherwise, false.
        /// </returns>
        public override bool ChangePasswordQuestionAndAnswer(string username, string password, string newPasswordQuestion, string newPasswordAnswer)
        {
            SecUtility.CheckParameter(ref username, true, true, InvalidUsernameCharacters, MAX_USERNAME_LENGTH, "username");
            SecUtility.CheckParameter(ref password, true, true, null, MAX_PASSWORD_LENGTH, "password");

            User user = GetUserByName(username, "ChangePasswordQuestionAndAnswer");
            if (!CheckPassword(user, password, true))
                return false;

            SecUtility.CheckParameter(ref newPasswordQuestion, this.RequiresQuestionAndAnswer, this.RequiresQuestionAndAnswer, null, MAX_PASSWORD_QUESTION_LENGTH, "newPasswordQuestion");
            if (newPasswordAnswer != null)
            {
                newPasswordAnswer = newPasswordAnswer.Trim();
            }

            SecUtility.CheckParameter(ref newPasswordAnswer, this.RequiresQuestionAndAnswer, this.RequiresQuestionAndAnswer, null, MAX_PASSWORD_ANSWER_LENGTH, "newPasswordAnswer");
            string encodedPasswordAnswer = null;
            if (!string.IsNullOrEmpty(newPasswordAnswer))
            {
                encodedPasswordAnswer = EncodePassword(newPasswordAnswer.ToLowerInvariant(), 
                    (MembershipPasswordFormat)Enum.Parse(typeof(MembershipPasswordFormat),  user.PasswordFormat),
                    user.PasswordSalt);
            }
            //SecUtility.CheckParameter(ref encodedPasswordAnswer, this.RequiresQuestionAndAnswer, this.RequiresQuestionAndAnswer, false, MAX_PASSWORD_ANSWER_LENGTH, "newPasswordAnswer");

            user.PasswordQuestion = newPasswordQuestion;
            user.PasswordAnswer = encodedPasswordAnswer;

            var msg = String.Format("Unable to save new password question and answer for user '{0}'", username);
            Save(user, msg, "ChangePasswordQuestionAndAnswer");

            return true;
        }

        /// <summary>
        /// Adds a new membership user to the data source.
        /// </summary>
        /// <param name="username">The user name for the new user.</param>
        /// <param name="password">The password for the new user.</param>
        /// <param name="email">The e-mail address for the new user.</param>
        /// <param name="passwordQuestion">The password question for the new user.</param>
        /// <param name="passwordAnswer">The password answer for the new user</param>
        /// <param name="isApproved">Whether or not the new user is approved to be validated.</param>
        /// <param name="providerUserKey">The unique identifier from the membership data source for the user.</param>
        /// <param name="status">A <see cref="T:System.Web.Security.MembershipCreateStatus"/> enumeration value indicating whether the user was created successfully.</param>
        /// <returns>
        /// A <see cref="T:System.Web.Security.MembershipUser"/> object populated with the information for the newly created user.
        /// </returns>
        public override MembershipUser CreateUser(string username, string password, string email, string passwordQuestion, string passwordAnswer, bool isApproved, object providerUserKey, out MembershipCreateStatus status)
        {
            #region Validation

            if (!SecUtility.ValidateParameter(ref username, true, true, InvalidUsernameCharacters, MAX_USERNAME_LENGTH))
            {
                status = MembershipCreateStatus.InvalidUserName;
                return null;
            }

            if (!SecUtility.ValidateParameter(ref email, this.RequiresUniqueEmail, this.RequiresUniqueEmail, InvalidEmailCharacters, MAX_EMAIL_LENGTH))
            {
                status = MembershipCreateStatus.InvalidEmail;
                return null;
            }

            if (!SecUtility.ValidateParameter(ref password, true, true, null, MAX_PASSWORD_LENGTH))
            {
                status = MembershipCreateStatus.InvalidPassword;
                return null;
            }

            if (password.Length > MAX_PASSWORD_LENGTH)
            {
                status = MembershipCreateStatus.InvalidPassword;
                return null;
            }

            if (null != passwordAnswer)
            {
                passwordAnswer = passwordAnswer.Trim();
            }

            if (string.IsNullOrEmpty(passwordAnswer))
            {
                if (RequiresQuestionAndAnswer)
                {
                    status = MembershipCreateStatus.InvalidAnswer;
                    return null;
                }
            }
            else
            {
                if (passwordAnswer.Length > MAX_PASSWORD_ANSWER_LENGTH)
                {
                    status = MembershipCreateStatus.InvalidAnswer;
                    return null;
                }
            }


            if (!SecUtility.ValidateParameter(ref passwordQuestion, this.RequiresQuestionAndAnswer, true, null, MAX_PASSWORD_QUESTION_LENGTH))
            {
                status = MembershipCreateStatus.InvalidQuestion;
                return null;
            }

            if ((null != providerUserKey) && !(providerUserKey is Guid))
            {
                status = MembershipCreateStatus.InvalidProviderUserKey;
                return null;
            }

            if (password.Length < this.MinRequiredPasswordLength)
            {
                status = MembershipCreateStatus.InvalidPassword;
                return null;
            }

            if (this.MinRequiredNonAlphanumericCharacters > 0)
            {
                int numNonAlphaNumericChars = 0;
                for (int i = 0; i < password.Length; i++)
                {
                    if (!char.IsLetterOrDigit(password, i))
                    {
                        numNonAlphaNumericChars++;
                    }
                }

                if (numNonAlphaNumericChars < this.MinRequiredNonAlphanumericCharacters)
                {
                    status = MembershipCreateStatus.InvalidPassword;
                    return null;
                }
            }

            if ((this.PasswordStrengthRegularExpression.Length > 0) && !Regex.IsMatch(password, this.PasswordStrengthRegularExpression))
            {
                status = MembershipCreateStatus.InvalidPassword;
                return null;
            }

            #endregion

            ValidatePasswordEventArgs args = new ValidatePasswordEventArgs(username, password, true);

            OnValidatingPassword(args);

            if (args.Cancel)
            {
                status = MembershipCreateStatus.InvalidPassword;
                return null;
            }

            if (RequiresUniqueEmail && !String.IsNullOrEmpty(GetUserNameByEmail(email)))
            {
                status = MembershipCreateStatus.DuplicateEmail;
                return null;
            }

            MembershipUser u = GetUser(username, false);
            if (null != u)
            {
                status = MembershipCreateStatus.DuplicateUserName;
                return null;
            }


            DateTime createDate = DateTime.UtcNow;

            if (null == providerUserKey)
            {
                providerUserKey = Guid.NewGuid();
            }
            else
            {
                if (!(providerUserKey is Guid))
                {
                    status = MembershipCreateStatus.InvalidProviderUserKey;
                    return null;
                }
            }

            var createAt = DateTime.UtcNow;
            string salt = GenerateSalt();

            var answer = passwordAnswer;
            if (null != answer)
            {
                answer = EncodePassword(passwordAnswer.ToLowerInvariant(), PasswordFormat, salt);
            }

            var user = new User();
            user.Id = (Guid)providerUserKey;
            user.Username = username;
            //user.LowercaseUsername = username.ToLowerInvariant();
            user.DisplayName = username;
            user.Email = email;
            //user.LowercaseEmail = (null == email) ? null : email.ToLowerInvariant();
            user.Password = EncodePassword(password, PasswordFormat, salt);
            user.PasswordQuestion = passwordQuestion;
            user.PasswordAnswer = answer;
            user.PasswordFormat = PasswordFormat.ToString();
            user.PasswordSalt = salt;
            user.IsApproved = isApproved;
            user.LastPasswordChangedDate = DateTime.MinValue;
            user.CreateDate = createAt;
            user.IsLockedOut = false;
            user.LastLockedOutDate = DateTime.MinValue;
            user.LastActivityDate = createAt;
            user.FailedPasswordAnswerAttemptCount = 0;
            user.FailedPasswordAnswerAttemptWindowStart = DateTime.MinValue;
            user.FailedPasswordAttemptCount = 0;
            user.FailedPasswordAttemptWindowStart = DateTime.MinValue;

            var msg = String.Format("Error creating new User '{0}'", username);
            SaveNew(user, msg, "CreateUser");

            status = MembershipCreateStatus.Success;
            return GetUser(username, false);
        }

        /// <summary>
        /// Removes a user from the membership data source.
        /// </summary>
        /// <param name="username">The name of the user to delete.</param>
        /// <param name="deleteAllRelatedData">true to delete data related to the user from the database; false to leave data related to the user in the database.</param>
        /// <returns>
        /// true if the user was successfully deleted; otherwise, false.
        /// </returns>
        public override bool DeleteUser(string username, bool deleteAllRelatedData)
        {
            if (String.IsNullOrWhiteSpace(username)) return false;

            var user = _userService.GetByUserName(username);
            _userService.Delete(user.Id);

            return true;

            //when implementing safe mode update this
            //return result.Ok;
        }


        /// <summary>
        /// Gets a collection of membership users where the user name matches the specified string.
        /// </summary>
        /// <param name="usernameToMatch">The user name to search for.
        /// Match string may contain the standard SQL LIKE wildcard: %
        ///   "sm%"  -> StartsWith("sm")
        ///   "%ith" -> EndsWith("ith")
        ///   "%mit%" -> Contains("mit")
        /// </param>
        /// <param name="pageIndex">The index of the page of results to return. <paramref name="pageIndex"/> is zero-based.</param>
        /// <param name="pageSize">The size of the page of results to return.</param>
        /// <param name="totalRecords">The total number of matched users.</param>
        /// <returns>
        /// A <see cref="T:System.Web.Security.MembershipUserCollection"/> collection that contains a page of <paramref name="pageSize"/><see cref="T:System.Web.Security.MembershipUser"/> objects beginning at the page specified by <paramref name="pageIndex"/>.
        /// </returns>
        public override MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            MembershipUserCollection users = new MembershipUserCollection();
            if (String.IsNullOrWhiteSpace(usernameToMatch))
            {
                totalRecords = 0;
                return users;
            }

            //return FindUsersBy(ElementNames.LowercaseUsername, usernameToMatch, pageIndex, pageSize, out totalRecords);
            var matchedUsers = _userService.GetAll()
                .Where(u => u.Username.Contains(usernameToMatch) ||
                            u.Username.StartsWith(usernameToMatch) ||
                            u.Username.EndsWith(usernameToMatch))
                .Skip(pageIndex * pageSize)
                .Take(pageSize)
                .ToList();


            foreach (var user in matchedUsers)
            {
                users.Add(ToMembershipUser(user));
            }

            // execute second query to get total count
            totalRecords = matchedUsers.Count();

            return users;
        }


        public override MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            MembershipUserCollection users = new MembershipUserCollection();
            if (String.IsNullOrWhiteSpace(emailToMatch))
            {
                totalRecords = 0;
                return users;
            }

            //return FindUsersBy(ElementNames.LowercaseUsername, usernameToMatch, pageIndex, pageSize, out totalRecords);
            var matchedUsers = _userService.GetAll()
                .Where(u => u.Email.Contains(emailToMatch) ||
                            u.Email.StartsWith(emailToMatch) ||
                            u.Email.EndsWith(emailToMatch))
                .Skip(pageIndex * pageSize)
                .Take(pageSize)
                .ToList();


            foreach (var user in matchedUsers)
            {
                users.Add(ToMembershipUser(user));
            }

            // execute second query to get total count
            totalRecords = matchedUsers.Count();

            return users;
        }
                
        public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
        {
            MembershipUserCollection users = new MembershipUserCollection();

            //return FindUsersBy(ElementNames.LowercaseUsername, usernameToMatch, pageIndex, pageSize, out totalRecords);
            var matchedUsers = _userService.GetAll()
                .Skip(pageIndex * pageSize)
                .Take(pageSize)
                .ToList();

            if (null == matchedUsers)
            {
                totalRecords = 0;
                return users;
            }

            // execute second query to get total count
            totalRecords = (int)matchedUsers.Count();

            matchedUsers.ForEach(u => users.Add(ToMembershipUser(u)));

            return users;
        }

        public override int GetNumberOfUsersOnline()
        {
            TimeSpan onlineSpan = new TimeSpan(0, Membership.UserIsOnlineTimeWindow, 0);
            DateTime compareTime = DateTime.UtcNow.Subtract(onlineSpan);

            var count = _userService.GetAll()
                .Where(u => u.LastActivityDate > compareTime).Count();

            return count;
        }

        public override string GetPassword(string username, string answer)
        {
            if (!EnablePasswordRetrieval)
            {
                throw new ProviderException(InfrastructureResources.Membership_PasswordRetrieval_not_supported);
            }

            User user = GetUserByName(username, "GetPassword");
            if (null == user)
            {
                throw new MembershipPasswordException(InfrastructureResources.Membership_UserNotFound);
            }

            if (user.IsLockedOut)
            {
                throw new MembershipPasswordException(InfrastructureResources.Membership_AccountLockOut);
            }


            if (RequiresQuestionAndAnswer && !ComparePasswords(answer, user.PasswordAnswer, user.PasswordSalt, 
                (MembershipPasswordFormat)Enum.Parse(typeof(MembershipPasswordFormat), user.PasswordFormat)))
            {
                UpdateFailureCount(user, "passwordAnswer", false);

                throw new MembershipPasswordException(InfrastructureResources.Membership_WrongAnswer);
            }

            var password = user.Password;
            if (user.PasswordFormat == MembershipPasswordFormat.Encrypted.ToString())
            {
                password = UnEncodePassword(password, 
                    (MembershipPasswordFormat)Enum.Parse(typeof(MembershipPasswordFormat), user.PasswordFormat));
            }

            return password;
        }

        public override MembershipUser GetUser(string username, bool userIsOnline)
        {
            if (String.IsNullOrWhiteSpace(username)) return null;

            if (userIsOnline)
            {
                User user = _userService.GetByUserName(username);
                user.LastActivityDate = DateTime.UtcNow;

                return ToMembershipUser(user);
            }
            else
            {
                //User user = Collection.AsQueryable().Where(u => u.LowercaseUsername == username.ToLowerInvariant()).FirstOrDefault();
                User user = _userService.GetByUserName(username);
                return ToMembershipUser(user);
            }
        }

        public override MembershipUser GetUser(object providerUserKey, bool userIsOnline)
        {
            if (!(providerUserKey is Guid))
            {
                throw new ArgumentException(InfrastructureResources.Membership_InvalidProviderUserKey, "providerUserKey");
            }
            if (userIsOnline)
            {
                User user = _userService.GetById((Guid)providerUserKey);
                user.LastActivityDate = DateTime.UtcNow;

                return ToMembershipUser(user);
            }
            else
            {
                //User user = Collection.AsQueryable().Where(u => u.LowercaseUsername == username.ToLowerInvariant()).FirstOrDefault();
                User user = _userService.GetById((Guid)providerUserKey);
                return ToMembershipUser(user);
            }
        }

        public override string GetUserNameByEmail(string email)
        {
			if (null == email)
				return null;

            var username = _userService.GetAll()
                .Where(u => u.Email == email)
                .Select(u => u.Username)
                .FirstOrDefault();

            return username;
        }

        /// <summary>
        /// Resets a user's password to a new, automatically generated password.
        /// </summary>
        /// <param name="username">The user to reset the password for.</param>
        /// <param name="passwordAnswer">The password answer for the specified user.</param>
        /// <returns>The new password for the specified user.</returns>
        /// <exception cref="T:System.Configuration.Provider.ProviderException">username is not found in the membership database.- or -The 
        /// change password action was canceled by a subscriber to the System.Web.Security.Membership.ValidatePassword
        /// event and the <see cref="P:System.Web.Security.ValidatePasswordEventArgs.FailureInformation"></see> property was null.- or -An 
        /// error occurred while retrieving the password from the database. </exception>
        /// <exception cref="T:System.NotSupportedException"><see cref="P:System.Web.Security.SqlMembershipProvider.EnablePasswordReset"></see> 
        /// is set to false. </exception>
        /// <exception cref="T:System.ArgumentException">username is an empty string (""), contains a comma, or is longer than 256 characters.
        /// - or -passwordAnswer is an empty string or is longer than 128 characters and 
        /// <see cref="P:System.Web.Security.SqlMembershipProvider.RequiresQuestionAndAnswer"></see> is true.- or -passwordAnswer is longer 
        /// than 128 characters after encoding.</exception>
        /// <exception cref="T:System.ArgumentNullException">username is null.- or -passwordAnswer is null and 
        /// <see cref="P:System.Web.Security.SqlMembershipProvider.RequiresQuestionAndAnswer"></see> is true.</exception>
        /// <exception cref="T:System.Web.Security.MembershipPasswordException">passwordAnswer is invalid. - or -The user account is currently locked out.</exception>
        public override string ResetPassword(string username, string answer)
        {
            if (!this.EnablePasswordReset)
            {
                throw new NotSupportedException(InfrastructureResources.Not_configured_to_support_password_resets);
            }

            User user = GetUserByName(username, "ResetPassword");
            if (null == user)
            {
                throw new ProviderException(InfrastructureResources.Membership_UserNotFound);
            }
            if (user.IsLockedOut)
            {
                throw new ProviderException(InfrastructureResources.Membership_AccountLockOut);
            }


            if (RequiresQuestionAndAnswer &&
                (null == answer || !ComparePasswords(answer, user.PasswordAnswer, user.PasswordSalt,
                (MembershipPasswordFormat)Enum.Parse(typeof(MembershipPasswordFormat), user.PasswordFormat)))) 
            {
                UpdateFailureCount(user, "passwordAnswer", false);

                throw new MembershipPasswordException(InfrastructureResources.Membership_InvalidAnswer);
            }

            string newPassword = Membership.GeneratePassword(NEW_PASSWORD_LENGTH, MinRequiredNonAlphanumericCharacters);

            ValidatePasswordEventArgs args =
              new ValidatePasswordEventArgs(username, newPassword, true);

            OnValidatingPassword(args);

            if (args.Cancel)
                if (args.FailureInformation != null)
                    throw args.FailureInformation;
                else
                    throw new MembershipPasswordException(InfrastructureResources.Membership_Custom_Password_Validation_Failure);

            user.Password = EncodePassword(newPassword,
                (MembershipPasswordFormat)Enum.Parse(typeof(MembershipPasswordFormat), user.PasswordFormat), 
                user.PasswordSalt);
            user.LastPasswordChangedDate = DateTime.UtcNow;

            {
                var msg = String.Format("Error saving User '{0}' while resetting password", username);
                Save(user, msg, "ResetPassword");
            }

            return newPassword;
        }


        public override bool UnlockUser(string userName)
        {
            User user = GetUserByName(userName, "UnlockUser");
            if (null == user)
            {
                return false;
            }

            user.IsLockedOut = false;
            user.LastLockedOutDate = DateTime.MinValue;
            user.FailedPasswordAnswerAttemptCount = 0;
            user.FailedPasswordAnswerAttemptWindowStart = DateTime.MinValue;
            user.FailedPasswordAttemptCount = 0;
            user.FailedPasswordAttemptWindowStart = DateTime.MinValue;

            var msg = String.Format("Error saving User '{0}' while attempting to remove account lock", userName);
            Save(user, msg, "UnlockUser");
            return true;
        }

        public override void UpdateUser(MembershipUser user)
        {
            User u = GetUserByName(user.UserName, "UpdateUser");
            if (null == user)
            {
                throw new ProviderException(InfrastructureResources.Membership_UserNotFound);
            }

            u.Email = user.Email;
            u.Comment = user.Comment;
            u.IsApproved = user.IsApproved;
            u.LastLoginDate = user.LastLoginDate;
            u.LastActivityDate = user.LastActivityDate;

            {
                var msg = "Error saving user while attempting to update Email and IsApproved status";
                Save(u, msg, "UpdateUser");
            }
        }

        #region Protected Methods

        protected void ValidatePwdStrengthRegularExpression()
        {
            // Validate regular expression, if supplied.
            if (null == _passwordStrengthRegularExpression)
                _passwordStrengthRegularExpression = String.Empty;

            _passwordStrengthRegularExpression = _passwordStrengthRegularExpression.Trim();
            if (_passwordStrengthRegularExpression.Length > 0)
            {
                try
                {
                    new Regex(_passwordStrengthRegularExpression);
                }
                catch (ArgumentException ex)
                {
                    throw new ProviderException(ex.Message, ex);
                }
            }
        }

        protected MembershipUser ToMembershipUser(User user)
        {
            if (null == user)
                return null;

            return new MembershipUser(this.ProviderName, user.Username, user.Id, user.Email,
                user.PasswordQuestion, user.Comment, user.IsApproved, user.IsLockedOut,
                user.CreateDate, user.LastLoginDate, user.LastActivityDate, user.LastPasswordChangedDate,
                user.LastLockedOutDate
            );
        }

        protected static string GenerateSalt()
        {
            byte[] data = new byte[0x10];
            new RNGCryptoServiceProvider().GetBytes(data);
            return Convert.ToBase64String(data);
        }


        protected bool ComparePasswords(string password, string dbpassword, string salt, MembershipPasswordFormat passwordFormat)
        {
            //   Compares password values based on the MembershipPasswordFormat.
            string pass1 = password;
            string pass2 = dbpassword;

            switch (passwordFormat)
            {
                case MembershipPasswordFormat.Encrypted:
                    pass2 = UnEncodePassword(dbpassword, passwordFormat);
                    break;
                case MembershipPasswordFormat.Hashed:
                    pass1 = EncodePassword(password, passwordFormat, salt);
                    break;
                default:
                    break;
            }

            if (pass1 == pass2)
            {
                return true;
            }

            return false;
        }

        public string EncodePassword(string password, MembershipPasswordFormat passwordFormat, string salt)
        {
            //   Encrypts, Hashes, or leaves the password clear based on the PasswordFormat.
            if (String.IsNullOrEmpty(password))
                return password;

            byte[] bytes = Encoding.Unicode.GetBytes(password);
            byte[] src = Convert.FromBase64String(salt);
            byte[] dst = new byte[src.Length + bytes.Length];
            byte[] inArray;

            Buffer.BlockCopy(src, 0, dst, 0, src.Length);
            Buffer.BlockCopy(bytes, 0, dst, src.Length, bytes.Length);

            switch (passwordFormat)
            {
                case MembershipPasswordFormat.Clear:
                    return password;
                case MembershipPasswordFormat.Encrypted:
                    inArray = EncryptPassword(dst);
                    break;
                case MembershipPasswordFormat.Hashed:
                    HashAlgorithm algorithm = HashAlgorithm.Create(Membership.HashAlgorithmType);
                    if (null == algorithm)
                    {
                        throw new ProviderException(String.Format(
                            "",//Resources.Provider_unrecognized_hash_algorithm,
                            Membership.HashAlgorithmType));
                    }
                    inArray = algorithm.ComputeHash(dst);

                    break;
                default:
                    throw new ProviderException("Unsupported password format.");
            }

            return Convert.ToBase64String(inArray);
        }

        protected string UnEncodePassword(string encodedPassword, MembershipPasswordFormat passwordFormat)
        {
            //   Decrypts or leaves the password clear based on the PasswordFormat.
            string password = encodedPassword;

            switch (passwordFormat)
            {
                case MembershipPasswordFormat.Clear:
                    break;
                case MembershipPasswordFormat.Encrypted:
                    byte[] bytes = base.DecryptPassword(Convert.FromBase64String(password));
                    if (null == bytes)
                    {
                        password = null;
                    }
                    else
                    {
                        // strip off salt
                        password = Encoding.Unicode.GetString(bytes, 0x10, bytes.Length - 0x10);
                    }
                    break;
                case MembershipPasswordFormat.Hashed:
                    throw new ProviderException("");//Resources.Provider_can_not_decode_hashed_password);
                default:
                    throw new ProviderException("Unsupported password format.");
            }

            return password;
        }

        protected void HandleDataExceptionAndThrow(Exception ex, string action)
        {
            if (WriteExceptionsToEventLog)
            {
                WriteToEventLog(ex, action);

                throw new ProviderException(_exceptionMessage);
            }

            throw ex;
        }

        /// <summary>
        /// WriteToEventLog
        ///   A helper function that writes exception detail to the event log. Exceptions
        /// are written to the event log as a security measure to avoid private database
        /// details from being returned to the browser. If a method does not return a status
        /// or boolean indicating the action succeeded or failed, a generic exception is also 
        /// thrown by the caller.
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="action"></param>
        protected void WriteToEventLog(Exception ex, string action)
        {
            EventLog log = new EventLog();
            log.Source = _eventSource;
            log.Log = _eventLog;

            string message = "An exception occurred communicating with the data source.\n\n";
            message += "Action: " + action + "\n\n";
            message += "Exception: " + ex.ToString();

            log.WriteEntry(message);
        }

        /// <summary>
        /// Convenience method that handles errors when retrieving a User
        /// </summary>
        /// <param name="username">The name of the user to retrieve</param>
        /// <param name="action">The name of the action that attempted the retrieval. Used in case exceptions are raised and written to EventLog</param>
        /// <returns></returns>
        protected User GetUserByName(string username, string action)
        {
            if (String.IsNullOrWhiteSpace(username))
            {
                return null;
            }

            User user = null;

            try
            {
                //user = Collection.AsQueryable().Where(u => u.LowercaseUsername == username.ToLowerInvariant()).SingleOrDefault();
                user = _userService.GetByUserName(username);
            }
            catch (Exception ex)
            {
                var msg = String.Format("Unable to retrieve User information for user '{0}'", username);
                HandleDataExceptionAndThrow(new ProviderException(msg, ex), action);
            }

            return user;
        }

        /// <summary>
        /// Saves a User to persistent storage
        /// </summary>
        /// <param name="user">The User to save</param>
        /// <param name="failureMessage">A message that will be used if an exception is raised during save</param>
        /// <param name="action">The name of the action which attempted the save (ex. "CreateUser"). Used in case exceptions are written to EventLog.</param>
        protected void SaveNew(User user, string failureMessage, string action)
        {
            _userService.Create(user);
        }

        /// <summary>
        /// Saves a User to persistent storage
        /// </summary>
        /// <param name="user">The User to save</param>
        /// <param name="failureMessage">A message that will be used if an exception is raised during save</param>
        /// <param name="action">The name of the action which attempted the save (ex. "CreateUser"). Used in case exceptions are written to EventLog.</param>
        protected void Save(User user, string failureMessage, string action)
        {
            _userService.Update(user);

            //TODO: Implement safe saving for the mongo DAO layer

            //pass safe (true/false) to saving methods
            //throw a dao exception if failed.
            
            /*
            try 
            {
                result = users.Save(user, SafeMode.True);
            }
            catch (Exception ex) {
                HandleDataExceptionAndThrow(new ProviderException(failureMessage, ex), action);
            }
            if (null == result)
            {
                HandleDataExceptionAndThrow(new ProviderException("Save to database did not return a status result"), action);
            }
            else if (!result.Ok)
            {
                HandleDataExceptionAndThrow(new ProviderException(result.LastErrorMessage), action);
            }
            */
        }

        protected bool CheckPassword(User user, string password, bool failIfNotApproved)
        {
            if (null == user) return false;
            if (!user.IsApproved && failIfNotApproved) return false;

            string encodedPwdFromUser = EncodePassword(password, 
                (MembershipPasswordFormat)Enum.Parse(typeof(MembershipPasswordFormat), user.PasswordFormat), 
                user.PasswordSalt);
            bool isAuthenticated = user.Password.Equals(encodedPwdFromUser);

            if ((isAuthenticated && (user.FailedPasswordAttemptCount == 0)) && (user.FailedPasswordAnswerAttemptCount == 0))
                return true;

            UpdateFailureCount(user, "password", isAuthenticated);

            return isAuthenticated;
        }

        /// <summary>
        /// A helper method that performs the checks and updates associated User with password failure tracking
        /// </summary>
        /// <param name="username"></param>
        /// <param name="failureType"></param>
        /// <param name="isAuthenticated"></param>
        protected void UpdateFailureCount(User user, string failureType, bool isAuthenticated)
        {
            if (!((failureType == "password") || (failureType == "passwordAnswer")))
            {
                throw new ArgumentException("Invalid value for failureType parameter. Must be 'password' or 'passwordAnswer'.", "failureType");
            }

            if (user.IsLockedOut)
                return; // Just exit without updating any fields if user is locked out


            if (isAuthenticated)
            {
                // User is valid, so make sure Attempt Counts and IsLockedOut fields have been reset
                if ((user.FailedPasswordAttemptCount > 0) || (user.FailedPasswordAnswerAttemptCount > 0))
                {
                    user.FailedPasswordAnswerAttemptCount = 0;
                    user.FailedPasswordAttemptCount = 0;
                    user.FailedPasswordAnswerAttemptWindowStart = DateTime.MinValue;
                    user.FailedPasswordAttemptWindowStart = DateTime.MinValue;
                    var msg = String.Format("Unable to reset Authenticated User's FailedPasswordAttemptCount property for user '{0}'", user.Username);

                    Save(user, msg, "UpdateFailureCount");
                }
                return;
            }

            // If we get here that means isAuthenticated = false, which means the user did not log on successfully.
            // Log the failure and possibly lock out the user if she exceeded the number of allowed attempts.

            DateTime windowStart = DateTime.MinValue;
            int failureCount = 0;
            if (failureType == "password")
            {
                windowStart = user.FailedPasswordAttemptWindowStart;
                failureCount = user.FailedPasswordAttemptCount;
            }
            else if (failureType == "passwordAnswer")
            {
                windowStart = user.FailedPasswordAnswerAttemptWindowStart;
                failureCount = user.FailedPasswordAnswerAttemptCount;
            }

            DateTime windowEnd = windowStart.AddMinutes(PasswordAttemptWindow);

            if (failureCount == 0 || DateTime.UtcNow > windowEnd)
            {
                // First password failure or outside of PasswordAttemptWindow. 
                // Start a new password failure count from 1 and a new window starting now.

                if (failureType == "password")
                {
                    user.FailedPasswordAttemptCount = 1;
                    user.FailedPasswordAttemptWindowStart = DateTime.UtcNow;
                }
                else if (failureType == "passwordAnswer")
                {
                    user.FailedPasswordAnswerAttemptCount = 1;
                    user.FailedPasswordAnswerAttemptWindowStart = DateTime.UtcNow;
                }

                var msg = String.Format("Unable to update failure count and window start for user '{0}'", user.Username);
                Save(user, msg, "UpdateFailureCount");

                return;
            }


            // within PasswordAttemptWindow

            failureCount++;

            if (failureCount >= MaxInvalidPasswordAttempts)
            {
                // Password attempts have exceeded the failure threshold. Lock out the user.
                user.IsLockedOut = true;
                user.LastLockedOutDate = DateTime.UtcNow;
                user.FailedPasswordAttemptCount = failureCount;

                var msg = String.Format("Unable to lock out user '{0}'", user.Username);
                Save(user, msg, "UpdateFailureCount");

                return;
            }


            // Password attempts have not exceeded the failure threshold. Update
            // the failure counts. Leave the window the same.

            if (failureType == "password")
            {
                user.FailedPasswordAttemptCount = failureCount;
            }
            else if (failureType == "passwordAnswer")
            {
                user.FailedPasswordAnswerAttemptCount = failureCount;
            }

            {
                var msg = String.Format("Unable to update failure count for user '{0}'", user.Username);
                Save(user, msg, "UpdateFailureCount");
            }

            return;
        }

        #endregion
    }

    /// <summary>
    /// Provides general purpose validation functionality.
    /// </summary>
    internal class SecUtility
    {
        /// <summary>
        /// Checks the parameter and throws an exception if one or more rules are violated.
        /// </summary>
        /// <param name="param">The parameter to check.</param>
        /// <param name="checkForNull">When <c>true</c>, verify <paramref name="param"/> is not null.</param>
        /// <param name="checkIfEmpty">When <c>true</c> verify <paramref name="param"/> is not an empty string.</param>
        /// <param name="invalidChars">When not null, verify <paramref name="param"/> does not contain any of the supplied characters.</param>
        /// <param name="maxSize">The maximum allowed length of <paramref name="param"/>.</param>
        /// <param name="paramName">Name of the parameter to check. This is passed to the exception if one is thrown.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="param"/> is null and <paramref name="checkForNull"/> is true.</exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="param"/> does not satisfy one of the remaining requirements.</exception>
        /// <remarks>This method performs the same implementation as Microsoft's version at System.Web.Util.SecUtility.</remarks>
        internal static void CheckParameter(ref string param, bool checkForNull, bool checkIfEmpty, string invalidChars, int maxSize, string paramName)
        {
            if (null == param && checkForNull)
            {
                throw new ArgumentNullException(paramName);
            }
            else
            {
                param = param.Trim();
                if (checkIfEmpty && (param.Length < 1))
                {
                    throw new ArgumentException(String.Format(InfrastructureResources.Parameter_can_not_be_empty, paramName), paramName);
                }
                if ((maxSize > 0) && (param.Length > maxSize))
                {
                    throw new ArgumentException(String.Format(InfrastructureResources.Parameter_too_long, paramName, maxSize.ToString(CultureInfo.InvariantCulture)), paramName);
                }
                if (!String.IsNullOrWhiteSpace(invalidChars))
                {
                    var chars = invalidChars.ToCharArray();
                    for (int i = 0; i < chars.Length; i++)
                    {
                        if (param.Contains(chars[i]))
                        {
                            throw new ArgumentException(String.Format(InfrastructureResources.Parameter_contains_invalid_characters,
                                paramName,
                                String.Join("','", invalidChars.Split())),
                                paramName);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Verifies that <paramref name="param"/> conforms to all requirements.
        /// </summary>
        /// <param name="param">The parameter to check.</param>
        /// <param name="checkForNull">When <c>true</c>, verify <paramref name="param"/> is not null.</param>
        /// <param name="checkIfEmpty">When <c>true</c> verify <paramref name="param"/> is not an empty string.</param>
        /// <param name="invalidChars">When not null, verify <paramref name="param"/> does not contain any of the supplied characters.</param>
        /// <param name="maxSize">The maximum allowed length of <paramref name="param"/>.</param>
        /// <returns>Returns <c>true</c> if all requirements are met; otherwise returns <c>false</c>.</returns>
        internal static bool ValidateParameter(ref string param, bool checkForNull, bool checkIfEmpty, string invalidChars, int maxSize)
        {
            if (null == param)
            {
                return !checkForNull;
            }
            param = param.Trim();

            bool valid = (!checkIfEmpty || (param.Length >= 1)) &&
                ((maxSize <= 0) || (param.Length <= maxSize));

            if (valid && !String.IsNullOrWhiteSpace(invalidChars))
            {
                var chars = invalidChars.ToCharArray();
                var i = 0;
                while (valid && i < chars.Length)
                {
                    valid &= !param.Contains(chars[i]);
                    i++;
                }
            }

            return valid;
        }


        /// <summary>
        /// Checks each element in the parameter array and throws an exception if one or more rules are violated.
        /// </summary>
        /// <param name="param">The parameter array to check.</param>
        /// <param name="checkForNull">When <c>true</c>, verify <paramref name="param"/> is not null.</param>
        /// <param name="checkIfEmpty">When <c>true</c> verify <paramref name="param"/> is not an empty string.</param>
        /// <param name="invalidChars">When not null, verify <paramref name="param"/> does not contain any of the supplied characters.</param>
        /// <param name="maxSize">The maximum allowed length of <paramref name="param"/>.</param>
        /// <param name="paramName">Name of the parameter to check. This is passed to the exception if one is thrown.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="param"/> is null and <paramref name="checkForNull"/> is true.</exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="param"/> does not satisfy one of the remaining requirements.</exception>
        /// <remarks>This method performs the same implementation as Microsoft's version at System.Web.Util.SecUtility.</remarks>
        internal static void CheckArrayParameter(ref string[] param, bool checkForNull, bool checkIfEmpty, string invalidChars, int maxSize, string paramName)
        {
            if (null == param)
            {
                throw new ArgumentNullException(paramName);
            }
            for (var i = 0; i < param.Length; i++)
            {
                CheckParameter(ref param[i], checkForNull, checkIfEmpty, invalidChars, maxSize, paramName);
            }
        }
    }
}