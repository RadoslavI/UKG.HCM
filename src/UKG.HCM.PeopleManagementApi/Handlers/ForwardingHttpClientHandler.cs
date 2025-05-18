namespace UKG.HCM.PeopleManagementApi.Handlers
{
    public class ForwardingHttpClientHandler : DelegatingHandler
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ForwardingHttpClientHandler(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var token = _httpContextAccessor.HttpContext?.Request.Headers.Authorization.FirstOrDefault();

            if (!string.IsNullOrEmpty(token))
            {
                request.Headers.Add("Authorization", token);
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}