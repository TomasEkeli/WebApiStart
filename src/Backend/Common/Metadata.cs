namespace Backend.Common;

public static class Metadata
{
  public static string CurrentVersion() =>
    // use the FileVersion of the entry assembly
    ThisAssembly.AssemblyInformationalVersion;

}