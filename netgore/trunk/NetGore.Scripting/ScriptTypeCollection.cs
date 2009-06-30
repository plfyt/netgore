﻿using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Microsoft.CSharp;
using Microsoft.VisualBasic;

namespace NetGore.Scripting
{
    /// <summary>
    /// Compiles a collection of scripts and presents the Types created from those scripts.
    /// </summary>
    public class ScriptTypeCollection : IEnumerable<Type>
    {
        readonly List<CompilerError> _compilerErrors = new List<CompilerError>();
        readonly string _name;
        readonly Dictionary<string, Type> _types = new Dictionary<string, Type>();
        bool _compilationFailed = false;

        /// <summary>
        /// Gets if the compilation of one or more of the scripts has failed.
        /// </summary>
        public bool CompilationFailed
        {
            get { return _compilationFailed; }
        }

        /// <summary>
        /// Gets the compiler errors and warnings generated from the compilation.
        /// </summary>
        public IEnumerable<CompilerError> CompilerErrors
        {
            get { return _compilerErrors; }
        }

        /// <summary>
        /// Gets a Type by its name.
        /// </summary>
        /// <param name="typeName">Name of the Type.</param>
        /// <returns>The Type with the specified name.</returns>
        public Type this[string typeName]
        {
            get { return _types[typeName]; }
        }

        /// <summary>
        /// Gets the name of this ScriptTypeCollection.
        /// </summary>
        public string Name
        {
            get { return _name; }
        }

        /// <summary>
        /// Gets an IEnumerable of all of the types in this ScriptCollection. This contains each Type defined in the
        /// scripts loaded into this ScriptCollection.
        /// </summary>
        public IEnumerable<Type> Types
        {
            get { return _types.Values; }
        }

        /// <summary>
        /// ScriptTypeCollection constructor.
        /// </summary>
        /// <param name="name">The name of this ScriptTypeCollection. This name should be unique from all other
        /// ScriptTypeCollections.</param>
        /// <param name="sourceFiles">IEnumerable of source code file paths.</param>
        public ScriptTypeCollection(string name, IEnumerable<string> sourceFiles)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");
            if (sourceFiles == null || sourceFiles.Count() == 0)
                throw new ArgumentNullException("sourceFiles");

            _name = name;

            var csFiles = sourceFiles.Where(x => x.EndsWith(".cs", StringComparison.OrdinalIgnoreCase));
            var vbFiles = sourceFiles.Where(x => x.EndsWith(".vb", StringComparison.OrdinalIgnoreCase));

            CompilerErrorCollection csErrors = AddScriptFiles(csFiles, ScriptLanguage.CS);
            CompilerErrorCollection vbErrors = AddScriptFiles(vbFiles, ScriptLanguage.VB);

            if (csErrors != null)
                _compilerErrors.AddRange(csErrors.Cast<CompilerError>());

            if (vbErrors != null)
                _compilerErrors.AddRange(vbErrors.Cast<CompilerError>());
        }

        /// <summary>
        /// Adds script files to this ScriptCollection.
        /// </summary>
        /// <param name="scriptFiles">An IEnumerable of paths to the script files to load into this collection.</param>
        /// <param name="language">Language of the scripts.</param>
        /// <returns>Errors and warnings generated when compiling the scripts.</returns>
        CompilerErrorCollection AddScriptFiles(IEnumerable<string> scriptFiles, ScriptLanguage language)
        {
            // Check for files
            if (scriptFiles.Count() == 0)
                return null;

            // Compile the code
            CompilerErrorCollection errors;
            Assembly asm = CompileCode(scriptFiles, language, out errors);

            if (asm == null)
                _compilationFailed = true;
            else
            {
                // Add the new types
                var newTypes = asm.GetExportedTypes();
                foreach (Type newType in newTypes)
                {
                    // TODO: Ensure each Type is defined only once
                    _types.Add(newType.Name, newType);
                }
            }

            return errors;
        }

        /// <summary>
        /// Compiles the source code files.
        /// </summary>
        /// <param name="files">Files to compile.</param>
        /// <param name="language">Language to use to compile the source code.</param>
        /// <param name="errors">Errors and warnings output from the compiler.</param>
        /// <returns>The resulting Assembly from the compiler.</returns>
        Assembly CompileCode(IEnumerable<string> files, ScriptLanguage language, out CompilerErrorCollection errors)
        {
            // Set the .NET framework version
            var providerOptions = new Dictionary<string, string> { { "CompilerVersion", "v3.5" } };

            // Get the CodeDomProvider to use
            CodeDomProvider codeDomProvider;
            switch (language)
            {
                case ScriptLanguage.CS:
                    codeDomProvider = new CSharpCodeProvider(providerOptions);
                    break;

                case ScriptLanguage.VB:
                    codeDomProvider = new VBCodeProvider(providerOptions);
                    break;

                default:
                    throw new ArgumentOutOfRangeException("language");
            }

            // Compile
            CompilerResults result;
            using (codeDomProvider)
            {
                CompilerParameters options = new CompilerParameters
                                             {
                                                 GenerateExecutable = false, GenerateInMemory = false,
                                                 OutputAssembly = Name + "." + language + ".dll"
                                             };

                Assembly currAssembly = Assembly.GetExecutingAssembly();
                options.ReferencedAssemblies.Add(currAssembly.Location);

                result = codeDomProvider.CompileAssemblyFromFile(options, files.ToArray());
            }

            errors = result.Errors;

            // Check for errors and warnings
            if (result.Errors.HasErrors)
                return null;

            return result.CompiledAssembly;
        }

        #region IEnumerable<Type> Members

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>1</filterpriority>
        public IEnumerator<Type> GetEnumerator()
        {
            return _types.Values.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}