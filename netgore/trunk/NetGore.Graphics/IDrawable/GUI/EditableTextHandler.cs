using System;
using System.Diagnostics;
using System.Linq;
using System.Security;
using Microsoft.Win32;
using SFML.Window;

namespace NetGore.Graphics.GUI
{
    /// <summary>
    /// Handles processing the keystrokes sent to a control and parsing the keystrokes to edit the
    /// text contained in the <see cref="IEditableText"/>.
    /// </summary>
    public class EditableTextHandler
    {
        /// <summary>
        /// The default blink rate of the insertion point cursor in milliseconds.
        /// </summary>
        const int _defaultCursorBlinkRate = 500;

        /// <summary>
        /// The blink rate of the insertion point cursor in milliseconds.
        /// </summary>
        public static readonly int CursorBlinkRate;

        readonly IEditableText _source;

        /// <summary>
        /// Initializes the <see cref="EditableTextHandler"/> class.
        /// </summary>
        static EditableTextHandler()
        {
            int blinkRate = _defaultCursorBlinkRate;

            try
            {
                var regSubKey = Registry.CurrentUser.OpenSubKey("Control Panel");
                if (regSubKey != null)
                {
                    var desktopKey = regSubKey.OpenSubKey("Desktop");
                    if (desktopKey != null)
                    {
                        object blinkRateObj = regSubKey.GetValue("CursorBlinkRate");
                        string blinkRateStr = blinkRateObj != null ? blinkRateObj.ToString() : null;

                        if (blinkRateStr == null || !int.TryParse(blinkRateStr, out blinkRate))
                            blinkRate = _defaultCursorBlinkRate;

                        // Keep in a reasonable range
                        if (blinkRate < 100)
                            blinkRate = 100;
                        if (blinkRate > 2000)
                            blinkRate = 2000;
                    }
                }
            }
            catch (SecurityException)
            {
                // Permission to the keys denied
                blinkRate = _defaultCursorBlinkRate;
            }
            catch (ObjectDisposedException)
            {
                // Key objects were disposed for some reason
                blinkRate = _defaultCursorBlinkRate;
            }

            CursorBlinkRate = blinkRate;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IEditableText"/> class.
        /// </summary>
        /// <param name="source">The <see cref="IEditableText"/> to control the input of.</param>
        public EditableTextHandler(IEditableText source)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            _source = source;
        }

        /// <summary>
        /// Gets the <see cref="IEditableText"/> that this <see cref="EditableTextHandler"/> handles.
        /// </summary>
        public IEditableText Source
        {
            get { return _source; }
        }

        public void HandleKey(KeyEventArgs e)
        {
            switch (e.Code)
            {
                case SFML.Window.KeyCode.Left:
                    Source.MoveCursor(MoveCursorDirection.Left);
                    break;

                case SFML.Window.KeyCode.Right:
                    Source.MoveCursor(MoveCursorDirection.Right);
                    break;

                case SFML.Window.KeyCode.Up:
                    Source.MoveCursor(MoveCursorDirection.Up);
                    break;

                case SFML.Window.KeyCode.Down:
                    Source.MoveCursor(MoveCursorDirection.Down);
                    break;

                case SFML.Window.KeyCode.Back:
                    Source.DeleteChar();
                    break;

                case SFML.Window.KeyCode.Return:
                    Source.BreakLine();
                    break;
            }
        }

        public void HandleText(TextEventArgs e)
        {
            var s = e.Unicode;
            if (string.IsNullOrEmpty(s))
                return;

            Debug.Assert(s.Length == 1);
            Source.InsertChar(s);
        }

        /// <summary>
        /// Updates the <see cref="EditableTextHandler"/>.
        /// </summary>
        /// <param name="currentTime">The current time.</param>
        /// <param name="ks">The latest keyboard state.</param>
        public void Update(int currentTime, object ks)
        {
            // TODO: ## Input
            /*
            _currentTime = currentTime;
            _ks = ks;

            // Check if our current key was raised
            if (_currentKey != null)
            {
                if (ks.IsKeyUp(_currentKey.Value))
                {
                    // Key was raised - set to null
                    ChangeCurrentKey(null);
                }
                else
                {
                    // Check to repeat the key
                    if (_currentTime >= _repeatTime)
                    {
                        _repeatTime = _currentTime + KeyboardRepeatRate;
                        HandleKey(_currentKey.Value, IsShiftDown);
                    }
                }
            }
            */
        }
    }
}