using AvisoFix.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvisoFix.Context
{
    public static class IgecexContext
    {
        public static List<ExpPedComModel> getExpPedCom()
        {
            List<ExpPedComModel> expPedcomLista = new List<ExpPedComModel>();

            using (SqlConnection cn = new SqlConnection(Tools.getConnectionString()))
            {
                var querie = "SELECT cod_empresa, cod_filial, num_pedcom, ind_fluxo_caixa, dias_aviso_fix ";
                querie += $"FROM GECEX.dbo.exp_pedcom WHERE num_pedcom >= '{Tools.getNumPedComInicial()}'";

                DataSet ds = new DataSet();
                using (SqlDataAdapter da = new SqlDataAdapter(querie, cn))
                {
                    da.Fill(ds);

                    foreach (DataRow item in ds.Tables[0].Rows)
                    {
                        expPedcomLista.Add(new ExpPedComModel
                        {
                            NumPedCom = item["num_pedcom"].ToString(),
                            CodEmpresa = item["cod_empresa"].ToString(),
                            CodFilial = item["cod_filial"].ToString(),
                            DiasAvisoFix = Convert.ToInt32(item["dias_aviso_fix"]),
                            FluxoCaixa = item["ind_fluxo_caixa"].ToString()
                        });
                    }
                }
            }

            return expPedcomLista;
        }
        
        public static List<ExpPedComItemModel> getExpPedComItem()
        {
            List<ExpPedComItemModel> expPedcomItemLista = new List<ExpPedComItemModel>();

            var querie = "SELECT cod_empresa, cod_filial, num_pedcom, SUBSTRING(cod_produto, 1, 9) as cod_produto, qtd_comprada, qtd_cancelada, val_grade1, ind_servico, cod_mod_prestacao ";
            querie += $"FROM GECEX.dbo.exp_pedcom_item WHERE num_pedcom >= '{Tools.getNumPedComInicial()}' And qtd_comprada - qtd_cancelada > 0";

            using (SqlConnection cn = new SqlConnection(Tools.getConnectionString()))
            {
                DataSet ds = new DataSet();
                using (SqlDataAdapter da = new SqlDataAdapter(querie, cn))
                {
                    da.Fill(ds);

                    foreach (DataRow item in ds.Tables[0].Rows)
                    {
                        expPedcomItemLista.Add(new ExpPedComItemModel
                        {
                            NumPedCom = item["num_pedcom"].ToString(),
                            CodEmpresa = item["cod_empresa"].ToString(),
                            CodFilial = item["cod_filial"].ToString(),
                            CodProduto = item["cod_produto"].ToString(),
                            ValGrade1 = item["val_grade1"].ToString(),
                            ind_servico = item["ind_servico"].ToString(),
                            CodModPrestacao = item["cod_mod_prestacao"].ToString()
                        });
                    }
                }
            }

            return expPedcomItemLista;
        }
        
        public static void resetDatabase()
        {
            var querieLista = new List<string>();
            querieLista.Add("UPDATE GECEX.dbo.exp_pedcom SET ind_fluxo_caixa = 'N', dias_aviso_fix = 0");
            querieLista.Add("UPDATE GECEX.dbo.exp_pedcom_item SET ind_servico = '',  cod_mod_prestacao = '99'");
            querieLista.Add("UPDATE PBI.dbo.exp_pedcom SET ind_fluxo_caixa = '', dias_aviso_fix = '0'");
            querieLista.Add("UPDATE PBI.dbo.exp_pedcom_item SET ind_servico = '',  cod_mod_prestacao = '99'");

            var querieAll = querieLista.Aggregate((i, j) => i + ";" + j);

            using SqlConnection cn = new SqlConnection();

            cn.ConnectionString = Tools.getConnectionString();
            cn.Open();

            using SqlCommand cmd = new SqlCommand(querieAll, cn);
            cmd.ExecuteNonQuery();
        }
        
        public static void updateDatabase(List<ExpPedComItemModel> lista)
        {
            var numpedcomLista = lista.OrderBy(x => x.NumPedCom).Select(x => new { x.NumPedCom, x.CodProduto }).Distinct().ToList();
            var numpedcom_string = "'";
            numpedcom_string += string.Join("','", numpedcomLista.ToList().Select(x => x.NumPedCom));
            numpedcom_string += "'";

            var querieLista = new HashSet<string>();
            querieLista.Add("UPDATE PBI.dbo.exp_pedcom SET ind_fluxo_caixa = 'S', dias_aviso_fix = 1 Where num_pedcom IN (" + numpedcom_string + ")");

            foreach (var itemQuerie in numpedcomLista)
                querieLista.Add($"UPDATE PBI.dbo.exp_pedcom_item SET ind_servico = 'S' Where cod_empresa = '01' and cod_filial = 1 and num_pedcom = '{itemQuerie.NumPedCom}' and cod_produto like '{itemQuerie.CodProduto}%'");

            var querieAll = querieLista.Aggregate((i, j) => i + ";" + j);

            using SqlConnection cn = new SqlConnection();

            cn.ConnectionString = Tools.getConnectionString();
            cn.Open();

            using SqlCommand cmd = new SqlCommand(querieAll, cn);
            cmd.ExecuteNonQuery();
        }
    }
}
