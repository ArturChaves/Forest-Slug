using UnityEngine;

namespace Plataforma.Visual
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class Efeito : MonoBehaviour
    {
        private AnimadorDeSprite animador;

        public static Efeito Criar(Sprite[] quadros, Vector3 posicao, float fps, int ordemDeDesenho, float escala = 1f)
        {
            if (quadros == null || quadros.Length == 0)
            {
                return null;
            }

            GameObject objetoEfeito = new GameObject("Efeito");
            objetoEfeito.transform.position = posicao;
            objetoEfeito.transform.localScale = Vector3.one * escala;

            SpriteRenderer desenho = objetoEfeito.AddComponent<SpriteRenderer>();
            desenho.sortingOrder = ordemDeDesenho;

            Efeito efeito = objetoEfeito.AddComponent<Efeito>();
            efeito.animador = objetoEfeito.AddComponent<AnimadorDeSprite>();
            efeito.animador.Tocar(quadros, fps, false);
            return efeito;
        }

        private void Update()
        {
            if (animador.Terminou)
            {
                Destroy(gameObject);
            }
        }
    }
}
