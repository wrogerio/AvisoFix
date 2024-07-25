Console.Clear();

// - 61229

Console.WriteLine("Resetando o banco de dados...");
AvisoFix.Context.IgecexContext.resetDatabase();

Console.WriteLine("Trazendo os dados do banco...");
var exp_pedcom_lista = AvisoFix.Context.IgecexContext.getExpPedCom();
var exp_pedcom_item_lista = AvisoFix.Context.IgecexContext.getExpPedComItem();

Console.WriteLine("Criando as listas das empresas...");
var pedcomItem_99_97 = exp_pedcom_item_lista.Where(x => x.CodEmpresa == "99" && x.CodFilial == "97" && !string.IsNullOrEmpty(x.ValGrade1.Trim())).ToList();
var pedcomItem_99_99 = exp_pedcom_item_lista.Where(x => x.CodEmpresa == "99" && x.CodFilial == "99").ToList();

Console.WriteLine("Identificar na pedcomItem_99_97, quem tem mais de um codigo de produto...");
var pedcomItem_99_97_varios = pedcomItem_99_97.GroupBy(x => new { x.CodEmpresa, x.CodFilial, x.NumPedCom, x.CodProduto }).Where(x => x.Count() > 1).ToList();

Console.WriteLine("remover da pedcomItem_99_99 os itens que nao estao na pedcomItem_99_97_varios...");
foreach (var item in pedcomItem_99_99)
{
    var existe = pedcomItem_99_97_varios.Any(x => x.Key.NumPedCom == item.NumPedCom && x.Key.CodProduto == item.CodProduto);
    if (!existe) item.CodEmpresa = "remover";
}
pedcomItem_99_99 = pedcomItem_99_99.Where(x => x.CodEmpresa != "remover").ToList();

Console.WriteLine("Verificar se todos os val_grade1 foram preenchidos...");
var pedidos_99_99 = pedcomItem_99_99.Select(x => new { x.NumPedCom, x.CodProduto }).Distinct().ToList();
foreach (var itemProcura in pedidos_99_99)
{
    var itemFiltrado = pedcomItem_99_99.Where(x => x.NumPedCom == itemProcura.NumPedCom && x.CodProduto == itemProcura.CodProduto).ToList();
    var itensEmBranco = itemFiltrado.Where(x => string.IsNullOrEmpty(x.ValGrade1)).ToList();

    if (itensEmBranco.Count == 0)
        foreach (var item in itemFiltrado)
            item.CodEmpresa = "remover";
}

Console.WriteLine("removendo...");
pedcomItem_99_99 = pedcomItem_99_99.Where(x => x.CodEmpresa != "remover").ToList();

Console.WriteLine("Update Database...");
AvisoFix.Context.IgecexContext.updateDatabase(pedcomItem_99_99);