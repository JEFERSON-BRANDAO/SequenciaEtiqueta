using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Drawing.Printing;
using System.Threading;
using System.Configuration;
using System.Globalization;

namespace Foxconn_Traceability
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            //           
            Clear_ProgressBar();

            #region RODAPÉ

            int anoCriacao = 2018;
            int anoAtual = DateTime.Now.Year;
            string texto = anoCriacao == anoAtual ? " Foxconn FBRLA All Rights Reserved." : "-" + anoAtual + " Foxconn FBRLA All Rights Reserved.";
            //
            lbRodape.Text = "Copyright © " + anoCriacao + texto;

            #endregion
            //
            CarregaComboModelos();
            txtQty.Select();

        }

        public void CarregaComboModelos()
        {
            string caminho = AppDomain.CurrentDomain.BaseDirectory + @"\CONFIGURACAO\MODELO.txt";
            string linha;
            //
            cboModelo.Items.Add("----SELECIONE----");
            cboModelo.SelectedItem = "----SELECIONE----";
            //
            if (System.IO.File.Exists(caminho))
            {
                System.IO.StreamReader arqTXT = new System.IO.StreamReader(caminho);
                //
                while ((linha = arqTXT.ReadLine()) != null)
                {
                    if (!string.IsNullOrEmpty(linha))
                    {
                        if (!cboModelo.Items.Contains(linha.ToUpper().Trim()))//não add duplicado
                            cboModelo.Items.Add(linha.ToUpper().Trim());
                    }
                }

                //deixa selecionado item 0
                cboModelo.SelectedIndex = 0;
                cboModelo.SelectAll();
            }

        }

        public void createNewRecords(int valor, int qtdMaxima)
        {
            progressBar1.Maximum = qtdMaxima;
            progressBar1.Value = valor;
            //progressBar1.Value += valor;

            this.Text = progressBar1.Value.ToString();//exibe na barra de status
            //
            if (progressBar1.Value == qtdMaxima)
            {
                lbProcessando.Text = "Concluído " + progressBar1.Value.ToString();
            }
            else
            {
                lbProcessando.Text = "De: " + progressBar1.Value.ToString() + " Até: " + qtdMaxima;
            }
        }

        public void Clear_ProgressBar()
        {
            //reconfigura a progressbar para o padrao.
            progressBar1.MarqueeAnimationSpeed = 0;
            progressBar1.Style = ProgressBarStyle.Blocks;
            progressBar1.Value = 0;
            lbProcessando.Text = string.Empty;
        }

        private void btnGerar_Click(object sender, EventArgs e)
        {
            Clear_ProgressBar();
            //
            string quantidade = txtQty.Text.Trim();
            string rowItem = cboModelo.SelectedIndex.ToString();
            //
            if (rowItem != "0")
            {
                if (string.IsNullOrEmpty(quantidade))
                {
                    MessageBox.Show("Quantidade não pode ser vázia", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    //para emitir som de alerta
                    Som objSom = new Som();
                    objSom.Falha();
                }
                else
                {
                    if (int.Parse(quantidade) > 0)
                    {
                        //limpa grid
                        gridLabel.DataSource = null;
                        gridLabel.Refresh();

                        Gerar();
                        txtQty.Select();
                    }
                    else
                    {
                        MessageBox.Show("Quantide dever ser maior que 0", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        //para emitir som de alerta
                        Som objSom = new Som();
                        objSom.Falha();
                    }
                }
            }
            else
            {
                MessageBox.Show("Selecione um Modelo", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //para emitir som de alerta
                Som objSom = new Som();
                objSom.Falha();
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Deseja fechar a aplicação?", "Fechar Aplicação",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                    == DialogResult.Yes)
            {
                Application.Exit();
            }

        }

        public void Gerar()
        {
            try
            {
                IList<Etiqueta_DTO> lista_Seriais = new List<Etiqueta_DTO>();
                Etiqueta_DTO etiqueta = new Etiqueta_DTO();
                etiqueta.modelo = cboModelo.SelectedItem.ToString().Trim();
                etiqueta.sequencia = int.Parse(string.IsNullOrEmpty(txtValor.Text.ToUpper().Trim()) ? "1" : txtValor.Text.ToUpper().Trim());
                etiqueta.quantidade = int.Parse(string.IsNullOrEmpty(txtQty.Text.Trim()) ? "1" : txtQty.Text.Trim());
                lista_Seriais = new Etiqueta_BLL().Seriais(etiqueta);
                //
                for (int index = 1; index <= etiqueta.quantidade; index++)
                {
                    //VALOR BARRA DE STATUS
                    createNewRecords(index, etiqueta.quantidade);
                }
                //
                int rowCount = lista_Seriais.Count;
                if (rowCount > 0)
                {
                    if (lista_Seriais[0].modelo.Equals("F273"))
                    {
                        gridLabel.DataSource = lista_Seriais;
                        gridLabel.Rows[0].Selected = false;
                        gridLabel.Columns[0].Width = 180;//serial inicial
                        gridLabel.Columns[1].Width = 180;//serial final
                        gridLabel.Columns[2].Width = 135;//modelo
                        gridLabel.Columns[3].Visible = false;//sequencia (não mostra)
                        gridLabel.Columns[4].Visible = false;//quantidade (não mostra)
                        //
                        etiqueta.inicial = lista_Seriais[0].inicial;
                        etiqueta.final = lista_Seriais[0].final;
                        //LOG                      
                        Log objLog = new Log();
                        objLog.Gravar(etiqueta, "Imprimir");
                    }
                }
                else
                {
                    //para emitir som de alerta
                    Som objSom = new Som();
                    objSom.Falha();
                    //  
                    Clear_ProgressBar();
                    MessageBox.Show("Erro ao gerar seriais", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    //
                    return;
                }

            }
            catch (Exception ex)
            {
                Som objSom = new Som();
                objSom.Falha();
                //
                MessageBox.Show(ex.Message.ToString());
                //
                return;
            }
        }
        //
        private void txtQty_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void cboModelo_SelectedValueChanged(object sender, EventArgs e)
        {
            string modelo = cboModelo.SelectedItem.ToString();
            txtValor.Clear();
            //
            if (modelo.Equals("F273"))
            {
                lbValorInicial.Visible = true;
                txtValor.Visible = true;
            }
            else
            {
                lbValorInicial.Visible = false;
                txtValor.Visible = false;

            }
            //
            txtValor.Focus();
        }

        private void txtValor_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

    }
}
