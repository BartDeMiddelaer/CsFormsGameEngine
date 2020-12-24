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
    public class DefaultParentForm : Form
    {
        public DefaultParentForm() => AssetLoading += GameAssets;

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
            // GameObject.LoopContainer.BackColor = Color.Black;

            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            // TopMost = true;
            // TransparencyKey = Color.Magenta;  

            // CreateButton en Controles worden hier aangemaakt
        }
        public virtual void StaticPaint(object sender, PaintEventArgs e)
        {
            // voor als je iets wilt tekenen in je form en niet in de loopContainer 
            // dit wort maar 1 keer getekent en niet geloopt als je dit wilt moet je een Refreash() oproepen
            // je LoopContainer mag dit niet overlappen dus met 
            //                                                   ||      ||
            // GameObject.LoopContainer.Bounds = new Rectangle(<HIER>, <Hier>, Width, Height); in GameAssets kan je 
            // je LoopContainer een offset geven 

        }
        public virtual void GameLoop(object sender, PaintEventArgs e)
        {
            // Hier teken je u aniematies in           
        }      
    }
}
