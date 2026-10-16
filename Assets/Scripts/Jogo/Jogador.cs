using Plataforma.Nucleo;
using Plataforma.Fases;
using Plataforma.Visual;
using UnityEngine;

namespace Plataforma.Jogo
{
    [RequireComponent(typeof(Rigidbody2D), typeof(CapsuleCollider2D))]
    public class Jogador : MonoBehaviour, IAtingivel
    {
        public enum Arma
        {
            Pistola = 0,
            Metralhadora = 1
        }
        private const float VelocidadeBala = 18f;
        private const float EsperaPistola = 0.16f;
        private const float IntervaloPistolaSegurada = 0.28f;
        private const float IntervaloMetralhadora = 0.075f;
        private const int MunicaoPorCaixa = 150;
        private const int GranadasIniciais = 10;
        private const float VelocidadeAndar = 5.5f;
        private const float AceleracaoNoChao = 70f;
        private const float AceleracaoNoAr = 45f;
        private const float VelocidadePulo = 14f;
        private const float EscalaGravidade = 3f;
        private const float MultiplicadorQueda = 1.5f;
        private const float QuedaMaxima = 18f;
        private const float TempoCoiote = 0.1f;
        private const float TempoBufferPulo = 0.12f;
        private const float MultiplicadorCortePulo = 0.5f;
        private static readonly Vector2 EmpurraoAoFerir = new Vector2(5f, 8f);
        private const float TravaAoFerir = 0.35f;
        private const float DuracaoInvencivel = 1.6f;

        private const float NormalMinimaDoChao = 0.55f;
        private const float DistanciaGrudarNoChao = 0.35f;

        private Rigidbody2D corpo;
        private CapsuleCollider2D capsula;
        private SpriteRenderer desenhoPernas;
        private SpriteRenderer desenhoTronco;
        private AnimadorDeSprite animadorPernas;
        private int lado = 1;
        private Vector2 mira = Vector2.right;
        private float esperaTiro;
        private float tempoPiscar;

        private readonly ContactPoint2D[] contatos = new ContactPoint2D[16];
        private readonly RaycastHit2D[] acertosRaio = new RaycastHit2D[8];

        private float entradaHorizontal;
        private bool puloSegurado;
        private float ultimoTempoPulo = -10f;
        private float ultimoTempoNoChao = -10f;
        private bool noChao;
        private bool estavaNoChao;
        private Vector2 normalDoChao = Vector2.up;
        private float velocidadeNoChao;
        private bool subindo;
        private bool puloCortado;
        private float tempoFerido;
        private float tempoInvencivel;
        private bool congelado;

        public bool EstaMorto { get; private set; }
        public Arma ArmaAtual { get; private set; } = Arma.Pistola;
        public int Municao { get; private set; }
        public int Granadas { get; private set; }
        public Equipe Equipe => Equipe.Jogador;
        public bool EstaVivo => !EstaMorto;
        public bool EstaInvencivel => tempoInvencivel > 0f || EstaMorto;

        private void Awake()
        {
            corpo = GetComponent<Rigidbody2D>();
            capsula = GetComponent<CapsuleCollider2D>();
            desenhoPernas = CriarParte("Pernas", 10);
            animadorPernas = desenhoPernas.gameObject.AddComponent<AnimadorDeSprite>();
            desenhoTronco = CriarParte("Tronco", 11);
            Granadas = GranadasIniciais;

            corpo.bodyType = RigidbodyType2D.Dynamic;
            corpo.gravityScale = EscalaGravidade;
            corpo.freezeRotation = true;
            corpo.interpolation = RigidbodyInterpolation2D.Interpolate;
            corpo.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

            PhysicsMaterial2D semAtrito = new PhysicsMaterial2D("SemAtrito") { friction = 0f, bounciness = 0f };
            corpo.sharedMaterial = semAtrito;
            capsula.sharedMaterial = semAtrito;

            capsula.direction = CapsuleDirection2D.Vertical;
            capsula.size = new Vector2(0.5f, 0.82f);
            capsula.offset = new Vector2(0f, -0.07f);

            animadorPernas.Tocar(ArteDoJogo.PernasParado, 1f);
            desenhoTronco.sprite = ArteDoJogo.TroncoFrente(false);
        }

