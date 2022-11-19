using IdentityModel.Client;
using IdentityModel.OidcClient.Browser;
using System.Diagnostics;

namespace Demo.Notes.Client.Mobile
{
    public class MauiAuthenticationBrowser : IdentityModel.OidcClient.Browser.IBrowser
    {
        public MauiAuthenticationBrowser(string callbackUrl)
        {
            CallbackUrl = callbackUrl;
        }

        public string CallbackUrl { get; }

        public async Task<BrowserResult> InvokeAsync(BrowserOptions options, CancellationToken cancellationToken = default)
        {
            try
            {
                var result = await Platforms.Windows.WebAuthenticator.AuthenticateAsync(
                    new Uri(options.StartUrl),
                    new Uri(options.EndUrl));

                var authorizeResponse = ToRawIdentityUrl(options.EndUrl, result);

                return new BrowserResult
                {
                    Response = authorizeResponse
                };
            }
            catch (TaskCanceledException)
            {
                return new BrowserResult
                {
                    ResultType = BrowserResultType.UserCancel
                };
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return new BrowserResult
                {
                    ResultType = BrowserResultType.UnknownError,
                    Error = ex.ToString()
                };
            }
        }

        public string ToRawIdentityUrl(string redirectUrl, WebAuthenticatorResult result)
        {
            try
            {
                IEnumerable<string> parameters = result.Properties.Select(pair => $"{pair.Key}={pair.Value}");
                var modifiedParameters = parameters.ToList();

                var stateParameter = modifiedParameters
                    .FirstOrDefault(p => p.StartsWith("state", StringComparison.OrdinalIgnoreCase));

                if (!string.IsNullOrWhiteSpace(stateParameter))
                {
                    // Remove the state key added by WebAuthenticator that includes appInstanceId
                    modifiedParameters = modifiedParameters.Where(p => !p.StartsWith("state", StringComparison.OrdinalIgnoreCase)).ToList();

                    stateParameter = System.Web.HttpUtility.UrlDecode(stateParameter).Split('&').Last();
                    modifiedParameters.Add(stateParameter);
                }

                var values = string.Join("&", modifiedParameters);
                return $"{redirectUrl}#{values}";
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                throw;
            }
        }
    }
}
