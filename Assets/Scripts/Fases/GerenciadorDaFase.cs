using System.Collections;
using Plataforma.Interface;
using Plataforma.Jogo;
using Plataforma.Nucleo;
using Plataforma.Visual;
using UnityEngine;

namespace Plataforma.Fases
{
    public class GerenciadorDaFase : MonoBehaviour
    {
        private const int PontosMedalha = 50;
        private const int PontosRefem = 300;
        private const int PontosVidaCheia = 200;
        private const int BonusDeFase = 1000;
        private const float EsperaProximaCena = 3f;

        [SerializeField, Range(1, SessaoDeJogo.QuantidadeDeFases)] private int numeroDaFase = 1;

        public static GerenciadorDaFase Instancia { get; private set; }

        public MapaDaFase Mapa { get; private set; }
        public Jogador Jogador { get; private set; }

        private DadosDaFase dados;
        private Hud hud;
        private Camera cam;
        private TanqueChefe chefe;
        private int totalDeRefens;
        private int refensResgatados;
        private int indicePremio;
        private bool encerrada;
        private bool pausado;

        private void Awake()
        {
            Instancia = this;
            Time.timeScale = 1f;
            SessaoDeJogo.UltimaFase = Mathf.Max(SessaoDeJogo.UltimaFase, numeroDaFase);

            dados = DadosDaFase.Obter(numeroDaFase);
            ConstrutorDeFase.Resultado fase = ConstrutorDeFase.Construir(dados, transform);
            Mapa = fase.Mapa;

            GameObject atores = new GameObject("Personagens e Itens");
            atores.transform.SetParent(transform, false);
            CriarEntidades(fase, atores.transform);
            Jogador = FabricaDeEntidades.CriarJogador(fase.InicioJogador, atores.transform);

            cam = Camera.main;
            cam.orthographic = true;
            cam.orthographicSize = 6f;
            cam.gameObject.AddComponent<CameraSeguidora>().Configurar(Jogador.transform, fase.Limites);
            ConstrutorDeFundo.Construir(cam, dados.CorDoFundo);

            hud = Hud.Criar(dados.Titulo, Jogador);
            hud.MostrarRefens(0, totalDeRefens);
        }

        private void Start()
        {
            hud.MostrarMensagem(dados.Titulo, dados.Subtitulo, 3f);
        }

        private void OnDestroy()
        {
            if (Instancia == this)
            {
                Instancia = null;
            }
            Time.timeScale = 1f;
        }

        private void Update()
        {
            if (encerrada)
            {
                return;
            }

            if (Controles.PausaApertada)
            {
                pausado = !pausado;
                Time.timeScale = pausado ? 0f : 1f;
                hud.MostrarPausa(pausado);
            }
            else if (pausado && Controles.MenuApertado)
            {
                Cenas.CarregarMenu();
            }
        }

        public bool EstaNaTela(Vector2 posicao, float margem)
        {
            float meiaAltura = cam.orthographicSize + margem;
            float meiaLargura = cam.orthographicSize * cam.aspect + margem;
            Vector3 c = cam.transform.position;
            return Mathf.Abs(posicao.x - c.x) < meiaLargura && Mathf.Abs(posicao.y - c.y) < meiaAltura;
        }

        private void CriarEntidades(ConstrutorDeFase.Resultado fase, Transform pai)
        {
            foreach (ConstrutorDeFase.Surgimento surgimento in fase.Surgimentos)
            {
                Vector2 centro = surgimento.Centro;
                switch (surgimento.Simbolo)
                {
                    case 'H':
                        FabricaDeEntidades.CriarRefem(centro, pai);
                        totalDeRefens++;
                        break;
                    case 'C':
                        FabricaDeEntidades.CriarItem(centro, TipoDeItem.Medalha, pai);
                        break;
                    case 'W':
                        FabricaDeEntidades.CriarItem(centro, TipoDeItem.Metralhadora, pai);
                        break;
                    case 'N':
                        FabricaDeEntidades.CriarItem(centro, TipoDeItem.Granadas, pai);
                        break;
                    case 'V':
                        FabricaDeEntidades.CriarItem(centro, TipoDeItem.Vida, pai);
                        break;
                    case 'S':
                        FabricaDeEntidades.CriarSoldado(centro, fase.Mapa, false, dados.VelocidadeSoldado, dados.IntervaloTiro, pai);
                        break;
                    case 'M':
                        FabricaDeEntidades.CriarSoldado(centro, fase.Mapa, true, 0f, dados.IntervaloTiro * 0.75f, pai);
                        break;
                    case 'D':
                        FabricaDeEntidades.CriarDrone(centro, dados.VelocidadeDrone, pai);
                        break;
                    case 'K':
                        chefe = FabricaDeEntidades.CriarTanqueChefe(centro, pai);
                        break;
                    default:
                        CatalogoDeObjetos.Criar(surgimento.Simbolo, surgimento.Celula, pai);
                        break;
                }
            }
        }

