using BlazorGoogleLogin.Client;
using BlazorGoogleLogin.Client.Providers;
using BlazorGoogleLogin.Client.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddHttpClient<AppAuthClient>(options =>
{
	options.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress);
});

builder.Services.AddOptions();
builder.Services.AddAuthorizationCore();

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services
	.AddOidcAuthentication(options => {
		builder.Configuration.Bind("Google", options.ProviderOptions);

		options.ProviderOptions.DefaultScopes.Add("email");
	})
	.AddAccountClaimsPrincipalFactory<RemoteAuthenticationState,
	RemoteUserAccount, CustomAccountFactory>(); 

await builder.Build().RunAsync();
