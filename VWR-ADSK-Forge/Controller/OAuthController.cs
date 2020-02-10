using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Threading.Tasks;

using Autodesk.Forge;

namespace Backend.Controller
{
    public class OAuthController : ControllerBase
    {
        #region Vars
        private static dynamic InternalToken { get; set; }
        //Token for frontend usage
        private static dynamic PublicToken { get; set; }
        #endregion

        #region Methods

        [HttpGet]
        [Route("/api/forge/oauth/token")]
        public async Task<AccessToken> GetPublicTokenAsync()
        {
            Model.Credentials credentials = await Model.Credentials.FromSessionAsync(Request.Cookies, Response.Cookies);

            if (credentials == null)
            {
                base.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                return new AccessToken();
            }

            // return the public (viewables:read) access token
            return new AccessToken()
            {
                access_token = credentials.TokenPublic,
                expires_in = (int)credentials.ExpiresAt.Subtract(DateTime.Now).TotalSeconds
            };
        }

        /// <summary>
        /// Response for GetPublicToken
        /// </summary>
        public struct AccessToken
        {
            public string access_token { get; set; }
            public int expires_in { get; set; }
        }

        [HttpGet]
        [Route("/api/forge/oauth/signout")]
        public IActionResult Singout()
        {
            // finish the session
            Model.Credentials.Signout(base.Response.Cookies);

            return Redirect("/");
        }

        [HttpGet]
        [Route("/api/forge/oauth/url")]
        public string GetOAuthURL()
        {
            // prepare the sign in URL
            Scope[] scopes = { Scope.DataRead };
            ThreeLeggedApi _threeLeggedApi = new ThreeLeggedApi();
            string oauthUrl = _threeLeggedApi.Authorize(
              Model.Credentials.GetAppSetting("FORGE_CLIENT_ID"),
              oAuthConstants.CODE,
              Model.Credentials.GetAppSetting("FORGE_CALLBACK_URL"),
              new Scope[] { Scope.BucketCreate, Scope.BucketRead, Scope.BucketDelete, Scope.DataRead, Scope.DataWrite, Scope.DataCreate, Scope.CodeAll, Scope.ViewablesRead });

            return oauthUrl;
        }

        [HttpGet]
        [Route("/api/forge/callback/oauth")] // see Web.Config FORGE_CALLBACK_URL variable
        public async Task<IActionResult> OAuthCallbackAsync(string code)
        {
            if (string.IsNullOrWhiteSpace(code)) return Redirect("/");
            // create credentials form the oAuth CODE
            Model.Credentials credentials = await Model.Credentials.CreateFromCodeAsync(code, Response.Cookies);

            return Redirect("/");
        }

        [HttpGet]
        [Route("/api/forge/clientid")] // see Web.Config FORGE_CALLBACK_URL variable
        public dynamic GetClientID()
        {
            return new { id = Model.Credentials.GetAppSetting("FORGE_CLIENT_ID") };
        }

        public static async Task<dynamic> GetInternalAsync()
        {
            if (InternalToken == null || InternalToken.ExpiresAt < DateTime.UtcNow)
            {
                InternalToken = await Get2LeggedTokenAsync(new Scope[] { Scope.BucketCreate, Scope.BucketRead, Scope.BucketDelete, Scope.DataRead, Scope.DataWrite, Scope.DataCreate, Scope.CodeAll });
                InternalToken.ExpiresAt = DateTime.UtcNow.AddSeconds(InternalToken.expires_in);
            }

            return InternalToken;
        }

        /// <summary>
        /// Get access token with public (viewables:read) scope
        /// </summary>
        [HttpGet]
        [Route("/api/forge/oauth/token2LO")]
        public async Task<dynamic> GetPublicAsync()
        {
            if (PublicToken == null || PublicToken.ExpiresAt < DateTime.UtcNow)
            {
                PublicToken = await Get2LeggedTokenAsync(new Scope[] { Scope.ViewablesRead });
                PublicToken.ExpiresAt = DateTime.UtcNow.AddSeconds(PublicToken.expires_in);
            }
            return PublicToken;
        }

        /// <summary>
        /// Get the access token from Autodesk
        /// </summary>
        private static async Task<dynamic> Get2LeggedTokenAsync(Scope[] scopes)
        {
            TwoLeggedApi oauth = new TwoLeggedApi();
            string grantType = "client_credentials";
            dynamic bearer = await oauth.AuthenticateAsync(
              Model.Credentials.GetAppSetting("FORGE_CLIENT_ID"),
              Model.Credentials.GetAppSetting("FORGE_CLIENT_SECRET"),
              grantType,
              scopes);
            return bearer;
        }
    }
    #endregion
}
