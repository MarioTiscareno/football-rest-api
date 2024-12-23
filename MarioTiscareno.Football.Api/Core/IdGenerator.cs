using System.Security.Cryptography;
using System.Text;

namespace MarioTiscareno.Football.Api.Core;

/// <summary>
/// Helper class that will create deterministic ids based on player names,
/// this just helps to avoid duplicating players during DB seeding.
/// IMPORTANT:Should never do this in production systems, this is just for the example.
/// </summary>
public sealed class IdGenerator : IDisposable
{
    // MD5 is not a secure hash function, but is just fine for this example
    private readonly MD5 md5 = MD5.Create();

    public void Dispose()
    {
        md5.Dispose();
    }

    /// <summary>
    /// Generates a deterministic id from an input string
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public string Generate(string name)
    {
        // Convert the input string to a byte array and compute the hash.
        var data = md5.ComputeHash(Encoding.UTF8.GetBytes(name));

        var sBuilder = new StringBuilder();

        // Loop through each byte of the hashed data
        // and format each one as a hexadecimal string.
        for (var i = 0; i < data.Length; i++)
        {
            sBuilder.Append(data[i].ToString("x2"));
        }

        // Return the hexadecimal string.
        return sBuilder.ToString();
    }
}
