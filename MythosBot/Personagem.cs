

using System.Reflection;

namespace MythosBot
{
    public class Personagem
    {
        public long Id { get; set; }
        public ulong Autor { get; set; }
        public string Nome { get; set; }
        [Editavel]
        public int Idade { get; set; }
        [Editavel]
        public decimal Altura { get; set; }
        [Editavel]
        public string Historia { get; set; }
        [Editavel]
        public string Personalidade { get; set; }

        [Editavel]
        public int HP { get; set; }
        [Editavel]
        public int PontosDeEsforço { get; set; }
        [Editavel]
        public int Sanidade { get; set; }
        [Editavel]
        public int Defesa { get; set; }
        [Editavel]
        public int Fome { get; set; }
        [Editavel]
        public int Inventário { get; set; }
        [Editavel]
        public int Medo { get; set; }

        [Editavel]
        public int Força { get; set; }
        [Editavel]
        public int Agilidade { get; set; }
        [Editavel]
        public int Constituição { get; set; }
        [Editavel]
        public int Inteligência { get; set; }
        [Editavel]
        public int Sabedoria { get; set; }
        [Editavel]
        public int Carisma { get; set; }

        [Editavel]
        public int Coragem { get; set; }
        [Editavel]
        public int Psique { get; set; }
        [Editavel]
        public int Acrobacia { get; set; }
        [Editavel]
        public int Iniciativa { get; set; }
        [Editavel]
        public int Medicina { get; set; }
        [Editavel]
        public int Pilotar { get; set; }

        [Editavel]
        public int Diplomacia { get; set; }
        [Editavel]
        public int Sorte { get; set; }
        [Editavel]
        public int Investigação { get; set; }
        [Editavel]
        public int Lutar { get; set; }
        [Editavel]
        public int Arrombamento { get; set; }
        [Editavel]
        public int Tecnologia { get; set; }

        [Editavel]
        public int Pontaria { get; set; }
        [Editavel]
        public int Furtividade { get; set; }
        [Editavel]
        public int Intimidar { get; set; }
        [Editavel]
        public int Arremessar { get; set; }
        [Editavel]
        public int Criação { get; set; }
        [Editavel]
        public int Atualidades { get; set; }

        internal string AtributosDisponiveis()
        {
            var propriedadesEditaveis = this.GetType()
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(prop => Attribute.IsDefined(prop, typeof(EditavelAttribute)))
                .Select(prop => prop.Name);

            return "Atributos disponíveis para edição:\n" + string.Join("\n", propriedadesEditaveis).Replace("PontosDeEsforço", "Pontos de Esforço");
        }
    }

}

[AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
public sealed class EditavelAttribute : Attribute;