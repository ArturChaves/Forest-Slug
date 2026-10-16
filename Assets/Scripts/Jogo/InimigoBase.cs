using Plataforma.Fases;
using UnityEngine;

namespace Plataforma.Jogo
{
    [RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D), typeof(SpriteRenderer))]
    public abstract class InimigoBase : MonoBehaviour, IAtingivel
    {
        public const int OrdemDeDesenho = 8;

        protected Rigidbody2D corpo;
        protected BoxCollider2D areaDeAcerto;
        protected SpriteRenderer desenho;
        protected int vida = 2;
        protected bool morto;
        protected bool mortoPorExplosao;
        protected Vector2 direcaoDoTiro = Vector2.right;
        private float tempoPiscar;

        protected virtual bool Sangra => true;

        public Equipe Equipe => Equipe.Inimigo;
        public bool EstaVivo => !morto;
        protected abstract int Pontos { get; }

        protected virtual void Awake()
        {
            corpo = GetComponent<Rigidbody2D>();
            areaDeAcerto = GetComponent<BoxCollider2D>();
            desenho = GetComponent<SpriteRenderer>();

            corpo.bodyType = RigidbodyType2D.Kinematic;
            corpo.interpolation = RigidbodyInterpolation2D.Interpolate;
            areaDeAcerto.isTrigger = true;
            desenho.sortingOrder = OrdemDeDesenho;
        }

        protected virtual void Update()
        {
            if (tempoPiscar > 0f)
            {
                tempoPiscar -= Time.deltaTime;
                desenho.color = tempoPiscar > 0f ? new Color(1f, 0.45f, 0.45f, 1f) : Color.white;
            }
        }

        public void ReceberDano(int quantidade, Vector2 origemDano)
        {
            if (morto)
            {
                return;
            }

            vida -= quantidade;
            tempoPiscar = 0.08f;
            direcaoDoTiro = (Vector2)transform.position - origemDano;
            if (Sangra)
            {
                Sangue.Acerto(transform.position, direcaoDoTiro);
            }
            AoSerAtingido();

            if (vida > 0)
            {
                return;
            }

            morto = true;
            areaDeAcerto.enabled = false;
            desenho.color = Color.white;
            mortoPorExplosao = quantidade >= 3;
            if (Sangra && mortoPorExplosao)
            {
                Sangue.Estouro(transform.position);
            }
            else if (Sangra)
            {
                Sangue.Morte(transform.position, direcaoDoTiro);
            }
            GerenciadorDaFase.Instancia.InimigoDerrotado(Pontos);
            AoSerDerrotado();
        }

        protected virtual void AoSerAtingido()
        {
        }

        protected abstract void AoSerDerrotado();

        protected static Jogador JogadorAlvo()
        {
            Jogador jogador = GerenciadorDaFase.Instancia.Jogador;
            return jogador.EstaVivo ? jogador : null;
        }

        protected static bool NaTela(Vector2 posicao)
        {
            return GerenciadorDaFase.Instancia.EstaNaTela(posicao, 0.5f);
        }
    }
}
