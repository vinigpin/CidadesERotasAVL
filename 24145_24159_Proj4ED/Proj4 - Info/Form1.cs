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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Proj4
{
  public partial class lsbCaminho : Form
  {
        Arvore<Cidade> arvore = new Arvore<Cidade>();
        Stack<string> caminhoEncontrado = new Stack<string>();
        public lsbCaminho()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            arvore.LerArquivoDeRegistros("../../Dados/cidades.dat");
            LerArquivoDeLigacoes(arvore, "../../Dados/GrafoOnibusSaoPaulo.txt");

            // atualizar periodicamente
            List<Cidade> cidades = new List<Cidade>();
            arvore.VisitarEmOrdem(cidades);
            foreach (var cidade in cidades)
            {
                cbxCidadeDestino.Items.Add(cidade.Nome);
            }

            // exibir localização
            pbMapa.Invalidate();
        }

        private void pbMapa_Paint(object sender, PaintEventArgs e)
        {
            float larguraMapa = pbMapa.Width;
            float alturaMapa = pbMapa.Height;
            float posX;
            float posY;

            List<Cidade> cidades = new List<Cidade>();
            arvore.VisitarEmOrdem(cidades);
            foreach (var cidade in cidades)
            {
                posX = (float)cidade.X;
                posY = (float)cidade.Y;
                string nome = cidade.Nome;
                ListaSimples<Ligacao> ligacoes = cidade.ligacoes;

                float centroX = posX * larguraMapa;
                float centroY = posY * alturaMapa;
                int raio = 3;
                float diametro = raio * 2;

                float x = centroX - raio;
                float y = centroY - raio;
                foreach (var liga in ligacoes.Listar())
                {
                    using (Pen pen = new Pen(Color.Green, 2)) // Cor verde e espessura 2 pixels
                    {
                        // O método DrawLine aceita quatro floats (x1, y1, x2, y2)
                        // Onde (x1, y1) é o ponto de partida e (x2, y2) é o ponto final.
                        arvore.Existe(new Cidade(liga.destino));
                        e.Graphics.DrawLine(
                            pen,
                            centroX, // x de partida (centro do ponto)
                            centroY, // y de partida (centro do ponto)
                            (float)arvore.Atual.Info.X * larguraMapa, // x de destino (100)
                            (float)arvore.Atual.Info.Y * alturaMapa  // y de destino (100)
                        );
                    }
                }
                using (SolidBrush brush = new SolidBrush(Color.Red))
                {
                    e.Graphics.FillEllipse(brush, x, y, diametro, diametro);
                }
                using (Font font = new Font("Arial", 8, FontStyle.Bold))
                using (SolidBrush textBrush = new SolidBrush(Color.Black))
                {
                    SizeF textSize = e.Graphics.MeasureString(nome, font);
                    e.Graphics.DrawString(nome, font, textBrush, centroX, centroY);
                }
                
            }

            float antX = -1;
            float antY = -1;;
            while (caminhoEncontrado.Count != 0)
            {
                using (Pen pen = new Pen(Color.Red, 2)) // Cor verde e espessura 2 pixels
                {
                    string cidade = caminhoEncontrado.Pop();
                    arvore.Existe(new Cidade(cidade));

                    posX = (float)arvore.Atual.Info.X * larguraMapa;
                    posY = (float)arvore.Atual.Info.Y * alturaMapa;
                    if (antX > -1 && antY > -1)
                        e.Graphics.DrawLine(pen, antX, antY, posX, posY);
                }
                antX = posX;
                antY = posY;
            }
        }

        private void btnBuscarCaminho_Click(object sender, EventArgs e)
        {
            // instânciar elementos 
            Grafo grafo = new Grafo();
            List<Cidade> cidades = new List<Cidade>();
            Dictionary<string, int> nomeIndice = new Dictionary<string, int>();
            arvore.VisitarEmOrdem(cidades);

            // criar tds os vértices
            int indice = 0;
            foreach (var cidade in cidades)
            {
                grafo.NovoVertice(cidade.Nome.Trim());
                nomeIndice.Add(cidade.Nome.Trim(), indice);
                indice++;
            }

            // criar tds as arestas
            foreach (var cidade in cidades)
                foreach (var ligacao in cidade.ligacoes.Listar())
                    grafo.NovaAresta(nomeIndice[ligacao.origem], nomeIndice[ligacao.destino], ligacao.distancia);

            // calcular o caminho
            int inicio = nomeIndice[txtNomeCidade.Text.Trim()];
            int fim = nomeIndice[cbxCidadeDestino.Text.Trim()];
            Stack<Tuple<string, int>> caminho = new Stack<Tuple<string, int>>();
            caminho = grafo.Caminho(inicio, fim, caminho);

            // exibir rotas no dgv
            dgvRotas.Rows.Clear(); 
            dgvRotas.RowCount = caminho.Count;
            int row = 0;
            int distancia = 0;
            string nome;

            while (caminho.Count != 0)
            {
                Tuple<string, int> segmento = caminho.Pop();

                nome = segmento.Item1;
                distancia = segmento.Item2;
                caminhoEncontrado.Push(nome);

                dgvRotas.Rows[row].Cells[0].Value = nome;
                dgvRotas.Rows[row].Cells[1].Value = distancia;
                row++;
            };
            lbDistanciaTotal.Text = "Distância total: " + distancia + "km";
            pbMapa.Invalidate();
        }

        private void cbxCidadeDestino_SelectedIndexChanged(object sender, EventArgs e)
        { }

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

        private void btnIncluirCidade_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtNomeCidade.Text))
                MessageBox.Show("Não foi digitado nenhum nome de cidade!");
            else
                textNomeCidade_Leave(sender, e);
        }

        private void textNomeCidade_Leave(object sender, EventArgs e)
        {
            double valX = Decimal.ToDouble(udX.Value);
            double valY = Decimal.ToDouble(udY.Value);
            Cidade exCidade = new Cidade(txtNomeCidade.Text, valX, valY);
            if (arvore.Existe(exCidade))
                MessageBox.Show($"A cidade {txtNomeCidade.Text} já existe!");
            else
            {
                MessageBox.Show("Clique no mapa para inserir a posição da cidade");
                pbMapa.MouseClick += oMapa_MouseClick;

            }
        }

        private void btnExcluirCidade_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtNomeCidade.Text))
            {
                MessageBox.Show("Não foi digitado nenhum nome de cidade!");
            }
            else
            {
                double valX = Decimal.ToDouble(udX.Value);
                double valY = Decimal.ToDouble(udY.Value);
                Cidade exCidade = new Cidade(txtNomeCidade.Text, valX, valY);
                if (arvore.Excluir(exCidade))
                    MessageBox.Show("Cidade excluída!");
                else
                    MessageBox.Show("Não foi possível excluir a cidade!");
            }
        }

        private void btnBuscarCidade_Click(object sender, EventArgs e)
        {
            double valX = Decimal.ToDouble(udX.Value);
            double valY = Decimal.ToDouble(udY.Value);
            Cidade aCidade = new Cidade(txtNomeCidade.Text, valX, valY);
            if (arvore.Existe(aCidade))
            {
                MessageBox.Show($"Cidade {txtNomeCidade.Text} encontrada!Buscando informações...");
                MessageBox.Show($"x: {arvore.Atual.Info.X} y: {arvore.Atual.Info.Y}");
            }
            else
            {
                MessageBox.Show("Não foi possível encontrar a cidade!");
            }
        }
        /// ------------------------------------****            OUTROS EVENTOS            ****------------------------------------
        private void oMapa_MouseClick(object sender, MouseEventArgs e)
        {
            double x = e.X;
            double y = e.Y;

            udX.Value = Convert.ToDecimal(x);
            udY.Value = Convert.ToDecimal(y);

            Cidade novaCidade = new Cidade(txtNomeCidade.Text,x,y);
            arvore.IncluirNovoDado(novaCidade);
            arvore.GravarArquivoDeRegistros("cidades.dat");

            pbMapa.MouseClick -= oMapa_MouseClick;
            MessageBox.Show($"A cidade {txtNomeCidade.Text} se localiza na posição  X:{x}    Y:{y}");
        }

        private void btnAlterarCidade_Click(object sender, EventArgs e)
        {
            double valX = Decimal.ToDouble(udX.Value);
            double valY = Decimal.ToDouble(udY.Value);
            Cidade aCidade = new Cidade(txtNomeCidade.Text, valX, valY);
            if (arvore.Existe(aCidade))
            {
                MessageBox.Show($"Cidade {txtNomeCidade.Text} encontrada!Buscando informações...");
                MessageBox.Show($"Clique na tela para alterar as coordenadas da cidade.");

                pbMapa.MouseClick += oMapa_MouseClick;
            }
            else
            {
                MessageBox.Show("Cidade não encontrada!");
            }
        }

        private void pnlArvore_Paint(object sender, PaintEventArgs e)
        {
            arvore.Desenhar(pnlArvore);
        }

        private void tpCadastro_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void pbMapa_Click(object sender, EventArgs e)
        {

        }

        
    }
}
