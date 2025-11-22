using AgendaAlfabetica;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace Proj4
{
      public partial class Form1 : Form
      {
        Arvore<Cidade> arvore = new Arvore<Cidade>();
        public Form1()
        {
          InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            arvore.LerArquivoDeRegistros("../../Dados/cidades.dat");
            LerArquivoDeLigacoes(arvore, "../../Dados/GrafoOnibusSaoPaulo.txt");
        }

        private void tpCadastro_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void pnlArvore_Paint(object sender, PaintEventArgs e)
        {
          arvore.Desenhar(pnlArvore);
        }

        private void LerArquivoDeLigacoes(Arvore<Cidade> arvore, string nomeArquivo)
        {
            try
            {
                StreamReader leitor = new StreamReader(nomeArquivo);
                string linha = leitor.ReadLine();
                while (linha != null)
                {
                    
                    string[] dados = linha.Split(';');

                    string stringCidade1 = RemoverAcentos(dados[0]);
                    Cidade cidade1 = new Cidade(stringCidade1);
                    bool existeCidade1 = arvore.Existe(cidade1);
                    NoArvore<Cidade> noCidade1 = arvore.Atual;

                    string stringCidade2 = RemoverAcentos(dados[1]);
                    Cidade cidade2 = new Cidade(RemoverAcentos(dados[1]));
                    bool existeCidade2 = arvore.Existe(cidade2);
                    NoArvore<Cidade> noCidade2 = arvore.Atual;

                    if (existeCidade1 && existeCidade2)
                    {
                        Ligacao ligacao1 = new Ligacao(stringCidade1, stringCidade2, int.Parse(dados[2]));
                        Ligacao ligacao2 = new Ligacao(stringCidade2, stringCidade1, int.Parse(dados[2]));
                        noCidade1.Info.ligacoes.InserirAposFim(ligacao1);
                        noCidade2.Info.ligacoes.InserirAposFim(ligacao2);
                    }

                    linha = leitor.ReadLine();
                }
                
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ocorreu um erro: {ex.Message}");
            }
        }

        private static string RemoverAcentos(string texto)
        {
                string textoNormalizado = texto.Normalize(NormalizationForm.FormD);
                var sb = new StringBuilder();

                foreach (char caractere in textoNormalizado)
                    if (CharUnicodeInfo.GetUnicodeCategory(caractere) != UnicodeCategory.NonSpacingMark)
                        sb.Append(caractere);

                return sb.ToString().Normalize(NormalizationForm.FormC);
        }
      }
}
