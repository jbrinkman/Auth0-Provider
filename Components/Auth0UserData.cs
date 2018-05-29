#region Usings

using System;
using System.Runtime.Serialization;

using DotNetNuke.Services.Authentication.OAuth;

#endregion

namespace Dnn.Authentication.Auth0.Components
{
    [DataContract]
    public class Auth0UserData : UserData
    {
        #region Overrides

        public override string FirstName
        {
            get { return Auth0FirstName; }
            set { }
        }

        public override string LastName
        {
            get { return Auth0LastName; }
            set { }
        }

        #endregion

        [DataMember(Name = "birthday")]
        public string Birthday { get; set; }

        [DataMember(Name = "link")]
        public Uri Link { get; set; }

        [DataMember(Name = "first_name")]
        public string Auth0FirstName { get; set; }

        [DataMember(Name = "last_name")]
        public string Auth0LastName { get; set; }
    }
}