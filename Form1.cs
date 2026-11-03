
using System;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Consumo_API_CSharp.Models;
using Consumo_API_CSharp.Services;

namespace Consumo_API_CSharp;

public partial class Form1 : Form
{
    private readonly QuoteService _quoteService = new QuoteService();

    private ListBox listaCitacoes = new ListBox();
    private Label estado = new Label();
    private Label contador = new Label();
    private TextBox txtPesquisa = new TextBox();
    private Button btnCarregar = new Button();
    private Button btnPesquisar = new Button();

    private QuotesResponse? dadosRecebidos;

    public Form1()
    {
        InitializeComponent();
        ConfigurarFormulario();
    }

    private void ConfigurarFormulario()
    {
        Text = "Consumo de Web API em C#";
        Size = new Size(1000, 700);
        StartPosition = FormStartPosition.CenterScreen;
        BackColor = Color.White;
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox = false;

        Label titulo = new Label();
        titulo.Text = "CONSUMO DE WEB API EM C#";
        titulo.Font = new Font("Arial", 20, FontStyle.Bold);
        titulo.ForeColor = Color.DarkBlue;
        titulo.Location = new Point(30, 25);
        titulo.AutoSize = true;
        Controls.Add(titulo);

        Label descricao = new Label();
        descricao.Text = "Sistema de consulta de citações através de uma API REST.";
        descricao.Font = new Font("Arial", 11);
        descricao.Location = new Point(32, 75);
        descricao.AutoSize = true;
        Controls.Add(descricao);

        btnCarregar.Text = "Carregar / Actualizar";
        btnCarregar.Font = new Font("Arial", 10, FontStyle.Bold);
        btnCarregar.Size = new Size(200, 42);
        btnCarregar.Location = new Point(30, 120);
        btnCarregar.BackColor = Color.DarkBlue;
        btnCarregar.ForeColor = Color.White;
        btnCarregar.FlatStyle = FlatStyle.Flat;
        btnCarregar.Click += async (sender, e) => await CarregarCitacoesAsync();
        Controls.Add(btnCarregar);

        Label pesquisaTitulo = new Label();
        pesquisaTitulo.Text = "Pesquisar por frase ou autor:";
        pesquisaTitulo.Font = new Font("Arial", 10, FontStyle.Bold);
        pesquisaTitulo.Location = new Point(30, 185);
        pesquisaTitulo.AutoSize = true;
        Controls.Add(pesquisaTitulo);

        txtPesquisa.Font = new Font("Arial", 11);
        txtPesquisa.Location = new Point(30, 215);
        txtPesquisa.Size = new Size(650, 30);
        Controls.Add(txtPesquisa);

        btnPesquisar.Text = "Pesquisar";
        btnPesquisar.Font = new Font("Arial", 10, FontStyle.Bold);
        btnPesquisar.Size = new Size(150, 32);
        btnPesquisar.Location = new Point(700, 213);
        btnPesquisar.BackColor = Color.SeaGreen;
        btnPesquisar.ForeColor = Color.White;
        btnPesquisar.FlatStyle = FlatStyle.Flat;
        btnPesquisar.Click += (sender, e) => PesquisarCitacoes();
        Controls.Add(btnPesquisar);

        Label resultadoTitulo = new Label();
        resultadoTitulo.Text = "Citações:";
        resultadoTitulo.Font = new Font("Arial", 12, FontStyle.Bold);
        resultadoTitulo.Location = new Point(30, 275);
        resultadoTitulo.AutoSize = true;
        Controls.Add(resultadoTitulo);

        listaCitacoes.Font = new Font("Arial", 10);
        listaCitacoes.Location = new Point(30, 305);
        listaCitacoes.Size = new Size(920, 270);
        listaCitacoes.HorizontalScrollbar = true;
        Controls.Add(listaCitacoes);

        contador.Text = "Citações carregadas: 0";
        contador.Font = new Font("Arial", 10, FontStyle.Bold);
        contador.ForeColor = Color.DarkBlue;
        contador.Location = new Point(30, 590);
        contador.AutoSize = true;
        Controls.Add(contador);

        estado.Text = "Estado: Aguardando ligação à API...";
        estado.Font = new Font("Arial", 10, FontStyle.Italic);
        estado.ForeColor = Color.Gray;
        estado.Location = new Point(30, 620);
        estado.AutoSize = true;
        Controls.Add(estado);
    }

    private async Task CarregarCitacoesAsync()
    {
        try
        {
            btnCarregar.Enabled = false;
            btnPesquisar.Enabled = false;
            btnCarregar.Text = "A carregar...";
            estado.Text = "Estado: A estabelecer ligação à API...";
            estado.ForeColor = Color.DarkOrange;
            listaCitacoes.Items.Clear();

            dadosRecebidos = await _quoteService.ObterCitacoesAsync();

            if (dadosRecebidos == null || dadosRecebidos.Quotes.Count == 0)
            {
                contador.Text = "Citações carregadas: 0";
                estado.Text = "Estado: Nenhuma citação encontrada.";
                estado.ForeColor = Color.DarkOrange;
                return;
            }

            ExibirCitacoes(dadosRecebidos.Quotes);

            contador.Text =
                $"Citações carregadas: {dadosRecebidos.Quotes.Count} | Total na API: {dadosRecebidos.Total}";

            estado.Text = "Estado: Ligação à API realizada com sucesso.";
            estado.ForeColor = Color.Green;
        }
        catch (Exception ex)
        {
            estado.Text = "Estado: Erro ao carregar os dados.";
            estado.ForeColor = Color.Red;

            MessageBox.Show(
                ex.Message,
                "Erro de comunicação",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error
            );
        }
        finally
        {
            btnCarregar.Enabled = true;
            btnPesquisar.Enabled = true;
            btnCarregar.Text = "Carregar / Actualizar";
        }
    }

    private void ExibirCitacoes(
        System.Collections.Generic.IEnumerable<Quote> citacoes)
    {
        listaCitacoes.Items.Clear();

        foreach (var citacao in citacoes)
        {
            listaCitacoes.Items.Add(
                $"ID: {citacao.Id} | {citacao.QuoteText} — {citacao.Author}"
            );
        }

        contador.Text = $"Citações apresentadas: {listaCitacoes.Items.Count}";
    }

    private void PesquisarCitacoes()
    {
        if (dadosRecebidos == null)
        {
            MessageBox.Show(
                "Primeiro, carregue as citações da API.",
                "Aviso",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );
            return;
        }

        string termo = txtPesquisa.Text.Trim();

        if (string.IsNullOrWhiteSpace(termo))
        {
            ExibirCitacoes(dadosRecebidos.Quotes);
            contador.Text =
                $"Citações carregadas: {dadosRecebidos.Quotes.Count} | Total na API: {dadosRecebidos.Total}";
            return;
        }

        var resultados = dadosRecebidos.Quotes
            .Where(c =>
                c.QuoteText.Contains(termo, StringComparison.OrdinalIgnoreCase) ||
                c.Author.Contains(termo, StringComparison.OrdinalIgnoreCase))
            .ToList();

        ExibirCitacoes(resultados);

        if (resultados.Count == 0)
        {
            estado.Text = "Estado: Nenhum resultado encontrado.";
            estado.ForeColor = Color.DarkOrange;
        }
        else
        {
            estado.Text = "Estado: Pesquisa concluída.";
            estado.ForeColor = Color.Green;
        }

        contador.Text = $"Resultados encontrados: {resultados.Count}";
    }
}
