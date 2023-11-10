using System.Diagnostics;

namespace Backend.Tests;

public class Version_endpoint : TestWithBackend
{
    [Fact]
    public async Task Returns_version()
    {
        var response = await _api.GetAsync("/version");

        var content = await response.Content.ReadAsStringAsync();

        // the full version is compiled into the assembly as Product Version
        var fileInfo = new FileInfo(typeof(Program).Assembly.Location);
        var versionInfo = FileVersionInfo.GetVersionInfo(fileInfo.FullName);
        var expectedVersion = versionInfo.ProductVersion;

        content.ShouldContain(expectedVersion!);
    }
}
