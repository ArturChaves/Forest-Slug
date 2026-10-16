using UnityEngine;

namespace Plataforma.Visual
{
    public static class ConstrutorDeFundo
    {
        private const float PixelsPorUnidadeDoFundo = 20f;

        public static readonly Color CorDoCeu = new Color(38f / 255f, 40f / 255f, 52f / 255f, 1f);

        public static void Construir(Camera cam, Color tom)
        {
            GameObject raiz = new GameObject("Fundo (Parallax)");

            Vector2 pivoBase = new Vector2(0.5f, 0f);
            CriarCamada(raiz.transform, cam, "Ceu", "Backgrounds/Ceu", 0.92f, -0.4f, -100, pivoBase, tom);
            CriarCamada(raiz.transform, cam, "Ruinas", "Backgrounds/Montanhas", 0.75f, -0.4f, -90, pivoBase, tom);
            CriarCamada(raiz.transform, cam, "Mata", "Backgrounds/Colinas", 0.5f, -1.2f, -80, pivoBase, tom);

            cam.backgroundColor = CorDoCeu * tom;
        }

        private static void CriarCamada(Transform pai, Camera cam, string nome, string caminho,
            float acompanhamento, float deslocamentoBase, int ordemDeDesenho, Vector2 pivo, Color tom)
        {
            Sprite sprite = FolhaDeSprites.CarregarImagem(caminho, PixelsPorUnidadeDoFundo, pivo);
            GameObject objetoCamada = new GameObject(nome);
            objetoCamada.transform.SetParent(pai, false);
            objetoCamada.AddComponent<SpriteRenderer>();
            CamadaParallax camada = objetoCamada.AddComponent<CamadaParallax>();
            camada.Configurar(cam, sprite, acompanhamento, deslocamentoBase, ordemDeDesenho, tom);
        }
    }
}
