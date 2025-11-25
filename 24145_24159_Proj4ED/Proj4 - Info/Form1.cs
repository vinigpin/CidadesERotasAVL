using AgendaAlfabetica;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Proj4
{
  public partial class lsbCaminho : Form
  {
        Arvore<Cidade> arvore = new Arvore<Cidade>();
        List<string> caminhoEncontrado = new List<string>();
        String qualBotao = "nenhum";
        public lsbCaminho()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            arvore.LerArquivoDeRegistros("../../Dados/cidades.dat");
            LerArquivoDeLigacoes(arvore, "../../Dados/GrafoOnibusSaoPaulo.txt");
            atualizarInfo();
        }

        private void lsbCaminho_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MessageBox.Show("Salvar alterações?", "Confirmar", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                arvore.GravarArquivoDeRegistros("../../Dados/cidades.dat");
                GravarArquivoDeLigacoes(arvore, "../../Dados/GrafoOnibusSaoPaulo.txt");
            }
        }


        // ---------------------Eventos do forms---------------------
        // --crud--
        private void btnIncluirCidade_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtNomeCidade.Text))
                MessageBox.Show("Não foi digitado nenhum nome de cidade!");
            else
                textNomeCidade_Leave(sender, e);
        }
        private void textNomeCidade_Leave(object sender, EventArgs e)
        {
            qualBotao = "Incluir";
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
                arvore.Existe(exCidade);
                foreach (var cidade in arvore.Atual.Info.ligacoes.Listar())
                {
                    arvore.Existe(new Cidade(cidade.destino));
                    foreach (var ligacoa in arvore.Atual.Info.ligacoes.Listar())
                        if (ligacoa.destino.CompareTo(ligacoa.destino) == 0)
                            arvore.Atual.Info.ligacoes.RemoverDado(ligacoa);
                }
                if (arvore.Excluir(exCidade))
                    MessageBox.Show("Cidade excluída!");
                else
                    MessageBox.Show("Não foi possível excluir a cidade!");
                atualizarInfo();
            }
        }


        private void btnBuscarCidade_Click(object sender, EventArgs e)
        {
            Cidade aCidade = new Cidade(txtNomeCidade.Text);
            if (arvore.Existe(aCidade))
            {
                MessageBox.Show($"Cidade {txtNomeCidade.Text} encontrada!Buscando informações...");
                udX.Value = (decimal)arvore.Atual.Info.X;
                udY.Value = (decimal)arvore.Atual.Info.Y;
                List<Ligacao> ligacaos = new List<Ligacao>();
                ligacaos = arvore.Atual.Info.ligacoes.Listar();
                dgvLigacoes.Rows.Clear();
                dgvLigacoes.RowCount = ligacaos.Count == 0 ? 1 : ligacaos.Count;
                int row = 0;
                for (int i = 0; i < ligacaos.Count; i++)
                {
                    dgvLigacoes.Rows[i].Cells[0].Value = ligacaos[i].destino;
                    dgvLigacoes.Rows[i].Cells[1].Value = ligacaos[i].distancia;
                }
            }
            else
            {
                MessageBox.Show("Não foi possível encontrar a cidade!");
            }
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
                arvore.Excluir(aCidade);
                pbMapa.MouseClick += oMapa_MouseClick;
            }
            else
            {
                MessageBox.Show("Cidade não encontrada!");
            }
        }
        private void oMapa_MouseClick(object sender, MouseEventArgs e)
        { 
            double x = (double)e.X / pbMapa.Width;
            double y = (double)e.Y / pbMapa.Height;
            udX.Value = Convert.ToDecimal(x);
            udY.Value = Convert.ToDecimal(y);
            Cidade novaCidade = new Cidade(txtNomeCidade.Text, x, y);
            if (arvore.Atual != null)
                foreach(var lig in arvore.Atual.Info.ligacoes.Listar())
                    novaCidade.ligacoes.InserirEmOrdem(lig);
            arvore.IncluirNovoDado(novaCidade);    


            atualizarInfo();
            pbMapa.MouseClick -= oMapa_MouseClick;
        }


        private void btnIncluirCaminho_Click(object sender, EventArgs e)
        {
            string cidadeOrigem = txtNomeCidade.Text;
            string cidadeDestino = txtNovoDestino.Text;
            decimal distancia = numericUpDown1.Value;

            arvore.Existe(new Cidade(cidadeDestino));
            arvore.Atual.Info.ligacoes.InserirEmOrdem(new Ligacao(cidadeDestino, cidadeOrigem, (int)distancia));
            arvore.Existe(new Cidade(cidadeOrigem));
            arvore.Atual.Info.ligacoes.InserirEmOrdem(new Ligacao(cidadeOrigem, cidadeDestino, (int)distancia));

            atualizarInfo();
        }

        private void btnExcluirCaminho_Click(object sender, EventArgs e)
        {
            string cidadeOrigem = txtNomeCidade.Text;
            string cidadeDestino = txtNovoDestino.Text;
            decimal distancia = numericUpDown1.Value;

            arvore.Existe(new Cidade(cidadeDestino));
            arvore.Atual.Info.ligacoes.RemoverDado(new Ligacao(cidadeDestino, cidadeOrigem, (int)distancia));
            arvore.Existe(new Cidade(cidadeOrigem));
            arvore.Atual.Info.ligacoes.RemoverDado(new Ligacao(cidadeOrigem, cidadeDestino, (int)distancia));

            atualizarInfo();
        }


        // --algoritimo de Dijkstra--
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
            caminhoEncontrado.Clear();
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
                caminhoEncontrado.Add(nome);

                dgvRotas.Rows[row].Cells[0].Value = nome;
                dgvRotas.Rows[row].Cells[1].Value = distancia;
                row++;
            }
            ;
            lbDistanciaTotal.Text = "Distância total: " + distancia + "km";
            pbMapa.Invalidate();
        }


        private void pbMapa_Paint(object sender, PaintEventArgs e)
        {
            using (Pen penLigacao = new Pen(Color.Green, 2))
            using (SolidBrush brushCidade = new SolidBrush(Color.Red))
            using (Font font = new Font("Arial", 8, FontStyle.Bold))
            using (SolidBrush textBrush = new SolidBrush(Color.Black))
            using (Pen penCaminho = new Pen(Color.Red, 2))
            {
                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                float larguraMapa = pbMapa.Width;
                float alturaMapa = pbMapa.Height;

                List<Cidade> cidades = new List<Cidade>();
                arvore.VisitarEmOrdem(cidades);

                Dictionary<string, (float X, float Y)> mapaCoordenadas =
                cidades.ToDictionary(
                    c => c.Nome.Trim(),
                    c => ((float)c.X * larguraMapa, (float)c.Y * alturaMapa)
                );

                foreach (var cidade in cidades)
                {
                    ListaSimples<Ligacao> ligacoes = cidade.ligacoes;
                    var (orgiemX, origemY) = mapaCoordenadas[cidade.Nome.Trim()];
                    string nome = cidade.Nome;
                    int raio = 3;

                    // desenhar as ligações entre cada cidade
                    foreach (var liga in ligacoes.Listar())
                    {
                        var (destinoX, destinoY) = mapaCoordenadas[liga.destino];
                        e.Graphics.DrawLine(
                            penLigacao,
                            orgiemX, origemY,
                            destinoX, destinoY
                        );

                    }
                    // desenhar as cidades
                    e.Graphics.FillEllipse(brushCidade, orgiemX - raio, origemY - raio, raio * 2, raio * 2);

                    // escrever o nome das cidades
                    SizeF textSize = e.Graphics.MeasureString(nome, font);
                    e.Graphics.DrawString(nome, font, textBrush, orgiemX, origemY);

                }
                // desenhar a rota desejada
                for (int i = 0; i < caminhoEncontrado.Count - 1; i++)
                {
                    string origem = caminhoEncontrado[i];
                    string destino = caminhoEncontrado[i+1];
                    var (orgiemX, origemY) = mapaCoordenadas[origem];
                    var (destinoX, destinoY) = mapaCoordenadas[destino];

                    e.Graphics.DrawLine(
                            penCaminho,
                            orgiemX, origemY,
                            destinoX, destinoY
                        );
                }
            }
        }


        //---------------------Metodos auxiliares---------------------

        private void LerArquivoDeLigacoes(Arvore<Cidade> arvore, string nomeArquivo)
        {
            try
            {
                using (StreamReader leitor = new StreamReader(nomeArquivo))
                {
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
                            noCidade1.Info.ligacoes.InserirEmOrdem(ligacao1);
                            noCidade2.Info.ligacoes.InserirEmOrdem(ligacao2);
                        }

                        linha = leitor.ReadLine();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ocorreu um erro: {ex.Message}");
            }
        }


        private void GravarArquivoDeLigacoes(Arvore<Cidade> arvore, string nomeArquivo)
        {
            try
            {
                using (StreamWriter arquivo = new StreamWriter(nomeArquivo))
                {
                    List<Cidade> cidades = new List<Cidade>();
                    arvore.VisitarEmOrdem(cidades);
                    HashSet<string> gravadas = new HashSet<string>();

                    foreach (var cidade in cidades)
                    {
                        foreach (var ligacao in cidade.ligacoes.Listar())
                        {
                            string chave1 = $"{ligacao.origem}-{ligacao.destino}";
                            string chave2 = $"{ligacao.destino}-{ligacao.origem}";

                            if (!gravadas.Contains(chave1) && !gravadas.Contains(chave2))
                            {
                                arquivo.WriteLine($"{ligacao.origem};{ligacao.destino};{ligacao.distancia}");
                                gravadas.Add(chave1);
                            }
                        }
                    }
                }
            } catch (Exception ex)
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


        private void atualizarInfo()
        { 
            // atualizar cbx
            List<Cidade> cidades = new List<Cidade>();
            arvore.VisitarEmOrdem(cidades);
            cbxCidadeDestino.Items.Clear();
            foreach (var cidade in cidades)
            {
                cbxCidadeDestino.Items.Add(cidade.Nome);
            }

            caminhoEncontrado.Clear();
            pbMapa.Invalidate();
        }


        private void pnlArvore_Paint(object sender, PaintEventArgs e)
        {
            arvore.Desenhar(pnlArvore);
        }


        private void tpCadastro_Click(object sender, EventArgs e) { }
        private void label2_Click(object sender, EventArgs e) { }
        private void pbMapa_Click(object sender, EventArgs e) { }
        private void cbxCidadeDestino_SelectedIndexChanged(object sender, EventArgs e) { }
    }
}
