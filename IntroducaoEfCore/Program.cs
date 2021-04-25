using System;
using System.Collections.Generic;
using System.Linq;
using IntroducaoEfCore.Domain;
using IntroducaoEfCore.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace IntroducaoEfCore
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            // InserirDados();
            // InserirDadosEmMassa();
            // ConsultarDados();
            // CadastratPedido();
            // ConsultarPedidoCarregamentoAdiantado();
            // AtualizarDados();
            // RemoverRegistro();
        }

        public static void RemoverRegistro()
        {
            using var db = new Data.ApplicationContext();
            
            // var cliente = db.Clientes.Find(2); // Exclusão de forma conectada, pois obteve o registro da base de dados
            var cliente = new Cliente {Id = 3}; // Exclusão de forma desconectada

            // 3 formas de excluir um registro recuperado da base de dados:
            db.Clientes.Remove(cliente);
            // db.Remove(cliente);
            // db.Entry(cliente).State = EntityState.Deleted;                     

            db.SaveChanges();
        }

        public static void AtualizarDados()
        {
            using var db = new Data.ApplicationContext();

            // var cliente = db.Clientes.Find(1);

            //--------------------------
            // Se chamar a função Update, por mais que altere uma única propriedade, o sql gerado irá atualizar todas
            //cliente.Nome = "Cliente alterado passo 1";
            // db.Clientes.Update(cliente);
            //--------------------------

            // Se somente alterar a coluna e já chamar o SaveChanges, sem chamar o método Update, ele altera apenas a coluna modificada
            // cliente.Nome = "Cliente alterado passo 2";

            //--------------------------
            // Para forçar o rastreamento de uma entidade, use o State, então mesmo sem alteração alguma, ele irá atualizar o objeto na base de dados
            // db.Entry(cliente).State = EntityState.Modified;
            //--------------------------

            //--------------------------
            // Forma desconectada de atualização: Atualiza seus dados dos objetos sem utilizar diretamente um objeto recuperado da base de dados.
            // Primeira Forma: Permitir atualizar apenas alguns campos através  da criação de um outro objeto que não seja o da entidade, 
            // somente com as propriedades que se deseja atualizar            
            // var clienteDesconectado = new 
            // {
            //     Nome = "TEste cliente desconectado",
            //     Telefone = "978877887"
            // };
            
            // db.Entry(cliente).CurrentValues.SetValues(clienteDesconectado);

            // Segunda Forma: Criar um objeto já com o Id da entidade que se deseja atualizar
            var cliente2 = new Cliente
            {
                Id = 1
            };
            
            var clienteDesconectado2 = new 
            {
                Nome = "TEste cliente desconectado 2",
                Telefone = "978877887"
            };

            db.Attach(cliente2);            
            db.Entry(cliente2).CurrentValues.SetValues(clienteDesconectado2);
            //--------------------------

            db.SaveChanges();
        }

        public static void ConsultarPedidoCarregamentoAdiantado()
        {
            using var db = new Data.ApplicationContext();
            var pedidos = db.Pedidos.Include(p => p.Itens).ThenInclude(p => p.Pedido).ToList();

            System.Console.WriteLine(pedidos.Count);
        }

        private static void CadastratPedido()
        {
            using var db = new Data.ApplicationContext();

            var cliente = db.Clientes.FirstOrDefault();
            var produto = db.Produtos.FirstOrDefault();

            var pedido = new Pedido
            {
                ClienteId = cliente.Id,
                IniciadoEm = DateTime.Now,
                FinalizadoEm = DateTime.Now,
                Observacao = "Pedido de teste",
                Status = StatusPedido.Analise,
                TipoFrete = TipoFrete.SemFrete,
                Itens = new List<PedidoItem>
                {
                    new PedidoItem
                    {
                        IdProduto = produto.Id,
                        Desconto = 0,
                        Quantidade = 1,
                        Valor = 10
                    }
                }
            };

            db.Pedidos.Add(pedido);
            db.SaveChanges();
        }

        private static void ConsultarDados()
        {
            //Linq Query Sintaxe: Forma mais próxima da de escrever uma consulta SQL
            using var db = new Data.ApplicationContext();

            //var consultaPorSintaxe = (from c in db.Clientes where c.Id > 0 select c).ToList();
            
            // -----------------------------------
            
            //Consulta por métodos linq
            //var consultaPorMetodo = db.Clientes.Where(p => p.Id > 0).ToList();

            //Cada registro obtido é mantido em memória, chamado track (o EfCore passa a rastrear esses objetos), 
            //Se recuperar uma lista de clientes, alterar algo em algum desses objetos de cliente, e chamar o método 
            //SaveChanges, ele irá atualizar o registro dos objetos que foram alterados.

            // foreach (var cliente in consultaPorMetodo)
            // {
            //     System.Console.WriteLine($"Id: {cliente.Id}");
            //     db.Clientes.Find(cliente.Id);
            // }

            // -----------------------------------

            //Consulta por métodos linq Sem Tracking
            //var consultaPorMetodoSemTracking = db.Clientes.AsNoTracking().Where(p => p.Id > 0).ToList();

            //Cada registro obtido é mantido em memória, chamado track (o EfCore passa a rastrear esses objetos), 
            //Se recuperar uma lista de clientes, alterar algo em algum desses objetos de cliente, e chamar o método 
            //SaveChanges, ele irá atualizar o registro dos objetos que foram alterados.

            // foreach (var cliente in consultaPorMetodoSemTracking)
            // {
            //     System.Console.WriteLine($"Id: {cliente.Id}");
            //     db.Clientes.Find(cliente.Id);
            // }

            // -----------------------------------

            // var consultaPorMetodoFirstOrDefaultSempreIraBaseDados = db.Clientes.Where(p => p.Id > 0).ToList();
            
            // foreach (var cliente in consultaPorMetodoFirstOrDefaultSempreIraBaseDados)
            // {
            //     db.Clientes.FirstOrDefault(p => p.Id == cliente.Id);
            // }
        }

        private static void InserirDados()
        {
            var produto = new Produto
            {
                Descricao = "Produto Teste",
                CodigoBarras = "1234567891231",
                Valor = 10m,
                TipoProduto = TipoProduto.MercadoriaParaRevenda,
                Ativo = true
            };

            using var db = new Data.ApplicationContext();
            // FORMAS DE INSERÇÃO:
            // Prefira a opção 2 ou a 1.

            // db.Produtos.Add(produto); //Opção 1
            // db.Set<Produto>().Add(produto); //Opção 2 : Genérica
            // db.Entry(produto).State = EntityState.Added;// Opção 3: Forçar o EFCore a fazer um rastreio na entidade. Tem que especificar qual operação quer rastrear. oOmo a de adicionar, no caso.
            // db.Add(produto); // Opção 4: Passa a entidade e o EFCore faz uma sobrecarga de processamento para ver qual a entidade que está sendo adicionada.            
            
            // -----------------------------------
            
            db.Set<Produto>().Add(produto);

            // Curiosidade
            // Se chamar várias vezes a funcionalidade de inserir o Produto, utilizando a mesma instância do objeto,
            // ele apenas irá inserir uma única vez, pq o EFCore rastreia a instância do objeto.

            var totalRegistrosAfetados = db.SaveChanges();// Só irá gravar ao chamar a função SaveChanges();
            System.Console.WriteLine(totalRegistrosAfetados);            
        }

        private static void InserirDadosEmMassa()
        {
            var produto = new Produto
            {
                Descricao = "Produto Teste",
                CodigoBarras = "1234567891231",
                Valor = 10m,
                TipoProduto = TipoProduto.MercadoriaParaRevenda,
                Ativo = true
            };

            var cliente = new Cliente
            {
                Nome = "Dayane Lima",
                CEP = "32000000",
                Cidade = "Contagem",
                Estado = "MG",
                Telefone = "999999999"
            };

            using var db = new Data.ApplicationContext();
            db.AddRange(produto, cliente);

            var totalRegistrosAfetados = db.SaveChanges();
            System.Console.WriteLine($"Total Registro(s): {totalRegistrosAfetados}");

            var listaCliente = new[]
            {
                new Cliente
                {
                    Nome = "Dayane Lima 0",
                    CEP = "32000000",
                    Cidade = "Contagem",
                    Estado = "MG",
                    Telefone = "999999999"
                },
                new Cliente
                {
                    Nome = "Dayane Lima 2",
                    CEP = "32000000",
                    Cidade = "Contagem",
                    Estado = "MG",
                    Telefone = "999999999"
                }
            };

            db.Set<Cliente>().AddRange(listaCliente);

            var totalRegistrosAfetados2 = db.SaveChanges();
            System.Console.WriteLine($"Total Registro(s): {totalRegistrosAfetados2}");
        }
    }
}
