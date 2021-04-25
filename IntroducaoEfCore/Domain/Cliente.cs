using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IntroducaoEfCore.Domain
{
    // [Table("Clientes")]
    public class Cliente
    {
        //[Key]//Usando data anotations para mapeamento de banco de dados 
        //[não é bom, utilize o Fluent API - como foi feito na pasta Data->Configurarions. Ele é mais rico e com mais opções]
        public int Id { get; set; }

        // [Required]
        public string Nome { get; set; }

        // [Column("Phone")]
        public string Telefone { get; set; }
        
        public string CEP { get; set; }
        public string Estado { get; set; }
        public string Cidade { get; set; }
        // public string Email { get; set; }
    }
}