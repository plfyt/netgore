using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using log4net;
using NetGore;
using NetGore.IO;
using NetGore.Scripting;

namespace DemoGame
{
    /// <summary>
    /// Class containing all of the messages for the correspoding GameMessage.
    /// </summary>
    public class GameMessageCollection : MessageCollectionBase<GameMessage>
    {
        static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Name of the default language to use.
        /// </summary>
        const string _defaultLanguageName = "English";

        /// <summary>
        /// Suffix for the language files.
        /// </summary>
        const string _languageFileSuffix = ".txt";

        const string _tempLanguageName = "TEMPORARY_COMPILATION_TEST_LANGUAGE";

        /// <summary>
        /// The <see cref="GameMessageCollection"/> instance for the default language.
        /// </summary>
        static readonly GameMessageCollection _defaultMessages;

        /// <summary>
        /// Contains the instances of the <see cref="GameMessageCollection"/> indexed by language.
        /// </summary>
        static readonly Dictionary<string, GameMessageCollection> _instances;

        readonly string _language;
        readonly bool _rawMessagesOnly;

        /// <summary>
        /// Initializes the <see cref="GameMessageCollection"/> class.
        /// </summary>
        static GameMessageCollection()
        {
            _instances = new Dictionary<string, GameMessageCollection>(StringComparer.OrdinalIgnoreCase);
            _defaultMessages = Create();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GameMessageCollection"/> class.
        /// </summary>
        /// <param name="language">Name of the language to load.</param>
        /// <param name="rawMessagesOnly">If true, only the raw messages will be loaded, and invoking the messages
        /// will not be supported.</param>
        GameMessageCollection(string language, bool rawMessagesOnly) : base(GetLanguageFile(language), _defaultMessages)
        {
            _rawMessagesOnly = rawMessagesOnly;
            _language = language;

            // Ensure we have all the messages loaded
            var missingKeys = EnumHelper<GameMessage>.Values.Except(this.Select(y => y.Key));
            if (missingKeys.Count() > 0)
            {
                const string errmsg = "GameMessages `{0}` for language `{1}` did not contain all GameMessages. Missing keys: {2}";
                string err = string.Format(errmsg, this, _language, missingKeys.Implode());
                if (log.IsErrorEnabled)
                    log.Error(err);
            }
        }

        /// <summary>
        /// Gets the file path to the global JScript file included in all generated <see cref="GameMessageCollection"/>s.
        /// </summary>
        public static string GlobalJScriptFilePath
        {
            get { return ContentPaths.Build.Languages.Join("global.js"); }
        }

        /// <summary>
        /// Gets the name of this language.
        /// </summary>
        public string Language
        {
            get { return _language; }
        }

        /// <summary>
        /// Compiles the assembly.
        /// </summary>
        /// <param name="assemblyCreator">The assembly creator to compile.</param>
        /// <returns>
        /// The <see cref="AssemblyClassInvoker"/> for the compiled assembly.
        /// </returns>
        protected override AssemblyClassInvoker CompileAssembly(JScriptAssemblyCreator assemblyCreator)
        {
            if (_rawMessagesOnly)
                return null;

            return base.CompileAssembly(assemblyCreator);
        }

        /// <summary>
        /// Gets the <see cref="GameMessageCollection"/> for the default language.
        /// </summary>
        /// <returns>The <see cref="GameMessageCollection"/> for the default language.</returns>
        public static GameMessageCollection Create()
        {
            return Create(_defaultLanguageName);
        }

        /// <summary>
        /// Gets the <see cref="GameMessageCollection"/> for the specified language.
        /// </summary>
        /// <param name="language">The game message language.</param>
        /// <returns>The <see cref="GameMessageCollection"/> for the specified language.</returns>
        public static GameMessageCollection Create(string language)
        {
            GameMessageCollection instance;
            if (!_instances.TryGetValue(language, out instance))
            {
                instance = new GameMessageCollection(language, false);
                _instances.Add(language, instance);
            }

            return instance;
        }

        /// <summary>
        /// Deletes the files for a language.
        /// </summary>
        /// <param name="language">The language to delete the files for.</param>
        public static void DeleteLanguageFiles(string language)
        {
            var file = GetLanguageFile(language);
            var jsFile = GetLanguageJScriptFile(file);

            // Delete the anguage messages file
            try
            {
                if (File.Exists(file))
                    File.Delete(file);
            }
            catch (IOException ex)
            {
                const string errmsg = "Failed to delete language file `{0}`. Exception: {1}";
                if (log.IsErrorEnabled)
                    log.ErrorFormat(errmsg, file, ex);
            }

            // Delete the JScript file
            try
            {
                if (File.Exists(jsFile))
                    File.Delete(jsFile);
            }
            catch (IOException ex)
            {
                const string errmsg = "Failed to delete language file `{0}`. Exception: {1}";
                if (log.IsErrorEnabled)
                    log.ErrorFormat(errmsg, file, ex);
            }
        }

        /// <summary>
        /// Gets the IEqualityComparer to use for collections created by this collection.
        /// </summary>
        /// <returns>
        /// The IEqualityComparer to use for collections created by this collection.
        /// </returns>
        protected override IEqualityComparer<GameMessage> GetEqualityComparer()
        {
            return EnumComparer<GameMessage>.Instance;
        }

        /// <summary>
        /// Gets the <see cref="GameMessageCollection"/> file for a certain language.
        /// </summary>
        /// <param name="language">The language to get the file for.</param>
        /// <returns>The <see cref="GameMessageCollection"/> file for the <paramref name="language"/>.</returns>
        public static string GetLanguageFile(string language)
        {
            return ContentPaths.Build.Languages.Join(language.ToLower() + _languageFileSuffix);
        }

        /// <summary>
        /// Gets the <see cref="GameMessageCollection"/> JScript file for a certain language.
        /// </summary>
        /// <param name="languageFilePath">The path to the language file to get the additional JScript file for.</param>
        /// <returns>The path to the JScript file.</returns>
        public static string GetLanguageJScriptFile(string languageFilePath)
        {
            return languageFilePath + ".js";
        }

        /// <summary>
        /// Gets the names of all of the available languages.
        /// </summary>
        /// <returns>The names of all of the available languages.</returns>
        public static IEnumerable<string> GetLanguages()
        {
            var comp = StringComparer.OrdinalIgnoreCase;

            var dir = ContentPaths.Build.Languages;
            var filePaths = Directory.GetFiles(dir, "*.txt", SearchOption.TopDirectoryOnly);

            var files = filePaths.Select(x => Path.GetFileNameWithoutExtension(x));
            files = files.Distinct(comp);
            files = files.OrderBy(x => x, comp);

            return files.ToImmutable();
        }

        /// <summary>
        /// When overridden in the derived class, allows for additional code to be added to the generated JScript.
        /// </summary>
        /// <param name="file">The file that is being loaded.</param>
        /// <param name="assemblyCreator">The assembly creator.</param>
        protected override void LoadAdditionalJScriptMembers(string file, JScriptAssemblyCreator assemblyCreator)
        {
            // global.js
            var globalFile = GlobalJScriptFilePath;
            if (File.Exists(globalFile))
                assemblyCreator.AddRawMember(File.ReadAllText(globalFile));

            // language.js
            var languageFile = GetLanguageJScriptFile(file);
            if (File.Exists(languageFile))
                assemblyCreator.AddRawMember(File.ReadAllText(languageFile));

            base.LoadAdditionalJScriptMembers(file, assemblyCreator);
        }

        /// <summary>
        /// Loads the raw <see cref="GameMessage"/>s for a language.
        /// </summary>
        /// <param name="language">The language to load the messages for.</param>
        /// <returns>The raw <see cref="GameMessage"/>s for the <paramref name="language"/>.</returns>
        public static IEnumerable<KeyValuePair<GameMessage, string>> LoadRawMessages(string language)
        {
            var coll = new GameMessageCollection(language, true);
            var messages = coll.ToImmutable();
            return messages;
        }

        /// <summary>
        /// Writes the raw <see cref="GameMessage"/>s to file.
        /// </summary>
        /// <param name="language">The language.</param>
        /// <param name="messages">The messages.</param>
        public static void SaveRawMessages(string language, IEnumerable<KeyValuePair<GameMessage, string>> messages)
        {
            StringBuilder sb = new StringBuilder(2048);
            foreach (var msg in messages)
            {
                sb.AppendLine(msg.Key + ": " + msg.Value);
            }

            File.WriteAllText(GetLanguageFile(language), sb.ToString());
        }

        /// <summary>
        /// Tests if the <see cref="GameMessageCollection"/> for a certain language exists and can compile
        /// successfully without error.
        /// </summary>
        /// <param name="language">The language to try to compile.</param>
        /// <param name="errors">When this method returns false, contains the compilation errors.</param>
        /// <returns>
        /// True if the <paramref name="language"/>'s <see cref="GameMessageCollection"/> compiled
        /// successfully; otherwise false.
        /// </returns>
        public static bool TestCompilation(string language, out IEnumerable<CompilerError> errors)
        {
            var coll = new GameMessageCollection(language, true);
            errors = coll.CompilationErrors;
            return coll.CompilationErrors.IsEmpty();
        }

        /// <summary>
        /// Tests if the <see cref="GameMessageCollection"/> for a certain language exists and can compile
        /// successfully without error.
        /// </summary>
        /// <param name="language">The language to try to compile.</param>
        /// <param name="errors">When this method returns false, contains the compilation errors as a string.</param>
        /// <returns>
        /// True if the <paramref name="language"/>'s <see cref="GameMessageCollection"/> compiled
        /// successfully; otherwise false.
        /// </returns>
        public static bool TestCompilation(string language, out string errors)
        {
            errors = string.Empty;

            IEnumerable<CompilerError> cerrors;
            var ret = TestCompilation(language, out cerrors);

            if (!ret)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("The following errors have caused the compilation to fail:");
                foreach (var e in cerrors)
                {
                    sb.AppendLine(e.ErrorNumber + ": " + e.ErrorText);
                }
                errors = sb.ToString();
            }

            return ret;
        }

