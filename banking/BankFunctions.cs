using System.ComponentModel;
using System;
using System.Globalization;
using System.Collections.Generic;

public class BankFunctions {

    public BankFunctions(ContaCorrente contaCorrentePrincipal, Boleto[] boletosPendentes, Dictionary<string, ContaCorrente> contasRelacionadas) {
        this.ContaCorrentePrincipal = contaCorrentePrincipal;
        this.BoletosPendentes = boletosPendentes; 
        this.ContasRelacionadas = contasRelacionadas;
    }

    Dictionary<string, ContaCorrente> ContasRelacionadas {get; set;}
    ContaCorrente ContaCorrentePrincipal {get; set;}
    Boleto[] BoletosPendentes {get; set;}
  
    [Description("Obtem o Saldo da Conta corrente do usuario atual")]
    public string Saldo()
    {
        System.Console.WriteLine("Saldo: " + ContaCorrentePrincipal.Saldo);
      return (ContaCorrentePrincipal.Saldo).ToString(CultureInfo.InvariantCulture);
    }


    [Description("Obtem os boletos pendentes do usuario atual")]
    public string ObtemBoletosPendentes()
    {
        string retVal = "Boletos Pendentes: \n";
        foreach(var boleto in this.BoletosPendentes)
        {
            if (!boleto.Pago) {
                retVal += "Numero:" + boleto.Numero + " - Valor:" + boleto.Valor + "\n";
            }
            
        }
        System.Console.WriteLine(retVal);
        return retVal;
    }

    [Description("Obtem os boletos pendentes do usuario atual")]
    
    public string PagarBoleto([Description("Numero do Boleto")] string numeroBoleto)
    {
        System.Console.WriteLine("Pagar Boleto " + numeroBoleto + " chamado");
        foreach(var boleto in this.BoletosPendentes)
        {
            if (boleto.Numero == numeroBoleto) {
                if (boleto.Pago) {
                    return "Boleto " + numeroBoleto + " ja foi pago!";
                }
                boleto.Pagar(ContaCorrentePrincipal);
                return "Boleto " + numeroBoleto + " pago com sucesso!";
            }
        }
        return "Boleto " + numeroBoleto + " nao encontrado!";
    }

    [Description("Obtem os numeros de contas relacionadas ao usuario atual pelo nome ou apelido")]
    public string ObterContaRelacionada([Description("Nome da Conta")]string nomeConta)
    {
        System.Console.WriteLine("ObterContaRelacionada " + nomeConta + " chamado");
        if (ContasRelacionadas.ContainsKey(nomeConta)) {
            return "Conta " + nomeConta + " encontrada! Número da Conta: " + ContasRelacionadas[nomeConta].Numero;
        } 
        return "Conta " + nomeConta + " não encontrada!";
    }

    [Description("Transferir um valor para uma conta relacionada")]
    public string Transferir([Description("Numero da Conta relacionada")]string numeroConta, [Description("Valor a ser transferido")]string valor)
    {
        System.Console.WriteLine("Transferir " + valor + " para " + numeroConta + " chamado");
        foreach(var conta in ContasRelacionadas.Values)
        {
            if (conta.Numero == numeroConta) {
                ContaCorrentePrincipal.Transferir(Convert.ToDouble(valor, CultureInfo.InvariantCulture), conta);
                return "Transferencia de " + valor + " para " + numeroConta + " realizada com sucesso!";
            }
        }
        return "Conta " + numeroConta + " não encontrada!";
    }

    public string ToString() {
        StringBuilder sb = new StringBuilder();
        sb.Append("Conta Corrente Principal - Saldo: " + ContaCorrentePrincipal.Saldo + "\n");
        sb.Append("Boletos Pendentes: \n");
        foreach(var boleto in this.BoletosPendentes)
        {
            sb.Append("  - Numero:" + boleto.Numero + " - Valor:" + boleto.Valor + " - Pago:" + boleto.Pago + "\n");
        }
        sb.Append("Contas Relacionadas: \n");
        foreach(var nome in ContasRelacionadas.Keys)
        {   
            var conta = ContasRelacionadas[nome];
            sb.Append("  - Nome: " + nome + " - Numero:" + conta.Numero + " - Saldo:" + conta.Saldo + "\n");
        }
        return sb.ToString();
    } 

}