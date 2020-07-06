using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using static GameEngineForms.Services.EventServices;
using static GameEngineForms.Resources.DynamicResources;


namespace GameEngineForms.Forms
{
    partial class GameOfLife
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            KillingTime.Dispose();
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            
            base.Dispose(disposing);
        }     

    }
}