        private SpriteRenderer CriarParte(string nomeParte, int ordemDeDesenho)
        {
            GameObject parte = new GameObject(nomeParte);
            parte.transform.SetParent(transform, false);
            SpriteRenderer desenhoParte = parte.AddComponent<SpriteRenderer>();
            desenhoParte.sortingOrder = ordemDeDesenho;
            return desenhoParte;
        }

        private void Update()
        {
            if (congelado || EstaMorto || Time.timeScale <= 0f)
            {
                entradaHorizontal = 0f;
                AtualizarAnimacao();
                return;
            }

            entradaHorizontal = Controles.Horizontal;
            puloSegurado = Controles.PuloSegurado;
            if (Controles.PuloApertado)
            {
                ultimoTempoPulo = Time.time;
            }

            if (entradaHorizontal > 0.1f)
            {
                lado = 1;
            }
            else if (entradaHorizontal < -0.1f)
            {
                lado = -1;
            }

            bool noAr = Time.time - ultimoTempoNoChao > 0.08f;
            if (Controles.MirarCima)
            {
                mira = Vector2.up;
            }
            else if (noAr && Controles.MirarBaixo)
            {
                mira = Vector2.down;
            }
            else
            {
                mira = new Vector2(lado, 0f);
            }

            AtualizarTiro();
            AtualizarAnimacao();
        }

        private void AtualizarTiro()
        {
            esperaTiro -= Time.deltaTime;
            tempoPiscar -= Time.deltaTime;

            if (tempoFerido > 0f)
            {
                return;
            }

            if (ArmaAtual == Arma.Metralhadora)
            {
                if (Controles.AtirarSegurado && esperaTiro <= 0f)
                {
                    float espalhamento = Random.Range(-3f, 3f);
                    Vector2 direcao = Quaternion.Euler(0f, 0f, espalhamento) * mira;
                    Atirar(direcao, ArteDoJogo.BalaPesada);
                    esperaTiro = IntervaloMetralhadora;
                    Municao--;
                    if (Municao <= 0)
                    {
                        ArmaAtual = Arma.Pistola;
                        Municao = 0;
                    }
                }
            }
            else
            {
                bool podeAtirar = (Controles.AtirarApertado && esperaTiro <= IntervaloPistolaSegurada - EsperaPistola)
                                  || (Controles.AtirarSegurado && esperaTiro <= 0f);
                if (podeAtirar)
                {
                    Atirar(mira, ArteDoJogo.Bala);
                    esperaTiro = IntervaloPistolaSegurada;
                }
            }

            if (Controles.GranadaApertada && Granadas > 0)
            {
                Granadas--;
                Vector2 velocidadeLancamento = new Vector2(lado * 5.5f + corpo.linearVelocity.x * 0.4f, 7.5f);
                Granada.Lancar((Vector2)transform.position + new Vector2(lado * 0.3f, 0.3f), velocidadeLancamento,
                    Equipe.Jogador, capsula);
            }
        }

        private void Atirar(Vector2 direcao, Sprite[] quadrosBala)
        {
            Vector2 ponta = PontaDaArma();
            Projetil.Disparar(quadrosBala, ponta, direcao.normalized * VelocidadeBala, Equipe.Jogador, 1, 0.9f);

            Efeito clarao = Efeito.Criar(ArteDoJogo.ClaraoDoTiro, ponta, 30f, 12);
            if (clarao != null)
            {
                clarao.transform.rotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(direcao.y, direcao.x) * Mathf.Rad2Deg);
            }
            tempoPiscar = 0.06f;
        }

        private Vector2 PontaDaArma()
        {
            Vector2 local;
            if (mira.y > 0.5f)
            {
                local = new Vector2(0.16f, 0.5f);
            }
            else if (mira.y < -0.5f)
            {
                local = new Vector2(0.09f, -0.47f);
            }
            else
            {
                local = new Vector2(0.44f, 0.03f);
            }
            local.x *= lado;
            return (Vector2)transform.position + local;
        }

