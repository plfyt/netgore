using System;
using System.Linq;

namespace NetGore
{
    /// <summary>
    /// Attribute for a Say command.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class SayCommandAttribute : StringCommandBaseAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SayCommandAttribute"/> class.
        /// </summary>
        /// <param name="command">The name of the command.</param>
        public SayCommandAttribute(string command) : base(command)
        {
        }
    }
}