        /// <summary>
        /// Tests if the <see cref="GameMessageCollection"/> for a certain language exists and can compile
        /// successfully without error.
        /// </summary>
        /// <param name="messages">The messages to try to compile.</param>
        /// <param name="errors">When this method returns false, contains the compilation errors as a string.</param>
        /// <returns>
        /// True if the <paramref name="messages"/>s compiled successfully; otherwise false.
        /// </returns>
        public static bool TestCompilation(IEnumerable<KeyValuePair<GameMessage, string>> messages, out string errors)
        {
            DeleteLanguageFiles(_tempLanguageName);

            bool success;
            try
            {
                SaveRawMessages(_tempLanguageName, messages);

                success = TestCompilation(_tempLanguageName, out errors);
            }
            finally
            {
                DeleteLanguageFiles(_tempLanguageName);
            }

            return success;
        }

        /// <summary>
        /// Tests if the <see cref="GameMessageCollection"/> for a certain language exists and can compile
        /// successfully without error.
        /// </summary>
        /// <param name="messages">The messages to try to compile.</param>
        /// <param name="errors">When this method returns false, contains the compilation errors.</param>
        /// <returns>
        /// True if the <paramref name="messages"/>s compiled successfully; otherwise false.
        /// </returns>
        public static bool TestCompilation(IEnumerable<KeyValuePair<GameMessage, string>> messages,
                                           out IEnumerable<CompilerError> errors)
        {
            bool success;
            try
            {
                SaveRawMessages(_tempLanguageName, messages);

                success = TestCompilation(_tempLanguageName, out errors);
            }
            finally
            {
                var langFile = GetLanguageFile(_tempLanguageName);
                if (File.Exists(langFile))
                    File.Delete(langFile);

                var jsFile = GetLanguageJScriptFile(langFile);
                if (File.Exists(jsFile))
                    File.Delete(jsFile);
            }

            return success;
        }

        /// <summary>
        /// When overridden in the derived class, tries to parse a string to get the ID.
        /// </summary>
        /// <param name="str">String to parse.</param>
        /// <param name="id">Parsed ID.</param>
        /// <returns>True if the ID was parsed successfully, else false.</returns>
        protected override bool TryParseID(string str, out GameMessage id)
        {
            return ParseEnumHelper(str, out id);
        }
    }
}