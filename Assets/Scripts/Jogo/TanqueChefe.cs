using System.Collections;
using Plataforma.Fases;
using Plataforma.Visual;
using UnityEngine;

namespace Plataforma.Jogo
{
    [RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D), typeof(SpriteRenderer))]
    [RequireComponent(typeof(AnimadorDeSprite))]
    public class TanqueChefe : MonoBehaviour, IAtingivel
    {
        public const int VidaMaxima = 70;
        public const int Pontos = 3000;

        private Rigidbody2D corpo;
        private BoxCollider2D casco;
        private SpriteRenderer desenho;
        private AnimadorDeSprite animador;
        private Vector2 origem;
        private int vida = VidaMaxima;
        private bool morto;
        private bool ativado;
        private float tempoTiro = 2.5f;
        private float tempoAviso;
        private int tirosDisparados;
        private float tempoRajada = 3f;
        private int tirosRestantes;
        private bool furioso;
        private float tempoPiscar;
        private float lado = -1f;

        public Equipe Equipe => Equipe.Inimigo;
        public bool EstaVivo => !morto;
        public float FracaoDeVida => Mathf.Clamp01(vida / (float)VidaMaxima);

        private void Awake()
        {
            corpo = GetComponent<Rigidbody2D>();
            casco = GetComponent<BoxCollider2D>();
            desenho = GetComponent<SpriteRenderer>();
            animador = GetComponent<AnimadorDeSprite>();

            corpo.bodyType = RigidbodyType2D.Kinematic;
            corpo.interpolation = RigidbodyInterpolation2D.Interpolate;
            casco.isTrigger = false;
            casco.size = new Vector2(2.7f, 1.45f);
            casco.offset = new Vector2(0.05f, -0.25f);
            desenho.sortingOrder = InimigoBase.OrdemDeDesenho - 1;
            animador.Tocar(ArteDoJogo.TanqueAndando, 8f);
            origem = transform.position;
        }

        private void Update()
        {
            if (tempoPiscar > 0f)
            {
                tempoPiscar -= Time.deltaTime;
                desenho.color = tempoPiscar > 0f ? new Color(1f, 0.5f, 0.5f, 1f) : Color.white;
            }
            else if (tempoAviso > 0f)
            {
                bool aceso = Mathf.Repeat(Time.time * 10f, 1f) < 0.5f;
                desenho.color = aceso ? new Color(1f, 0.9f, 0.4f, 1f) : Color.white;
            }
            else if (!morto)
            {
                desenho.color = Color.white;
            }
        }

        private void FixedUpdate()
        {
            if (morto)
            {
                return;
            }

            GerenciadorDaFase fase = GerenciadorDaFase.Instancia;
            Jogador jogador = fase.Jogador;

            if (!ativado)
            {
                if (!fase.EstaNaTela(corpo.position, -1f))
                {
                    return;
                }
                ativado = true;
                fase.ChefeApareceu(this);
            }

            float dt = Time.fixedDeltaTime;
            float ritmo = furioso ? 1.2f : 1f;
            float x = origem.x + Mathf.Sin(Time.time * (furioso ? 0.7f : 0.5f)) * 3f;
            corpo.MovePosition(new Vector2(x, origem.y));

            if (jogador == null || !jogador.EstaVivo)
            {
                return;
            }

            lado = jogador.transform.position.x < corpo.position.x ? -1f : 1f;
            desenho.flipX = lado > 0f;

            tempoTiro -= dt * ritmo;
            if (tempoTiro <= 0f)
            {
                Atirar(jogador);
                tempoTiro = 3.2f;
            }

            tempoRajada -= dt * ritmo;
            tempoAviso = Mathf.Max(0f, tempoAviso - dt);
            if (tirosRestantes == 0 && tempoRajada <= 0f)
            {
                tirosRestantes = 3;
                tempoAviso = 0.6f;
                tempoRajada = 0.6f;
            }
            else if (tirosRestantes > 0 && tempoRajada <= 0f)
            {
                Metralhar();
                tirosRestantes--;
                tempoRajada = tirosRestantes > 0 ? 0.2f : 3.5f;
            }

            Bounds limitesTanque = casco.bounds;
            limitesTanque.Expand(0.15f);
            if (limitesTanque.Intersects(new Bounds(jogador.transform.position, new Vector3(0.5f, 0.8f, 1f))))
            {
                jogador.ReceberDano(1, corpo.position);
            }
        }

        private void Atirar(Jogador jogador)
        {
            tirosDisparados++;

            Vector2 ponta = corpo.position + new Vector2(lado * 1.5f, 0.31f);
            Efeito.Criar(ArteDoJogo.Explosao, ponta, 30f, Explosao.OrdemDeDesenho, 0.5f);

            if (tirosDisparados % 2 == 0)
            {
                const float tempoDeVoo = 1.7f;
                const float gravidade = 10f;
                Vector2 alvo = jogador.transform.position;
                Vector2 velocidade = new Vector2((alvo.x - ponta.x) / tempoDeVoo,
                    (alvo.y - ponta.y) / tempoDeVoo + 0.5f * gravidade * tempoDeVoo);
                Projetil.Disparar(ArteDoJogo.Obus, ponta, velocidade, Equipe.Inimigo, 1, 4f, gravidade, 1f);
            }
            else
            {
                Vector2 baixo = corpo.position + new Vector2(lado * 1.5f, -0.4f);
                Projetil.Disparar(ArteDoJogo.Obus, baixo, new Vector2(lado * 4.5f, 0f), Equipe.Inimigo, 1, 5f);
            }
        }

        private void Metralhar()
        {
            Vector2 ponta = corpo.position + new Vector2(lado * 1.45f, -0.45f);
            Projetil.Disparar(ArteDoJogo.BalaInimiga, ponta, new Vector2(lado * 6f, 0f), Equipe.Inimigo, 1, 3f);
            Efeito.Criar(ArteDoJogo.ClaraoDoTiro, ponta, 30f, Explosao.OrdemDeDesenho);
        }

        public void ReceberDano(int quantidade, Vector2 origemDano)
        {
            if (morto)
            {
                return;
            }

            vida -= quantidade;
            tempoPiscar = 0.06f;

            GerenciadorDaFase fase = GerenciadorDaFase.Instancia;
            if (!ativado)
            {
                ativado = true;
                fase.ChefeApareceu(this);
            }

            if (!furioso && vida <= VidaMaxima / 2 && vida > 0)
            {
                furioso = true;
                fase.ChefeFurioso(this);
            }

            if (vida <= 0)
            {
                morto = true;
                StartCoroutine(SequenciaDeDestruicao());
            }
        }

        private IEnumerator SequenciaDeDestruicao()
        {
            animador.Tocar(ArteDoJogo.TanqueAndando, 0.01f);
            for (int i = 0; i < 8; i++)
            {
                Vector2 deslocamento = new Vector2(Random.Range(-1.3f, 1.3f), Random.Range(-0.5f, 0.8f));
                Explosao.Criar(corpo.position + deslocamento, 0f, Equipe.Inimigo, 0, Random.Range(0.8f, 1.4f));
                yield return new WaitForSeconds(0.2f);
            }

            Explosao.Criar(corpo.position, 0f, Equipe.Inimigo, 0, 2.2f);
            animador.Tocar(ArteDoJogo.TanqueDestruido, 1f, false);
            desenho.color = Color.white;
            casco.enabled = false;

            GerenciadorDaFase.Instancia.ChefeDerrotado(this);
        }
    }
}
