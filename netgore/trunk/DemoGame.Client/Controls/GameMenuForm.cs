﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NetGore.Graphics.GUI;
using SFML.Graphics;

namespace DemoGame.Client
{
    /// <summary>
    /// The main game form for while in the game.
    /// </summary>
    class GameMenuForm : Form
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GameMenuForm"/> class.
        /// </summary>
        /// <param name="parent">The parent.</param>
        public GameMenuForm(Control parent) : base(parent, Vector2.Zero, new Vector2(32))
        {
            var quitLbl = new Label(this, new Vector2(3, 3)) { Text = "Quit" };
            quitLbl.Clicked += quitLbl_Clicked;

            // Center on the parent
            Position = (Parent.ClientSize / 2f) - (Size / 2f);

            IsVisible = false;

            parent.KeyPressed += parent_KeyPressed;
        }

        /// <summary>
        /// Handles the KeyPressed event of the parent control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="SFML.Window.KeyEventArgs"/> instance containing the event data.</param>
        void parent_KeyPressed(object sender, SFML.Window.KeyEventArgs e)
        {
            if (e.Code == SFML.Window.KeyCode.Escape)
                IsVisible = !IsVisible;
        }

        /// <summary>
        /// Notifies listeners that the Quit button has been clicked
        /// </summary>
        public event EventHandler ClickedQuit;

        /// <summary>
        /// Sets the default values for the <see cref="Control"/>. This should always begin with a call to the
        /// base class's method to ensure that changes to settings are hierchical.
        /// </summary>
        protected override void SetDefaultValues()
        {
            base.SetDefaultValues();

            ResizeToChildren = true;
            Text = "Menu";
        }

        /// <summary>
        /// Handles the Clicked event of the quitLbl control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="SFML.Window.MouseButtonEventArgs"/> instance containing the event data.</param>
        void quitLbl_Clicked(object sender, SFML.Window.MouseButtonEventArgs e)
        {
            if (e.Button != SFML.Window.MouseButton.Left)
                return;

            if (ClickedQuit != null)
                ClickedQuit(this, EventArgs.Empty);
        }
    }
}
