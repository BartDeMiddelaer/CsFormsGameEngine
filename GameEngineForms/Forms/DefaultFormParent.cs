using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using static GameEngineForms.Services.ControlServices;
using static GameEngineForms.Services.GameEngineServices;
using static GameEngineForms.Resources.GameEngineObjects;
using System.Drawing;
using System.Drawing.Drawing2D;
using GameEngineForms.Resources;

namespace GameEngineForms.Forms
{
    public class DefaultFormParent : Form
    {
        public DefaultFormParent() => AssetLoading += GameAssets;

        public virtual void GameAssets()
        {
            // Render Modus
            GameObject.RenderMode = SmoothingMode.HighSpeed;
            Controls.Add(GameObject.LoopContainer);
            Paint += StaticPaint;
            GameCycle += GameLoop;

            // De groot van je window en de achtergrond color Dit wort in je StaticPaint gebruikt
            // je LoopContainer mag niet overlappen of je ziet je Btns niet en dergelijke
            // Height = 600;  
            // Width = 800;
            BackColor = Color.Linen;

            // De groote van je loop Container -> om in te animeeren dit is waar je op tekent in je game loop
            GameObject.LoopContainer.Bounds = new Rectangle(0, 0, Width, Height);
            //GameObject.LoopContainer.BackColor = Color.Black;

            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            //TopMost = true;
            //TransparencyKey = Color.Magenta;  
        }
        public virtual void StaticPaint(object sender, PaintEventArgs e)
        {
            // hier gebruik je je CreateButton en dergelijke. hier maak je je controls aan
            // BV: Controls.Add(new Button());
            // je LoopContainer mag niet overlappen of je ziet je Btns niet en dergelijke
        }
        public virtual void GameLoop(object sender, PaintEventArgs e)
        {
            // Hier teken je u aniematies in           
        }      
    }
}
