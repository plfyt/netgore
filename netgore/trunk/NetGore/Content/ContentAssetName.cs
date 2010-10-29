using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using NetGore.IO;

namespace NetGore.Content
{
    /// <summary>
    /// An immutable string that represents the name, or virtual path, to a content asset.
    /// </summary>
    [TypeConverter(typeof(ContentAssetNameConverter))]
    public class ContentAssetName : IEquatable<ContentAssetName>, IComparable<ContentAssetName>
    {
        /// <summary>
        /// The string used to separate the directories.
        /// </summary>
        public const string PathSeparator = "/";

        readonly string _assetName;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContentAssetName"/> class.
        /// </summary>
        /// <param name="assetName">Name of the asset.</param>
        public ContentAssetName(string assetName)
        {
            _assetName = Sanitize(assetName);
        }

        /// <summary>
        /// Gets the complete name of the asset. This differs from <see cref="ToString"/> as this will always
        /// be the complete name of the asset, while <see cref="ToString"/> may only be a partial path in
        /// derived classes.
        /// </summary>
        public string Value
        {
            get { return _assetName; }
        }

        /// <summary>
        /// Checks if the content with the given name exists.
        /// </summary>
        /// <returns>True if the content exists in the build; otherwise false.</returns>
        public bool ContentExists()
        {
            var filePath = ContentPaths.Build.Root.Join(GetFileName());
            return File.Exists(filePath);
        }

        /// <summary>
        /// Checks if this object is equal to another object.
        /// </summary>
        /// <param name="obj">The other object.</param>
        /// <returns>True if the two are equal; otherwise false.</returns>
        public override bool Equals(object obj)
        {
            var casted = obj as ContentAssetName;
            if (casted != null)
                return Equals(casted);

            return false;
        }

        /// <summary>
        /// Creates a <see cref="ContentAssetName"/> from an absolute file path.
        /// </summary>
        /// <param name="filePath">The absolute file path to the asset.</param>
        /// <param name="contentRoot">The root path to the content directory.</param>
        /// <returns>The <see cref="ContentAssetName"/> from the <paramref name="filePath"/>.</returns>
        public static ContentAssetName FromAbsoluteFilePath(string filePath, string contentRoot)
        {
            var start = contentRoot.Length;
            if (!contentRoot.EndsWith("/") && !contentRoot.EndsWith("\\") &&
                !contentRoot.EndsWith(Path.DirectorySeparatorChar.ToString()))
                ++start;

            var len = filePath.Length - start;
            if (ContentPaths.ContentFileSuffix.Length > 0 &&
                filePath.EndsWith(ContentPaths.ContentFileSuffix, StringComparison.OrdinalIgnoreCase))
                len -= ContentPaths.ContentFileSuffix.Length;

            var substr = filePath.Substring(start, len);
            return new ContentAssetName(substr);
        }

        /// <summary>
        /// Gets the absolute file path for the content asset.
        /// </summary>
        /// <param name="rootPath">The root content path.</param>
        /// <returns>The absolute file path for the content asset.</returns>
        /// <exception cref="ArgumentException">Either zero or more than one files matching this <see cref="ContentAssetName"/>
        /// were found in the <paramref name="rootPath"/>.</exception>
        public string GetAbsoluteFilePath(ContentPaths rootPath)
        {
            var sb = new StringBuilder();

            // Get the root path
            var rootPathStr = rootPath.Root.ToString();
            sb.Append(rootPathStr);

            // Ensure the root ends with a separator
            if (!rootPathStr.EndsWith(Path.DirectorySeparatorChar.ToString()) &&
                !rootPathStr.EndsWith(Path.AltDirectorySeparatorChar.ToString()))
                sb.Append(PathSeparator);

            // Append the name of the file
            var fileName = GetFileName();
            sb.Append(fileName);

            // If the dev path, try to find the suffix
            if (rootPath == ContentPaths.Dev)
            {
                var files = Directory.GetFiles(rootPathStr, fileName + ".*").ToImmutable();
                if (files.Count() == 0)
                {
                    throw new ArgumentException(
                        string.Format("Could not find a file named `{0}` in path `{1}` with a file suffix.", rootPathStr, fileName));
                }
                if (files.Count() > 1)
                {
                    throw new ArgumentException(
                        string.Format("Found multiple suffixes for the file named `{0}` in path `{1}`. Was expecting just one.",
                                      rootPathStr, fileName));
                }

                var fileToUse = files.First();
                var ext = Path.GetExtension(fileToUse);
                sb.Append(ext);
            }

            // Ensure we use the correct path separator
            sb.Replace(PathSeparator, Path.DirectorySeparatorChar.ToString());

            return sb.ToString();
        }

