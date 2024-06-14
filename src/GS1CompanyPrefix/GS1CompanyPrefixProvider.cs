namespace GS1CompanyPrefix;

/// <summary>
/// GS1 GCP length store/provider.
/// </summary>
/// <remarks>
/// This class uses a tree structure to store the company prefixes instead of a dictionary as recommended by GS1.
/// It allows to have a more memory and time performant searching at the cost of a slightly slower storage.
/// For example the entire GS1 GCP list as of 2024 only takes 2Mb in memory, and the search for a specific prefix
/// always takes less than 12 property access operations.
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

        // Loop through the current value until we reach a Leaf node or we exceed 12 char deep.
        // This last condition is only a safety measure as we must have exited the tree before that.
        for (var i = 0; i < 12 && !current.IsLeaf; i++)
        {
            var charIndex = value[i] - '0';
            current = current[charIndex];
        }

        // At that point the current.Value will contain the GCP length or return -1 if the current node is not a Leaf
        // It is then safe to return without having to check if the Node is a Leaf or not
        return current.Value;
    }

    /// <summary>
    /// Updates the current GCP tree values with the provided prefix/length tuple.
    /// </summary>
    /// <param name="prefix">The prefix to be stored</param>
    /// <param name="length">The length of the GCP</param>
    public void SetPrefix(string prefix, int length)
    {
        var current = _root;
        int i;

        // This loop makes sure that all the nodes in the path until the last character
        // of the prefix does not contain any Leaf node.
        for (i = 0; i < prefix.Length - 1; i++)
        {
            var charIndex = prefix[i] - '0';

            // If the next value is a leaf convert it to a default Node.
            if (current[charIndex].IsLeaf)
            {
                current[charIndex] = Node.Default;
            }

            // Move to the child node
            current = current[charIndex];
        }

        // At that point i contains the latest value of the GCP, so we set the child
        // of the current node to a Leaf node with the length value.
        current[prefix[i] - '0'] = Node.Leaf(length);
    }

    /// <summary>
    /// A Node holds a specific value for a goven character in a GCP path.
    /// It can be an intermediate node that contains an array of 10 child 
    /// Nodes for each character between '0' and '9'.
    /// or a leaf that contains the GCP length for the current path.
    /// </summary>
    private sealed class Node
    {
        private readonly Node[] _children;
        private readonly int _length = -1;

        private Node(Node[] children) => _children = children;
        private Node(int length) => _length = length;

        /// <summary>
        /// Determines if a node is a leaf (end of GCP prefix) or if it contains children
        /// </summary>
        public bool IsLeaf => _children is null;

        /// <summary>
        /// Returns the GCP length of the current path, or -1 if it is not a leaf
        /// </summary>
        public int Value => _length;

        /// <summary>
        /// Gets or sets the children for the specified numerical character (between '0' and '9')
        /// </summary>
        /// <param name="index">The character to get or set the value</param>
        /// <returns>The children for the specified character</returns>
        public Node this[int index]
        {
            get
            {
                EnsureInRange(index, 0, 9);

                return _children?[index] ?? _values[0];
            }
            set
            {
                EnsureInRange(index, 0, 9);

                _children![index] = value;
            }
        }

        /// <summary>
        /// Returns a new Node with an empty array of children
        /// </summary>
        public static Node Default => new(new Node[10]);

        /// <summary>
        /// Returns a leaf node with the specified GCP length value
        /// </summary>
        /// <param name="value">The GCP length value</param>
        /// <returns>The leaf Node</returns>
        public static Node Leaf(int value) => _values[value + 1];

        private static readonly Node[] _values = Enumerable.Range(-1, 14).Select(x => new Node(x)).ToArray();

        /// <summary>
        /// Verifies that the provided value is between the min and max values.
        /// </summary>
        /// <param name="value">The provided value to verify</param>
        /// <param name="min">The invlusive min value allowed</param>
        /// <param name="max">The inclusive max value allowed</param>
        private static void EnsureInRange(int value, int min, int max)
        {
            if (value < min || value > max)
            {
                throw new ArgumentOutOfRangeException(nameof(value));
            }
        }
    }
}