        public void DarMetralhadora()
        {
            ArmaAtual = Arma.Metralhadora;
            Municao = MunicaoPorCaixa;
        }

        public void AdicionarGranadas(int quantidade)
        {
            Granadas = Mathf.Min(99, Granadas + quantidade);
        }

        public void ReceberDano(int quantidade, Vector2 origemDano)
        {
            Ferir(origemDano.x);
        }

        private void FixedUpdate()
        {
            float dt = Time.fixedDeltaTime;
            if (tempoInvencivel > 0f)
            {
                tempoInvencivel -= dt;
            }

            if (EstaMorto)
            {
                return;
            }

            if (congelado)
            {
                corpo.gravityScale = EscalaGravidade;
                Vector2 v = corpo.linearVelocity;
                corpo.linearVelocity = new Vector2(Mathf.MoveTowards(v.x, 0f, AceleracaoNoChao * dt), v.y);
                return;
            }

            AtualizarChao();

            if (tempoFerido > 0f)
            {
                tempoFerido -= dt;
                corpo.gravityScale = EscalaGravidade;
                return;
            }

            Vector2 velocidade = corpo.linearVelocity;
            float velocidadeAlvo = entradaHorizontal * VelocidadeAndar;

            bool podePular = Time.time - ultimoTempoPulo <= TempoBufferPulo
                           && Time.time - ultimoTempoNoChao <= TempoCoiote
                           && !subindo;

            if (podePular)
            {
                velocidade = new Vector2(velocidade.x, VelocidadePulo);
                subindo = true;
                puloCortado = false;
                noChao = false;
                ultimoTempoPulo = -10f;
                ultimoTempoNoChao = -10f;
                corpo.gravityScale = EscalaGravidade;
            }
            else if (noChao)
            {
                velocidadeNoChao = Mathf.MoveTowards(velocidadeNoChao, velocidadeAlvo, AceleracaoNoChao * dt);
                Vector2 tangente = new Vector2(normalDoChao.y, -normalDoChao.x);
                velocidade = tangente * velocidadeNoChao - normalDoChao * 1.5f;
                corpo.gravityScale = 0f;
            }
            else
            {
                float vx = Mathf.MoveTowards(velocidade.x, velocidadeAlvo, AceleracaoNoAr * dt);
                float vy = Mathf.Max(velocidade.y, -QuedaMaxima);

                if (subindo && !puloCortado && !puloSegurado && vy > 0f)
                {
                    vy *= MultiplicadorCortePulo;
                    puloCortado = true;
                }

                velocidade = new Vector2(vx, vy);
                velocidadeNoChao = vx;
                corpo.gravityScale = vy < 0f ? EscalaGravidade * MultiplicadorQueda : EscalaGravidade;
            }

            if (subindo && velocidade.y <= 0f)
            {
                subindo = false;
            }

            corpo.linearVelocity = velocidade;
        }

        private void AtualizarChao()
        {
            estavaNoChao = noChao;
            noChao = false;
            normalDoChao = Vector2.up;

            if (subindo && corpo.linearVelocity.y > 0.1f)
            {
                return;
            }

            Vector2 centro = capsula.bounds.center;
            int quantos = corpo.GetContacts(contatos);
            float melhor = 0f;
            for (int i = 0; i < quantos; i++)
            {
                ContactPoint2D contato = contatos[i];
                if (contato.collider == null || contato.otherCollider == null
                    || contato.collider.isTrigger || contato.otherCollider.isTrigger)
                {
                    continue;
                }

                Vector2 normal = contato.normal;
                if (Vector2.Dot(normal, centro - contato.point) < 0f)
                {
                    normal = -normal;
                }

                if (normal.y > NormalMinimaDoChao && normal.y > melhor)
                {
                    melhor = normal.y;
                    normalDoChao = normal;
                    noChao = true;
                }
            }

            if (!noChao && estavaNoChao && !subindo)
            {
                GrudarNoChao();
            }

            if (noChao)
            {
                ultimoTempoNoChao = Time.time;
            }
        }

