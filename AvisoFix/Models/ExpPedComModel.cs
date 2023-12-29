using System.Reflection.Metadata.Ecma335;

namespace AvisoFix.Models;

public class ExpPedComModel
{
    public string CodEmpresa { get; set; }
    public string CodFilial { get; set; }
    public string NumPedCom { get; set; }
    public string FluxoCaixa { get; set; }
    public int DiasAvisoFix { get; set; }
}
