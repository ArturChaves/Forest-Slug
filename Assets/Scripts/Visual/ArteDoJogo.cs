using UnityEngine;

namespace Plataforma.Visual
{
    public static class ArteDoJogo
    {
        public static Sprite[] PernasParado => FolhaDeSprites.Recortar(FolhaPernas, 0, 1);
        public static Sprite[] PernasCorrendo => FolhaDeSprites.Recortar(FolhaPernas, 1, 4);
        public static Sprite[] PernasPulo => FolhaDeSprites.Recortar(FolhaPernas, 5, 1);

        public static Sprite TroncoFrente(bool balanco) => Quadro(FolhaTronco, balanco ? 1 : 0);
        public static Sprite TroncoCima(bool balanco) => Quadro(FolhaTronco, balanco ? 3 : 2);
        public static Sprite TroncoBaixo => Quadro(FolhaTronco, 4);
        public static Sprite JogadorFerido => Quadro(FolhaTronco, 5);

        public static Sprite[] InimigoAndando => FolhaDeSprites.Recortar(FolhaInimigo, 0, 4);
        public static Sprite[] InimigoMirando => FolhaDeSprites.Recortar(FolhaInimigo, 4, 1);
        public static Sprite[] InimigoAtirando => FolhaDeSprites.Recortar(FolhaInimigo, 5, 1);
        public static Sprite[] InimigoMorrendo => FolhaDeSprites.Recortar(FolhaInimigo, 6, 3);

        public static Sprite[] DroneVoando => FolhaDeSprites.Recortar(FolhaDrone, 0, 3);
        public static Sprite[] DroneDestruido => FolhaDeSprites.Recortar(FolhaDrone, 3, 1);

        public static Sprite[] RefemAmarrado => FolhaDeSprites.Recortar(FolhaRefem, 0, 2);
        public static Sprite[] RefemContinencia => FolhaDeSprites.Recortar(FolhaRefem, 2, 1);
        public static Sprite[] RefemCorrendo => FolhaDeSprites.Recortar(FolhaRefem, 3, 2);

        public static Sprite[] TanqueAndando => FolhaDeSprites.Recortar(FolhaTanque, 0, 2);
        public static Sprite[] TanqueDestruido => FolhaDeSprites.Recortar(FolhaTanque, 2, 1);

        public static Sprite[] Bala => FolhaDeSprites.Recortar(FolhaProjeteis, 0, 1);
        public static Sprite[] BalaPesada => FolhaDeSprites.Recortar(FolhaProjeteis, 1, 1);
        public static Sprite[] BalaInimiga => FolhaDeSprites.Recortar(FolhaProjeteis, 2, 1);
        public static Sprite[] Obus => FolhaDeSprites.Recortar(FolhaProjeteis, 3, 1);
        public static Sprite[] SpriteGranada => FolhaDeSprites.Recortar(FolhaProjeteis, 4, 1);
        public static Sprite[] Bomba => FolhaDeSprites.Recortar(FolhaProjeteis, 5, 1);

        public static Sprite[] Explosao => FolhaDeSprites.Carregar("Sprites/Explosao", 48, 48);
        public static Sprite[] ClaraoDoTiro => FolhaDeSprites.Carregar("Sprites/Clarao", 16, 16);

        public static Sprite[] CaixaMetralhadora => FolhaDeSprites.Recortar(FolhaCaixas, 0, 2);
        public static Sprite[] CaixaGranadas => FolhaDeSprites.Recortar(FolhaCaixas, 2, 2);

        public static Sprite[] Medalha => FolhaDeSprites.Carregar("Sprites/Medalha", 16, 16);
        public static Sprite[] KitMedico => FolhaDeSprites.Carregar("Sprites/KitMedico", 16, 16);
        public static Sprite[] Brilho => FolhaDeSprites.Carregar("Sprites/Brilho", 16, 16);
        public static Sprite IconeVida => PrimeiroQuadro(FolhaDeSprites.Carregar("Sprites/IconeVida", 16, 16));

        public static Sprite[] RespingoDeSangue => FolhaDeSprites.Carregar("Sprites/Sangue", 16, 16);
        public static Sprite[] PocasDeSangue => FolhaDeSprites.Carregar("Sprites/Poca", 32, 16);
        public static Sprite[] GotasDeSangue => FolhaDeSprites.Carregar("Sprites/Gotas", 8, 8);

        public static Sprite PedacoCabeca => Quadro(FolhaPedacos, 0);
        public static Sprite PedacoBraco => Quadro(FolhaPedacos, 1);
        public static Sprite PedacoPerna => Quadro(FolhaPedacos, 2);
        public static Sprite PedacoTronco => Quadro(FolhaPedacos, 3);
        public static Sprite PedacoCarne => Quadro(FolhaPedacos, 4);
        public static Sprite PedacoCarnePequeno => Quadro(FolhaPedacos, 5);
        private static Sprite[] FolhaPedacos => FolhaDeSprites.Carregar("Sprites/Pedacos", 16, 16);

        private static Sprite[] FolhaPernas => FolhaDeSprites.Carregar("Sprites/SoldadoPernas", 32, 32);
        private static Sprite[] FolhaTronco => FolhaDeSprites.Carregar("Sprites/SoldadoTronco", 32, 32);
        private static Sprite[] FolhaInimigo => FolhaDeSprites.Carregar("Sprites/Inimigo", 32, 32);
        private static Sprite[] FolhaDrone => FolhaDeSprites.Carregar("Sprites/Drone", 32, 32);
        private static Sprite[] FolhaRefem => FolhaDeSprites.Carregar("Sprites/Refem", 32, 32);
        private static Sprite[] FolhaTanque => FolhaDeSprites.Carregar("Sprites/Tanque", 96, 64);
        private static Sprite[] FolhaProjeteis => FolhaDeSprites.Carregar("Sprites/Projeteis", 16, 16);
        private static Sprite[] FolhaCaixas => FolhaDeSprites.Carregar("Sprites/Caixas", 16, 16);

        public static Sprite PrimeiroQuadro(Sprite[] quadros)
        {
            return quadros != null && quadros.Length > 0 ? quadros[0] : null;
        }

        private static Sprite Quadro(Sprite[] quadros, int indice)
        {
            return quadros != null && indice >= 0 && indice < quadros.Length ? quadros[indice] : null;
        }
    }
}
