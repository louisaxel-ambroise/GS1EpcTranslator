namespace GS1CompanyPrefix;

/// <summary>
/// GS1 GCP length store/provider.
/// </summary>
/// <remarks>
/// This class uses a trie structure to store the company prefixes instead of a dictionary as recommended by GS1.
/// It allows to have a more memory and time performant searching at the cost of a slightly slower storage.
/// For example the entire GS1 GCP list as of 2024 only takes 2Mb in memory, and the search for a specific prefix
/// always takes at most 12 property access operations, and to exit as early as possible in case of no match.
/// </remarks>
public sealed class GS1CompanyPrefixProvider
{
    /// <summary>
    /// The root of the company prefix tree. It can be seen as the prefix for an empty string "".
    /// It is initialized as a node as at least one prefix should be loaded for the provider to be useful.
    /// </summary>
    private readonly Node _root = Node.Default;

    /// <summary>
    /// Gets the GS1 Global Company Prefix length for the specified value, or -1 is the value doesn't match any prefix.
    /// </summary>
    /// <remarks>
    /// This method will exit early if no GCP that matches the input was stored. It guarantees that the value will
    /// be retrieved in maximum 12 operations (max. GCP length from GS1)
    /// </remarks>
    /// <param name="value">The value to extract the GCP length from</param>
    /// <returns>The GCP length or -1 if it doesn't exist</returns>
    public int GetCompanyPrefixLength(string value)
    {
        var current = _root;

        // Loop through the trie values until we reach a Leaf node or we went through 12 chars already.
        // This last condition is only a safety measure as we must have exited the trie before that.
        for (var i = 0; i < 12 && !current.IsLeaf; i++)
        {
            current = current[value[i]];
        }

        // At that point the current.Length will contain the GCP length or -1 if the current node is not a Leaf
        // It is then safe to return without having to check the type of the current Node value
        return current.Length;
    }

    /// <summary>
    /// Updates the current GCP tree values with the provided prefix/length tuple.
    /// </summary>
    /// <param name="prefix">The prefix to be stored</param>
    /// <param name="length">The length of the GCP</param>
    public void SetPrefix(string prefix, int length)
    {
        var current = _root;

        // This loop makes sure that all the nodes in the path until the last character
        // of the prefix does not contain any Leaf node.
        for (var i = 0; i < prefix.Length - 1; i++)
        {
            // If the next value is a leaf convert it to a default Node.
            if (current[prefix[i]].IsLeaf)
            {
                current[prefix[i]] = Node.Default;
            }

            // Set the current node to the next one in the path
            current = current[prefix[i]];
        }

        // At that point i contains the latest value of the GCP, so we set the child
        // of the current node to a Leaf node with the length value.
        current[prefix[^1]] = length;
    }

    /// <summary>
    /// A Node holds a specific value for a given character in a GCP path.
    /// It can be an intermediate node that contains an array of 10 child 
    /// Nodes for each character between '0' and '9'.
    /// or a leaf that contains the GCP length for the current path.
    /// </summary>
    private sealed record Node
    {
        private readonly Node[] _children;
        public int Length { get; private set; } = -1;

        private Node(Node[] children) => _children = children;
        private Node(int length) => Length = length;

        /// <summary>
        /// Determines if a node is a leaf (end of GCP prefix) or if it contains children
        /// </summary>
        public bool IsLeaf => _children is null;

        /// <summary>
        /// Gets or sets the children for the specified character (between '0' and '9')
        /// </summary>
        /// <param name="index">The character to get or set the value</param>
        /// <returns>The children for the specified character</returns>
        public Node this[char index]
        {
            get
            {
                EnsureInRange(index);

                return _children?[index - '0'] ?? _values[0];
            }
            set
            {
                EnsureInRange(index);

                _children![index - '0'] = value;
            }
        }

        /// <summary>
        /// Returns a new Node with an empty array of children
        /// </summary>
        public static Node Default => new(new Node[10]);

        /// <summary>
        /// Returns the leaf node matching the specified GCP length value
        /// </summary>
        /// <param name="value">The GCP length value</param>
        /// <returns>The leaf Node</returns>
        public static implicit operator Node(int value) => _values[value + 1];

        private static readonly Node[] _values = Enumerable.Range(-1, 14).Select(x => new Node(x)).ToArray();

        /// <summary>
        /// Verifies that the provided value is between the min and max values.
        /// </summary>
        /// <param name="value">The provided value to verify</param>
        /// <param name="lowest">The invlusive min value allowed</param>
        /// <param name="highest">The inclusive max value allowed</param>
        private static void EnsureInRange(char value)
        {
            if (value < '0' || value > '9')
            {
                throw new ArgumentOutOfRangeException(nameof(value));
            }
        }
    }
}
