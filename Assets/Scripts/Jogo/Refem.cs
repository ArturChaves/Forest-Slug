using Plataforma.Fases;
using Plataforma.Visual;
using UnityEngine;

namespace Plataforma.Jogo
{
    [RequireComponent(typeof(BoxCollider2D), typeof(SpriteRenderer), typeof(AnimadorDeSprite))]
    public class Refem : MonoBehaviour
    {
        public const int OrdemDeDesenho = 6;

        private AnimadorDeSprite animador;
        private SpriteRenderer desenho;
        private bool resgatado;
        private float tempo;

        private void Awake()
        {
            animador = GetComponent<AnimadorDeSprite>();
            desenho = GetComponent<SpriteRenderer>();
            desenho.sortingOrder = OrdemDeDesenho;

            BoxCollider2D gatilho = GetComponent<BoxCollider2D>();
            gatilho.isTrigger = true;
            gatilho.size = new Vector2(0.8f, 0.9f);

            animador.Tocar(ArteDoJogo.RefemAmarrado, 3f);
            animador.Embaralhar();
        }

        private void OnTriggerEnter2D(Collider2D outro)
        {
            if (resgatado)
            {
                return;
            }

            Jogador jogador = outro.GetComponent<Jogador>();
            if (jogador == null || !jogador.EstaVivo)
            {
                return;
            }

            resgatado = true;
            GetComponent<BoxCollider2D>().enabled = false;
            animador.Tocar(ArteDoJogo.RefemContinencia, 1f);
            Efeito.Criar(ArteDoJogo.Brilho, transform.position + Vector3.up * 0.4f, 12f, OrdemDeDesenho + 10, 2f);

            GerenciadorDaFase.Instancia.RefemResgatado(this);
        }

        private void Update()
        {
            if (!resgatado)
            {
                return;
            }

            tempo += Time.deltaTime;
            if (tempo > 0.8f)
            {
                animador.Tocar(ArteDoJogo.RefemCorrendo, 10f);
                desenho.flipX = true;
                transform.position += Vector3.left * 3.5f * Time.deltaTime;
                desenho.color = new Color(1f, 1f, 1f, Mathf.Clamp01(3f - tempo));
                if (tempo > 3f)
                {
                    Destroy(gameObject);
                }
            }
        }
    }
}
