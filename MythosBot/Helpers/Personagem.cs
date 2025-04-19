
namespace MythosBot
{
    public class Personagem
    {
        public int Relação { get; set; }
        public string Nome { get; set; }
        public int Idade { get; set; }
        public string HistoriaCurta { get; set; }
        public string? HistoriaLonga { get; set; }
        public string Personalidade { get; set; }

        public Característica[] Características { get; set; }

        internal static string ValorParaRelação(int relação)
        {
            switch (relação)
            {
                case -1:
                    return "Inimigo";
                case 1:
                    return "Amigo";
                case 0:
                default:
                    return "Neutro";
            }
        }
    }

    public class Característica
    {
        public string Nome { get; set; }
        public int Valor { get; set; }
    }
}