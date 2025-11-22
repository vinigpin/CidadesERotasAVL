using AgendaAlfabetica;
using System;
using System.IO;
using System.Windows.Forms;

namespace Proj4
{
  public class Cidade : IComparable<Cidade>, IRegistro
  {
    string nome;
    double x, y;
    public ListaSimples<Ligacao> ligacoes = new ListaSimples<Ligacao>();

    const int tamanhoNome = 25;
    const int tamanhoRegistro = tamanhoNome+ (2 * sizeof(double));

    public string Nome
    {
      get => nome;
      set => nome = value.PadRight(tamanhoNome, ' ').Substring(0, tamanhoNome);
    }

    public Cidade(string nome, double x, double y)
    {
      this.Nome = nome;
      this.x = x;
      this.y = y;
    }
    public override string ToString()
    {
      return Nome.TrimEnd() + " (" + ligacoes.QuantosNos + ")";
    }

    public Cidade()
    {
      this.Nome = "";
      this.x = 0;
      this.y = 0;
    }

    public Cidade(string nome)
    {
      this.Nome = nome;
    }

    public int CompareTo(Cidade outraCid)
    {
        return Nome.CompareTo(outraCid.Nome);
    }

    public int TamanhoRegistro { get => tamanhoRegistro; }
    public double X { get => x; set => x = value; }
    public double Y { get => y; set => y = value; }

    public void LerRegistro(BinaryReader arquivo, long qualRegistro)
    {
        if (arquivo == null)
            return;
        try
        {
            long bytes = qualRegistro * tamanhoRegistro;
            arquivo.BaseStream.Seek(bytes, SeekOrigin.Begin);

            // ler o nome da cidade
            char[] nomeChar = new char[tamanhoNome];
            string nomeLido = "";
            nomeChar = arquivo.ReadChars(tamanhoNome);
            for (int i = 0; i < tamanhoNome; i++)
                nomeLido += nomeChar[i];
            this.nome = nomeLido;

            // ler posicao x, y
            this.x = arquivo.ReadDouble();
            this.y = arquivo.ReadDouble();
        }
        catch (Exception e)
        {
            MessageBox.Show(e.Message);
        }
        
    }

    public void GravarRegistro(BinaryWriter arquivo)
    {

    }

  }
}
