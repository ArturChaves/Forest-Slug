using UnityEngine;
using UnityEngine.SceneManagement;

namespace Plataforma.Nucleo
{
    public static class Cenas
    {
        private static int ultimoQuadroCarregado = -1;

        public static void CarregarMenu()
        {
            Carregar("Menu");
        }

        public static void CarregarFase(int fase)
        {
            Carregar("Fase" + fase);
        }

        public static void CarregarFim()
        {
            Carregar("Fim");
        }

        public static void NovoJogo()
        {
            SessaoDeJogo.NovoJogo();
            CarregarFase(1);
        }

        private static void Carregar(string nomeDaCena)
        {
            if (Time.frameCount == ultimoQuadroCarregado)
            {
                return;
            }
            ultimoQuadroCarregado = Time.frameCount;
            Time.timeScale = 1f;
            SceneManager.LoadScene(nomeDaCena);
        }
    }
}