        /// <summary>
        /// Gets the relative file path and name for the content asset. This must still be prefixed by a path created
        /// with the <see cref="ContentPaths"/> to generate an absolute path.
        /// </summary>
        /// <returns>The relative file path and name for the content asset..</returns>
        public string GetFileName()
        {
            return _assetName + ContentPaths.ContentFileSuffix;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures
        /// like a hash table. </returns>
        public override int GetHashCode()
        {
            return StringComparer.OrdinalIgnoreCase.GetHashCode(Value);
        }

        /// <summary>
        /// Sanitizes the asset name. This will fix aspects of the asset name that can be fixed without
        /// making too large of assumptions.
        /// </summary>
        /// <param name="assetName">The asset name.</param>
        /// <returns>The sanitized asset name.</returns>
        public static string Sanitize(string assetName)
        {
            var suffixLen = ContentPaths.ContentFileSuffix.Length;

            // Replace \\ with the proper character
            assetName = assetName.Replace("\\", PathSeparator);

            // Remove any prefixed or suffixed path separators
            if (assetName.StartsWith(PathSeparator))
                assetName = assetName.Substring(1);

            if (assetName.EndsWith(PathSeparator))
                assetName = assetName.Substring(0, assetName.Length - 1);

            if (assetName.Length > suffixLen &&
                (ContentPaths.ContentFileSuffix.Length == 0 ||
                 assetName.EndsWith(ContentPaths.ContentFileSuffix, StringComparison.OrdinalIgnoreCase)))
                assetName = assetName.Substring(0, assetName.Length - suffixLen);

            return assetName;
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String"/> that represents this instance.</returns>
        public override string ToString()
        {
            return _assetName;
        }

        #region IComparable<ContentAssetName> Members

        /// <summary>
        /// Compares the current object with another object of the same type.
        /// </summary>
        /// <returns>
        /// A 32-bit signed integer that indicates the relative order of the objects being compared. The return value has the following meanings: Value Meaning Less than zero This object is less than the <paramref name="other"/> parameter.Zero This object is equal to <paramref name="other"/>. Greater than zero This object is greater than <paramref name="other"/>. 
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public int CompareTo(ContentAssetName other)
        {
            return StringComparer.OrdinalIgnoreCase.Compare(Value, other.Value);
        }

        #endregion

        #region IEquatable<ContentAssetName> Members

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public bool Equals(ContentAssetName other)
        {
            return Value.Equals(other.Value, StringComparison.OrdinalIgnoreCase);
        }

        #endregion

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="a">A.</param>
        /// <param name="b">The b.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(ContentAssetName a, ContentAssetName b)
        {
            if (ReferenceEquals(a, null) || ReferenceEquals(b, null))
                return ReferenceEquals(a, b);

            return a.Equals(b);
        }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="a">A.</param>
        /// <param name="b">The b.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(ContentAssetName a, ContentAssetName b)
        {
            return !(a == b);
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="System.String"/> to <see cref="ContentAssetName"/>.
        /// </summary>
        /// <param name="assetName">The asset name.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator ContentAssetName(string assetName)
        {
            return new ContentAssetName(assetName);
        }
    }
}