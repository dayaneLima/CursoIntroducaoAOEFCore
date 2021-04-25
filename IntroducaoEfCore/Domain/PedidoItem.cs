namespace IntroducaoEfCore.Domain
{
    public class PedidoItem
    {
        public int Id { get; set; }
        public int PedidoId { get; set; }
        public Pedido Pedido { get; set; } // Propriedade de navegação
        public int IdProduto { get; set; }
        public Produto Produto { get; set; } // Propriedade de navegação
        public int Quantidade { get; set; }
        public decimal Valor { get; set; }
        public decimal Desconto { get; set; }
    }
}