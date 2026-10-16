using UnityEngine;

namespace Plataforma.Nucleo
{
    public enum ResultadoDoJogo
    {
        Nenhum,
        Vitoria,
        Derrota
    }

    public static class SessaoDeJogo
    {
        public const int VidasIniciais = 3;
        public const int VidasMaximas = 5;
        public const int QuantidadeDeFases = 2;

        public static int Pontos;
        public static int Vidas = VidasIniciais;
        public static int Medalhas;
        public static int Refens;
        public static int InimigosDerrotados;
        public static int UltimaFase = 1;
        public static ResultadoDoJogo Resultado;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        public static void NovoJogo()
        {
            Pontos = 0;
            Vidas = VidasIniciais;
            Medalhas = 0;
            Refens = 0;
            InimigosDerrotados = 0;
            UltimaFase = 1;
            Resultado = ResultadoDoJogo.Nenhum;
        }

        public static bool GanharVida()
        {
            if (Vidas >= VidasMaximas)
            {
                return false;
            }
            Vidas++;
            return true;
        }

        public static int PerderVida()
        {
            Vidas = Mathf.Max(0, Vidas - 1);
            return Vidas;
        }
    }
}
