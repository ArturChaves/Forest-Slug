using Plataforma.Fases;
using Plataforma.Visual;
using UnityEngine;

namespace Plataforma.Jogo
{
    [RequireComponent(typeof(AnimadorDeSprite))]
    public class SoldadoInimigo : InimigoBase
    {
        private const float VisaoX = 10f;
        private const float VisaoY = 4f;

        private AnimadorDeSprite animador;
        private MapaDaFase mapa;
        private bool parado;
        private float rapidez = 1.2f;
        private float intervaloTiro = 1.6f;
        private float tempoTiro;
        private float tempoPoseTiro;
        private float direcao = -1f;
        private bool perdeuBraco;

        protected override int Pontos => 100;

        protected override void Awake()
        {
            base.Awake();
            animador = GetComponent<AnimadorDeSprite>();
            areaDeAcerto.size = new Vector2(0.5f, 0.85f);
            areaDeAcerto.offset = new Vector2(0f, -0.06f);
            animador.Tocar(ArteDoJogo.InimigoAndando, 7f);
            animador.Embaralhar();
        }

        public void Configurar(MapaDaFase mapaDaFase, bool ehParado, float velocidadeAndar, float intervaloTiros)
        {
            mapa = mapaDaFase;
            parado = ehParado;
            rapidez = velocidadeAndar;
            intervaloTiro = intervaloTiros;
            tempoTiro = Random.Range(0.8f, 1.6f);
            direcao = Random.value < 0.5f ? -1f : 1f;
            vida = parado ? 3 : 2;
        }

        private void FixedUpdate()
        {
            if (morto || mapa == null)
            {
                return;
            }

            float dt = Time.fixedDeltaTime;
            Jogador jogador = JogadorAlvo();
            Vector2 posicao = corpo.position;

            bool veJogador = jogador != null
                              && NaTela(posicao)
                              && Mathf.Abs(jogador.transform.position.x - posicao.x) < VisaoX
                              && Mathf.Abs(jogador.transform.position.y - posicao.y) < VisaoY;

            if (veJogador)
            {
                direcao = jogador.transform.position.x < posicao.x ? -1f : 1f;
                tempoTiro -= dt;
                tempoPoseTiro -= dt;
                if (tempoTiro <= 0f)
                {
                    Atirar(jogador);
                    tempoTiro = intervaloTiro + Random.Range(-0.2f, 0.3f);
                }
                animador.Tocar(tempoPoseTiro > 0f ? ArteDoJogo.InimigoAtirando : ArteDoJogo.InimigoMirando, 1f);
            }
            else if (!parado)
            {
                Patrulhar(posicao, dt);
                animador.Tocar(ArteDoJogo.InimigoAndando, 7f);
            }
            else
            {
                animador.Tocar(ArteDoJogo.InimigoMirando, 1f);
            }

            desenho.flipX = direcao > 0f;
        }

        private void Patrulhar(Vector2 posicao, float dt)
        {
            int linha = Mathf.FloorToInt(posicao.y);
            int colunaAFrente = Mathf.FloorToInt(posicao.x + direcao * 0.4f);
            bool paredeAFrente = mapa.EhSolido(colunaAFrente, linha);
            bool chaoAFrente = mapa.EhChaoPlano(colunaAFrente, linha - 1);
            if (paredeAFrente || !chaoAFrente)
            {
                direcao = -direcao;
                return;
            }
            corpo.MovePosition(posicao + new Vector2(direcao * rapidez * dt, 0f));
        }

        private void Atirar(Jogador jogador)
        {
            Vector2 ponta = corpo.position + new Vector2(direcao * 0.44f, 0.03f);
            Vector2 alvo = (Vector2)jogador.transform.position + new Vector2(0f, 0.05f);
            Vector2 mira = (alvo - ponta).normalized;

            if (mira.x * direcao < 0.35f)
            {
                mira = new Vector2(direcao, Mathf.Clamp(mira.y, -0.8f, 0.8f)).normalized;
            }

            Projetil.Disparar(ArteDoJogo.BalaInimiga, ponta, mira * 6.5f, Equipe.Inimigo, 1, 3f);
            tempoPoseTiro = 0.18f;
        }

        protected override void AoSerAtingido()
        {
            tempoTiro = Mathf.Min(tempoTiro, 0.5f);

            if (vida > 0 && !perdeuBraco && Random.value < 0.5f)
            {
                perdeuBraco = true;
                Sangue.ArrancarMembro(ArteDoJogo.PedacoBraco, (Vector2)transform.position + new Vector2(0f, 0.15f), direcaoDoTiro, 6f);
            }
        }

        protected override void AoSerDerrotado()
        {
            Vector2 centro = transform.position;
            Vector2 empurrao = direcaoDoTiro;

            if (mortoPorExplosao)
            {
                Sangue.ArrancarMembro(ArteDoJogo.PedacoCabeca, centro + new Vector2(0f, 0.3f), empurrao, 9f);
                Sangue.ArrancarMembro(ArteDoJogo.PedacoTronco, centro, empurrao, 6f);
                Sangue.ArrancarMembro(ArteDoJogo.PedacoPerna, centro + new Vector2(-0.1f, -0.3f), empurrao, 7f);
                Sangue.ArrancarMembro(ArteDoJogo.PedacoPerna, centro + new Vector2(0.1f, -0.3f), -empurrao, 7f);
                if (!perdeuBraco)
                {
                    Sangue.ArrancarMembro(ArteDoJogo.PedacoBraco, centro + new Vector2(0f, 0.1f), -empurrao, 8f);
                }
                Sangue.ArrancarMembro(ArteDoJogo.PedacoBraco, centro + new Vector2(0f, 0.1f), empurrao, 8f);
                for (int i = 0; i < 4; i++)
                {
                    Sangue.ArrancarMembro(i % 2 == 0 ? ArteDoJogo.PedacoCarne : ArteDoJogo.PedacoCarnePequeno, centro, Random.insideUnitCircle, 8f);
                }
                Destroy(gameObject);
                return;
            }

            Sangue.ArrancarMembro(ArteDoJogo.PedacoCabeca, centro + new Vector2(0f, 0.3f), empurrao, 7f);
            if (!perdeuBraco && Random.value < 0.5f)
            {
                Sangue.ArrancarMembro(ArteDoJogo.PedacoBraco, centro + new Vector2(0f, 0.1f), empurrao, 6f);
            }
            if (Random.value < 0.3f)
            {
                Sangue.ArrancarMembro(ArteDoJogo.PedacoPerna, centro + new Vector2(0f, -0.3f), empurrao, 5f);
            }

            animador.Tocar(ArteDoJogo.InimigoMorrendo, 8f, false);
            desenho.sortingOrder = Sangue.OrdemDasManchas + 2;
            gameObject.AddComponent<DesbotarESumir>().Configurar(6f, 1.5f);
        }
    }
}