        public void ItemColetado(TipoDeItem tipo)
        {
            switch (tipo)
            {
                case TipoDeItem.Medalha:
                    SessaoDeJogo.Medalhas++;
                    SessaoDeJogo.Pontos += PontosMedalha;
                    break;
                case TipoDeItem.Metralhadora:
                    Jogador.DarMetralhadora();
                    hud.MostrarMensagem(string.Empty, "METRALHADORA PESADA!", 1.2f);
                    break;
                case TipoDeItem.Granadas:
                    Jogador.AdicionarGranadas(10);
                    hud.MostrarMensagem(string.Empty, "+10 GRANADAS", 1.2f);
                    break;
                case TipoDeItem.Vida:
                    if (SessaoDeJogo.GanharVida())
                    {
                        hud.MostrarMensagem(string.Empty, "+1 VIDA!", 1.2f);
                    }
                    else
                    {
                        SessaoDeJogo.Pontos += PontosVidaCheia;
                    }
                    break;
            }
        }

        public void RefemResgatado(Refem refem)
        {
            refensResgatados++;
            SessaoDeJogo.Refens++;
            SessaoDeJogo.Pontos += PontosRefem;
            hud.MostrarRefens(refensResgatados, totalDeRefens);
            hud.MostrarMensagem(string.Empty, "Refém resgatado! (" + refensResgatados + "/" + totalDeRefens + ")", 1.5f);

            TipoDeItem[] premios = { TipoDeItem.Metralhadora, TipoDeItem.Granadas, TipoDeItem.Medalha };
            FabricaDeEntidades.CriarItem((Vector2)refem.transform.position + new Vector2(0.9f, 0.2f),
                premios[indicePremio++ % premios.Length], refem.transform.parent);

            VerificarObjetivos();
        }

        public void InimigoDerrotado(int pontos)
        {
            SessaoDeJogo.InimigosDerrotados++;
            SessaoDeJogo.Pontos += pontos;
        }

        public void ChefeApareceu(TanqueChefe tanque)
        {
            hud.Chefe = tanque;
            hud.MostrarMensagem("ALERTA!", "Destrua o tanque!", 2f);
        }

        public void ChefeFurioso(TanqueChefe tanque)
        {
            hud.MostrarMensagem("REFORÇOS!", "O General mandou mais soldados!", 2f);

            float meiaLargura = cam.orthographicSize * cam.aspect - 1f;
            foreach (float bordaX in new[] { cam.transform.position.x + meiaLargura, cam.transform.position.x - meiaLargura })
            {
                int coluna = Mathf.Clamp(Mathf.FloorToInt(bordaX), 1, Mapa.Largura - 2);
                int linha = Mapa.Altura - 1;
                while (linha > 0 && !Mapa.EhSolido(coluna, linha - 1))
                {
                    linha--;
                }
                FabricaDeEntidades.CriarSoldado(new Vector2(coluna + 0.5f, linha + 0.5f), Mapa, false,
                    dados.VelocidadeSoldado, dados.IntervaloTiro * 0.8f, tanque.transform.parent);
            }
        }

        public void ChefeDerrotado(TanqueChefe tanque)
        {
            InimigoDerrotado(TanqueChefe.Pontos);
            chefe = null;
            VerificarObjetivos();
        }

        private void VerificarObjetivos()
        {
            if (encerrada || refensResgatados < totalDeRefens)
            {
                return;
            }
            if (chefe != null)
            {
                hud.MostrarMensagem(string.Empty, "Falta destruir o tanque!", 2f);
                return;
            }
            StartCoroutine(ConcluirFase());
        }

        public bool JogadorAtingido()
        {
            if (encerrada)
            {
                return true;
            }
            if (SessaoDeJogo.PerderVida() > 0)
            {
                return true;
            }
            StartCoroutine(FimDeJogo());
            return false;
        }

        private IEnumerator ConcluirFase()
        {
            encerrada = true;
            Jogador.Congelar();
            SessaoDeJogo.Pontos += BonusDeFase;
            hud.MostrarMensagem("MISSÃO CUMPRIDA!", "Bônus +" + BonusDeFase, 0f);

            yield return new WaitForSeconds(EsperaProximaCena);

            if (numeroDaFase < SessaoDeJogo.QuantidadeDeFases)
            {
                Cenas.CarregarFase(numeroDaFase + 1);
            }
            else
            {
                SessaoDeJogo.Resultado = ResultadoDoJogo.Vitoria;
                Cenas.CarregarFim();
            }
        }

        private IEnumerator FimDeJogo()
        {
            encerrada = true;
            Jogador.Morrer();
            hud.MostrarMensagem("Fim de jogo", "As vidas acabaram...", 0f);

            yield return new WaitForSeconds(EsperaProximaCena * 0.7f);

            SessaoDeJogo.Resultado = ResultadoDoJogo.Derrota;
            Cenas.CarregarFim();
        }
    }
}
