using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using static GameEngineForms.Services.ControlServices;



namespace GameEngineForms.Forms.GameOfLifeDemo
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


            cdgCelColor.Dispose();
            cdgColorQuadrantBorder.Dispose();
            cdgColorSubQuadrantBorder.Dispose();
            cdgColorMousQuadrantBorder.Dispose();
            cdgColorShapeQuadrantBorder.Dispose();

            if (disposing && (components != null))
            {
                components.Dispose();
            }
            
            base.Dispose(disposing);
        }     

    }
}