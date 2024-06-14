﻿using System.Text.Json;

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
        var buffer = new byte[512];
        var reader = default(Utf8JsonReader);
        var status = default(GcpState);
        var prefix = string.Empty;
        var length = -1;

        // Iterate while the stream is not read until the end
        while (stream.CanRead && stream.Position < stream.Length)
        {
            // Read next chunk of data from the stream keeping the current reader state
            Read(buffer, stream, ref reader);

            while (reader.Read())
            {
                // If the Prefix flag is set the latest token read must be a string (prefix of the next GCP)
                if (status.HasFlag(GcpState.Prefix))
                {
                    prefix = reader.GetString()!;
                    status ^= GcpState.Prefix;
                }
                // If the GcpLength flag is set the next token must be the GCP prefix length (int)
                else if (status.HasFlag(GcpState.GcpLength))
                {
                    length = reader.GetInt32();
                    status ^= GcpState.GcpLength;
                }
                // A close object in the Entry context (array) indicates that both the prefix and length of the 
                // GCP were parsed. We can then safely load it into the Provider.
                else if (reader.TokenType == JsonTokenType.EndObject && status.HasFlag(GcpState.Entry))
                {
                    provider.SetPrefix(prefix, length);
                    prefix = string.Empty;
                    length = -1;
                }
                else if (reader.TokenType == JsonTokenType.PropertyName && Enum.TryParse<GcpState>(reader.GetString(), true, out var state))
                {
                    status |= state;
                }
                // End of array while in the "entry" property means we reach the end of the GCP length list.
                // We can exit from the function and discard the rest of the stream
                else if (reader.TokenType == JsonTokenType.EndArray && status.HasFlag(GcpState.Entry))
                {
                    return;
                }
            }
        }
    }

    /// <summary>
    /// Enum to keep track of the <see cref="Utf8JsonReader"/> current state in the GCP document
    /// </summary>
    [Flags]
    private enum GcpState
    {
        // Default state
        None = 0,
        // Indicates the JSON reader reached the start of the Entry array
        Entry = 1,
        // The Prefix property was read
        Prefix = 2,
        // GcpLength property was read
        GcpLength = 4
    }

    /// <summary>
    /// Reads the next chunk of bytes from the provided stream. Keeps the CurrentState from the provided reader
    /// </summary>
    /// <param name="buffer">The buffer to hold the read data</param>
    /// <param name="stream">The stream to read from</param>
    /// <param name="reader">The <see cref="Utf8JsonReader"/> to fill with the data</param>
    private static void Read(byte[] buffer, Stream stream, ref Utf8JsonReader reader)
    {
        var startFrom = 0;

        // If not all the data were read from the current reader, keep the remaining bytes
        // into the new byte buffer
        if (reader.TokenType != JsonTokenType.None && reader.BytesConsumed < buffer.Length)
        {
            var remaining = buffer[(int)reader.BytesConsumed..];

            remaining.CopyTo(buffer, 0);
            startFrom = remaining.Length;
        }

        // Try to read from the stream the remaining bytes in the buffer
        var bytesRead = stream.Read(buffer, startFrom, buffer.Length - startFrom);
        var isFinalBlock = bytesRead + startFrom < buffer.Length;

        // Return a new JSON reader with the new buffer with the same CurrentState
        reader = new Utf8JsonReader(buffer, isFinalBlock, reader.CurrentState);
    }
}