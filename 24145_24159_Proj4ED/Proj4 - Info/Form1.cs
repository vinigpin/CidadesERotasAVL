using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Proj4
{
  public partial class Form1 : Form
  {
    Arvore<Cidade>  arvore = new Arvore<Cidade>();
    public Form1()
    {
      InitializeComponent();
      this.MouseClick += Form1_MouseClick;
     }

    private void tpCadastro_Click(object sender, EventArgs e)
    {

    }

    private void label2_Click(object sender, EventArgs e)
    {

    }

    private void Form1_Load(object sender, EventArgs e)
    {

    }

    private void pnlArvore_Paint(object sender, PaintEventArgs e)
    {
      arvore.Desenhar(pnlArvore);
    }

    private void Form1_MouseClick(object sender, MouseEventArgs e)
    {
        decimal x = e.X; // Coordenada X relativa ao formulário
        decimal y = e.Y; // Coordenada Y relativa ao formulário

        // Exibe as coordenadas em uma caixa de mensagem ou em um Label
        udX.Value = x;
        udY.Value = y;
    }

        private void btnIncluirCidade_Click(object sender, EventArgs e)
        {
            textNomeCidade_Leave(sender, e);
        }

    private void textNomeCidade_Leave(object sender, EventArgs e)
        {
            if(txtNomeCidade.Text == null)
            {
                MessageBox.Show("Não foi digitado nenhum nome de cidade!");
            }
            else
            {
                MessageBox.Show(udX.Value.ToString());
                //if(arvore.Existe(txtNomeCidade.Text))
            }
        }
    }
}
