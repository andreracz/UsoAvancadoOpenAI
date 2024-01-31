public class ContaCorrente {

    public ContaCorrente(string numero)  {
        this.Saldo = 0;
        this.Numero = numero;
    }

    public string Numero { get; set; }


    public double Saldo { get; set; }

    public void Depositar(double valor) {
        this.Saldo += valor;
    }

    public void Sacar(double valor) {
        this.Saldo -= valor;
    }

    public void Transferir(double valor, ContaCorrente contaDestino) {
        this.Sacar(valor);
        contaDestino.Depositar(valor);
    }
}