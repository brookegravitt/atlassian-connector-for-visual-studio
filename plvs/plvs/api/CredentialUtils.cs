using System;
using System.Net;

namespace Atlassian.plvs.api {
    public static class CredentialUtils {
        public static CredentialCache getCredentialCacheForUserAndPassword(string url, string userName, string password) {
            CredentialCache credsCache = new CredentialCache();
            NetworkCredential creds = new NetworkCredential(getUserNameWithoutDomain(userName), password, getUserDomain(userName));
            credsCache.Add(new Uri(url), "Basic", creds);
            return credsCache;
        }

        public static string getUserNameWithoutDomain(string userName) {
            string userWithoutDomain = userName.Contains("\\")
                                           ? userName.Substring(userName.IndexOf("\\") + 1)
                                           : userName;
            return userWithoutDomain;
        }

        public static string getUserDomain(string userName) {
            string domain = userName.Contains("\\")
                                ? userName.Substring(0, userName.IndexOf("\\"))
                                : null;
            return domain;
        }
    }
}