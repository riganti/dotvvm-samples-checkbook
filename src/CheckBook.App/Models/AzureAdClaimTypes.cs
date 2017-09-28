using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CheckBook.App.Models
{
    public class AzureAdClaimTypes
    {
        public const string TenantId = "http://schemas.microsoft.com/identity/claims/tenantid";
        public const string ObjectId = "http://schemas.microsoft.com/identity/claims/objectidentifier";
        public const string Role = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role";
        public const string IdentityProvider = "http://schemas.microsoft.com/identity/claims/identityprovider";
        public const string Groups = "http://schemas.microsoft.com/ws/2008/06/identity/claims/groups";
        public const string GivenName = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname";
        public const string Surname = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/surname";
        public const string Upn = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/upn";
        public const string DisplayName = "name";
        public const string IdentityName = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name";
        public const string AuthTime = "auth_time";
    }
}