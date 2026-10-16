using Plataforma.Fases;
using Plataforma.Visual;
using UnityEngine;

namespace Plataforma.Jogo
{
    public enum TipoDeItem
    {
        Medalha,
        Metralhadora,
        Granadas,
        Vida
    }

    [RequireComponent(typeof(SpriteRenderer), typeof(AnimadorDeSprite), typeof(CircleCollider2D))]
    [RequireComponent(typeof(Rigidbody2D))]
    public class Item : MonoBehaviour
    {
        public const int OrdemDeDesenho = 5;

        private Vector3 posicaoBase;
        private float defasagem;
        private bool coletado;

        public TipoDeItem Tipo { get; private set; }

        public void Configurar(TipoDeItem tipo)
        {
            Tipo = tipo;
            posicaoBase = transform.position;
            defasagem = transform.position.x * 0.7f;

            Rigidbody2D corpo = GetComponent<Rigidbody2D>();
            corpo.bodyType = RigidbodyType2D.Kinematic;

            CircleCollider2D gatilho = GetComponent<CircleCollider2D>();
            gatilho.isTrigger = true;
            gatilho.radius = 0.35f;

            GetComponent<SpriteRenderer>().sortingOrder = OrdemDeDesenho;
            transform.localScale = tipo == TipoDeItem.Medalha ? Vector3.one : Vector3.one * 1.4f;

            AnimadorDeSprite animador = GetComponent<AnimadorDeSprite>();
            switch (tipo)
            {
                case TipoDeItem.Medalha:
                    animador.Tocar(ArteDoJogo.Medalha, 10f);
                    break;
                case TipoDeItem.Metralhadora:
                    animador.Tocar(ArteDoJogo.CaixaMetralhadora, 3f);
                    break;
                case TipoDeItem.Granadas:
                    animador.Tocar(ArteDoJogo.CaixaGranadas, 3f);
                    break;
                default:
                    animador.Tocar(ArteDoJogo.KitMedico, 3f);
                    break;
            }
            animador.Embaralhar();
        }

        private void Update()
        {
            float balanco = Mathf.Sin(Time.time * 3f + defasagem) * 0.08f;
            transform.position = posicaoBase + new Vector3(0f, balanco, 0f);
        }

        private void OnTriggerEnter2D(Collider2D outro)
        {
            if (coletado)
            {
                return;
            }

            Jogador jogador = outro.GetComponent<Jogador>();
            if (jogador == null || !jogador.EstaVivo)
            {
                return;
            }

            coletado = true;
            Efeito.Criar(ArteDoJogo.Brilho, transform.position, 16f, OrdemDeDesenho + 10, 1.5f);

            GerenciadorDaFase.Instancia.ItemColetado(Tipo);

            Destroy(gameObject);
        }
    }
}