        private void GrudarNoChao()
        {
            ContactFilter2D filtro = new ContactFilter2D();
            filtro.useTriggers = false;

            Vector2 origem = new Vector2(capsula.bounds.center.x, capsula.bounds.min.y - 0.02f);
            int acertos = Physics2D.Raycast(origem, Vector2.down, filtro, acertosRaio, DistanciaGrudarNoChao);
            for (int i = 0; i < acertos; i++)
            {
                RaycastHit2D acerto = acertosRaio[i];
                if (acerto.collider == null || acerto.collider == capsula || acerto.collider.isTrigger)
                {
                    continue;
                }
                if (acerto.normal.y <= NormalMinimaDoChao)
                {
                    continue;
                }

                corpo.position += Vector2.down * Mathf.Max(0f, acerto.distance - 0.005f);
                noChao = true;
                normalDoChao = acerto.normal;
                return;
            }
        }

        private void AtualizarAnimacao()
        {
            bool poseFerido = EstaMorto || tempoFerido > 0f;
            desenhoPernas.enabled = !poseFerido;
            desenhoPernas.flipX = lado < 0;
            desenhoTronco.flipX = lado < 0;

            if (poseFerido)
            {
                desenhoTronco.sprite = ArteDoJogo.JogadorFerido;
            }
            else
            {
                bool noAr = Time.time - ultimoTempoNoChao > 0.08f;
                bool correndo = !noAr && Mathf.Abs(entradaHorizontal) > 0.1f;
                if (noAr)
                {
                    animadorPernas.Tocar(ArteDoJogo.PernasPulo, 1f);
                }
                else if (correndo)
                {
                    animadorPernas.Tocar(ArteDoJogo.PernasCorrendo, 12f);
                }
                else
                {
                    animadorPernas.Tocar(ArteDoJogo.PernasParado, 1f);
                }

                bool balanco = correndo && Mathf.Repeat(Time.time * 6f, 1f) < 0.5f;
                if (mira.y > 0.5f)
                {
                    desenhoTronco.sprite = ArteDoJogo.TroncoCima(balanco);
                }
                else if (mira.y < -0.5f)
                {
                    desenhoTronco.sprite = ArteDoJogo.TroncoBaixo;
                }
                else
                {
                    desenhoTronco.sprite = ArteDoJogo.TroncoFrente(balanco);
                }
                float coice = tempoPiscar > 0f && mira.y == 0f ? -lado / 32f : 0f;
                desenhoTronco.transform.localPosition = new Vector3(coice, 0f, 0f);
            }

            float alfa = 1f;
            if (tempoInvencivel > 0f && !EstaMorto)
            {
                alfa = Mathf.Repeat(Time.time * 12f, 1f) < 0.5f ? 0.35f : 1f;
            }
            Color tom = new Color(1f, 1f, 1f, alfa);
            desenhoPernas.color = tom;
            desenhoTronco.color = tom;
        }

        public void Ferir(float origemX)
        {
            if (EstaInvencivel || congelado)
            {
                return;
            }

            bool aindaVivo = GerenciadorDaFase.Instancia.JogadorAtingido();

            float direcao = transform.position.x >= origemX ? 1f : -1f;
            Sangue.Acerto(transform.position, new Vector2(direcao, 0.3f));
            if (!aindaVivo)
            {
                Morrer();
                return;
            }

            tempoFerido = TravaAoFerir;
            tempoInvencivel = DuracaoInvencivel;
            subindo = false;
            noChao = false;
            ultimoTempoNoChao = -10f;
            corpo.gravityScale = EscalaGravidade;
            corpo.linearVelocity = new Vector2(EmpurraoAoFerir.x * direcao, EmpurraoAoFerir.y);
        }

        public void Morrer()
        {
            if (EstaMorto)
            {
                return;
            }

            EstaMorto = true;
            Sangue.Morte(transform.position, Vector2.up);
            capsula.enabled = false;
            corpo.gravityScale = EscalaGravidade;
            corpo.linearVelocity = new Vector2(0f, 10f);
        }

        public void Congelar()
        {
            congelado = true;
            entradaHorizontal = 0f;
        }
    }
}
