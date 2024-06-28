using System.Text.Json;

namespace GS1CompanyPrefix;

/// <summary>
/// Helper class to parse the GS1 provided list of GCP into a <see cref="GS1CompanyPrefixProvider"/>.
/// </summary>
/// <remarks>
/// This class avoid to load the entire file into memory at once, but rather loads small chunks of data
/// and parse it while the stream is being read.
/// </remarks>
public static class GS1CompanyPrefixLoader
{
    /// <summary>
    /// Parses the JSON GCP prefixes into the specified <see cref="GS1CompanyPrefixProvider"/>.
    /// </summary>
    /// <remarks>
    /// The stream provided must contain a JSON value of the GCP prefix format list 
    /// (ex: https://www.gs1.org/sites/default/files/docs/gcp_length/gcpprefixformatlist.json)
    /// </remarks>
    /// <param name="provider">The <see cref="GS1CompanyPrefixProvider"/> to be filled with the GCP values</param>
    /// <param name="stream">The open stream of the JSON file</param>
    public static void LoadFromJsonStream(GS1CompanyPrefixProvider provider, Stream stream)
    {
        LoadFromJsonStream(provider, stream, stream.Length);
    }

    public static void LoadFromJsonStream(GS1CompanyPrefixProvider provider, Stream stream, long remainingLength)
    {
        var buffer = new byte[256];
        var reader = default(Utf8JsonReader);
        var status = default(int);
        var prefix = string.Empty;
        var length = -1;

        // Iterate while there is still data in the stream
        while (stream.CanRead && remainingLength > 0)
        {
            // Read next chunk of data from the stream keeping the current reader state
            remainingLength -= Read(buffer, stream, ref reader);

            while (reader.Read())
            {
                switch (reader.TokenType)
                {
                    case JsonTokenType.PropertyName when TryParseGcpState(reader.GetString(), out int state):
                        status |= state;
                        break;

                    // If the Prefix flag is set the latest token read must be a string (prefix of the next GCP)
                    case JsonTokenType.String when HasFlag(status, GcpState.Prefix):
                        prefix = reader.GetString()!;
                        status ^= GcpState.Prefix;
                        break;

                    // If the GcpLength flag is set the next token must be the GCP prefix length (int)
                    case JsonTokenType.Number when HasFlag(status, GcpState.GcpLength):
                        length = reader.GetInt32();
                        status ^= GcpState.GcpLength;
                        break;

                    // A close object in the Entry context (array) indicates that both the prefix and length of the 
                    // GCP were parsed. We can then safely load it into the Provider.
                    case JsonTokenType.EndObject when HasFlag(status, GcpState.Entry):
                        provider.SetPrefix(prefix, length);
                        prefix = string.Empty;
                        length = -1;
                        break;

                    // End of array while in the "entry" property means we reach the end of the GCP length list.
                    // We can exit from the function and discard the rest of the stream
                    case JsonTokenType.EndArray when HasFlag(status, GcpState.Entry):
                        return;
                }
            }
        }
    }

    public static bool HasFlag(int status, int flag) => (status & flag) != 0x00;

    public static bool TryParseGcpState(string value, out int state)
    {
        state = value.ToLower() switch
        {
            "entry" => GcpState.Entry,
            "prefix" => GcpState.Prefix,
            "gcplength" => GcpState.GcpLength,
            _ => 0x00,
        };

        return state != 0x00;
    }

    public static class GcpState
    {
        public const int Entry = 0x01;
        public const int Prefix = 0x02;
        public const int GcpLength = 0x04;
    }

    /// <summary>
    /// Reads the next chunk of bytes from the provided stream. Keeps the CurrentState from the provided reader
    /// </summary>
    /// <param name="buffer">The buffer to hold the read data</param>
    /// <param name="stream">The stream to read from</param>
    /// <param name="reader">The <see cref="Utf8JsonReader"/> to fill with the data</param>
    /// <returns>The number of bytes read from the stream</returns>
    private static int Read(byte[] buffer, Stream stream, ref Utf8JsonReader reader)
    {
        var startFrom = 0;
        var chunkSize = 0;
        int bytesRead;

        // If not all the data were read from the current reader, keep the remaining bytes
        // into the new byte buffer
        if (reader.TokenType != JsonTokenType.None && reader.BytesConsumed < buffer.Length)
        {
            var remaining = buffer[(int)reader.BytesConsumed..];

            remaining.CopyTo(buffer, 0);
            startFrom = remaining.Length;
        }

        // Try to read from the stream the remaining bytes in the buffer
        do
        {
            bytesRead = stream.Read(buffer, startFrom, buffer.Length - startFrom);
            chunkSize += bytesRead;
            startFrom += bytesRead;
        }
        while (bytesRead > 0 && startFrom < buffer.Length);

        // Return a new JSON reader with the new buffer with the same CurrentState
        reader = new Utf8JsonReader(buffer, bytesRead == 0, reader.CurrentState);

        return chunkSize;
    }
}